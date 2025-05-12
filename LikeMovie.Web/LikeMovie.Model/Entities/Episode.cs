using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LikeMovie.DataAccess;

public partial class Episode
{
    [Key]
    [Column("EpisodeID")]
    public int EpisodeId { get; set; }

    [Column("SeasonID")]
    public int SeasonId { get; set; }

    public int EpisodeNumber { get; set; }

    [StringLength(255)]
    public string? Title { get; set; }

    public int? Duration { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    [Column("VideoURL")]
    public string? VideoUrl { get; set; }

    [ForeignKey("SeasonId")]
    [InverseProperty("Episodes")]
    public virtual Season Season { get; set; } = null!;
}
