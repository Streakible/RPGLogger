using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCTest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCTest.Controllers
{
    public class LogController : Controller
    {
        private readonly MVCTestContext _context;

        public LogController(MVCTestContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Campaign.ToListAsync());
        }

        public IActionResult Campaign(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var logs = _context.Log.Where(m => m.Campaign.Id == id).ToList();
            return View(logs);
        }

        public IActionResult Log(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var messages = _context.Message.Where(m => m.log.Id == id).ToList();
            return View(messages);
        }
    }
}
