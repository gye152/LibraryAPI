using System;
using Library_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Library_API.Data
{
	public class ApplicationContext : IdentityDbContext<ApplicationUser> // ? bu kısmı dbcontexten neden ıdenitity db context yaptık sor 
	{
		public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
		{
		}



        public DbSet<Author>? Authors { get; set; }
		public DbSet<AuthorBook>? AuthorBooks { get; set; }
		public DbSet<Book>? Books { get; set; }
		public DbSet<Category>? Categories { get; set; }
		public DbSet<Language>? Languages { get; set; }
		public DbSet<LanguageBook>? LanguageBooks { get; set; }
		public DbSet<Location>? Locations { get; set; }
		public DbSet<Publisher>? Publishers { get; set; }
		public DbSet<SubCategory>? SubCategories { get; set; }
		public DbSet<SubCategoryBook>? SubCategoryBooks { get; set; }
		public DbSet<Member>? Members { get; set; }
		public DbSet<Employee>? Employees { get; set; }
        public DbSet<BookCopy>? BookCopies { get; set; }
        public DbSet<Loan>? Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<AuthorBook>().HasKey(a => new { a.AuthorsId, a.BooksId });

			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<LanguageBook>().HasKey(b => new { b.LanguagesCode, b.BooksId });

			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<SubCategoryBook>().HasKey(c => new { c.SubCategoriesId, c.BooksId });



        }

     
    }
}

