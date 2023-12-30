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
builder.Services.AddHttpContextAccessor();//client'tan gele request neticesinde olu�turulan httpcontext nesnesine katmanlardak 
//classlar �zerinden(business logic) �zerinden eri�ebilmemizi sa�layan bir servistir.


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



            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            //jwt �mr�n� belirliyor 15 saniyelik token'� 15saniye sonra etkisiz yap�yor ve eri�imi engelliyor-jwtRefreshToken yap�land�rmas� i�in
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

            //Log mekan�zmas� i�in
            NameClaimType = ClaimTypes.Name   //JWT �zerinde Name claime kar��l�k gelen de�eri
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

