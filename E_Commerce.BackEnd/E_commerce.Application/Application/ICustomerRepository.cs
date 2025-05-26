using E_commerce.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace E_commerce.Application.Application
{
    public interface ICustomerRepository : IRepository<_Customer>
    {
        public Task<_Customer> IsCustomerIdExists(string id);
        public Task<object> UpdateCustomerRank(string id, int rank_id);
    } 
}