using LearnAPI.DB;
using LearnAPI.Model;
using LearnAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LearnAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly LearnAPIDbContext _context;
        private readonly JwtSettingsModel _jwtSettings;
        private readonly IRefreshHandler _refreshHandler;
        public AuthorizeController(LearnAPIDbContext context, IOptions<JwtSettingsModel> jwtSettings, IRefreshHandler refreshHandler)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
            _refreshHandler = refreshHandler;

        }
        [HttpPost("GenerateToken")]
        public async Task<IActionResult> GenerateToken([FromBody] UserCredModel userCred)
        {
            var user = await _context.TblUsers.FirstOrDefaultAsync(item => item.Email == userCred.email && item.Password == userCred.password);
            if (user != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_jwtSettings.securityKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Role, user.Role),
                    }),
                    Expires = DateTime.UtcNow.AddSeconds(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)

                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var finalToken = tokenHandler.WriteToken(token);
                return Ok(new TokenResponseModel() { Token = finalToken, RefreshToken = await _refreshHandler.RefreshTokenKey(userCred.email) });
            }
            else
            {
                return Unauthorized();
            }
        }



        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenResponseModel token)
        {
            var _refreshToken = await _context.TblTokens.FirstOrDefaultAsync(item => item.Refreshtoken == token.RefreshToken);
            if (_refreshToken != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_jwtSettings.securityKey);

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token.Token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out securityToken);

                if (securityToken is JwtSecurityToken jwtSecurityToken)
                {
                    var claims = jwtSecurityToken.Claims.ToArray();
                    string email = principal.Identity.Name;
                    var user = await _context.TblUsers.FirstOrDefaultAsync(item => item.Email == email);
                    var _existUser = await _context.TblTokens.FirstOrDefaultAsync(item => item.Userid == user.Code
                    && item.Refreshtoken == token.RefreshToken);
                    if (_existUser != null)
                    {
                        var _newToken = new JwtSecurityToken(
                            claims: claims,
                            expires: DateTime.Now.AddSeconds(30),
                            signingCredentials: new SigningCredentials(
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(_jwtSettings.securityKey)), SecurityAlgorithms.HmacSha256)
                            );


                        var _finalToken = tokenHandler.WriteToken(_newToken);

                        return Ok(new TokenResponseModel() { Token = _finalToken, RefreshToken = await _refreshHandler.RefreshTokenKey(email) });
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}
