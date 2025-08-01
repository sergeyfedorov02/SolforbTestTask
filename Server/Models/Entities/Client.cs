using System.ComponentModel.DataAnnotations.Schema;

namespace SolvoTestTask.Server.Models.Entities
{
    public class Client
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsArchived { get; set; }
    }
}
