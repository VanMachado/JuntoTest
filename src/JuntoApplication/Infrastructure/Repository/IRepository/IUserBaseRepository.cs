using JuntoApplication.Dto;

namespace JuntoApplication.Infrastructure.Repository.IRepository
{
    public interface IUserBaseRepository
    {
        Task<string> TokenGeneration(string userName, string password);
        Task<IEnumerable<UserDto>> FindAllAsync();
        Task<UserDto> FindByIdAsync(long id, string role);        
        Task<UserDto> CreateAsync(UserDto user);
        Task<UserDto> UpdateAsync(long id, string role, string password);
        Task<bool> DeleteAsync(long id, string role);
    }
}
