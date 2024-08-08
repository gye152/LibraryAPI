using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Library_API.Models
{
	public class BookCopy
	{
		public int Id { get; set; }

		public int BooksId { get; set; }

		public bool IsAvailable { get; set; } = true; //kitap boşta mı dolu mu ! Default değeri true

        [ForeignKey(nameof(BooksId))]
        public Book? Book { get; set; }

		public bool IsDeleted { get; set; } = false;




    }
}

