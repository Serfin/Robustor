using Consumer;
using Microsoft.EntityFrameworkCore;
using Robustor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKafkaMessageBroker(builder.Configuration);
builder.Services.AddHostedService<KafkaConsumerBackgroundService>();

builder.Services.AddDbContext<TestDbContext>(optionsBuilder => 
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString(Variables.DefaultConnection)));

var app = builder.Build();

app.MapGet("/messages", async (TestDbContext dbContext, CancellationToken cancellationToken) 
    => TypedResults.Ok(await dbContext.Orders.CountAsync(cancellationToken)));

app.MapDelete("/messages/cleanup", async (TestDbContext dbContext, CancellationToken cancellationToken) =>
{
    dbContext.Orders.ExecuteDelete();
    await dbContext.SaveChangesAsync(cancellationToken);
    return TypedResults.NoContent();
});


app.Run();