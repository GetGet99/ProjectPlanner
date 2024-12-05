using System.Threading.Tasks;

namespace Get.ProjectPlanner;

interface IAsyncProperty<T>
{
    public Task<T> GetAsync();
    public Task SetAsync(T value);
}
