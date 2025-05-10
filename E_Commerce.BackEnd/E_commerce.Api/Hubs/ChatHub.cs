using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_commerce.Application.DTOs.Common;
using Microsoft.AspNetCore.SignalR;

namespace E_commerce.Api.Hubs
{
    public class ChatHub: Hub
    {

        /// <summary>
        /// Hàm này sẽ xử lý sự kiện khi người dùng kết nối đên hub.
        /// Phương thức này được gọi khi kết nối được thiết lập
        /// </summary>
        public override async Task OnConnectedAsync(){
            
            await Clients.All.SendAsync("ReceviceMessage",$"{Context.ConnectionId} has connected "); 
        }

        ///<summary>
        /// Hàm này sẽ xử lý sự kiện khi người dùng ngắt kết nối khỏi hub.
        /// Phương thức này được gọi khi kết nối bị ngắt
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception ex){
            await base.OnDisconnectedAsync(ex);
        }

        //Hàm này sẽ thông báo thông tin người dùng khi người dùng tham gia
        public async Task JoinChat(AccountInforDTO conn){
            await Clients.All.
                SendAsync(method:"ReceiveMessage", arg1:$"{conn.user_id} Tham gia chat thành công");
        }

        //


        //Kiểm nghiệm
        public async Task GetInfor(string name){
            await Clients.All.SendAsync($"Ho ten:{name}, tuoi: 22"); 
        }
    }
}