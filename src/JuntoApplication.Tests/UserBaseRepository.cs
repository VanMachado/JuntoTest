using AutoMapper;
using JuntoApplication.Dto;
using JuntoApplication.Infrastructure.DataBase;
using JuntoApplication.Infrastructure.Repository;
using JuntoApplication.Model;
using Microsoft.EntityFrameworkCore;

namespace JuntoApplication.Tests
{
    public class UserBaseRepositoryTests
    {
        private readonly IMapper _mapper;

        public UserBaseRepositoryTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BaseUser, UserDto>();
                cfg.CreateMap<Admin, UserDto>();
                cfg.CreateMap<User, UserDto>();
                cfg.CreateMap<UserDto, Admin>();
                cfg.CreateMap<UserDto, User>();
            });

            _mapper = config.CreateMapper();
        }

        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task FindAllAsync_ReturnsAllUsers()
        {            
            using var context = GetInMemoryDbContext();
            var userBaseRepository = new UserBaseRepository(context, _mapper);

            var admin = new Admin(1, "Admin1", "admin1@example.com", "password", Model.Enums.Role.Admin);
            var user = new User(2, "User1", "user1@example.com", "password", "12345678900");

            await context.Admins.AddAsync(admin);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
                     
            var result = await userBaseRepository.FindAllAsync();
                        
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task FindByIdAsync_ReturnsAdminById()
        {            
            using var context = GetInMemoryDbContext();
            var userBaseRepository = new UserBaseRepository(context, _mapper);

            var admin = new Admin(1, "Admin1", "admin1@example.com", "password", Model.Enums.Role.Admin);
            await context.Admins.AddAsync(admin);
            await context.SaveChangesAsync();
                     
            var result = await userBaseRepository.FindByIdAsync(1, "admin");
                        
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Admin1", result.Name);
            Assert.Equal("admin1@example.com", result.Email);
        }

        [Fact]
        public async Task FindByIdAsync_ReturnsUserById()
        {            
            using var context = GetInMemoryDbContext();
            var userBaseRepository = new UserBaseRepository(context, _mapper);

            var user = new User(2, "User1", "user1@example.com", "password", "12345678900");
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            
            var result = await userBaseRepository.FindByIdAsync(2, "user");
            
            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("User1", result.Name);
            Assert.Equal("user1@example.com", result.Email);
        }

        [Fact]
        public async Task CreateAsync_CreatesAdmin()
        {            
            using var context = GetInMemoryDbContext();
            var userBaseRepository = new UserBaseRepository(context, _mapper);

            var userDto = new UserDto
            {
                Id = 1,
                Name = "Admin1",
                Email = "admin1@example.com",
                Password = "password",
                Role = Dto.EnumsDto.RoleDto.Admin
            };
                     
            var result = await userBaseRepository.CreateAsync(userDto);
            
            Assert.NotNull(result);
            Assert.Equal("Admin1", result.Name);
            Assert.Equal("admin1@example.com", result.Email);

            var admin = await context.Admins.FindAsync((long)result.Id);
            Assert.NotNull(admin);
            Assert.Equal("Admin1", admin.Name);
        }

        [Fact]
        public async Task CreateAsync_CreatesUser()
        {            
            using var context = GetInMemoryDbContext();
            var userBaseRepository = new UserBaseRepository(context, _mapper);

            var userDto = new UserDto
            {
                Id = 2,
                Name = "User1",
                Email = "user1@example.com",
                Password = "password",
                CPF = "12345678900"
            };
                     
            var result = await userBaseRepository.CreateAsync(userDto);
                        
            Assert.NotNull(result);
            Assert.Equal("User1", result.Name);
            Assert.Equal("user1@example.com", result.Email);

            var user = await context.Users.FindAsync((long)result.Id);
            Assert.NotNull(user);
            Assert.Equal("User1", user.Name);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesPassword()
        {            
            using var context = GetInMemoryDbContext();
            var userBaseRepository = new UserBaseRepository(context, _mapper);

            var admin = new Admin(1, "Admin1", "admin1@example.com", "password", Model.Enums.Role.Admin);
            await context.Admins.AddAsync(admin);
            await context.SaveChangesAsync();

            var newPassword = "newpassword";
                     
            var result = await userBaseRepository.UpdateAsync(1, "admin", newPassword);
                        
            Assert.NotNull(result);
            Assert.Equal(newPassword, result.Password);

            var updatedAdmin = await context.Admins.FindAsync((long)1);
            Assert.Equal(newPassword, updatedAdmin.Password);
        }

        [Fact]
        public async Task DeleteAsync_DeletesUser()
        {            
            using var context = GetInMemoryDbContext();
            var userBaseRepository = new UserBaseRepository(context, _mapper);

            var user = new User(2, "User1", "user1@example.com", "password", "12345678900");
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
                        
            var result = await userBaseRepository.DeleteAsync(2, "user");
                        
            Assert.True(result);

            var deletedUser = await context.Users.FindAsync((long)2);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task DeleteAsync_DeletesAdmin()
        {            
            using var context = GetInMemoryDbContext();
            var userBaseRepository = new UserBaseRepository(context, _mapper);

            var admin = new Admin(1, "Admin1", "admin1@example.com", "password", Model.Enums.Role.Admin);
            await context.Admins.AddAsync(admin);
            await context.SaveChangesAsync();
                     
            var result = await userBaseRepository.DeleteAsync(1, "admin");
                        
            Assert.True(result);

            var deletedAdmin = await context.Admins.FindAsync((long)1);
            Assert.Null(deletedAdmin);
        }
    }
}
