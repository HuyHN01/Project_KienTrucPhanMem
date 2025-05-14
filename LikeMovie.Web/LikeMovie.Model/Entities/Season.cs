using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LikeMovie.Model.Entities;

public partial class Season
{
    [Key]
    [Column("SeasonID")]
    public int SeasonId { get; set; }

    [Column("MovieID")]
    public int MovieId { get; set; }

    public int SeasonNumber { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public int? TotalEpisodes { get; set; }

    [InverseProperty("Season")]
    public virtual ICollection<Episode> Episodes { get; set; } = new List<Episode>();

    [ForeignKey("MovieId")]
    [InverseProperty("Seasons")]
    public virtual Movie Movie { get; set; } = null!;
}
