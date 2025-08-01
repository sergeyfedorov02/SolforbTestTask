using System.ComponentModel.DataAnnotations.Schema;

namespace SolforbTestTask.Server.Models.Entities
{
    public class Balance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long ResourceId { get; set; }
        [ForeignKey("ResourceId")]
        public long MeasurementId { get; set; }
        [ForeignKey("MeasurementId")]
        public int Count { get; set; }
    }
}
