using AutoMapper;
using JuntoApplication.Dto;
using JuntoApplication.Dto.EnumsDto;
using JuntoApplication.Infrastructure.DataBase;
using JuntoApplication.Infrastructure.Repository;
using JuntoApplication.Model;
using JuntoApplication.Model.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Xunit;
using Moq;

namespace JuntoApplication.Tests
{
    public class UserBaseRepositoryTests
    {       
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly UserBaseRepository _userBaseRepository;
        private readonly UserManager<IdentityUser> _userManager;
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

            var store = new Mock<IUserStore<IdentityUser>>();
            store.As<IUserPasswordStore<IdentityUser>>();            
            _userManager = new UserManager<IdentityUser>(store.Object, null, null, null, null, null, null, null, null);
            _userManagerMock = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            var user = new IdentityUser { Id = "1", UserName = "admin1@example.com", Email = "admin1@example.com" };
            _userManagerMock.Setup(s => s.FindByIdAsync("1")).ReturnsAsync(user);
            
            var context = GetInMemoryDbContext();
            _userBaseRepository = new UserBaseRepository(context, _userManagerMock.Object, _mapper);

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
            var userBaseRepository = new UserBaseRepository(context, _userManager, _mapper);

            var admin = new Admin(1, "Admin1", "admin1@example.com", Role.Admin);
            var user = new User(2, "User1", "user1@example.com", "12345678900");

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
            var userBaseRepository = new UserBaseRepository(context, _userManager, _mapper);

            var admin = new Admin(1, "Admin1", "admin1@example.com", Role.Admin);
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
            var userBaseRepository = new UserBaseRepository(context, _userManager, _mapper);

            var user = new User(2, "User1", "user1@example.com", "12345678900");
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var result = await userBaseRepository.FindByIdAsync(2, "user");

            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("User1", result.Name);
            Assert.Equal("user1@example.com", result.Email);
        }

        [Fact]
        public async Task CreateAsync_CreatesUser()
        {
            using var context = GetInMemoryDbContext();
            var userBaseRepository = new UserBaseRepository(context, _userManager, _mapper);

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
        public async Task DeleteAsync_DeletesAdmin()
        {
            using var context = GetInMemoryDbContext();
            var userBaseRepository = new UserBaseRepository(context, _userManager, _mapper);

            var admin = new Admin(1L, "Admin1", "admin1@example.com", Role.Admin);
            await context.Admins.AddAsync(admin);
            await context.SaveChangesAsync();

            var result = await userBaseRepository.DeleteAsync(1L, "admin");

            Assert.True(result);

            var deletedAdmin = await context.Admins.FindAsync(1L);
            Assert.Null(deletedAdmin);
        }

        [Fact]
        public async Task DeleteAsync_DeletesUser()
        {
            using var context = GetInMemoryDbContext();
            var userBaseRepository = new UserBaseRepository(context, _userManager, _mapper);

            var user = new User(2L, "User1", "user1@example.com", "12345678900");
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var result = await userBaseRepository.DeleteAsync(2L, "user");

            Assert.True(result);

            var deletedUser = await context.Users.FindAsync(2L);
            Assert.Null(deletedUser);
        }
    }
}
