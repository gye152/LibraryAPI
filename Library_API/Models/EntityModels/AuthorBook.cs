using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Library_API.Models
{
	public class AuthorBook
	{
		public long AuthorsId { get; set; }

		public int BooksId { get; set; }

		[ForeignKey(nameof(AuthorsId))] //Bu oluşturduğumuz foreign key (AuthorsId) Author modelindeki primary key(Id) ile eşleşir. 
		public Author? Author { get; set; } // Bu sayede burada 

		[JsonIgnore]
		[ForeignKey(nameof(BooksId))] //Book sınıfının PK si ile (Id) burada yazdığımız FK'yi (BooksId) ilişkilendirdik.
		public Book? Book { get; set; }//Book adında Book sınıfından bir ilişki tanımladık.






	}
}

