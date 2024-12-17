using CommunityApiDemo.Models;

namespace CommunityApiDemo.Interfaces
{
    public interface ICategoryRepo
    {
        public Task<List<Category>> GetIDs();
    }
}
