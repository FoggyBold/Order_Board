using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order_Board.Data.Models;
using Order_Board.Data.Repository;
using Order_Board.Models;
using Order_Board.ViewModels;

namespace Order_Board.Controllers
{
    public class OrderController : Controller
    {
        private AppDBContent db;
        public OrderController(AppDBContent context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(OrderModel model)
        {
            User currUser = await db.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            if (ModelState.IsValid)
            {
                Order order = new Order { Name = model.Name, Description = model.Description, isInWork = false, isReady = false, CustomerID = currUser.Id, Cost = model.Price };
                if (order != null)
                {
                    db.Orders.Add(order);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректное заполнение формы");
            }
            return View(model);
        }

        public async Task<IActionResult> TakeAsync(int orderId)
        {
            User user = await db.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            Order order = await db.Orders.FirstOrDefaultAsync(u => u.Id == orderId);
            order.isInWork = true;
            order.WorkerID = user.Id;
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> EndAsync(int orderId)
        {
            User user = await db.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            Order order = await db.Orders.FirstOrDefaultAsync(u => u.Id == orderId);
            order.isReady = true;
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
