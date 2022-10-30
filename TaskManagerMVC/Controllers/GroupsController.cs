using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManagerModels;

namespace TaskManagerMVC.Controllers
{
    public class GroupsController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFlurlClient _flurlClient;

        public GroupsController(IHttpContextAccessor httpContextAccessor,
            IFlurlClientFactory flurlClientFactory)
        {
            _flurlClient = flurlClientFactory.Get(Constants.WebApiLink);
            _httpContextAccessor = httpContextAccessor;
            var token = httpContextAccessor.HttpContext.Request.Cookies[Constants.UserJWT];
            if (!string.IsNullOrWhiteSpace(token))
            {
                _flurlClient.WithOAuthBearerToken(token);
            }
        }

        // GET: Groups
        public async Task<IActionResult> Index()
        {
            return View(await _flurlClient.Request("Groups").GetJsonAsync<List<Group>>());
        }

        // GET: Groups/Details/5
        //public async Task<IActionResult> Details(Guid? id)
        //{
        //    if (id == null || _context.Groups == null)
        //    {
        //        return NotFound();
        //    }
        //
        //    var @group = await _context.Groups
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (@group == null)
        //    {
        //        return NotFound();
        //    }
        //
        //    return View(@group);
        //}
        //
        //// GET: Groups/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}
        //
        //// POST: Groups/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Caption,Description")] Group @group)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        @group.Id = Guid.NewGuid();
        //        _context.Add(@group);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(@group);
        //}
        //
        //// GET: Groups/Edit/5
        //public async Task<IActionResult> Edit(Guid? id)
        //{
        //    if (id == null || _context.Groups == null)
        //    {
        //        return NotFound();
        //    }
        //
        //    var @group = await _context.Groups.FindAsync(id);
        //    if (@group == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(@group);
        //}
        //
        //// POST: Groups/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, [Bind("Id,Caption,Description")] Group @group)
        //{
        //    if (id != @group.Id)
        //    {
        //        return NotFound();
        //    }
        //
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(@group);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!GroupExists(@group.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(@group);
        //}
        //
        //// GET: Groups/Delete/5
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null || _context.Groups == null)
        //    {
        //        return NotFound();
        //    }
        //
        //    var @group = await _context.Groups
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (@group == null)
        //    {
        //        return NotFound();
        //    }
        //
        //    return View(@group);
        //}
        //
        //// POST: Groups/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    if (_context.Groups == null)
        //    {
        //        return Problem("Entity set 'TaskManagerContext.Groups'  is null.");
        //    }
        //    var @group = await _context.Groups.FindAsync(id);
        //    if (@group != null)
        //    {
        //        _context.Groups.Remove(@group);
        //    }
        //    
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        //
        //private bool GroupExists(Guid id)
        //{
        //  return _context.Groups.Any(e => e.Id == id);
        //}
    }
}
