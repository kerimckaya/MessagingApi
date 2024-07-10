using MessagingApi.Data;
using MessagingApi.Hubs;
using MessagingApi.Models;
using MessagingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MessagingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : BaseController
    {
        private readonly ApplicationDbContext _c;
        private readonly IHubContext<ChatHub> _hub;

        public ChatController(ApplicationDbContext c, IHubContext<ChatHub> hub)
        {
            _c = c;
            _hub = hub;
        }

        [HttpGet("GetMessageHistory")]
        public async Task<IActionResult> GetMessageHistory(string senderNumber, string receiverNumber)
        {
            try
            {
                int senderId = (await _c.User.FirstOrDefaultAsync(x => x.PhoneNumber == senderNumber))?.Id ?? 0;
                int receiverId = (await _c.User.FirstOrDefaultAsync(x => x.PhoneNumber == receiverNumber))?.Id ?? 0;
                return Ok(await _c.Messages.Where(x => x.SenderId == senderId && x.ReceiverId == receiverId).ToListAsync());
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }
        [HttpGet("GetMyUsers")]
        public async Task<IActionResult> GetMyUsers(string phoneNumber)
        {
            try
            {
                int senderId = (await _c.User.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber))?.Id ?? 0;
                return Ok(await _c.Messages.Where(x => x.SenderId == senderId).Select(X => X.Receiver).Distinct().ToListAsync());
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }

        [HttpPost("SendMessageTest")]
        public async Task<IActionResult> SendMessageTest(string phoneNumber, string message, string token)
        {
            try
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
                await _hub.Clients.All.SendAsync("ReceiveMessage", phoneNumber, content);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }
    }
}
