using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lylo.University.Immigration.Data;
using Lylo.University.Immigration.Models;

namespace Lylo.University.Immigration.Controllers
{
    public class ImmigrationDatasController : Controller
    {
        private readonly LyloUniversityImmigrationContext _context;

        public ImmigrationDatasController(LyloUniversityImmigrationContext context)
        {
            _context = context;
        }

        // GET: ImmigrationDatas
        public async Task<IActionResult> Index()
        {
              return _context.ImmigrationDatas != null ? 
                          View(await _context.ImmigrationDatas.ToListAsync()) :
                          Problem("Entity set 'LyloUniversityImmigrationContext.ImmigrationData'  is null.");
        }

        // GET: ImmigrationDatas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ImmigrationDatas == null)
            {
                return NotFound();
            }

            var immigrationData = await _context.ImmigrationDatas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (immigrationData == null)
            {
                return NotFound();
            }

            return View(immigrationData);
        }

        // GET: ImmigrationDatas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ImmigrationDatas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Country,Children")] ImmigrationData immigrationData)
        {
            if (ModelState.IsValid)
            {
                _context.Add(immigrationData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(immigrationData);
        }

        // GET: ImmigrationDatas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ImmigrationDatas == null)
            {
                return NotFound();
            }

            var immigrationData = await _context.ImmigrationDatas.FindAsync(id);
            if (immigrationData == null)
            {
                return NotFound();
            }
            return View(immigrationData);
        }

        // POST: ImmigrationDatas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Country,Children")] ImmigrationData immigrationData)
        {
            if (id != immigrationData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(immigrationData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImmigrationDataExists(immigrationData.Id))
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
            return View(immigrationData);
        }

        // GET: ImmigrationDatas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ImmigrationDatas == null)
            {
                return NotFound();
            }

            var immigrationData = await _context.ImmigrationDatas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (immigrationData == null)
            {
                return NotFound();
            }

            return View(immigrationData);
        }

        // POST: ImmigrationDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ImmigrationDatas == null)
            {
                return Problem("Entity set 'LyloUniversityImmigrationContext.ImmigrationData'  is null.");
            }
            var immigrationData = await _context.ImmigrationDatas.FindAsync(id);
            if (immigrationData != null)
            {
                _context.ImmigrationDatas.Remove(immigrationData);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImmigrationDataExists(int id)
        {
          return (_context.ImmigrationDatas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
