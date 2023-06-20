using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class WorkLogController : Controller
    {
        private readonly WebApplication3Context _context;

        public WorkLogController(WebApplication3Context context)
        {
            _context = context;
        }

        // GET: WorkLog
        public async Task<IActionResult> Index()
        {
            int? userId = GetUserIdFromCookies();

            if (userId.HasValue)
            {
                var userInformation = await _context.UserInformationModel.FindAsync(userId);

                var user = await _context.UserInformationModel.FindAsync(userId.Value);
                if (user != null)
                {
                    ViewBag.UserName = user.Username;

                    var workLogs = await _context.WorkLogModel.Where(w => w.UserId == userId).ToListAsync();
                    return View(workLogs);
                }
            }
            return Problem("User Information not found!");

        }
        private int? GetUserIdFromCookies()
        {
            if (Request.Cookies.TryGetValue("UserId", out string userIdString))
            {
                if (int.TryParse(userIdString, out int userId))
                {
                    return userId;
                }
            }

            return null;
        }

        // GET: WorkLog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.WorkLogModel == null)
            {
                return NotFound();
            }

            var workLogModel = await _context.WorkLogModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workLogModel == null)
            {
                return NotFound();
            }

            return View(workLogModel);
        }

        // GET: WorkLog/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WorkLog/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Title,Content,Date")] WorkLogModel workLogModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workLogModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workLogModel);
        }

        // GET: WorkLog/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.WorkLogModel == null)
            {
                return NotFound();
            }

            var workLogModel = await _context.WorkLogModel.FindAsync(id);
            if (workLogModel == null)
            {
                return NotFound();
            }
            return View(workLogModel);
        }

        // POST: WorkLog/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Title,Content,Date")] WorkLogModel workLogModel)
        {
            if (id != workLogModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workLogModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkLogModelExists(workLogModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(workLogModel);
        }

        // GET: WorkLog/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.WorkLogModel == null)
            {
                return NotFound();
            }

            var workLogModel = await _context.WorkLogModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workLogModel == null)
            {
                return NotFound();
            }

            return View(workLogModel);
        }

        // POST: WorkLog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.WorkLogModel == null)
            {
                return Problem("Entity set 'WebApplication3Context.WorkLogModel'  is null.");
            }
            var workLogModel = await _context.WorkLogModel.FindAsync(id);
            if (workLogModel != null)
            {
                _context.WorkLogModel.Remove(workLogModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkLogModelExists(int id)
        {
          return (_context.WorkLogModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
