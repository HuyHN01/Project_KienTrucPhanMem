//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LikeMovies.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ratings
    {
        public int RatingID { get; set; }
        public int MovieID { get; set; }
        public int UserID { get; set; }
        public int Rating { get; set; }
        public Nullable<System.DateTime> DateRated { get; set; }
    
        public virtual Users Users { get; set; }
        public virtual Movies Movies { get; set; }
    }
}
