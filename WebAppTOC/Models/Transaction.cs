using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppTOC.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int WalletId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(10)]
        public string Type { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; }

        // Propiedad de navegación para la relación con Wallet
        [ForeignKey("WalletId")]
        public virtual Wallet? Wallet { get; set; }
    }
} 