using ActivityService.DataModel;
using ActivityService.Repository;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ActivityServiceTests
{
    public class InMemoryActivityRepoTests
    {
        readonly IActivityRepo _repo;
        public InMemoryActivityRepoTests()
        {
            _repo = new InMemoryActivityRepo(TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task can_add_activity_and_get_total()
        {
            var activity = new Activity("Activity1", DateTime.Now, 10);
            await _repo.AddActivity(activity);
            var total = _repo.GetActivityTotal(activity.ActivityName).GetAwaiter().GetResult();
            total.Should().Be(activity.ActivityEvents);
        }

        [Fact]
        public async Task can_add_multiple_activities_and_get_total()
        {
            var activity1 = new Activity("Activity1", DateTime.Now, 10);
            var activity2 = new Activity("Activity1", DateTime.Now, 20);
            var activity3 = new Activity("Activity2", DateTime.Now, 5);

            await _repo.AddActivity(activity1);
            await _repo.AddActivity(activity2);
            await _repo.AddActivity(activity3);

            var total1 = _repo.GetActivityTotal(activity1.ActivityName).GetAwaiter().GetResult();
            var total2 = _repo.GetActivityTotal(activity3.ActivityName).GetAwaiter().GetResult();

            total1.Should().Be(activity1.ActivityEvents + activity2.ActivityEvents);
            total2.Should().Be(activity3.ActivityEvents);
        }

        [Fact]
        public void unknown_activity_total_should_be_negative()
        {
            var total = _repo.GetActivityTotal("Activity").GetAwaiter().GetResult();
            total.Should().Be(-1);
        }

        [Fact]
        public async Task old_activities_should_be_removed()
        {
            var activity1 = new Activity("Activity1", DateTime.Now, 10);

            await _repo.AddActivity(activity1);

            var total1 = _repo.GetActivityTotal(activity1.ActivityName).GetAwaiter().GetResult();
            total1.Should().Be(activity1.ActivityEvents);

            Thread.Sleep(6000);
            var total2 = _repo.GetActivityTotal(activity1.ActivityName).GetAwaiter().GetResult();
            total2.Should().Be(0);
        }
    }
}
