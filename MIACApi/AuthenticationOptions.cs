using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MIACApi
{
    public class AuthenticationOptions
    {
        public const string ISSUER = "JwtIssuer";
        public const string AUDIENCE = "JwtClient";
        private const string KEY = "yH6IgajaUDlDlaCK";
        public const int LIFTTIME = 10;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}