using AutoMapper;
using JuntoApplication.Dto;
using JuntoApplication.Infrastructure.DataBase;
using JuntoApplication.Infrastructure.Repository.IRepository;
using JuntoApplication.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JuntoApplication.Infrastructure.Repository
{
    public class UserBaseRepository : IUserBaseRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserBaseRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<string> TokenGeneration(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserDto>> FindAllAsync()
        {
            var admins = await _context.Admins.ToListAsync();
            var users = await _context.Users.ToListAsync();
            var combinedList = admins.Cast<BaseUser>().Concat(users.Cast<BaseUser>()).ToList();

            var result = combinedList.Select(user => new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                UserType = user is Admin ? "Admin" : "User"
            }).ToList();

            return result;
        }

        public async Task<UserDto> FindByIdAsync(long id, string role)
        {
            var userRole = role.ToLower();
            BaseUser user = userRole == "admin" ? await _context.Admins.Where(x => x.Id == id).FirstOrDefaultAsync() :
                await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();


            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateAsync(UserDto userDto)
        {
            if (userDto.Role != null)
            {
                var userAdmin = _mapper.Map<Admin>(userDto);

                _context.Add(userAdmin);
                await _context.SaveChangesAsync();

                return _mapper.Map<UserDto>(userAdmin);
            }

            var user = _mapper.Map<User>(userDto);

            _context.Add(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateAsync(long id, string role, string password)
        {
            var userRole = role.ToLower();
            BaseUser user = userRole == "admin" ? await _context.Admins.Where(x => x.Id == id).FirstOrDefaultAsync() :
                await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();

            user.Password = password;

            _context.Update(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> DeleteAsync(long id, string role)
        {
            try
            {
                var userRole = role.ToLower();
                BaseUser user = userRole == "admin" ? await _context.Admins.Where(x => x.Id == id).FirstOrDefaultAsync() :
                    await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();

                _context.Remove(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
