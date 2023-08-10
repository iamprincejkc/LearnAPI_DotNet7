using LearnAPI.DB;
using LearnAPI.DB.Models;
using LearnAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace LearnAPI.Container
{
    public class RefreshHandler : IRefreshHandler
    {
        private readonly LearnAPIDbContext _context;
        public RefreshHandler(LearnAPIDbContext context)
        {
            _context = context;
        }
        public async Task<string> RefreshTokenKey(string email)
        {
            var randomNumber = new byte[32];
            using(var randomNumberGenerate = RandomNumberGenerator.Create())
            {
                randomNumberGenerate.GetBytes(randomNumber);
                string refreshToken = Convert.ToBase64String(randomNumber);

                var user = await _context.TblUsers.FirstOrDefaultAsync(user => user.Email == email);
                var existingToken = await _context.TblTokens.FirstOrDefaultAsync(item => item.Userid == user.Code);
                if (existingToken != null)
                {
                    existingToken.Refreshtoken = refreshToken;

                }
                else
                {
                    await _context.TblTokens.AddAsync(new TblToken
                    {
                       Userid= user.Code,
                       Tokenid= Guid.NewGuid().ToString(),
                       Refreshtoken=refreshToken
                    });
                }
                await _context.SaveChangesAsync();

                return refreshToken;
            }
        }
    }
}
