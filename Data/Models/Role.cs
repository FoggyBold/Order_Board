using Order_Board.Models;

namespace Order_Board.Data.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
        public Role()
        {
            Users = new List<User>();
        }
        public Role(string name)
        {
            Name = name;
        }
    }
}
