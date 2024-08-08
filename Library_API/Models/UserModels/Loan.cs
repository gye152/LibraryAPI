using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library_API.Models
{
	public class Loan
	{
		public int Id { get; set; }

		public int BookCopiesId { get; set; }

		public string? MembersId { get; set; } 

		public string? EmployeesId { get; set; }

        public int Amount { get; set; }

        [Required]
		public DateTime BorrowTime { get; set; }//ödünç alınan tarih !!

		[Required]
		public DateTime DueTime { get; set; }//kitabın teslim edilmesi gereken son tarih !!

		public DateTime? ReturnTime { get; set; } //kitabın geri getirildiği tarih !

		public bool IsReturned { get; set; } = false; //Teslim edildi mi ? (Default değer = False )

        public bool IsDeleted { get; set; } = false;

        [ForeignKey(nameof(MembersId))]
		public Member? Member { get; set; }

		[ForeignKey(nameof(EmployeesId))]
		public Employee? Employee { get; set; }

		[ForeignKey(nameof(BookCopiesId))]
		public BookCopy? BookCopy { get; set; }
	}
}

