using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LikeMovie.DataAccess;

public partial class Movie
{
    [Key]
    [Column("MovieID")]
    public int MovieId { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public int? Duration { get; set; }

    [Column(TypeName = "decimal(3, 2)")]
    public decimal? Rating { get; set; }

    public int Type { get; set; }

    [StringLength(100)]
    public string? Director { get; set; }

    public string? Thumbnail { get; set; }

    [Column("TrailerURL")]
    public string? TrailerUrl { get; set; }

    [Column("MovieURL")]
    public string? MovieUrl { get; set; }

    public int? ViewCount { get; set; }

    [Column("VIPType")]
    public int? Viptype { get; set; }

    [InverseProperty("Movie")]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [InverseProperty("Movie")]
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    [InverseProperty("Movie")]
    public virtual ICollection<PosterMovie> PosterMovies { get; set; } = new List<PosterMovie>();

    [InverseProperty("Movie")]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    [InverseProperty("Movie")]
    public virtual ICollection<Season> Seasons { get; set; } = new List<Season>();

    [InverseProperty("Movie")]
    public virtual ICollection<WatchHistory> WatchHistories { get; set; } = new List<WatchHistory>();

    [ForeignKey("MovieId")]
    [InverseProperty("Movies")]
    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
