using CatagolService.API.Extensions;
using CatalogService.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Mikroservice.Application.Abstraction;
using Mikroservice.Domain.Entities;
using Mikroservice.Infrastructure;
using Mikroservice.Infrastructure.Concrete;
using Mikroservice.Persistence.Context;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Consul-Configuration
builder.Services.ConfigureConsul(builder.Configuration);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



//jwtTok-authonication
builder.Services.AddHttpContextAccessor();//client'tan gele request neticesinde oluþturulan httpcontext nesnesine katmanlardak 
//classlar üzerinden(business logic) üzerinden eriþebilmemizi saðlayan bir servistir.


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
            ValidateAudience = true, //oluþturulacak token deðerini kimlerin/sitelerin kullanacaðýný belirler
            ValidateIssuer = true, //oluþturulacak token deðerini kimin daðýttýðýný ifade edeceðimiz alandýr
            ValidateLifetime = true, //Token süresini kontrol eder doðrulama yeri
            ValidateIssuerSigningKey = true, //üretilecek token deðerinin uygulamamýza ait bir deðer olduðunu ifade eden seciry key verisisinin doðrulamasýdýr.



            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            //jwt ömrünü belirliyor 15 saniyelik token'ý 15saniye sonra etkisiz yapýyor ve eriþimi engelliyor-jwtRefreshToken yapýlandýrmasý için
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

            //Log mekanýzmasý için
            NameClaimType = ClaimTypes.Name   //JWT üzerinde Name claime karþýlýk gelen deðeri
            //User.Identity.Name propertysinden elde edebiliriz.

        };
    });
//jwtTok-authentication



//db
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<CatalogDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});


//builder.Services.ConfigureAuth(builder.Configuration);


// IRepository ve Repository'yi ekleyin
builder.Services.AddScoped<IRepository<CatalogItem>, Repository<CatalogItem>>();
builder.Services.AddScoped<IRepository<CatalogBrand>, Repository<CatalogBrand>>();
builder.Services.AddScoped<IRepository<CatalogType>, Repository<CatalogType>>();

// ICatalogService ve CatalogService'yi ekleyin
builder.Services.AddScoped<ICatalogService, Mikroservice.Infrastructure.Concrete.CatalogService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.RegisterWithConsul(builder.Configuration);

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseRouting();
app.MapControllers();


app.UseAuthentication();
app.UseAuthorization();



app.Run();

