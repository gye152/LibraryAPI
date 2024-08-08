using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Library_API.Models
{
	public class Language
	{
		[Key]
		[Required]
		[StringLength(3,MinimumLength =3)]
		[Column(TypeName = "char(3)")]
		public string Code { get; set; } = "";

		[Required]
		public string Name { get; set; } = "";

        public bool IsDeleted { get; set; } = false;

        [JsonIgnore]
		public List<LanguageBook>? LanguageBooks { get; set; }
	}
}


