using System.ComponentModel.DataAnnotations.Schema;

namespace SolvoTestTask.Server.Models.Entities
{
    public class Resource
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; }
        public bool IsArchived { get; set; }
    }
}
