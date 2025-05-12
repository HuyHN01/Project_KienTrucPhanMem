using System;

namespace LikeMovie.Model.Entities
{
    public class WatchHistory
    {
        public int HistoryID { get; set; }
        public int MovieID { get; set; }
        public int UserID { get; set; }
        public DateTime? DateWatched { get; set; }
        public int? Progress { get; set; }

        public virtual Users Users { get; set; }
        public virtual Movies Movies { get; set; }
    }
}