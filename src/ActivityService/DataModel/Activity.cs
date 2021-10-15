namespace ActivityService.DataModel
{
    using System;

    public class Activity
    {
        public DateTime ActivityTime { get; }

        public int ActivityEvents { get; }

        public string ActivityName { get; }

        public Activity(string activityName, DateTime activityTime, int activityEvents)
        {
            ActivityName = activityName;
            ActivityTime = activityTime;
            ActivityEvents = activityEvents;
        }
    }
}
