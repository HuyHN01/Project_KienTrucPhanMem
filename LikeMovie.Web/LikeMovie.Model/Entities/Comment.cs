using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LikeMovie.DataAccess;

public partial class Comment
{
    [Key]
    [Column("CommentID")]
    public int CommentId { get; set; }

    [Column("MovieID")]
    public int MovieId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    public string? CommentText { get; set; }

    public int? Rating { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateCreated { get; set; }

    public int? Likes { get; set; }

    [ForeignKey("MovieId")]
    [InverseProperty("Comments")]
    public virtual Movie Movie { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Comments")]
    public virtual User User { get; set; } = null!;
}
