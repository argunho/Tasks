using Hanssens.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tasks.Interfaces;
using Tasks.Models;

namespace Tasks.Repositories
{
    public class HelpRepository : IHelpers
    {

        public static LocalStorageConfiguration config = new LocalStorageConfiguration();

        public static LocalStorage loc = new LocalStorage(config, "TasksPage");

        public string TokenGenerator(Users user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("12345abcd-ListToDo"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Jti, user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email)
                };

            var token = new JwtSecurityToken
            (
                issuer: "webapi.com",
                audience: "webapi.com",
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            loc.Store<string>("hashToken", encodetoken);
            return encodetoken;
        }
    }
}
