using ASP.NET_Core_Check.Models;

namespace ASP.NET_Core_Check.Infrastructure.Repositories
{
    public class RoomRepository
    {
        public Room GetById(int id)
        {
            return new Room
            {
                IsOpen = true,
                Number = "205b",
                NumberOfOccupants = 4
            };
        }
    }
}
