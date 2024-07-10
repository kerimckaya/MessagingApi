using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace MessagingApi.Services
{
    public class TokenService
    {
        public static string GetUserPhoneByToken(string token)
        {
            try
            {
                JwtSecurityTokenHandler handler = new();
                JwtSecurityToken jwtSecurityToken = handler.ReadJwtToken(token);
                string phoneNumber = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "Telephone")?.Value ?? "-1";
                return phoneNumber;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "-1";
            }
        }


    }
}
