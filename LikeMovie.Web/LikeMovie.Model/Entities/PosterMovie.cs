using System;

namespace LikeMovie.Model.Entities
{
    public class PosterMovie
    {
        public int PosterID { get; set; }
        public int? MovieID { get; set; }
        public string PosterURL { get; set; }
        public bool? IsSlider { get; set; }
        public int? SliderOrder { get; set; }

        public virtual Movies Movies { get; set; }
    }
}