namespace ChatApp.Services
{
    public interface IUserService
    {
        public Task<Tuple<string, string>> GetUserName(string userId);
    }
}
