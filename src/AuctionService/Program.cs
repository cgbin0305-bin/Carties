using AuctionService;
using AuctionService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
  opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddMassTransit(x =>
{
  x.AddEntityFrameworkOutbox<AuctionDbContext>(opt =>
  {
    /*
    each 10 second it will check the outbox 
    
    if the service is available => the message will be delivered  
    
    if it's is not => every 10s, it's going to attemp inside our outbox and see if anything hasn't been delivered yet
    */

    opt.QueryDelay = TimeSpan.FromSeconds(20);
    opt.UsePostgres();
    opt.UseBusOutbox();
  });
  //make the connection to rabbitMQ (use local host) 
  x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();
  x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));
  x.UsingRabbitMq((context, cfg) =>
  {
    cfg.ConfigureEndpoints(context);
  });
});
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

try
{
  DbInitializer.InitDb(app);
}
catch (Exception e)
{
  System.Console.WriteLine(e);
}
app.Run();
