using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessagingApi.Models
{
    public class Messages
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        [Required]
        public DateTime SendDate { get; set; }

        [ForeignKey("Sender")]
        public int  SenderId{ get; set; }
        public virtual User Sender{ get; set; }
        [ForeignKey("Receiver")]
        public int ReceiverId { get; set; }
        public virtual User Receiver { get; set; }
    }
}
