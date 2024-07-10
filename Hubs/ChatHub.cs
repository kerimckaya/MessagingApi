using MessagingApi.Data;
using MessagingApi.Models;
using MessagingApi.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MessagingApi.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _c;

        public ChatHub(ApplicationDbContext c)
        {
            _c = c;
        }

        public async Task SendMessage(string phoneNumber, string message, string token)
        {
            User sender = await _c.User.FirstOrDefaultAsync(x => x.PhoneNumber == TokenService.GetUserPhoneByToken(token));
            User reciver = await _c.User.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            Messages content = new Messages();
            if (sender is User)
            {
                content = new Messages()
                {
                    Message = message,
                    SenderId = sender.Id,
                    ReceiverId = reciver.Id,
                    SendDate = DateTime.Now
                };

                await _c.AddAsync(content);
                await _c.SaveChangesAsync();
            }
            await Clients.All.SendAsync("ReceiveMessage", phoneNumber, content);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
