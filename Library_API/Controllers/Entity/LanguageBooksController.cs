using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library_API.Data;
using Library_API.Models;
using Microsoft.AspNetCore.Authorization;

namespace Library_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguageBooksController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public LanguageBooksController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/LanguageBooks
        [HttpGet]
        [Authorize(Roles ="Employee")]
        public async Task<ActionResult<IEnumerable<LanguageBook>>> GetLanguageBooks()
        {
          if (_context.LanguageBooks == null)
          {
              return NotFound();
          }
            return await _context.LanguageBooks.ToListAsync();
        }

        // GET: api/LanguageBooks/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult<LanguageBook>> GetLanguageBook(string id)
        {
          if (_context.LanguageBooks == null)
          {
              return NotFound();
          }
            var languageBook = await _context.LanguageBooks.FindAsync(id);

            if (languageBook == null)
            {
                return NotFound();
            }

            return languageBook;
        }


        // POST: api/LanguageBooks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult<LanguageBook>> PostLanguageBook(LanguageBook languageBook)
        {
          if (_context.LanguageBooks == null)
          {
              return Problem("Entity set 'ApplicationContext.LanguageBooks'  is null.");
          }

            var language = await _context.Languages!.FindAsync(languageBook.LanguagesCode);
            var book = await _context.Books!.FindAsync(languageBook.BooksId);

            if (language != null && language.IsDeleted == true || book != null && book.IsDeleted == true)
            {
                return BadRequest();
            }
            _context.LanguageBooks.Add(languageBook);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LanguageBookExists(languageBook.LanguagesCode))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLanguageBook", new { id = languageBook.LanguagesCode }, languageBook);
        }

        // DELETE: api/LanguageBooks/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeleteLanguageBook(string id)
        {
            if (_context.LanguageBooks == null)
            {
                return NotFound();
            }
            var languageBook = await _context.LanguageBooks.FindAsync(id);
            if (languageBook == null)
            {
                return NotFound();
            }

            _context.LanguageBooks.Remove(languageBook);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LanguageBookExists(string id)
        {
            return (_context.LanguageBooks?.Any(e => e.LanguagesCode == id)).GetValueOrDefault();
        }
    }
}
