﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MovieEntities : DbContext
    {
        public MovieEntities()
            : base("name=MovieEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Comments> Comments { get; set; }
        public virtual DbSet<Episodes> Episodes { get; set; }
        public virtual DbSet<Favorites> Favorites { get; set; }
        public virtual DbSet<Ratings> Ratings { get; set; }
        public virtual DbSet<Seasons> Seasons { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<WatchHistory> WatchHistory { get; set; }
        public virtual DbSet<Genres> Genres { get; set; }
        public virtual DbSet<Movies> Movies { get; set; }
        public virtual DbSet<PosterMovie> PosterMovie { get; set; }
        public virtual DbSet<DiscountCodes> DiscountCodes { get; set; }
        public virtual DbSet<Payments> Payments { get; set; }
        public virtual DbSet<SubscriptionPlans> SubscriptionPlans { get; set; }
        public virtual DbSet<AdminMovie> AdminMovie { get; set; }
        public virtual DbSet<MENUs> MENUs { get; set; }
    }
}
