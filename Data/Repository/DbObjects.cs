using Microsoft.EntityFrameworkCore;
using Order_Board.Data.Models;
using Order_Board.Models;

namespace Order_Board.Data.Repository
{
    public class DbObjects
    {
        public static void Initial(AppDBContent content)
        {
            if (!content.Roles.Any())
            {
                content.Roles.AddRange(new Role[]
                {
                    new Role("Customer"),
                    new Role("Worker")
                });
            }

            content.SaveChanges();
        }
    }
}
