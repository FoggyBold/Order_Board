using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order_Board.Data.Models;
using Order_Board.Data.Repository;
using Order_Board.Models;
using Order_Board.ViewModels;
using System.Security.Claims;


/*Для входа на сайт определена пара методов Login. Get-версия метода просто возвращает представление с формой. Post-версия принимает в качестве параметра модель LoginModel.
* Вначале смотрим, а есть ли с таким же email в базе данных какой-либо пользователь, если такой пользователь имеется в БД, то выполняем аутентификацию и устанавливаем
* аутентификационные куки. Чтобы не повторяться (в соответствии с принципом DRY), данный код вынесен в отдельный метод Authenticate:*/


namespace Order_Board.Controllers
{
    public class AccountController : Controller
    {
        private AppDBContent db;
        public AccountController(AppDBContent context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    // добавляем пользователя в бд
                    user = new User { Email = model.Email, Password = model.Password };
                    Role userRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == model.Role);
                    if (userRole != null)
                    {
                        user.RoleName = userRole.Name;
                        user.RoleId = userRole.Id;
                    }

                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                    if (user != null)
                    {
                        await Authenticate(user); // аутентификация
                    }

                    return RedirectToAction("Index", "Home");
                    //if (userRole.Name == "Customer")
                    //{
                    //    return RedirectToAction("Customer", "Home");
                    //}
                    //else
                    //{
                    //    return RedirectToAction("Worker", "Home");
                    //}
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        /*Для установки кук применяется асинхронный метод контекста HttpContext.SignInAsync(). В качестве параметра он принимает схему аутентификации, которая была использована
        при вызове метода services.AddAuthentication() в методе ConfigureServices() в классе Startup. То есть в нашем случае это CookieAuthenticationDefaults.AuthenticationScheme. 
        А в качестве второго параметра передается объект ClaimsPrincipal, который представляет пользователя.
        Для правильного создания и настройки объекта ClaimsPrincipal вначале создается список claims - набор данных, которые шифруются и добавляются в аутентификационные куки.
        Каждый такой claim принимает тип и значение. В нашем случае у нас только один claim, который в качестве типа принимает константу ClaimsIdentity.DefaultNameClaimType, а в качестве значения - email пользователя.
        Далее создается объект ClaimsIdentity, который нужен для инициализации ClaimsPrincipal. В ClaimsIdentity передается:
        Ранее созданный список claims
        Тип аутентификации, в данном случае "ApplicationCookie"
        Тип данных в списке claims, который преставляет логин пользователя. То есть при добавлении claimа мы использовали в качестве типа ClaimsIdentity.DefaultNameClaimType, 
        поэтому и тут нам надо указать то же самое значение. Мы, конечно, можем указать и разные значения, но тогда система не сможет связать различные claim с 
        логином пользователя.
        Тип данных в списке claims, который представляет роль пользователя. Хотя у нас такого claim нет, который бы представлял роль пользователя, но но опционально мы можем 
        указать константу ClaimsIdentity.DefaultRoleClaimType. В данном случае она ни на что не влияет.
        И после вызова метода расширения HttpContext.SignInAsync в ответ клиенту будут отправляться аутентификационные куки, которые при последующих запросах будут 
        передаваться обратно на сервер, десериализоваться и использоваться для аутентификации пользователя.*/

        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.RoleName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
