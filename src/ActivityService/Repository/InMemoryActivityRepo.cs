namespace ActivityService.Repository
{
    using ActivityService.DataModel;
    using Serilog;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class InMemoryActivityRepo : IActivityRepo, IDisposable
    {
        readonly ConcurrentDictionary<string, ConcurrentBag<Activity>> _activities;
        readonly TimeSpan _activityHoldTime;
        readonly CancellationTokenSource _cancellationSource = new CancellationTokenSource();

        public InMemoryActivityRepo(TimeSpan activityHoldTime)
        {
            _activityHoldTime = activityHoldTime;
            _activities = new ConcurrentDictionary<string, ConcurrentBag<Activity>>();
            CleanRepo(_cancellationSource.Token).Start();
        }

        public Task AddActivity(Activity activity)
        {
            Log.Information($"Adding activity to repo activity name: {activity.ActivityName}");
            return Task.Run(() =>
            {
                if (_activities.TryGetValue(activity.ActivityName, out var activities))
                {
                    activities.Add(activity);
                    return;
                }

                _activities.TryAdd(activity.ActivityName, new ConcurrentBag<Activity> { activity });
            });
        }

        public Task<int> GetActivityTotal(string activityName)
        {
            Log.Information($"calculating total for activity: {activityName}");
            var activityTotal = -1;
            return Task.Run(() =>
            {
                if (_activities.TryGetValue(activityName, out var activities))
                {
                    activityTotal = 0;
                    foreach(var activity in activities)
                    {
                        activityTotal += activity.ActivityEvents;
                    }

                    return activityTotal;
                }

                return activityTotal;
            });
        }

        Task CleanRepo(CancellationToken cancellationToken)
        {
            return new Task(() =>
            {
                while(!cancellationToken.IsCancellationRequested)
                {
                    foreach(var activityCollection in _activities)
                    {
                        var activitiesToKeep = new List<Activity>();
                        foreach(var activity in activityCollection.Value)
                        {
                            if(activity.ActivityTime.Add(_activityHoldTime) > DateTime.Now)
                            {
                                activitiesToKeep.Add(activity);
                            }
                        }

                        if(activitiesToKeep.Count > 0)
                        {
                            _activities.TryUpdate(activityCollection.Key, new ConcurrentBag<Activity>(activitiesToKeep), activityCollection.Value);
                        }
                        else
                        {
                            activityCollection.Value.Clear();
                        }
                    }

                    Thread.Sleep(1000);
                }
            });
        }

        public void Dispose()
        {
            _cancellationSource.Cancel();
        }
    }
}
