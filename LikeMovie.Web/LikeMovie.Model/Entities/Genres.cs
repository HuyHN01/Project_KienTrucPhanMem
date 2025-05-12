using System.Collections.Generic;

namespace LikeMovie.Model.Entities
{
    public class Genres
    {
        public Genres()
        {
            this.Movies = new HashSet<Movies>();
        }

        public int GenreID { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Movies> Movies { get; set; }
    }
}