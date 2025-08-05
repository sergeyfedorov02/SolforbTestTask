using System.ComponentModel.DataAnnotations.Schema;

namespace SolforbTestTask.Server.Models.Entities
{
    public class ShipmentsDocument
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int Number { get; set; }

        public long ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        public DateTime Date { get; set; }
        public int Status { get; set; }
    }
}
