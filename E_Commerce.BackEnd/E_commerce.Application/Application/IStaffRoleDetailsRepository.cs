using E_commerce.Core.Entities;

namespace E_commerce.Application.Application
{
    public interface IStaffRoleDetailsRepository: IRepository<_StaffRoleDetails>
    {
        /// <summary>
        /// Xóa chi tiết vai trò của nhân viên
        /// </summary>
        public Task<bool> DeleteStaffRoleDetails(string uid, string oldRoleId);
        
        /// <summary>
        /// Lấy thông tin vai trò dựa trên ID nhân viên
        /// </summary>
        public Task<IReadOnlyList<Role>> GetRoleInforByStaffID(string uid);

        /// <summary>
        /// Lấy thông tin các nhân viên dựa trên Role ID
        /// </summary>
        public Task<IReadOnlyList<_Staff>> GetStaffInforByRoleID(string role_id);
    }
}