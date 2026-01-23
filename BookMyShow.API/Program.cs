using Microsoft.EntityFrameworkCore;
using BookMyShow.Application.Interfaces;
using BookMyShow.Infrastructure.Data;
using BookMyShow.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);




builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IBookingService, BookingService>();

// background service for releasing expired held seats
builder.Services.AddHostedService<ExpiredSeatsReleaseService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    Console.WriteLine("CONNECTED DB: " + db.Database.GetDbConnection().Database);
    Console.WriteLine("CONNECTED SERVER: " + db.Database.GetDbConnection().DataSource);
}


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
