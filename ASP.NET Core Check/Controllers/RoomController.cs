using System.Threading.Tasks;
using ASP.NET_Core_Check.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppConstants = ASP.NET_Core_Check.Constants.Constants;

namespace ASP.NET_Core_Check.Controllers
{
    public class RoomController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public RoomController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> RoomInfo(int roomId)
        {
            var roomRepository = new RoomRepository();
            var room = roomRepository.GetById(roomId);
            var authorizeResult = await _authorizationService.AuthorizeAsync(User, room, AppConstants.Policies.CanAccessToRoom);
            if (authorizeResult.Succeeded)
            {
                return View();
            }

            return new ChallengeResult();
        }
    }
}