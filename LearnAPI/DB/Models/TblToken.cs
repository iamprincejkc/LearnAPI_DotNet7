using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LearnAPI.DB.Models;

[Table("tbl_token")]
public partial class TblToken
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("userid")]
    public int? Userid { get; set; }

    [Column("tokenid")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Tokenid { get; set; }

    [Column("refreshtoken")]
    [Unicode(false)]
    public string? Refreshtoken { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("TblTokens")]
    public virtual TblUser? User { get; set; }
}
