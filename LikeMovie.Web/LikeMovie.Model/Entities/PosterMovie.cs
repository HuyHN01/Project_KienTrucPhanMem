using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LikeMovie.Model.Entities;

[Table("PosterMovie")]
public partial class PosterMovie
{
    [Key]
    [Column("PosterID")]
    public int PosterId { get; set; }

    [Column("MovieID")]
    public int? MovieId { get; set; }

    [Column("PosterURL")]
    [StringLength(200)]
    public string? PosterUrl { get; set; }

    public bool? IsSlider { get; set; }

    public int? SliderOrder { get; set; }

    [ForeignKey("MovieId")]
    [InverseProperty("PosterMovies")]
    public virtual Movie? Movie { get; set; }
}
