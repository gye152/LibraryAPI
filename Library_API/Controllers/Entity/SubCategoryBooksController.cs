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
    public class SubCategoryBooksController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public SubCategoryBooksController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/SubCategoryBooks
        [HttpGet]
        [Authorize(Roles ="Employee")]
        public async Task<ActionResult<IEnumerable<SubCategoryBook>>> GetSubCategoryBooks()
        {
          if (_context.SubCategoryBooks == null)
          {
              return NotFound();
          }
            return await _context.SubCategoryBooks.ToListAsync();
        }

        // GET: api/SubCategoryBooks/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult<SubCategoryBook>> GetSubCategoryBook(short id)
        {
          if (_context.SubCategoryBooks == null)
          {
              return NotFound();
          }
            var subCategoryBook = await _context.SubCategoryBooks.FindAsync(id);

            if (subCategoryBook == null)
            {
                return NotFound();
            }

            return subCategoryBook;
        }


        // POST: api/SubCategoryBooks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult<SubCategoryBook>> PostSubCategoryBook(SubCategoryBook subCategoryBook)
        {
          if (_context.SubCategoryBooks == null)
          {
              return Problem("Entity set 'ApplicationContext.SubCategoryBooks'  is null.");
          }

            var subCategory = await _context.SubCategories!.FindAsync(subCategoryBook.SubCategoriesId);
            var book = await _context.Books!.FindAsync(subCategoryBook.BooksId);

            if (subCategory != null && subCategory.IsDeleted == true || book != null && book.IsDeleted == true)
            {
                return BadRequest();
            }
            _context.SubCategoryBooks.Add(subCategoryBook);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SubCategoryBookExists(subCategoryBook.SubCategoriesId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSubCategoryBook", new { id = subCategoryBook.SubCategoriesId }, subCategoryBook);
        }

        // DELETE: api/SubCategoryBooks/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeleteSubCategoryBook(short id)
        {
            if (_context.SubCategoryBooks == null)
            {
                return NotFound();
            }
            var subCategoryBook = await _context.SubCategoryBooks.FindAsync(id);
            if (subCategoryBook == null)
            {
                return NotFound();
            }

            _context.SubCategoryBooks.Remove(subCategoryBook);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubCategoryBookExists(short id)
        {
            return (_context.SubCategoryBooks?.Any(e => e.SubCategoriesId == id)).GetValueOrDefault();
        }
    }
}
