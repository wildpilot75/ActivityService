namespace ActivityServiceTests
{
    using ActivityService.Controllers;
    using ActivityService.DataModel;
    using ActivityService.DataTransferObjects;
    using ActivityService.Repository;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class ActivityControllerTests
    {
        ActivityController _controller;
        public ActivityControllerTests()
        {
            var repo = new Mock<IActivityRepo>();
            repo.Setup(repo => repo.AddActivity(It.IsAny<Activity>()))
                .Returns(Task.CompletedTask);
            repo.Setup(repo => repo.GetActivityTotal(It.Is<string>(s => s == "Activity1")))
                .Returns(Task.FromResult(10));
            repo.Setup(repo => repo.GetActivityTotal(It.Is<string>(s => s == "Activity2")))
                .Returns(Task.FromResult(-1));

            _controller = new ActivityController(repo.Object);
        }

        [Fact]
        public void cannot_create_controller_with_null_repo()
        {
            Action a = () => new ActivityController(null);

            a.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task can_add_activity()
        {
            var okResult = await _controller.AddActivity("Activity", new ActionValue { Value = 10 });

            okResult.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task can_get_total()
        {
            var result = await _controller.GetTotal("Activity1");
            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(10);
        }

        [Fact]
        public async Task total_for_unknown_activity_should_return_no_content()
        {
            var result = await _controller.GetTotal("Activity2");
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
