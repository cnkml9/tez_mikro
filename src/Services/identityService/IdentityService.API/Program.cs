using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Mikroservices.Application.Abstractions.Services;
using Mikroservices.Application.Abstractions.Services.Token;
using Mikroservices.Domain.Entities.Identity;
using Mikroservices.Infrastructure.Services.Token;
using Mikroservices.Persistence.Concretes;
using Mikroservices.Persistence.Context;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//identity

//builder.Services.AddDbContext<MikroserviceIdentityDbContext>(options =>
//              options.UseSqlServer("Server=CAN\\SQLEXPRESS;Database=MicDb;Trusted_Connection=True;"));


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MikroserviceIdentityDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequiredLength = 3;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<MikroserviceIdentityDbContext>();

//builder.Services.AddScoped<RoleManager<IdentityRole>>();



//builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
//    policy.WithOrigins("http://c_react:3000", "https://c_react:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials()

//));

builder.Services.AddCors(opt => opt.AddDefaultPolicy(
    policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
    ));

builder.Services.AddDistributedMemoryCache(); // Bellek tabanlý oturum yönetimi kullanmak için

builder.Services.AddSession();
//identity





builder.Services.AddScoped<ITokenHandler, Mikroservices.Infrastructure.Services.Token.TokenHandler>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService,AuthService>();



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



            ValidAudience = "www.bilmemne.com",
            ValidIssuer = "www.myapi.com",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Kim bakar ardýna sen mi..")),
            //jwt ömrünü belirliyor 15 saniyelik token'ý 15saniye sonra etkisiz yapýyor ve eriþimi engelliyor-jwtRefreshToken yapýlandýrmasý için
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

           

            //Log mekanýzmasý için
            NameClaimType = ClaimTypes.Name  //JWT üzerinde Name claime karþýlýk gelen deðeri
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
