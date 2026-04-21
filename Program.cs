var app = WebApplication.Create(args);

// Хранилище заметок (в памяти)
var notes = new List<Note>();
var nextId = 1;

// Эндпоинт /health
app.MapGet("/health", () => new { status = "ok", time = DateTime.UtcNow });

// Эндпоинт /version
app.MapGet("/version", () => new { appName = "IsLabApp", version = "1.0.0" });

// Эндпоинт /db/ping (заглушка)
app.MapGet("/db/ping", () => new { connected = false, message = "SQL Server not configured yet" });

// GET /api/notes - получить все заметки
app.MapGet("/api/notes", () => notes);

// GET /api/notes/{id} - получить заметку по id
app.MapGet("/api/notes/{id}", (int id) =>
{
    var note = notes.FirstOrDefault(n => n.Id == id);
    return note != null ? Results.Ok(note) : Results.NotFound();
});

// POST /api/notes - создать заметку
app.MapPost("/api/notes", (NoteInput input) =>
{
    var note = new Note
    {
        Id = nextId++,
        Title = input.Title,
        Text = input.Text,
        CreatedAt = DateTime.UtcNow
    };
    notes.Add(note);
    return Results.Created($"/api/notes/{note.Id}", note);
});

// DELETE /api/notes/{id} - удалить заметку
app.MapDelete("/api/notes/{id}", (int id) =>
{
    var note = notes.FirstOrDefault(n => n.Id == id);
    if (note == null) return Results.NotFound();
    notes.Remove(note);
    return Results.NoContent();
});

app.Run();

// Классы моделей
class Note
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Text { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

class NoteInput
{
    public string Title { get; set; } = "";
    public string Text { get; set; } = "";
}