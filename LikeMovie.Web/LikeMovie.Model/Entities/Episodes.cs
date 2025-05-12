using System;

namespace LikeMovie.Model.Entities
{
    public class Episodes
    {
        public int EpisodeID { get; set; }
        public int SeasonID { get; set; }
        public int EpisodeNumber { get; set; }
        public string Title { get; set; }
        public int? Duration { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string VideoURL { get; set; }

        public virtual Seasons Seasons { get; set; }
    }
}