using Order_Board.Data.Models;

namespace Order_Board.ViewModels
{
    public class OrdersViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
    }
}
