using System.ComponentModel.DataAnnotations.Schema;

namespace SolforbTestTask.Server.Models.Entities
{
    public class ReceiptsDocument
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long Number { get; set; }
        public DateTime Date { get; set; }

        // навигационное свойство
        public ReceiptsResource ReceiptsResource { get; set; }
    }
}
