using System;
using System.Collections.Generic;

namespace LikeMovie.Model.Entities
{
    public class Seasons
    {
        public Seasons()
        {
            this.Episodes = new HashSet<Episodes>();
        }

        public int SeasonID { get; set; }
        public int MovieID { get; set; }
        public int SeasonNumber { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? TotalEpisodes { get; set; }

        public virtual ICollection<Episodes> Episodes { get; set; }
        public virtual Movies Movies { get; set; }
    }
}