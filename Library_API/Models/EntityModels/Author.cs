using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Library_API.Models
{
	public class Author
	{
		public long Id { get; set; }

		[Required] //Boş geçilemez anlamına gelir. Kırmızı uyarı verir doldurmadığımızda.
		[StringLength(800)]
		public string FullName { get; set; } = "";

        [StringLength(1000)] 
        [Column(TypeName = "nvarchar(1000)")]
		public string? Biography { get; set; }

		[Range(-4000,2100)]
		public short? BirthYear { get; set; }

		[Range(-4000,2100)]
		public short? DeathYear { get; set; }

		public string? UserId { get; set; } //yazarı kaydeden employeenin id ' si .

		[JsonIgnore] 
		public List<AuthorBook>? AuthorBooks { get; set; }

		public bool IsDeleted { get; set; } = false;


	}
}

