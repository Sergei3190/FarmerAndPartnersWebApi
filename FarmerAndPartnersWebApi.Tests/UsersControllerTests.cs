using FarmerAndPartnersEF.Repository;
using FarmerAndPartnersModels;
using FarmerAndPartnersWebApi.Controllers;
using FarmerAndPartnersWebApi.Tests.TestData;
using FarmerAndPartnersWebApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FarmerAndPartnersWebApi.Tests
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task GetUsersAsyncOk_Test()
        {
            var mockRepo = new Mock<IAsyncRepository>();

            mockRepo.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(UserTestData.Users);

            var mapper = UserTestData.GetMapper();

            var usersController = new UsersController(mockRepo.Object, mapper);

            var result = await usersController.GetUsersAsync();

            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);

            var viewModel = Assert.IsAssignableFrom<IEnumerable<UserViewModel>>(okObjectResult.Value);

            Assert.Equal(mockRepo.Object.GetUsersAsync().Result.Count, viewModel.Count());
        }

        [Fact]
        public async Task GetUsersAsyncNotFound_Test()
        {
            var mockRepo = new Mock<IAsyncRepository>();

            mockRepo.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(null as List<User>);

            var mapper = UserTestData.GetMapper();

            var usersController = new UsersController(mockRepo.Object, mapper);

            var result = await usersController.GetUsersAsync();

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetUserAsyncOk_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.GetUserAsync(It.IsAny<int>())).ReturnsAsync(UserTestData.GetUser(UserTestData.GoodId));

            var mapper = UserTestData.GetMapper();

            var usersController = new UsersController(mock.Object, mapper);

            var result = await usersController.GetUserAsync(UserTestData.GoodId);

            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);

            var viewModel = Assert.IsAssignableFrom<UserViewModel>(okObjectResult.Value);

            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Id, viewModel.Id);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Name, viewModel.Name);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.TimeStamp, viewModel.TimeStamp);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Login, viewModel.Login);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Password, viewModel.Password);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.CompanyId, viewModel.CompanyId);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Company.Id, viewModel.Company.Id);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Company.Name, viewModel.Company.Name);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Company.TimeStamp, viewModel.Company.TimeStamp);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Company.ContractStatusId, viewModel.Company.ContractStatusId);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Company.ContractStatus.Id, viewModel.Company.ContractStatus.Id);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Company.ContractStatus.Definition, viewModel.Company.ContractStatus.Definition);
        }

        [Fact]
        public async Task GetUserAsyncNotFound_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.GetUserAsync(It.IsAny<int>())).ReturnsAsync(UserTestData.GetUser(UserTestData.BadId));

            var mapper = UserTestData.GetMapper();

            var usersController = new UsersController(mock.Object, mapper);

            var result = await usersController.GetUserAsync(UserTestData.BadId);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateUserAsyncOk_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.FindLoginUserAsync(It.IsAny<string>()))
                .ReturnsAsync(UserTestData.FindLoginUser(UserTestData.NewUserViewModel.Login));

            mock.Setup(repo => repo.GetIdLastUserAsync()).ReturnsAsync(UserTestData.GetIdLastUser());

            mock.Setup(repo => repo.AddUserAsync(It.IsAny<User>()))
                .ReturnsAsync(UserTestData.AddUser(new User
                {
                    Id = UserTestData.Users.LastOrDefault().Id + 1,
                    Name = UserTestData.NewUserViewModel.Name,
                    Login = UserTestData.NewUserViewModel.Login,
                    Password = UserTestData.NewUserViewModel.Password,
                    CompanyId = UserTestData.NewUserViewModel.CompanyId,
                }));

            mock.Setup(repo => repo.GetCompanyAsync(It.IsAny<int>()))
                .ReturnsAsync(UserTestData.GetCompany(UserTestData.NewUserViewModel.CompanyId));

            var mapper = UserTestData.GetMapper();

            var usersController = new UsersController(mock.Object, mapper);

            var result = await usersController.CreateUserAsync(UserTestData.NewUserViewModel);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);

            var viewModel = Assert.IsAssignableFrom<UserViewModel>(createdResult.Value);

            Assert.Equal(UserTestData.Users.LastOrDefault().Id, viewModel.Id);
            Assert.Equal(UserTestData.NewUserViewModel.Name, viewModel.Name);
            Assert.Equal(UserTestData.NewUserViewModel.Login, viewModel.Login);
            Assert.Equal(UserTestData.NewUserViewModel.Password, viewModel.Password);
            Assert.Equal(UserTestData.NewUserViewModel.CompanyId, viewModel.CompanyId);
            Assert.Equal(UserTestData.GetCompany(UserTestData.NewUserViewModel.CompanyId).Id, viewModel.Company.Id);
            Assert.Equal(UserTestData.GetCompany(UserTestData.NewUserViewModel.CompanyId).Name, viewModel.Company.Name);
            Assert.Equal(UserTestData.GetCompany(UserTestData.NewUserViewModel.CompanyId).TimeStamp, viewModel.Company.TimeStamp);
            Assert.Equal(UserTestData.GetCompany(UserTestData.NewUserViewModel.CompanyId).ContractStatusId, viewModel.Company.ContractStatusId);
            Assert.Equal(UserTestData.GetCompany(UserTestData.NewUserViewModel.CompanyId).ContractStatus.Id, viewModel.Company.ContractStatus.Id);
            Assert.Equal(UserTestData.GetCompany(UserTestData.NewUserViewModel.CompanyId).ContractStatus.Definition, viewModel.Company.ContractStatus.Definition);
        }

        [Fact]
        public async Task CreateUserAsyncBadRequest_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            var mapper = UserTestData.GetMapper();

            var usersController = new UsersController(mock.Object, mapper);

            var result = await usersController.CreateUserAsync(null);

            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task CreateUserAsyncBadRequestModelState_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.FindLoginUserAsync(It.IsAny<string>()))
                   .ReturnsAsync(UserTestData.FindLoginUser(UserTestData.BadLogin));

            var mapper = UserTestData.GetMapper();

            var usersController = new UsersController(mock.Object, mapper);

            var result = await usersController.CreateUserAsync(new UserViewModel { Login = UserTestData.BadLogin });

            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            var error = Assert.IsAssignableFrom<SerializableError>(badRequestObjectResult.Value);

            string[] messageError = (string[])error.Values.First();

            Assert.Equal(UserTestData.CustomError.Login, error.Keys.First());
            Assert.Equal(UserTestData.CustomError.Message, messageError[0]);
        }

        [Fact]
        public async Task CreateUserAsyncStatusCode500_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.AddUserAsync(It.IsAny<User>())).ReturnsAsync(UserTestData.AddUserStatusCode500(It.IsAny<User>()));

            var mapper = UserTestData.GetMapper();

            var usersController = new UsersController(mock.Object, mapper);

            var result = await usersController.CreateUserAsync(UserTestData.NewUserViewModel);

            Assert.IsType<StatusCodeResult>(result.Result);
        }

        [Fact]
        public async Task UpdateUserAsyncOk_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.CheckLoginUserAsync(It.IsAny<string>()))
                .ReturnsAsync(UserTestData.CheckLoginUser(UserTestData.EditUserViewModel.Login));

            mock.Setup(repo => repo.GetIdLastUserAsync()).ReturnsAsync(UserTestData.GetIdLastUser());

            mock.Setup(repo => repo.UpdateUserAsync(It.IsAny<User>()))
                .ReturnsAsync(UserTestData.AddUser(new User
                {
                    Id = UserTestData.EditUserViewModel.Id,
                    Name = UserTestData.EditUserViewModel.Name,
                    TimeStamp = UserTestData.EditUserViewModel.TimeStamp,
                    Login = UserTestData.EditUserViewModel.Login,
                    Password = UserTestData.EditUserViewModel.Password,
                    CompanyId = UserTestData.EditUserViewModel.CompanyId
                }));

            mock.Setup(repo => repo.GetCompanyAsync(It.IsAny<int>()))
                .ReturnsAsync(UserTestData.GetCompany(UserTestData.EditUserViewModel.CompanyId));

            var mapper = UserTestData.GetMapper();

            var usersController = new UsersController(mock.Object, mapper);

            var result = await usersController.UpdateUserAsync(UserTestData.EditUserViewModel);

            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);

            var viewModel = Assert.IsAssignableFrom<UserViewModel>(okObjectResult.Value);

            Assert.Equal(UserTestData.EditUserViewModel.Id, viewModel.Id);
            Assert.Equal(UserTestData.EditUserViewModel.Name, viewModel.Name);
            Assert.Equal(UserTestData.EditUserViewModel.TimeStamp, viewModel.TimeStamp);
            Assert.Equal(UserTestData.EditUserViewModel.Login, viewModel.Login);
            Assert.Equal(UserTestData.EditUserViewModel.Password, viewModel.Password);
            Assert.Equal(UserTestData.EditUserViewModel.CompanyId, viewModel.CompanyId);
            Assert.Equal(UserTestData.GetCompany(UserTestData.EditUserViewModel.CompanyId).Id, viewModel.Company.Id);
            Assert.Equal(UserTestData.GetCompany(UserTestData.EditUserViewModel.CompanyId).Name, viewModel.Company.Name);
            Assert.Equal(UserTestData.GetCompany(UserTestData.EditUserViewModel.CompanyId).TimeStamp, viewModel.Company.TimeStamp);
            Assert.Equal(UserTestData.GetCompany(UserTestData.EditUserViewModel.CompanyId).ContractStatusId, viewModel.Company.ContractStatusId);
            Assert.Equal(UserTestData.GetCompany(UserTestData.EditUserViewModel.CompanyId).ContractStatus.Id, viewModel.Company.ContractStatus.Id);
            Assert.Equal(UserTestData.GetCompany(UserTestData.EditUserViewModel.CompanyId).ContractStatus.Definition, viewModel.Company.ContractStatus.Definition);
        }

        [Fact]
        public async Task UpdateUserAsyncBadRequestModelState_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.CheckLoginUserAsync(It.IsAny<string>())).ReturnsAsync(UserTestData.CheckLoginUser(UserTestData.BadLogin));

            var mapper = UserTestData.GetMapper();

            var usersController = new UsersController(mock.Object, mapper);

            var result = await usersController.UpdateUserAsync(UserTestData.EditUserViewModel);

            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            var error = Assert.IsAssignableFrom<SerializableError>(badRequestObjectResult.Value);

            string[] messageError = (string[])error.Values.First();

            Assert.Equal(UserTestData.CustomError.Login, error.Keys.First());
            Assert.Equal(UserTestData.CustomError.Message, messageError[0]);
        }

        [Fact]
        public async Task UpdateUserAsyncConflict_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(UserTestData.UpdateConflictUser(new User
            {
                Id = UserTestData.ConflictEditUserViewModel.Id,
                Name = UserTestData.ConflictEditUserViewModel.Name,
                TimeStamp = UserTestData.ConflictEditUserViewModel.TimeStamp,
                Login = UserTestData.ConflictEditUserViewModel.Login,
                Password = UserTestData.ConflictEditUserViewModel.Password,
                CompanyId = UserTestData.ConflictEditUserViewModel.CompanyId
            }));

            var mapper = UserTestData.GetMapper();

            var usersController = new UsersController(mock.Object, mapper);

            var result = await usersController.UpdateUserAsync(UserTestData.ConflictEditUserViewModel);

            Assert.IsType<ConflictResult>(result.Result);
        }

        [Fact]
        public async Task DeleteUserAsyncOk_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.GetUserAsync(It.IsAny<int>())).ReturnsAsync(UserTestData.GetUser(UserTestData.GoodId));

            mock.Setup(repo => repo.DeleteUserAsync(It.IsAny<User>()))
                .ReturnsAsync(UserTestData.DeleteUser(mock.Object.GetUserAsync(UserTestData.GoodId).Result));

            var mapper = UserTestData.GetMapper();

            var usersController = new UsersController(mock.Object, mapper);

            var result = await usersController.DeleteUserAsync(UserTestData.GoodId);

            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);

            var viewModel = Assert.IsAssignableFrom<UserViewModel>(okObjectResult.Value);

            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Id, viewModel.Id);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Name, viewModel.Name);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.TimeStamp, viewModel.TimeStamp);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Login, viewModel.Login);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Password, viewModel.Password);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.CompanyId, viewModel.CompanyId);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Company.Id, viewModel.Company.Id);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Company.Name, viewModel.Company.Name);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Company.TimeStamp, viewModel.Company.TimeStamp);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Company.ContractStatusId, viewModel.Company.ContractStatusId);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Company.ContractStatus.Id, viewModel.Company.ContractStatus.Id);
            Assert.Equal(mock.Object.GetUserAsync(UserTestData.GoodId).Result.Company.ContractStatus.Definition, viewModel.Company.ContractStatus.Definition);
        }

        [Fact]
        public async Task DeleteUserAsyncConflict_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.GetUserAsync(It.IsAny<int>())).ReturnsAsync(UserTestData.GetUser(UserTestData.ConflictId));

            mock.Setup(repo => repo.DeleteUserAsync(It.IsAny<User>()))
                .ReturnsAsync(UserTestData.DeleteConflictUser(mock.Object.GetUserAsync(UserTestData.ConflictId).Result));

            var mapper = UserTestData.GetMapper();

            var usersController = new UsersController(mock.Object, mapper);

            var result = await usersController.DeleteUserAsync(UserTestData.ConflictId);

            Assert.IsType<ConflictResult>(result.Result);
        }
    }
}
