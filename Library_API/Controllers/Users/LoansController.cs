using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library_API.Data;
using Library_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Library_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoansController(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Loans
        [HttpGet]
        [Authorize(Roles ="Employee")]
        public async Task<ActionResult<IEnumerable<Loan>>> GetLoans()
        {
          if (_context.Loans == null)
          {
              return NotFound();
          }
            return await _context.Loans.ToListAsync();
        }

        // GET: api/Loans/5
        [HttpGet("{Own}")]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<IEnumerable<Loan>>> GetLoan()
        {
            if (_context.Loans == null)
            {
                return NotFound();
            }

            string ownId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            if (await _context.Loans.Where(a => a.MembersId == ownId).ToListAsync() == null)
            {
                return NotFound();
            }

            return await _context.Loans.Where(a => a.MembersId == ownId).ToListAsync();
        }

        // PUT: api/Loans/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Authorize(Roles ="Employee")]
        public async Task<IActionResult> PutLoan(int returnedBookId , Loan loan)
        {
            if (loan.IsDeleted == true)
            {
                return BadRequest();
            }

            var selectedBookCopy = await _context.Loans!.FirstOrDefaultAsync(b => b.BookCopiesId == returnedBookId && b.IsReturned == false);

            if (selectedBookCopy==null)
            {
                return BadRequest();
            }

            selectedBookCopy.ReturnTime = loan.ReturnTime;

            selectedBookCopy.IsReturned = true;

            selectedBookCopy.EmployeesId = User.FindFirstValue(ClaimTypes.NameIdentifier); //giriş yapmış kullanıcının id sini alır.

            _context.Entry(selectedBookCopy).State = EntityState.Modified;

            var bookCopy = await _context.BookCopies!.FindAsync(returnedBookId);

            bookCopy!.IsAvailable = true;

            _context.BookCopies.Update(bookCopy);

            if (selectedBookCopy.ReturnTime > selectedBookCopy.DueTime)
            {
                var delayDays = (int)(selectedBookCopy.ReturnTime - selectedBookCopy.DueTime).Value.TotalDays;

                selectedBookCopy.Amount += delayDays * 30;
            }

            else
            {
                selectedBookCopy.Amount = 0;
            }

            var member = await _context.Members!.FindAsync(selectedBookCopy.MembersId);
           
                member!.Amount = selectedBookCopy.Amount;
                _context.Members.Update(member);
            

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanExists(selectedBookCopy.Id))
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

        // POST: api/Loans
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult<Loan>> PostLoan(Loan loan, string userName)
        {
          if (_context.Loans == null)
          {
              return Problem("Entity set 'ApplicationContext.Loans'  is null.");
          }

            var user = await _userManager.FindByNameAsync(userName);
            var bookCopy = await _context.BookCopies!.FindAsync(loan.BookCopiesId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (bookCopy == null)
            {
                return NotFound();
            }

            if(user.IsDeleted || bookCopy.IsDeleted)
            {
                return BadRequest();
            }

            if (!bookCopy.IsAvailable) // (bookCopy.IsAvailable == false) anlamına geliyor.
            {
                return BadRequest();
            }

            loan.MembersId = user.Id;

            loan.EmployeesId = User.FindFirstValue(ClaimTypes.NameIdentifier); //log in olan kullanıcının (employee) ıd'sini bulup değişkene atar.

            loan.IsReturned = false;
           
            bookCopy.IsAvailable = false; //kitap artık müsait değildir. Yani ödünç verilmiştir.

            _context.BookCopies.Update(bookCopy); //Kitap ödünç verildiğine dair bilgileri kaydedildi.

            _context.Loans.Add(loan);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLoans", new { id = loan.Id }, loan);
        }

        // DELETE: api/Loans/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            if (_context.Loans == null)
            {
                return NotFound();
            }
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }

            loan.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LoanExists(int id)
        {
            return (_context.Loans?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
