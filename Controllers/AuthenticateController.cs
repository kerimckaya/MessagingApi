using MessagingApi.Data;
using MessagingApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MessagingApi.Controllers
{
    public class AuthenticateController : BaseController
    {
        private IConfiguration _config;
        private readonly ApplicationDbContext _c;
        public AuthenticateController(IConfiguration config, ApplicationDbContext c)
        {
            _config = config;
            _c = c;
        }

        private IConfiguration Get_config()
        {
            return _config;
        }

        private async Task<string> GenerateJSONWebTokenWithTelephone(LoginModelWithTelephone userInfo)
        {
            string key = _config["Jwt:Key"] ?? "";
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(s: key));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new();

            User userData = await _c.User.FirstOrDefaultAsync(X => X.PhoneNumber == userInfo.Telephone);
            claims.Add(new Claim("ApiUserId", userData.Id.ToString()));
            claims.Add(new Claim("Telephone", (userData.PhoneNumber ?? "").ToString()));

            JwtSecurityToken token = new(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddHours(1),
              signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [AllowAnonymous]
        [HttpPost(nameof(Auth))]
        public async Task<IActionResult> Auth([FromBody] LoginModelWithTelephone data)
        {
            IActionResult response = Unauthorized();
            var user = _c.User.FirstOrDefault(x => x.PhoneNumber == data.Telephone);
            if (user != null)
            {
                string tokenString = await GenerateJSONWebTokenWithTelephone(data);
                response = Ok(new { Token = tokenString, Message = "Success", userData = user });
            }
            return response;
        }

    }
    public class LoginModelWithTelephone
    {
        [Required]
        public string Telephone { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
    }
}
