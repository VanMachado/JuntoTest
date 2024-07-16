namespace JuntoApplication.Infrastructure.Repository.IRepository
{
    public interface ITokenRepository
    {
        Task<string> Authenticate(string username, string password);
    }
}
