using Microsoft.AspNetCore.Identity;
using Mikroservices.Application.Abstractions.Services;
using Mikroservices.Application.DTOs.User;
using Mikroservices.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikroservices.Persistence.Concretes
{
    public class UserService : IUserService
    {
        readonly UserManager<AppUser> _userManager;

        public UserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CreateUserResponse> createAsync(CreateUser model)
        {
            IdentityResult result = await _userManager.CreateAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Email = model.Email,
                NameSurnname = model.NameSurname
            }, model.Password);

            CreateUserResponse response = new()
            {
                Succeded = result.Succeeded
            };

            if (result.Succeeded)
                response.Message = "kullanıcı başarıyla oluşuruldu";
            else
            {
                foreach (var error in result.Errors)
                {
                    response.Message = $"{error.Code}-{error.Description}\n";
                }
            }

            return response;

        }

        public async Task UpdateRefreshToken(string refreshToken, AppUser user, DateTime AccessTokenDate, int addOnAccessTokenDate)
        {
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenEndDate = AccessTokenDate.AddSeconds(addOnAccessTokenDate);
                await _userManager.UpdateAsync(user);
            }
            else
                throw new Exception("Hata oluştu");
        }
    }
}
