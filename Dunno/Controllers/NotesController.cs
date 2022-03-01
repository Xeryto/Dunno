using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dunno;
using Dunno.Models;
using AutoMapper;
using Dunno.Models.Helpers;
using System.IO;

namespace Dunno.Controllers
{
    public class NotesController : Controller
    {
        private readonly DunnoContext _context;
        private readonly MapperConfiguration config = new(cfg => cfg
        .CreateMap<NotePost, Note>().ForMember("Image", opt => opt.Ignore()));
        private readonly MapperConfiguration editConfig = new(cfg => cfg
        .CreateMap<Note, NotePost>().ForMember("Image", opt => opt.Ignore()));

        public NotesController(DunnoContext context)
        {
            _context = context;
        }

        // GET: Notes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Notes.ToListAsync());
        }

        // GET: Notes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Notes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // GET: Notes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Notes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Image")] NotePost notePost)
        {
            if (ModelState.IsValid)
            {
                var mapper = new Mapper(config);
                var note = mapper.Map<NotePost, Note>(notePost);
                if (notePost.Image != null)
                {
                    var stream = new MemoryStream();
                    await notePost.Image.CopyToAsync(stream);
                    note.Image = stream.ToArray();
                }

                _context.Add(note);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(notePost);
        }

        // GET: Notes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            var mapper = new Mapper(editConfig);
            var noteEdit = mapper.Map<Note, NotePost>(note);

            return View(noteEdit);
        }

        // POST: Notes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Image")] NotePost notePost)
        {
            if (id != notePost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var mapper = new Mapper(config);
                var note = mapper.Map<NotePost, Note>(notePost);
                if (notePost.Image != null)
                {
                    var stream = new MemoryStream();
                    await notePost.Image.CopyToAsync(stream);
                    note.Image = stream.ToArray();
                }
                else
                {
                    var prev = _context.Notes.AsNoTracking().FirstOrDefault(i => i.Id == id).Image;
                    note.Image = prev;
                }

                try
                {
                    _context.Update(note);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteExists(notePost.Id))
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
            return View(notePost);
        }

        // GET: Notes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Notes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NoteExists(int id)
        {
            return _context.Notes.Any(e => e.Id == id);
        }
    }
}
