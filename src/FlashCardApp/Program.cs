using FlashCardApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddFlashCardApp();

var app = builder.Build();

app.UseFlashCardApp();

app.Run();
