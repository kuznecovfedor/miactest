using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MIACApi
{
    public class AuthenticationOptions
    {
        public const string ISSUER = "JwtIssuer";
        public const string AUDIENCE = "JwtClient";
        public const string KEY = "yH6IgajasqweqweqweyH6IgajasqweqweqweyH6Igajasqweqweqwe";
        public const int LIFTTIME = 10;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}