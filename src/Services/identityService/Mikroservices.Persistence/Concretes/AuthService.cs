using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mikroservices.Application.Abstractions.Services;
using Mikroservices.Application.Abstractions.Services.Token;
using Mikroservices.Application.DTOs;
using Mikroservices.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikroservices.Persistence.Concretes
{
    public class AuthService : IAuthService
    {
        readonly UserManager<AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;
        readonly IUserService _userService;
        readonly IConfiguration _configuration;


        public AuthService(UserManager<AppUser> userManager, ITokenHandler tokenHandler, IUserService userService, IConfiguration configuration)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _userService = userService;
            _configuration = configuration;
        }

        async Task<Token> CreateUserExternalAsync(AppUser user, string email, string name, UserLoginInfo info, int accesTokenLifeTime)
        {
            //user null ise result false olur değilse true olur...
            bool result = user != null;

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = email,
                        UserName = email,
                        NameSurnname = name
                    };
                    var identityResult = await _userManager.CreateAsync(user);
                    result = identityResult.Succeeded;
                }
            }
            if (result)
            {
                await _userManager.AddLoginAsync(user, info);
                Token token = _tokenHandler.CreateAccessToken(accesTokenLifeTime, user);
                await _userService.UpdateRefreshToken(token.refreshToken, user, token.Expiration, 5);
                return token;
            }
            else
                throw new Exception("Invalid External Authentication.");


        }

        public async Task<Token> GoogleLoginAsync(string idToken, int accesTokenLifeTime)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["ExternalLoginSettings:Google:ClientId"] }
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            //dış kaynaktan gelen kullanıcı bilgilerini sorguluyor kayıtlı değilse aspNetUserLogins e kaydeder bilgilerini,örneğin
            //google dan ilk defa girş yapan kullanıcının  bilgilerini bu tabloya kaydeder.
            var info = new UserLoginInfo("GOOGLE", payload.Subject, "GOOGLE");

            AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            return await CreateUserExternalAsync(user, payload.Email, payload.Name, info, accesTokenLifeTime);

        }
        public Task<Token> LoginAsync(string UserNameOrEmail, string Password, int accesTokenLifeTime)
        {
            throw new NotImplementedException();
        }

        public async Task<Token> RefreshTokenLoginAsync(string refreshToken)
        {
            AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user != null && user?.RefreshTokenEndDate > DateTime.UtcNow)
            {
                Token token = _tokenHandler.CreateAccessToken(45, user);
                await _userService.UpdateRefreshToken(token.refreshToken, user, token.Expiration, 300);
                return token;
            }
            else
                throw new NotImplementedException();
        }
    }
}
