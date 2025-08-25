using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KitchenEquipmentManagement.Models
{
    public class Site
    {
        [Key]
        public int SiteId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public bool Active { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<RegisteredEquipment> RegisteredEquipments { get; set; } = new List<RegisteredEquipment>();

        [NotMapped]
        public string Status => Active ? "Active" : "Inactive";

        [NotMapped]
        public int WorkingEquipmentCount => RegisteredEquipments?
            .Count(re => re.Equipment?.Condition == "Working") ?? 0;
    }
}