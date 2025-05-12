using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LikeMovie.DataAccess;

[Index("Email", Name = "UQ__Users__A9D10534BCDEF2F6", IsUnique = true)]
[Index("UserName", Name = "UQ__Users__C9F2845600774680", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(100)]
    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    public int Role { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateCreated { get; set; }

    public bool? IsActive { get; set; }

    [Column("AvatarURL")]
    [StringLength(200)]
    public string? AvatarUrl { get; set; }

    [Column("levelVIP")]
    public int? LevelVip { get; set; }

    [Column("TimeVIP", TypeName = "datetime")]
    public DateTime? TimeVip { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [InverseProperty("User")]
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    [InverseProperty("User")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("User")]
    public virtual ICollection<WatchHistory> WatchHistories { get; set; } = new List<WatchHistory>();
}
