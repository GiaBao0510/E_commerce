
using E_commerce.Application.DTOs.Common;
using Microsoft.AspNetCore.SignalR;

namespace E_commerce.Api.Hubs
{
    public class ChatHub : Hub
    {
        /// <summary>
        /// Hàm này sẽ xử lý sự kiện khi người dùng kết nối đên hub.
        /// Phương thức này được gọi khi kết nối được thiết lập
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            // Xử lý khi người dùng kết nối....
            await Clients.All.SendAsync("ReceviceMessage", $"{Context.ConnectionId} has connected ");
        }

        ///<summary>
        /// Hàm này sẽ xử lý sự kiện khi người dùng ngắt kết nối khỏi hub.
        /// Phương thức này được gọi khi kết nối bị ngắt
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            // Xử lý khi người dùng ngắt kết nối....
            await base.OnDisconnectedAsync(ex);
        }

        //Gửi tin nhắn đến tất cả người dùng
        public async Task SendMessageToAll(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        //Người dùng gửi tin nhắn đến 1 người nào đó
        public async Task SendMessageToUser(string recipientId, string message)
        {
            await Clients.User(recipientId).SendAsync("ReceiveMessage", message);
        }

        //Người dùng gửi tin nhắn trong nhóm
        public async Task SendMessageToGroup(string userID ,string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }

    }
} 