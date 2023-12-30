using IdentityService.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Mikroservices.Application.Abstractions.Services;
using Mikroservices.Application.Abstractions.Services.Token;
using Mikroservices.Application.DTOs;
using Mikroservices.Domain.Entities.Identity;
using Mikroservices.Persistence.Concretes;
using Mikroservices.Persistence.Context;

namespace IdentityService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        readonly SignInManager<AppUser> _signInManager;
        readonly ITokenHandler _tokenHandler;
        readonly IHttpContextAccessor _contextAccessor;
        readonly IAuthService _authService;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenHandler tokenHandler, IHttpContextAccessor contextAccessor, IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenHandler = tokenHandler;
            _contextAccessor = contextAccessor;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { Id=Guid.NewGuid().ToString(), UserName=model.UserName,Email=model.Email,NameSurnname=model.UserName };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // You can customize this part based on your requirements, for example, generate a token, send an email confirmation, etc.
                    return Ok(new { Message = "User created successfully!" });
                }
                else
                {
                    return BadRequest(new { Errors = result.Errors });
                }
            }

            return BadRequest(new { Message = "Invalid model state" });
        }

        [HttpPost("Login")]
        public async Task<Token> LoginAsync(string UserNameOrEmail, string Password, int accesTokenLifeTime)
        {
            AppUser user = await _userManager.FindByNameAsync(UserNameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(UserNameOrEmail);
            }
            if (user == null)
            {
                throw new Exception("Kullanıcı adı veya şifre hatalı");
            }
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, Password, false);

            if (result.Succeeded)
            {
                Token token = _tokenHandler.CreateAccessToken(accesTokenLifeTime, user);
                await UpdateRefreshToken(token.refreshToken, user, token.Expiration, 5);

                return token;
            }
            else
            {
                throw new Exception("hata oluştu");
            }
            //return new LoginUserErrorCommandResponse()
            //{
            //    Message="Kullanıcı adı veya şifre hatalı..."
            //};
        }


        [HttpPost("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginRequest request)
        {
            var token = await _authService.GoogleLoginAsync(request.IdToken, 900);
            return Ok(token);

        }


        //[Authorize(AuthenticationSchemes = "Admin")]
        [Authorize]
        [HttpGet("Deneme")]
        public IActionResult TryGet()
        {
            var userName = _contextAccessor?.HttpContext?.User?.Identity?.Name;

            return Ok(userName);
        }

        [HttpGet("tken")]
        public async Task UpdateRefreshToken(string refreshToken, AppUser user, DateTime AccessTokenDate, int addOnAccessTokenDate)
        {
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenEndDate = AccessTokenDate.AddSeconds(addOnAccessTokenDate);
                await _userManager.UpdateAsync(user);
            }
            else
                throw new Exception("hata oluştu");
        }
    }
}
