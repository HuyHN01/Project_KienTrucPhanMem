using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LikeMovie.DataAccess;

[Table("AdminMovie")]
public partial class AdminMovie
{
    [Key]
    [Column("AdminID")]
    public int AdminId { get; set; }

    [StringLength(60)]
    public string? NameAd { get; set; }

    [StringLength(50)]
    public string? Email { get; set; }

    [StringLength(100)]
    public string UsernameAd { get; set; } = null!;

    public string? PasswordAd { get; set; }

    [Column("AvartarURL")]
    [StringLength(200)]
    public string? AvartarUrl { get; set; }
}
