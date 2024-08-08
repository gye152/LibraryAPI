using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Library_API.Models
{
	public class Category
	{
		public short Id { get; set; }

		[Required]
		[StringLength(800)]
		[Column(TypeName = "nvarchar(800)")]
		public string Name { get; set; } = "";

		public bool IsDeleted { get; set; } = false;

        //Bir kategorinin birden fazla alt kategorisi vardır.
        //Ara tablo oluşturmadan önceki tanımladığımız many to many ilişki aş. gibidir.
        //public List<SubCategory>? SubCategories { get; set; }

    }
}

