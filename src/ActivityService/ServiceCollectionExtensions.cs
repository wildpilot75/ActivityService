namespace ActivityService
{
    using ActivityService.Repository;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class ServiceCollectionExtensions
    {
        public static void AddImMemoryActivityRepo(this IServiceCollection services, IConfiguration configuration)
        {
            var activityHoldTime = TimeSpan.FromMinutes(configuration.GetValue<double>("HoldActivitiesTimeMinutes"));
            services.AddSingleton(typeof(IActivityRepo), new InMemoryActivityRepo(activityHoldTime));
        }
    }
}
