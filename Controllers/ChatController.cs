using Api.Dtos;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("register-user")]
        public IActionResult RegisterUser(UserDto user)
        {
            if (_chatService.AddUserToList(user.Name))
            {
                // 204 Status Code
                return NoContent();
            }
            return BadRequest("This Name Is Already Taken Please Choose Another One");
        }
    }
}
