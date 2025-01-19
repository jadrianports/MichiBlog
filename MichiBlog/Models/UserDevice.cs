using MichiBlog.Models;

namespace MichiBlog.WebApp.Models
{
    public class UserDevice
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string DeviceIdentifier { get; set; }
        public DateTime LastLoggedIn { get; set; }
        public bool IsVerified { get; set; }
    }
}
