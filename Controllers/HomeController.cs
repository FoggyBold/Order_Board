using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order_Board.Data.Repository;
using Order_Board.Models;
using Order_Board.ViewModels;
using System.Diagnostics;
using System.Security.Claims;

namespace Order_Board.Controllers
{
    public class HomeController : Controller
    {
        private AppDBContent db;
        public HomeController(AppDBContent context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Customer, Worker")]
        public async Task<IActionResult> OrdersAsync()
        {
            User currUser = await db.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            OrdersViewModel orderObj = null;
            if (User.IsInRole("Customer"))
            {
                orderObj = new OrdersViewModel
                {
                    Orders = db.Orders.Where(u => u.CustomerID == currUser.Id).ToList(),
                };
            }
            else
            {
                orderObj = new OrdersViewModel
                {
                    Orders = db.Orders.Where(u => u.WorkerID == currUser.Id && !u.isReady).ToList(),
                };
            }
            return View("Orders", orderObj);
        }

        [Authorize(Roles = "Worker")]
        public IActionResult AllOrdersAsync()
        {
            OrdersViewModel orderObj = new OrdersViewModel
            {
                Orders = db.Orders.Where(u => !u.isInWork && !u.isReady).ToList(),
            };
            return View("AllOrders", orderObj);
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}