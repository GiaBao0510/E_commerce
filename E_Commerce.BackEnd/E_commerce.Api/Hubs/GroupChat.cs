using Microsoft.AspNetCore.SignalR;

namespace E_commerce.Api.Hubs
{
    public class GroupChat : Hub
    {
        public GroupChat()
        {
            
        }

        //Thêm người dùng vào trong nhóm
        public async Task AddUserToGroup(string groupName, string userID)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        //Người dùng gửi yêu cầu tham gia nhóm
        //Chấp nhận yêu cầu tham gia nhóm

        //Xóa người dùng ra khỏi nhóm
        public async Task RemoveUserFromGroup(string groupName, string userID)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        //Kiểm tra người dùng có trong nhóm hay không
        // public async Task<bool> IsUserInGroup(string groupName, string userID)
        // {
        //     //return await Groups.IsInGroupAsync(userID, groupName);
        // }

        // Lấy danh sách người dùng trong nhóm
    }
}