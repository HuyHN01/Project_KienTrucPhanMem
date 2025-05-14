using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LikeMovie.Model.Entities;

public partial class Rating
{
    [Key]
    [Column("RatingID")]
    public int RatingId { get; set; }

    [Column("MovieID")]
    public int MovieId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [Column("Rating")]
    public int Rating1 { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateRated { get; set; }

    [ForeignKey("MovieId")]
    [InverseProperty("Ratings")]
    public virtual Movie Movie { get; set; } = null!;
}
