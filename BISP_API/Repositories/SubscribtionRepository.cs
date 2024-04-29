using BISP_API.Context;
using BISP_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BISP_API.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly BISPdbContext _context;

        public SubscriptionRepository(BISPdbContext context)
        {
            _context = context;
        }

        public async Task<Subscriber> CreateAsync(Subscriber subscription)
        {
            await _context.Subscribers.AddAsync(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task DeleteAsync(Subscriber subscription)
        {
            _context.Subscribers.Remove(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Subscriber>> GetAsync()
        {
            return await _context.Subscribers.ToListAsync();
        }

        public async Task<Subscriber> GetByCustomerIdAsync(string id)
        {
            return await _context.Subscribers.SingleOrDefaultAsync(x => x.CustomerId == id);
        }

        public Task GetByCustomerIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Subscriber> GetByIdAsync(string id)
        {
            return await _context.Subscribers.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Subscriber> UpdateAsync(Subscriber subscription)
        {
            _context.Subscribers.UpdateRange(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

    }
}
