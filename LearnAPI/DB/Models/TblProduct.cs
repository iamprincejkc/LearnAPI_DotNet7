using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LearnAPI.DB.Models;

[Table("tbl_product")]
public partial class TblProduct
{
    [Key]
    [Column("productid")]
    public int Productid { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string? Name { get; set; }

    [Column("price", TypeName = "decimal(18, 2)")]
    public decimal? Price { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<TblProductImage> TblProductImages { get; set; } = new List<TblProductImage>();
}
