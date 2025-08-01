using System.ComponentModel.DataAnnotations.Schema;

namespace SolvoTestTask.Server.Models.Entities
{
    public class IncomesResource
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int Number { get; set; }
        public long ClientId { get; set; }
        [ForeignKey("ClientId")]
        public DateTime Date { get; set; }
        public bool IsArchived { get; set; }
    }
}
