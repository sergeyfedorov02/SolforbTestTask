using System.ComponentModel.DataAnnotations.Schema;

namespace SolvoTestTask.Server.Models.Entities
{
    public class IncomesDocument
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
    }
}
