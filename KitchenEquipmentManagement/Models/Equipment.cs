using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KitchenEquipmentManagement.Models
{
    public class Equipment
    {
        [Key]
        public int EquipmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string SerialNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Condition { get; set; } = string.Empty; // "Working" or "Not Working"

        [Required]
        public int UserId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<RegisteredEquipment> RegisteredEquipments { get; set; } = new List<RegisteredEquipment>();

        // Helper property to get current site
        [NotMapped]
        public Site? CurrentSite => RegisteredEquipments.FirstOrDefault()?.Site;

        [NotMapped]
        public string CurrentSiteDescription => CurrentSite?.Description ?? "Not Assigned";
    }
}