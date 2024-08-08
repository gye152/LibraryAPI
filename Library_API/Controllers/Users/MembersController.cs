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
using Microsoft.IdentityModel.Tokens;
using NuGet.Packaging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Library_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        

        public MembersController(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Members
        [Authorize(Roles ="Employee")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
         
          if (_context.Members == null)
          {
              return NotFound();
          }
            var member = await _context.Members!.Include(m => m.ApplicationUser).ToListAsync();
            return member;
        }

        // GET: api/Members/5
        [HttpGet("{username}")]
        [Authorize(Roles = "Employee,Member")]
        public async Task<ActionResult<Member>> GetMember(string userName)
        {
          if (_context.Members == null)
          {
              return NotFound();
          }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(userName);

          if (user == null)
          {
              return NotFound();
          }

           var member = await _context.Members
                                      .Include(m => m.ApplicationUser)
                                      .FirstOrDefaultAsync(m => m.Id == user.Id);

            if (member == null)
            {
                return NotFound();
            }
            //Log in olan kullanıcının rolü Employee ise görmek istediği member bilgilerini döndürür.
            if (User.IsInRole("Employee"))
            { 
                return member;
             }
            //Başka bir member başka bir member'ın bilgilerini göremez.
            if (user.Id != userId)
            {
                return BadRequest();
            }

            //Member kendi bilgilerini görmüş olacak.
            return member;
             
        }

        // PUT: api/Members/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles ="Member")]
        public async Task<IActionResult> PutMember(string id, Member member, string? currentPassword = null)
        {
            string ownId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ApplicationUser applicationUser = _userManager.FindByIdAsync(id).Result;

            if (applicationUser.IsDeleted == true)
            {
                return BadRequest();
            }

            applicationUser.IdNumber = member.ApplicationUser!.IdNumber;
            applicationUser.Name = member.ApplicationUser!.Name;
            applicationUser.MiddleName = member.ApplicationUser!.MiddleName;
            applicationUser.FamilyName = member.ApplicationUser!.FamilyName;
            applicationUser.Address = member.ApplicationUser!.Address;
            applicationUser.Gender = member.ApplicationUser!.Gender;
            applicationUser.BirthDate = member.ApplicationUser!.BirthDate;
            applicationUser.RegisterDate = member.ApplicationUser!.RegisterDate;
            applicationUser.Status = member.ApplicationUser!.Status;
            applicationUser.Email = member.ApplicationUser!.Email;

            _userManager.UpdateAsync(applicationUser).Wait();//identitynin yaptığı kısmı güncelledik.

            if (currentPassword != null)
            {
                _userManager.ChangePasswordAsync(applicationUser, currentPassword, applicationUser.Password).Wait();
            }
            member.ApplicationUser = null;

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(ownId))
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

        // POST: api/Members
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
          if (_context.Members == null)
          {
              return Problem("Entity set 'ApplicationContext.Members'  is null.");
          }

            _userManager.CreateAsync(member.ApplicationUser!, member.ApplicationUser!.Password).Wait();
            _userManager.AddToRoleAsync(member.ApplicationUser, "Member").Wait();
            member.Id = member.ApplicationUser!.Id;
            member.ApplicationUser = null;
            _context.Members.Add(member);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MemberExists(member.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMember", new { id = member.Id }, member);
        }

        // DELETE: api/Members/5
        [HttpDelete("{UserName}")]
        [Authorize(Roles ="Member")]
        public async Task<IActionResult> DeleteMember(string userName)
        {
            if (_context.Members == null)
            {
                return NotFound();
            }
            var member = await _userManager.FindByNameAsync(userName);

            if (member == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(member);
            if (!roles.Contains("Member"))
            {
                return Forbid();//Silmek istenen kullanıcının rolü "Member" değilse işlemi yasakla.
            }

            member.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
            
        } 

        private bool MemberExists(string id)
        {
            return (_context.Members?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
