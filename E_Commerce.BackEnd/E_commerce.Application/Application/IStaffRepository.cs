using E_commerce.Core.Entities;

namespace E_commerce.Application.Application
{
    public interface IStaffRepository: IRepository<_Staff>
    {
        /// <summary>
        /// Kiểm tra có phải là nhân viên không
        /// </summary>
        public Task<bool> IsStaff(string uid); 
    }
}