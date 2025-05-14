using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LikeMovie.Model.Entities;

[Table("WatchHistory")]
public partial class WatchHistory
{
    [Key]
    [Column("HistoryID")]
    public int HistoryId { get; set; }

    [Column("MovieID")]
    public int MovieId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateWatched { get; set; }

    public int? Progress { get; set; }

    [ForeignKey("MovieId")]
    [InverseProperty("WatchHistories")]
    public virtual Movie Movie { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("WatchHistories")]
    public virtual User User { get; set; } = null!;
}
