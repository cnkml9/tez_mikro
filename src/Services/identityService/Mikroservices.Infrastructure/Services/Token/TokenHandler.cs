﻿using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Mikroservices.Application.Abstractions.Services.Token;
using Mikroservices.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Mikroservices.Infrastructure.Services.Token
{
    public class TokenHandler : ITokenHandler
    {
        readonly IConfiguration _configuration;

        public TokenHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Application.DTOs.Token CreateAccessToken(int minute,AppUser appUser)
        {
            Application.DTOs.Token token = new Application.DTOs.Token();

            //security key simetriği alınıyor 2.kez doğrulama gibi
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));

            //şifrelenmiş kimliği oluşturuyruz
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            //oluşturulacak token ayarlarını veriyoruz.
            token.Expiration = DateTime.UtcNow.AddMinutes(minute);
            JwtSecurityToken securityToken = new(
                    audience: _configuration["Token:Audience"],
                    issuer: _configuration["Token:Issuer"],
                    expires:token.Expiration,
                    notBefore:DateTime.UtcNow,
                    signingCredentials:signingCredentials,
                    claims:new List<Claim> { new(ClaimTypes.Name, appUser.UserName),new(ClaimTypes.NameIdentifier,appUser.Id) }
               );

            //Token oluşturucu sınıftan bir örnek alalım
            JwtSecurityTokenHandler tokenHandler = new();
           token.AccessToken= tokenHandler.WriteToken(securityToken);

            token.refreshToken= CreateRefreshToken();

            return token;

        }

        public string CreateRefreshToken()
        {
            byte[] number = new byte[32];
            using RandomNumberGenerator random =  RandomNumberGenerator.Create();
            random.GetBytes(number);
            return Convert.ToBase64String(number);
        }
    }
}
