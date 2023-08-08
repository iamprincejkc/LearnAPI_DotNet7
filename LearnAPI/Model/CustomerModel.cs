using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LearnAPI.Model
{
    public class CustomerModel
    {
        [Key]
        public int Code { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? Name { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? Email { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? Phone { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? CreditLimit { get; set; }

        public bool? IsActive { get; set; }

        public int? TaxCode { get; set; }
        public string? StatusName { get; set; }
    }
}
