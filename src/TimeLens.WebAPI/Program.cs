using Microsoft.EntityFrameworkCore;
using TimeLens.Infrastructure.Persistence;
using TimeLens.Domain.Interfaces;
using TimeLens.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký AppDbContext với SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IJournalRepository, JournalEntryRepository>();
builder.Services.AddScoped<IMoodRepository, MoodEntryRepository>();
builder.Services.AddScoped<IReflectionRepository, ReflectionEntryRepository>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();