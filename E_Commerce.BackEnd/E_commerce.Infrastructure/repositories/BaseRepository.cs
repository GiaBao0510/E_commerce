using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using E_commerce.Application.Application;
using Microsoft.AspNetCore.JsonPatch;

namespace E_commerce.Infrastructure.repositories
{   
    //Lớp cơ sở Repository để chia sẻ connnection/transaction
    public abstract class BaseRepository<T> : IRepository<T> where T : class 
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger _logger;

        protected IDbConnection Connection => _unitOfWork.Connection;
        protected IDbTransaction Transaction => _unitOfWork.Transaction;

        protected BaseRepository(IUnitOfWork unitOfWork, ILogger logger){
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        // Các phương thức trừ tượng CRUD 
        public abstract Task<IReadOnlyList<T>> GetAllAsync();   //GET
        public abstract Task<T> GetByIdAsync(string id);        //GET
        public abstract Task<string> AddAsync(T entity);        //POST
        public abstract Task<string> UpdateAsync(T entity);     //PUT
        public abstract Task<string> DeleteAsync(string id);   //DELETE
        public abstract Task<string> PatchAsync(string id, JsonPatchDocument<T> patchDoc);   //PATCH
    }
}