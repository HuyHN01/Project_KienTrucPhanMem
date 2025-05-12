namespace LikeMovie.Model.Entities
{
    public class Favorites
    {
        public int FavoriteID { get; set; }
        public int MovieID { get; set; }
        public int UserID { get; set; }

        public virtual Users Users { get; set; }
        public virtual Movies Movies { get; set; }
    }
}