using System;
using System.Collections.Generic;

namespace LikeMovie.Model.Entities 
{
    public class Movies 
    {
        public Movies()
        {
            this.Comments = new HashSet<Comments>();
            this.Favorites = new HashSet<Favorites>();
            this.Ratings = new HashSet<Ratings>();
            this.Seasons = new HashSet<Seasons>();
            this.WatchHistory = new HashSet<WatchHistory>();
            this.Genres = new HashSet<Genres>();
            this.PosterMovie = new HashSet<PosterMovie>();
        }

        public int MovieID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReleaseDate { get; set; } 
        public int? Duration { get; set; }      
        public decimal? Rating { get; set; }    
        public int Type { get; set; }         
        public string Director { get; set; }
        public string Thumbnail { get; set; }
        public string TrailerURL { get; set; }
        public string MovieURL { get; set; }
        public int? ViewCount { get; set; }       
        public int? VIPType { get; set; }         
        public virtual ICollection<Comments> Comments { get; set; }
        public virtual ICollection<Favorites> Favorites { get; set; }
        public virtual ICollection<Ratings> Ratings { get; set; }
        public virtual ICollection<Seasons> Seasons { get; set; }
        public virtual ICollection<WatchHistory> WatchHistory { get; set; }
        public virtual ICollection<Genres> Genres { get; set; }
        public virtual ICollection<PosterMovie> PosterMovie { get; set; }
    }
}