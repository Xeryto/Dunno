using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dunno;
using Dunno.Models;
using Dunno.Models.Helpers;
using AutoMapper;
using System.IO;

namespace Dunno.Controllers
{
    public class NewsController : Controller
    {
        private readonly DunnoContext _context;
        private readonly MapperConfiguration config = new(cfg => cfg
        .CreateMap<NewsPost, News>().ForMember("Image", opt => opt.Ignore()));
        private readonly MapperConfiguration editConfig = new(cfg => cfg
        .CreateMap<News, NewsPost>().ForMember("Image", opt => opt.Ignore()));

        public NewsController(DunnoContext context)
        {
            _context = context;
        }

        // GET: News
        public async Task<IActionResult> Index()
        {
            return View(await _context.News.ToListAsync());
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: News/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Image")] NewsPost newsPost)
        {
            if (ModelState.IsValid)
            {
                var mapper = new Mapper(config);
                var news = mapper.Map<NewsPost, News>(newsPost);
                if (newsPost.Image != null)
                {
                    var stream = new MemoryStream();
                    await newsPost.Image.CopyToAsync(stream);
                    news.Image = stream.ToArray();
                }

                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newsPost);
        }

        // GET: News/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            var mapper = new Mapper(editConfig);
            var newsEdit = mapper.Map<News, NewsPost>(news);

            return View(newsEdit);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Image")] NewsPost newsPost)
        {
            if (id != newsPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var mapper = new Mapper(config);
                var news = mapper.Map<NewsPost, News>(newsPost);
                if (newsPost.Image != null)
                {
                    var stream = new MemoryStream();
                    await newsPost.Image.CopyToAsync(stream);
                    news.Image = stream.ToArray();
                }
                else
                {
                    var prev = _context.News.AsNoTracking().FirstOrDefault(i => i.Id == id).Image;
                    news.Image = prev;
                }

                try
                {
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id))
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
            return View(newsPost);
        }

        // GET: News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }
    }
}
