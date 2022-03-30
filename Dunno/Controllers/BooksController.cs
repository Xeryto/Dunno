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
    public class BooksController : Controller
    {
        private readonly DunnoContext _context;
        private readonly MapperConfiguration config = new(cfg => cfg
        .CreateMap<BookPost, Book>().ForMember("Image", opt => opt.Ignore()));
        private readonly MapperConfiguration editConfig = new(cfg => cfg
        .CreateMap<Book, BookPost>().ForMember("Image", opt => opt.Ignore()));

        public BooksController(DunnoContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View(await _context.Books.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string searchString)
        {
            return View(await _context.Books.Where(i => i.Title.Contains(searchString) || i.Description.Contains(searchString)).ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Displacement,Image")] BookPost bookPost)
        {
            if (ModelState.IsValid)
            {
                var mapper = new Mapper(config);
                var book = mapper.Map<BookPost, Book>(bookPost);
                if (bookPost.Image != null)
                {
                    var stream = new MemoryStream();
                    await bookPost.Image.CopyToAsync(stream);
                    book.Image = stream.ToArray();
                }

                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bookPost);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var mapper = new Mapper(editConfig);
            var bookEdit = mapper.Map<Book, BookPost>(book);

            return View(bookEdit);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Displacement,Image")] BookPost bookPost)
        {
            if (id != bookPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var mapper = new Mapper(config);
                var book = mapper.Map<BookPost, Book>(bookPost);
                if (bookPost.Image != null)
                {
                    var stream = new MemoryStream();
                    await bookPost.Image.CopyToAsync(stream);
                    book.Image = stream.ToArray();
                }
                else
                {
                    var prev = _context.Books.AsNoTracking().FirstOrDefault(i => i.Id == id).Image;
                    book.Image = prev;
                }

                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(bookPost.Id))
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
            return View(bookPost);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
