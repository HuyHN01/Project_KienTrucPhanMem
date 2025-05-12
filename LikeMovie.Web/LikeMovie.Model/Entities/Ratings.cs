using System;

namespace LikeMovie.Model.Entities
{
    public class Ratings
    {
        public int RatingID { get; set; }
        public int MovieID { get; set; }
        public int UserID { get; set; }
        public int Rating { get; set; } 
        public DateTime? DateRated { get; set; }

        public virtual Users Users { get; set; }
        public virtual Movies Movies { get; set; }
    }
}