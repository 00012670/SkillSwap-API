using BISP_API.Models;

namespace BISP_API.Repositories
{
    public interface ISubscriptionRepository
    {
        Task<Subscriber> UpdateAsync(Subscriber subscription);
        Task<IEnumerable<Subscriber>> GetAsync();
        Task<Subscriber> GetByIdAsync(string id);
        Task<Subscriber> GetByCustomerIdAsync(string id);
        Task<Subscriber> CreateAsync(Subscriber subscription);
        Task DeleteAsync(Subscriber subscription);
        Task GetByCustomerIdAsync(int userId);
    }
}
