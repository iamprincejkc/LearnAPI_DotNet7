using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LearnAPI.DB.Models;

[Table("tbl_customer")]
public partial class TblCustomer
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
}
