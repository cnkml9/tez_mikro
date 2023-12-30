using BasketService.Api.Extensions;
using BasketService.API.Core.Application.Repository;
using BasketService.API.Core.Application.Services;
using BasketService.API.Extensions;
using BasketService.API.Infrastructure.Repository;
using BasketService.API.IntegrationEvents.EventHandlers;
using BasketService.API.IntegrationEvents.Events;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
            ValidateAudience = true, //oluşturulacak token değerini kimlerin/sitelerin kullanacağını belirler
            ValidateIssuer = true, //oluşturulacak token değerini kimin dağıttığını ifade edeceğimiz alandır
            ValidateLifetime = true, //Token süresini kontrol eder doğrulama yeri
            ValidateIssuerSigningKey = true, //üretilecek token değerinin uygulamamıza ait bir değer olduğunu ifade eden seciry key verisisinin doğrulamasıdır.



            ValidAudience = "www.bilmemne.com",
            ValidIssuer = "www.myapi.com",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Kim bakar ardına sen mi..")),
            //jwt ömrünü belirliyor 15 saniyelik token'ı 15saniye sonra etkisiz yapıyor ve erişimi engelliyor-jwtRefreshToken yapılandırması için
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

            //Log mekanızması için
            NameClaimType = ClaimTypes.Name  //JWT üzerinde Name claime karşılık gelen değeri
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


//configurationExt
//builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.AddSingleton(sp=>sp.ConfigureRedis(builder.Configuration));
builder.Services.ConfigureConsul(builder.Configuration);
builder.Services.AddHttpContextAccessor();

//dependecy-inj.
builder.Services.AddTransient<IBasketRepository, RedisBasketRepository>();
builder.Services.AddTransient<IIdentityService,IdentityService >();
//builder.Services.AddScoped<OrderCreatedIntegrationEventHandler>();


builder.Services.AddTransient<OrderCreatedIntegrationEventHandler>();

//eventbus
builder.Services.AddSingleton<IEventBus>(sp =>
{

    EventBusConfig config = new()
    {
        ConnectionRetryCoun = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppNmae = "BasketService",
        EventBusType = EventBusType.RabbitMQ,
        Connection = new ConnectionFactory()
        {
            HostName = "c_rabbitmq"
        }
    };

    return EventBusFactory.Create(config, sp);
});

//configurationExt




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


//consul registration
app.RegisterWithConsul(builder.Configuration);

//app.UseHttpsRedirection();

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.Run();

