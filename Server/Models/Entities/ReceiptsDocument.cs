using System.ComponentModel.DataAnnotations.Schema;

namespace SolforbTestTask.Server.Models.Entities
{
    public class ReceiptsDocument
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Number { get; set; }
        public DateOnly Date { get; set; }

        // навигационное свойство
        public List<ReceiptsResource> ReceiptsResources { get; set; }
    }
}
