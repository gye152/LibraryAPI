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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Net;
using System.Data;
using System.Diagnostics.Metrics;

namespace Library_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
      
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        

        public EmployeesController(ApplicationContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
           
        }

        // GET: api/Employees
        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {

          if (_context.Employees == null)
          {
              return NotFound();
          }
            var employee = await _context.Employees!.Include(e => e.ApplicationUser).ToListAsync();
            return employee;
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Employee>> GetEmployee(string id)
        {

          if (_context.Employees == null)
          {
              return NotFound();
          }

            // Include ApplicationUser and filter by id
            var employee = await _context.Employees
                                         .Include(e => e.ApplicationUser)
                                         .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> PutEmployee(string id, Employee employee, string? currentPassword=null)
        {
            ApplicationUser applicationUser = _userManager.FindByIdAsync(id).Result;

            if (id != employee.Id || applicationUser.IsDeleted)
            {
                return BadRequest();
            }

            applicationUser.IdNumber = employee.ApplicationUser!.IdNumber;
            applicationUser.Name = employee.ApplicationUser!.Name;
            applicationUser.MiddleName = employee.ApplicationUser!.MiddleName;
            applicationUser.FamilyName = employee.ApplicationUser!.FamilyName;
            applicationUser.Address = employee.ApplicationUser!.Address;
            applicationUser.Gender = employee.ApplicationUser!.Gender;
            applicationUser.BirthDate = employee.ApplicationUser!.BirthDate;
            applicationUser.RegisterDate = employee.ApplicationUser!.RegisterDate;
            applicationUser.Status = employee.ApplicationUser!.Status;
            applicationUser.Email = employee.ApplicationUser!.Email;

            _userManager.UpdateAsync(applicationUser).Wait();//identitynin yaptığı kısmı güncelledik.
            if (currentPassword != null)
            {
                _userManager.ChangePasswordAsync(applicationUser, currentPassword, applicationUser.Password).Wait();
            }
            employee.ApplicationUser = null;

            _context.Entry(employee).State = EntityState.Modified;//identitynin dışında kalan özelliklerin güncellemesini yaptık.

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
  
          if (_context.Employees == null)
          {
              return Problem("Entity set 'ApplicationContext.Employees'  is null.");
          }

            _userManager.CreateAsync(employee.ApplicationUser!, employee.ApplicationUser!.Password).Wait();
            _userManager.AddToRoleAsync(employee.ApplicationUser, "Employee").Wait();
            employee.Id = employee.ApplicationUser!.Id;
            employee.ApplicationUser = null;
            _context.Employees.Add(employee);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EmployeeExists(employee.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(string userName)
        {
            
            if (_context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _userManager.FindByNameAsync(userName);

            if (employee == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(employee);
            if (!roles.Contains("Employee"))
            {
                return Forbid();//Silmek istenen kullanıcının rolü "Employee" değilse işlemi yasakla.
            }

            employee.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
           
        }

        private bool EmployeeExists(string id)
        {
            return (_context.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        
    }
}
