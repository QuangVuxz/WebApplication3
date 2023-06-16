using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class UserInformationController : Controller
    {
        private readonly WebApplication3Context _context;

        public UserInformationController(WebApplication3Context context)
        {
            _context = context;
        }

        // GET: UserInformation
        public async Task<IActionResult> Index()
        {
              return _context.UserInformationModel != null ? 
                          View(await _context.UserInformationModel.ToListAsync()) :
                          Problem("Entity set 'WebApplication3Context.UserInformationModel'  is null.");
        }
        // GET: UserInformation/Search
        public async Task<IActionResult> Search(string searchString)
        {
            var users = await _context.UserInformationModel.ToListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u =>
                    u.Username.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View("Index", users);
        }



        // GET: UserInformation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UserInformationModel == null)
            {
                return NotFound();
            }

            var userInformationModel = await _context.UserInformationModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userInformationModel == null)
            {
                return NotFound();
            }

            return View(userInformationModel);
        }

        // GET: UserInformation/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserInformation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,Role")] UserInformationModel userInformationModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userInformationModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userInformationModel);
        }

        // GET: UserInformation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UserInformationModel == null)
            {
                return NotFound();
            }

            var userInformationModel = await _context.UserInformationModel.FindAsync(id);
            if (userInformationModel == null)
            {
                return NotFound();
            }
            return View(userInformationModel);
        }

        // POST: UserInformation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,Role")] UserInformationModel userInformationModel)
        {
            if (id != userInformationModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userInformationModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserInformationModelExists(userInformationModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }
            return View(userInformationModel);
        }

        // GET: UserInformation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UserInformationModel == null)
            {
                return NotFound();
            }

            var userInformationModel = await _context.UserInformationModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userInformationModel == null)
            {
                return NotFound();
            }

            return View(userInformationModel);
        }

        // POST: UserInformation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserInformationModel == null)
            {
                return Problem("Entity set 'WebApplication3Context.UserInformationModel'  is null.");
            }
            var userInformationModel = await _context.UserInformationModel.FindAsync(id);
            if (userInformationModel != null)
            {
                _context.UserInformationModel.Remove(userInformationModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Register()
        {
            return View();
        }

        // POST: UserInformation/Register
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,Username,Password,Role")] UserInformationModel userInformationModel)
        {
            if (ModelState.IsValid)
            {
                bool usernameExists = await _context.UserInformationModel.AnyAsync(u => u.Username == userInformationModel.Username);
                
                if (usernameExists)
                {
                    ModelState.AddModelError("Username", "Username already exists");
                    return View(userInformationModel);
                }
                try
                {
                    _context.Add(userInformationModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            return View(userInformationModel);
        }



        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                // Check if the username and password combination exists in the database
                bool isValidUser = await _context.UserInformationModel.AnyAsync(u => u.Username == username && u.Password == password);
                var user = await _context.UserInformationModel.FirstOrDefaultAsync(u => u.Username == username);

                if (isValidUser)
                {
                    if (user.Role == "Admin")
                    {
                        
                        Response.Cookies.Append("UserId", user.Id.ToString());
                        
                        return RedirectToAction(nameof(Index), new { id = user.Id });
                    }
                    else if (user.Role == "User")
                    {
                        
                        Response.Cookies.Append("UserId", user.Id.ToString());
                        
                        return RedirectToAction("Create", "WorkLog", new {id= user.Id});
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                }
            }

            // If the ModelState is invalid or the login failed, return to the login view with the provided username
            return View("Login", username);
        }

        //GetUsername

        private bool UserInformationModelExists(int id)
        {
          return (_context.UserInformationModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
