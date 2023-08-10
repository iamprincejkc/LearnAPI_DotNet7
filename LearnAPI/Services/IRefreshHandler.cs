namespace LearnAPI.Services
{
    public interface IRefreshHandler
    {
        Task<string> RefreshTokenKey(string email);
    }
}
