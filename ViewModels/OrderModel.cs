using System.ComponentModel.DataAnnotations;

namespace Order_Board.ViewModels
{
    public class OrderModel
    {
        [Required(ErrorMessage = "Не указано название")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указано описание")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Не указана цена")]
        [DataType(DataType.Text)]
        public double Price { get; set; }
    }
}
