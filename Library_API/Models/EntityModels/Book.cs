using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Library_API.Models
{
	public class Book
	{
		public int Id { get; set; }

		[StringLength(13,MinimumLength = 10)] //ısbn max 13 karakter olabilir ve çince falan olamaz. Bu yüzden nvarchar değil,
		[Column(TypeName = "varchar(13)")] //nvarchar ı varchar(13) yaptık.  varchar yaptık . 
		public string? ISBN { get; set; }

		[Required] //kitaba başlık yazılmadığında arayüzde uyarı verir.
		[StringLength(2000)]
		public string Title { get; set; } = "";//bir kitabın başlığı olmalıdır.Başlığı olmayan bir kitap olamaz.

		[Range(1,short.MaxValue)]
		public short PageCount { get; set; }//sayfa sayısı.

		[Range(-4000,2100)]
		public short PublishingYear { get; set; }//basım yılı.

		[StringLength(5000)]//5000 den fazla karakter girildiğinde arayüzde uyarı verir.
		public string? Description { get; set; }//açıklama.

		[Range(0,int.MaxValue)]
		public int PrintCount { get; set; }//basım sayısı.

		public bool Banned { get; set; }//yasaklı mı değil mi

		public int PublisherId { get; set; }

		[NotMapped]
		public List<long>? AuthorIds { get; set; }

		[NotMapped]
		public List<string>? LanguageCodes { get; set; }

		[NotMapped]
		public List<short>? SubCategoryIds { get; set; }

		
		[StringLength(6,MinimumLength =3)]
		[Column(TypeName ="varchar(6)")]
		public string LocationShelf { get; set; } = "";

		[JsonIgnore]
		public List<AuthorBook>? AuthorBooks { get; set; }//book ile author ar. ara tablo .

		[ForeignKey(nameof(PublisherId))] //one to many.
		public Publisher? Publisher { get; set; }

		[JsonIgnore]
		public List<SubCategoryBook>? SubCategoryBooks { get; set; }//many to many ilişkiden dolayı olan ara tablo.

		[JsonIgnore]
		public List<LanguageBook>? LanguageBooks { get; set; }//include(controller içinde) için gerekli olan bir şey yani kitabı görüntülediğimizde 
															 //kitabın dilini de görüntülemek istersek bunu yazmak zorundayız.
															 //Aynı şey yukarıda yazdığımız ara tablolar içinde geçerli.
		[JsonIgnore]
		[ForeignKey(nameof(LocationShelf))]
		public Location? Location { get; set; }

		public string? UserId { get; set; } //kitap kaydeden employee' nin id' si .

		public bool IsDeleted { get; set; } = false;







	}
}

