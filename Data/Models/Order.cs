using Order_Board.Models;

namespace Order_Board.Data.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Cost { get; set; }
        public bool isReady { get; set; }
        public bool isInWork { get; set; }
        public int CustomerID { get; set; }
        public int WorkerID { get; set; }
    }
}
