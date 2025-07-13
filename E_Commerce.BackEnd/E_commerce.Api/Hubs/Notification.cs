
using Microsoft.AspNetCore.SignalR;

namespace E_commerce.Api.Hubs
{
    public class Notification : Hub
    {
        //Thông báo cho tất cả người dùng
        public async Task NoticeToAllUsers(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }

        //Thông báo cho nhóm chat
        public async Task NoticeToGroupChat(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveNotification", message);
        }

        //Thông báo cho nhóm người dùng cụ thể
        public async Task NotifySpecificUserGroups(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveNotification", message);
        }
    }
}