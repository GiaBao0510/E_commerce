using Microsoft.AspNetCore.JsonPatch;

namespace E_commerce.Application.Application
{
    public interface IRepository<T> where T : class
    {
        // define the CRUD methods
        Task<IReadOnlyList<T>> GetAllAsync();   //GET
        Task<T> GetByIdAsync(string id);        //GET
        Task<string> AddAsync(T entity);        //POST
        Task<string> UpdateAsync(T entity);     //PUT
        Task<string> DeleteAsync(string id);   //DELETE
        Task<string> PatchAsync(string id, JsonPatchDocument<T> patchDoc);   //PATCH
        
    }
}