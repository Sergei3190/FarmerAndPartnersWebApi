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
    public class CompaniesControllerTests
    {
        [Fact]
        public async Task GetCompaniesAsyncOk_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.GetCompaniesWithoutUsersAsync()).ReturnsAsync(CompanyTestData.Companies);

            var mapper = CompanyTestData.GetMapper();

            var companiesController = new CompaniesController(mock.Object, mapper);

            var result = await companiesController.GetCompaniesAsync();

            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);

            var viewModel = Assert.IsAssignableFrom<IEnumerable<CompanyViewModel>>(okObjectResult.Value);

            Assert.Equal(mock.Object.GetCompaniesWithoutUsersAsync().Result.Count, viewModel.Count());
        }

        [Fact]
        public async Task GetCompaniesAsyncNotFound_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.GetCompaniesWithoutUsersAsync()).ReturnsAsync(null as List<Company>);

            var mapper = CompanyTestData.GetMapper();

            var companiesController = new CompaniesController(mock.Object, mapper);

            var result = await companiesController.GetCompaniesAsync();

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCompanyAsyncOk_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.GetCompanyAsync(It.IsAny<int>())).ReturnsAsync(CompanyTestData.GetCompany(CompanyTestData.GoodId));

            var mapper = CompanyTestData.GetMapper();

            var companiesController = new CompaniesController(mock.Object, mapper);

            var result = await companiesController.GetCompanyAsync(CompanyTestData.GoodId);

            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);

            var viewModel = Assert.IsAssignableFrom<CompanyViewModel>(okObjectResult.Value);

            Assert.Equal(mock.Object.GetCompanyAsync(CompanyTestData.GoodId).Result.Id, viewModel.Id);
            Assert.Equal(mock.Object.GetCompanyAsync(CompanyTestData.GoodId).Result.Name, viewModel.Name);
            Assert.Equal(mock.Object.GetCompanyAsync(CompanyTestData.GoodId).Result.TimeStamp, viewModel.TimeStamp);
            Assert.Equal(mock.Object.GetCompanyAsync(CompanyTestData.GoodId).Result.ContractStatusId, viewModel.ContractStatusId);
            Assert.Equal(mock.Object.GetCompanyAsync(CompanyTestData.GoodId).Result.ContractStatus.Id, viewModel.ContractStatus.Id);
            Assert.Equal(mock.Object.GetCompanyAsync(CompanyTestData.GoodId).Result.ContractStatus.Definition, viewModel.ContractStatus.Definition);
        }

        [Fact]
        public async Task GetCompanyAsyncNotFound_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.GetCompanyAsync(It.IsAny<int>())).ReturnsAsync(CompanyTestData.GetCompany(CompanyTestData.BadId));

            var mapper = CompanyTestData.GetMapper();

            var companiesController = new CompaniesController(mock.Object, mapper);

            var result = await companiesController.GetCompanyAsync(CompanyTestData.BadId);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateCompanyAsyncOk_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.FindNameCompanyAsync(It.IsAny<string>()))
                .ReturnsAsync(CompanyTestData.FindNameCompany(CompanyTestData.NewCompanyViewModel.Name));

            mock.Setup(repo => repo.GetIdLastCompanyAsync()).ReturnsAsync(CompanyTestData.GetIdLastCompany());

            mock.Setup(repo => repo.AddCompanyAsync(It.IsAny<Company>()))
                .ReturnsAsync(CompanyTestData.AddCompany(new Company
                {
                    Id = CompanyTestData.Companies.LastOrDefault().Id + 1,
                    Name = CompanyTestData.NewCompanyViewModel.Name,
                    ContractStatusId = CompanyTestData.NewCompanyViewModel.ContractStatusId,
                }));

            mock.Setup(repo => repo.GetContractStatusAsync(It.IsAny<int>()))
                .ReturnsAsync(CompanyTestData.GetContractStatus(CompanyTestData.NewCompanyViewModel.ContractStatusId));

            var mapper = CompanyTestData.GetMapper();

            var companiesController = new CompaniesController(mock.Object, mapper);

            var result = await companiesController.CreateCompanyAsync(CompanyTestData.NewCompanyViewModel);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);

            var viewModel = Assert.IsAssignableFrom<CompanyViewModel>(createdResult.Value);

            Assert.Equal(CompanyTestData.Companies.LastOrDefault().Id, viewModel.Id);
            Assert.Equal(CompanyTestData.NewCompanyViewModel.Name, viewModel.Name);
            Assert.Equal(CompanyTestData.NewCompanyViewModel.ContractStatusId, viewModel.ContractStatusId);
            Assert.Equal(CompanyTestData.GetContractStatus(CompanyTestData.NewCompanyViewModel.ContractStatusId).Id, viewModel.ContractStatus.Id);
            Assert.Equal(CompanyTestData.GetContractStatus(CompanyTestData.NewCompanyViewModel.ContractStatusId).Definition, viewModel.ContractStatus.Definition);
        }

        [Fact]
        public async Task CreateCompanyAsyncBadRequest_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            var mapper = CompanyTestData.GetMapper();

            var companiesController = new CompaniesController(mock.Object, mapper);

            var result = await companiesController.CreateCompanyAsync(null);

            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task CreateCompanyAsyncBadRequestModelState_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.FindNameCompanyAsync(It.IsAny<string>()))
               .ReturnsAsync(CompanyTestData.FindNameCompany(CompanyTestData.BadName));

            var mapper = CompanyTestData.GetMapper();

            var companiesController = new CompaniesController(mock.Object, mapper);

            var result = await companiesController.CreateCompanyAsync(new CompanyViewModel { Name = CompanyTestData.BadName });

            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            var error = Assert.IsAssignableFrom<SerializableError>(badRequestObjectResult.Value);

            string[] messageError = (string[])error.Values.First();

            Assert.Equal(CompanyTestData.CustomError.Name, error.Keys.First());
            Assert.Equal(CompanyTestData.CustomError.Message, messageError[0]);
        }

        [Fact]
        public async Task CreateCompanyAsyncStatusCode500_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.AddCompanyAsync(It.IsAny<Company>())).ReturnsAsync(CompanyTestData.AddCompanyStatusCode500(It.IsAny<Company>()));

            var mapper = CompanyTestData.GetMapper();

            var companiesController = new CompaniesController(mock.Object, mapper);

            var result = await companiesController.CreateCompanyAsync(CompanyTestData.NewCompanyViewModel);

            Assert.IsType<StatusCodeResult>(result.Result);
        }

        [Fact]
        public async Task UpdateCompanyAsyncOk_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.CheckNameCompanyAsync(It.IsAny<string>()))
                .ReturnsAsync(CompanyTestData.CheckNameCompany(CompanyTestData.EditCompanyViewModel.Name));

            mock.Setup(repo => repo.UpdateCompanyAsync(It.IsAny<Company>()))
                .ReturnsAsync(CompanyTestData.UpdateCompany(new Company
                {
                    Id = CompanyTestData.EditCompanyViewModel.Id,
                    Name = CompanyTestData.EditCompanyViewModel.Name,
                    TimeStamp = CompanyTestData.EditCompanyViewModel.TimeStamp,
                    ContractStatusId = CompanyTestData.EditCompanyViewModel.ContractStatusId,
                }));

            mock.Setup(repo => repo.GetContractStatusAsync(It.IsAny<int>()))
                .ReturnsAsync(CompanyTestData.GetContractStatus(CompanyTestData.EditCompanyViewModel.ContractStatusId));

            var mapper = CompanyTestData.GetMapper();

            var companiesController = new CompaniesController(mock.Object, mapper);

            var result = await companiesController.UpdateCompanyAsync(CompanyTestData.EditCompanyViewModel);

            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);

            var viewModel = Assert.IsAssignableFrom<CompanyViewModel>(okObjectResult.Value);

            Assert.Equal(CompanyTestData.EditCompanyViewModel.Id, viewModel.Id);
            Assert.Equal(CompanyTestData.EditCompanyViewModel.Name, viewModel.Name);
            Assert.Equal(CompanyTestData.EditCompanyViewModel.TimeStamp, viewModel.TimeStamp);
            Assert.Equal(CompanyTestData.EditCompanyViewModel.ContractStatusId, viewModel.ContractStatusId);
            Assert.Equal(CompanyTestData.GetContractStatus(CompanyTestData.EditCompanyViewModel.ContractStatusId).Id, viewModel.ContractStatus.Id);
            Assert.Equal(CompanyTestData.GetContractStatus(CompanyTestData.EditCompanyViewModel.ContractStatusId).Definition, viewModel.ContractStatus.Definition);
        }

        [Fact]
        public async Task UpdateCompanyAsyncBadRequestModelState_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.CheckNameCompanyAsync(It.IsAny<string>()))
               .ReturnsAsync(CompanyTestData.CheckNameCompany(CompanyTestData.BadName));

            var mapper = CompanyTestData.GetMapper();

            var companiesController = new CompaniesController(mock.Object, mapper);

            var result = await companiesController.UpdateCompanyAsync(CompanyTestData.EditCompanyViewModel);

            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            var error = Assert.IsAssignableFrom<SerializableError>(badRequestObjectResult.Value);

            string[] messageError = (string[])error.Values.First();

            Assert.Equal(CompanyTestData.CustomError.Name, error.Keys.First());
            Assert.Equal(CompanyTestData.CustomError.Message, messageError[0]);
        }

        [Fact]
        public async Task UpdateCompanyAsyncConflict_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.UpdateCompanyAsync(It.IsAny<Company>())).ReturnsAsync(CompanyTestData.UpdateConflictCompany(new Company
            {
                Id = CompanyTestData.ConflictEditCompanyViewModel.Id,
                Name = CompanyTestData.ConflictEditCompanyViewModel.Name,
                TimeStamp = CompanyTestData.ConflictEditCompanyViewModel.TimeStamp,
                ContractStatusId = CompanyTestData.ConflictEditCompanyViewModel.ContractStatusId
            }));

            var mapper = CompanyTestData.GetMapper();

            var companiesController = new CompaniesController(mock.Object, mapper);

            var result = await companiesController.UpdateCompanyAsync(CompanyTestData.ConflictEditCompanyViewModel);

            Assert.IsType<ConflictResult>(result.Result);
        }

        [Fact]
        public async Task DeleteCompanyAsyncOk_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.GetCompanyAsync(It.IsAny<int>())).ReturnsAsync(CompanyTestData.GetCompany(CompanyTestData.GoodId));

            mock.Setup(repo => repo.DeleteCompanyAsync(It.IsAny<Company>()))
               .ReturnsAsync(CompanyTestData.DeleteCompany(mock.Object.GetCompanyAsync(CompanyTestData.GoodId).Result));

            var mapper = CompanyTestData.GetMapper();

            var companiesController = new CompaniesController(mock.Object, mapper);

            var result = await companiesController.DeleteCompanyAsync(CompanyTestData.GoodId);

            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);

            var viewModel = Assert.IsAssignableFrom<CompanyViewModel>(okObjectResult.Value);

            Assert.Equal(mock.Object.GetCompanyAsync(CompanyTestData.GoodId).Result.Id, viewModel.Id);
            Assert.Equal(mock.Object.GetCompanyAsync(CompanyTestData.GoodId).Result.Name, viewModel.Name);
            Assert.Equal(mock.Object.GetCompanyAsync(CompanyTestData.GoodId).Result.TimeStamp, viewModel.TimeStamp);
            Assert.Equal(mock.Object.GetCompanyAsync(CompanyTestData.GoodId).Result.ContractStatusId, viewModel.ContractStatusId);
            Assert.Equal(mock.Object.GetCompanyAsync(CompanyTestData.GoodId).Result.ContractStatus.Id, viewModel.ContractStatus.Id);
            Assert.Equal(mock.Object.GetCompanyAsync(CompanyTestData.GoodId).Result.ContractStatus.Definition, viewModel.ContractStatus.Definition);
        }

        [Fact]
        public async Task DeleteCompanyAsyncConflict_Test()
        {
            var mock = new Mock<IAsyncRepository>();

            mock.Setup(repo => repo.GetCompanyAsync(It.IsAny<int>())).ReturnsAsync(CompanyTestData.GetCompany(CompanyTestData.ConflictId));

            mock.Setup(repo => repo.DeleteCompanyAsync(It.IsAny<Company>()))
                .ReturnsAsync(CompanyTestData.DeleteConflictCompany(mock.Object.GetCompanyAsync(CompanyTestData.ConflictId).Result));

            var mapper = CompanyTestData.GetMapper();

            var companiesController = new CompaniesController(mock.Object, mapper);

            var result = await companiesController.DeleteCompanyAsync(CompanyTestData.ConflictId);

            Assert.IsType<ConflictResult>(result.Result);
        }
    }
}
