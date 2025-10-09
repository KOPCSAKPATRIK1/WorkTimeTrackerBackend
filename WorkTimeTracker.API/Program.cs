using Microsoft.EntityFrameworkCore;
using WorkTimeTracker.Business;
using WorkTimeTracker.Core.Interfaces.Business;
using WorkTimeTracker.Core.Interfaces.Repository;
using WorkTimeTracker.Repository.Data;
using WorkTimeTracker.Repository.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<WorkTimeTrackerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Server=(localdb)\\mssqllocaldb;Database=WorkTimeTracker;Trusted_Connection=true;TrustServerCertificate=true;"))
// vagy InMemory adatbázishoz fejlesztéshez:
// options.UseInMemoryDatabase("WorkTimeTrackerDb")
);
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProjectService, ProjectService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
