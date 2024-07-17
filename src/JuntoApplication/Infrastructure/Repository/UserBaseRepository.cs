using AutoMapper;
using JuntoApplication.Dto;
using JuntoApplication.Infrastructure.DataBase;
using JuntoApplication.Infrastructure.Repository.IRepository;
using JuntoApplication.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JuntoApplication.Infrastructure.Repository
{
    public class UserBaseRepository : IUserBaseRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;

        public UserBaseRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager,
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
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
            try
            {
                if (userDto.Role != null)
                {
                    var userAdmin = _mapper.Map<Admin>(userDto);
                    var adminUser = new IdentityUser { UserName = userDto.Email, Email = userDto.Email };
                    var result = await _userManager.CreateAsync(adminUser, userDto.Password);

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new Exception($"Failed to reset password: {errors}");
                    }
                    
                    await _userManager.AddToRoleAsync(adminUser, "Admin");

                    var adminClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, userDto.Email),
                        new Claim(ClaimTypes.Role, "Admin"),
                    };

                    await _userManager.AddClaimsAsync(adminUser, adminClaims);

                    _context.Add(userAdmin);
                    await _context.SaveChangesAsync();

                    return _mapper.Map<UserDto>(userAdmin);
                }

                var user = _mapper.Map<User>(userDto);

                _context.Add(user);
                await _context.SaveChangesAsync();

                return _mapper.Map<UserDto>(user);
            }
            catch (Exception e)
            {
                throw new Exception($"Somenthing went wrong when calling API. Error: {e.Message} ");
            }
        }

        public async Task<UserDto> UpdateAsync(long id, string role, string password)
        {
            try
            {
                if (role == "Admin")
                {
                    var userAdmin = await _context.Admins.Where(x => x.Id == id).FirstOrDefaultAsync();
                    var adminUser = await _userManager.FindByEmailAsync(userAdmin.Email);

                    if (adminUser == null)
                        throw new Exception("Admin user not found in Identity");

                    var token = await _userManager.GeneratePasswordResetTokenAsync(adminUser);
                    var result = await _userManager.ResetPasswordAsync(adminUser, token, password);

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new Exception($"Failed to reset password: {errors}");
                    }

                    return _mapper.Map<UserDto>(userAdmin);
                }

                var user = await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();

                _context.Update(user);
                await _context.SaveChangesAsync();

                return _mapper.Map<UserDto>(user);
            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong when calling API. Error: {e.Message}");
            }
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
