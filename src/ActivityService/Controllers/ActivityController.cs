namespace ActivityService.Controllers
{
    using ActivityService.DataModel;
    using ActivityService.DataTransferObjects;
    using ActivityService.Repository;
    using Microsoft.AspNetCore.Mvc;
    using Serilog;
    using System;
    using System.Threading.Tasks;

    [ApiController]
    public class ActivityController : ControllerBase
    {
        readonly IActivityRepo _activityRepo;

        public ActivityController(IActivityRepo activityRepo)
        {
            _activityRepo = activityRepo ?? throw new ArgumentNullException("Activity Repo cannot be null");
        }

        [HttpPost("{activityName:alpha}")]
        [Route("[controller]/{activityName}")]
        public async Task<IActionResult> AddActivity(string activityName, [FromBody] ActionValue value)
        {
            var activity = new Activity(activityName, DateTime.Now, value.Value);
            await _activityRepo.AddActivity(activity);
            Log.Information($"Activity Added Name={activity.ActivityName}, Events={activity.ActivityEvents}, Time={activity.ActivityTime}");

            return Ok();
        }

        [HttpGet("{activityName:alpha}")]
        [Route("[controller]/{activityName}/total")]
        public async Task<IActionResult> GetTotal(string activityName)
        {
            Log.Information($"Total requested for Activity Name={activityName}");
            var total = await _activityRepo.GetActivityTotal(activityName);
            if (total == -1)
                return new NoContentResult();

            return Ok(total);
        }
    }
}
