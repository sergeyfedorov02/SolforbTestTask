using System.ComponentModel.DataAnnotations.Schema;

namespace SolforbTestTask.Server.Models.Entities
{
    public class ReceiptsResource
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long DocumentId { get; set; }

        public long ResourceId { get; set; }
        [ForeignKey("ResourceId")]
        public Resource Resource { get; set; }

        public long MeasurementId { get; set; }
        [ForeignKey("MeasurementId")]
        public Measurement Measurement { get; set; }

        public int Count { get; set; }

        [ForeignKey("DocumentId")]
        public ReceiptsDocument ReceiptsDocument { get; set; }
    }
}
