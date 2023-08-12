using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LearnAPI.DB.Models;

[Table("tbl_productImage")]
public partial class TblProductImage
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("productid")]
    public int? Productid { get; set; }

    [Column("productimage", TypeName = "image")]
    public byte[]? Productimage { get; set; }

    [ForeignKey("Productid")]
    [InverseProperty("TblProductImages")]
    public virtual TblProduct? Product { get; set; }
}
