using ActivityService.DataModel;
using System.Threading.Tasks;

namespace ActivityService.Repository
{
    public interface IActivityRepo
    {
        public Task AddActivity(Activity activity);

        public Task<int> GetActivityTotal(string activityName);
    }
}
