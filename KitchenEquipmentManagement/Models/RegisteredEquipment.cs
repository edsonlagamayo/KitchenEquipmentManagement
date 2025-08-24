using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KitchenEquipmentManagement.Models
{
    public class RegisteredEquipment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EquipmentId { get; set; }

        [Required]
        public int SiteId { get; set; }

        public DateTime RegisteredDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("EquipmentId")]
        public virtual Equipment Equipment { get; set; } = null!;

        [ForeignKey("SiteId")]
        public virtual Site Site { get; set; } = null!;
    }
}