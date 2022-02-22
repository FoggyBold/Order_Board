using Order_Board.Data.Models;

namespace Order_Board.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
