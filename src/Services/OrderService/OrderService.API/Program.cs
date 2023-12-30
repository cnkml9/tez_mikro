using Consul;
using OrderService.API.Configurations;
using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using EventBus.Base.Abstraction;
using EventBus.Base;
using EventBus.Factory;

using OrderService.API.IntegrationEvents.EventHandlers;
using OrderService.API.IntegrationEvents.Events;
using OrderService.Infrastructure;
using Microsoft.AspNetCore;
using OrderService.Appliation.Abstract;
using OrderService.Infrastructure.Repositories;
using OrderService.API.Services;
using RabbitMQ.Client;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//AddServiceDiscoveryRegistration
builder.Services.AddServiceDiscoveryRegistration(builder.Configuration);
//AddServiceDiscoveryRegistration

//AddApplicationRegistration
var assm = Assembly.GetExecutingAssembly();
builder.Services.AddMediatR(assm);
builder.Services.AddAutoMapper(assm);


//AddApplicationRegistration

//AddPersistenceRegistration

//builder.Services.AddDbContext<OrderDbContext>(opt =>
//{
//    opt.UseSqlServer("Server=CAN\\SQLEXPRESS;Database=MicDb;Trusted_Connection=True;");
//},ServiceLifetime.Transient);





//jwtTok-authonication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true, //olu�turulacak token de�erini kimlerin/sitelerin kullanaca��n� belirler
            ValidateIssuer = true, //olu�turulacak token de�erini kimin da��tt���n� ifade edece�imiz aland�r
            ValidateLifetime = true, //Token s�resini kontrol eder do�rulama yeri
            ValidateIssuerSigningKey = true, //�retilecek token de�erinin uygulamam�za ait bir de�er oldu�unu ifade eden seciry key verisisinin do�rulamas�d�r.



            ValidAudience = "www.bilmemne.com",
            ValidIssuer = "www.myapi.com",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Kim bakar ard�na sen mi..")),
            //jwt �mr�n� belirliyor 15 saniyelik token'� 15saniye sonra etkisiz yap�yor ve eri�imi engelliyor-jwtRefreshToken yap�land�rmas� i�in
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

            //Log mekan�zmas� i�in
            NameClaimType = ClaimTypes.Name  //JWT �zerinde Name claime kar��l�k gelen de�eri
            //User.Identity.Name propertysinden elde edebiliriz.

        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($"Challenge: {context.Error}, {context.ErrorDescription}");
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                Console.WriteLine($"Forbidden: {context.Response.StatusCode}");
                return Task.CompletedTask;
            }
        };
    });
//jwtTok-authentication


//Cors
builder.Services.AddCors(opt => opt.AddDefaultPolicy(
    policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
    ));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mySqlOptions => mySqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore));
   
});


//builder.Services.AddDbContext<OrderDbContext>(options =>
//{
//    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
//}, ServiceLifetime.Scoped);


builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<IBuyerRepository, BuyerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<OrderCreatedIntegrationEventHandler>();
builder.Services.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
builder.Services.AddTransient<OrderPaymentSuccessIntegrationEventHandler>();
builder.Services.AddTransient<IIdentityService, IdentityService>();



//AddPersistenceRegistration

builder.Services.AddLogging(configure => configure.AddConsole());



//eventbus
builder.Services.AddSingleton<IEventBus>(sp =>
{

    EventBusConfig config = new()
    {
        ConnectionRetryCoun = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppNmae = "OrderService",
        EventBusType = EventBusType.RabbitMQ,
        Connection = new ConnectionFactory()
        {
            HostName = "c_rabbitmq"
        }
    };

    return EventBusFactory.Create(config, sp);
});






var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//configureSubscription-event
var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
eventBus.subscribe<OrderPaymentSuccessIntegrationEvent, OrderPaymentSuccessIntegrationEventHandler>();
eventBus.subscribe<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();


app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
