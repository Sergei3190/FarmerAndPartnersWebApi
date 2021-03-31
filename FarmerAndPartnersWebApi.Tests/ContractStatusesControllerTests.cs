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
    public class ContractStatusesControllerTests
    {
        [Fact]
        public async Task GetContractStatusesAsyncOk_Test()
        {
            var mockRepo = new Mock<IAsyncRepository>();

            mockRepo.Setup(repo => repo.GetContractStatusesAsync()).ReturnsAsync(ContractStatusTestData.ContractStatuses);

            var mapper = ContractStatusTestData.GetMapper();

            var contractStatusController = new ContractStatusesController(mockRepo.Object, mapper);

            var result = await contractStatusController.GetContractStatusesAsync();

            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);

            var viewModel = Assert.IsAssignableFrom<IEnumerable<ContractStatusViewModel>>(okObjectResult.Value);

            Assert.Equal(mockRepo.Object.GetContractStatusesAsync().Result.Count, viewModel.Count());
        }

        [Fact]
        public async Task GetContractStatusesAsyncNotFound_Test()
        {
            var mockRepo = new Mock<IAsyncRepository>();

            mockRepo.Setup(repo => repo.GetContractStatusesAsync()).ReturnsAsync(null as List<ContractStatus>);

            var mapper = ContractStatusTestData.GetMapper();

            var contractStatusController = new ContractStatusesController(mockRepo.Object, mapper);

            var result = await contractStatusController.GetContractStatusesAsync();

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
