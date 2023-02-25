using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Dto;
using DigitalLibary.WebApi.Helper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.WebApi.Common
{
    public class JwtService
    {
        #region Variables
        private readonly AppSettingModel _appSettingModel;
        public DataContext _DbContext;
        #endregion

        #region Contructor
        public JwtService(IOptionsMonitor<AppSettingModel> optionsMonitor,
        DataContext dataContext)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _DbContext = dataContext;
        }
        #endregion

        #region Method
        // create jwt when user register
        public string GeneralByRegister(UserDTO UserInfor)
        {
            var symmetricSecureKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettingModel.SecretKey));
            var credentials = new SigningCredentials(symmetricSecureKey, SecurityAlgorithms.HmacSha256Signature);
            var header = new JwtHeader(credentials);

            var payload = new JwtPayload(UserInfor.Email,
            null, null, null, System.DateTime.Now.AddMinutes(15));

            payload.Add("user", UserInfor);

            var securerityToken = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(securerityToken);
        }

        // verify token method
        public JwtSecurityToken Verify(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettingModel.SecretKey);
            tokenHandler.ValidateToken(jwt, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken securityToken);

            return (JwtSecurityToken)securityToken;
        }

        // create token when user login 
        public TokenModel GenerateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettingModel.SecretKey);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Fullname),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);

                return Convert.ToBase64String(random);
            }
        }
        #endregion
    }
}
