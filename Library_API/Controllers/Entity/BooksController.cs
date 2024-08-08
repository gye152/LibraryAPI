using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library_API.Data;
using Library_API.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Library_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public BooksController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
          if (_context.Books == null)
          {
              return NotFound();
          }
            return await _context.Books.ToListAsync();
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
          if (_context.Books == null)
          {
              return NotFound();
          }
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles ="Employee")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {

            book.Id = id;
            book.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (book.IsDeleted == true)
            {
                return BadRequest();
            }   
            var location = await _context.Locations!.FindAsync(book.LocationShelf);
            var publisher = await _context.Publishers!.FindAsync(book.Publisher);

            if (location != null && location.IsDeleted || publisher != null && publisher.IsDeleted == true)
            {
                return BadRequest();
            }
            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles="Employee")]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            AuthorBook authorBook;
            LanguageBook languageBook;
            SubCategoryBook subCategoryBook;


          if (_context.Books == null)
          {
              return Problem("Entity set 'ApplicationContext.Books'  is null.");
          }
                            //Log in olan kullanıcının id'sini bulur. 
            book.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var location = await _context.Locations!.FindAsync(book.LocationShelf);
            var publisher = await _context.Publishers!.FindAsync(book.Publisher);

            if (location != null && location.IsDeleted == true || publisher != null && publisher.IsDeleted == true)
            {
                return BadRequest();
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            if (book.AuthorIds != null)
            {
                foreach (long authorId in book.AuthorIds)
                {
                    var author = await _context.Authors!.FindAsync(authorId);
                    if (author == null || author.IsDeleted)
                    {
                        return BadRequest("Yazar mevcut değil veya silinmiş.");
                    }
                    authorBook = new AuthorBook { AuthorsId = authorId, BooksId = book.Id };
                    _context.AuthorBooks!.Add(authorBook);
                }
                _context.SaveChanges();
            }

            if(book.LanguageCodes != null)
            {
                foreach (string languageCode in book.LanguageCodes)
                {
                    var language = await _context.Languages!.FirstOrDefaultAsync(l => l.Code == languageCode);
                    if (language == null)
                    {
                        return BadRequest("Dil kodu geçersiz veya mevcut değil.");
                    }
                    languageBook = new LanguageBook { LanguagesCode = languageCode, BooksId = book.Id };
                    _context.LanguageBooks!.Add(languageBook);
                }
                _context.SaveChanges();
            }

            if(book.SubCategoryIds != null)
            {
                foreach (short subCategoryId in book.SubCategoryIds)
                {
                    var subCategory = await _context.SubCategories!.FindAsync(subCategoryId);
                    if (subCategory == null || subCategory.IsDeleted)
                    {
                        return BadRequest("Alt kategori mevcut değil veya silinmiş.");
                    }
                    subCategoryBook = new SubCategoryBook { SubCategoriesId = subCategoryId, BooksId = book.Id };
                    _context.SubCategoryBooks!.Add(subCategoryBook);
                }
                _context.SaveChanges();

            }

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        [Authorize(Roles ="Employee")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (_context.Books == null)
            {
                return NotFound();
            }
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            book.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}


