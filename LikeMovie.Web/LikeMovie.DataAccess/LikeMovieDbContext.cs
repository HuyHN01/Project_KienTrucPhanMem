using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LikeMovie.DataAccess;

public partial class LikeMovieDbContext : DbContext
{
    public LikeMovieDbContext()
    {
    }

    public LikeMovieDbContext(DbContextOptions<LikeMovieDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminMovie> AdminMovies { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<DiscountCode> DiscountCodes { get; set; }

    public virtual DbSet<Episode> Episodes { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PosterMovie> PosterMovies { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Season> Seasons { get; set; }

    public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WatchHistory> WatchHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=45.119.85.244;Database=QL_Movie;User ID=QL_Movie;Password=QL_Movie@123;TrustServerCertificate=True;MultipleActiveResultSets=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminMovie>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK_Admin");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFAA25B7B38C");

            entity.ToTable(tb => tb.HasTrigger("trg_UpdateMovieRating"));

            entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Movie).WithMany(p => p.Comments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comments__MovieI__3C69FB99");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Users");
        });

        modelBuilder.Entity<DiscountCode>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__E43F6DF66947B248");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Plan).WithMany(p => p.DiscountCodes).HasConstraintName("FK__DiscountC__PlanI__60A75C0F");
        });

        modelBuilder.Entity<Episode>(entity =>
        {
            entity.HasKey(e => e.EpisodeId).HasName("PK__Episodes__AC667615DA6D063E");

            entity.HasOne(d => d.Season).WithMany(p => p.Episodes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Episodes__Season__52593CB8");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PK__Favorite__CE74FAF570BD04C9");

            entity.HasOne(d => d.Movie).WithMany(p => p.Favorites)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Favorites__Movie__4BAC3F29");

            entity.HasOne(d => d.User).WithMany(p => p.Favorites)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Favorites__UserI__4CA06362");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK__Genres__0385055E4E5B03A0");

            entity.Property(e => e.GenreId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Menu");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK__Movies__4BD2943AA248C85F");

            entity.Property(e => e.ViewCount).HasDefaultValue(0);

            entity.HasMany(d => d.Genres).WithMany(p => p.Movies)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieGenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .HasConstraintName("FK__MovieGenr__Genre__5629CD9C"),
                    l => l.HasOne<Movie>().WithMany()
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_MovieGenres_Movies"),
                    j =>
                    {
                        j.HasKey("MovieId", "GenreId").HasName("PK__MovieGen__BBEAC46F3CBE6DBB");
                        j.ToTable("MovieGenres");
                        j.IndexerProperty<int>("MovieId").HasColumnName("MovieID");
                        j.IndexerProperty<int>("GenreId").HasColumnName("GenreID");
                    });
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A58769353B0");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TransactionDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TransactionStatus).HasDefaultValue("Pending");

            entity.HasOne(d => d.Plan).WithMany(p => p.Payments).HasConstraintName("FK__Payments__PlanID__656C112C");

            entity.HasOne(d => d.User).WithMany(p => p.Payments).HasConstraintName("FK__Payments__UserID__6477ECF3");
        });

        modelBuilder.Entity<PosterMovie>(entity =>
        {
            entity.HasOne(d => d.Movie).WithMany(p => p.PosterMovies).HasConstraintName("FK_PosterMovie_Movies");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__Ratings__FCCDF85CB4707D39");

            entity.Property(e => e.DateRated).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Movie).WithMany(p => p.Ratings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ratings__MovieID__412EB0B6");
        });

        modelBuilder.Entity<Season>(entity =>
        {
            entity.HasKey(e => e.SeasonId).HasName("PK__Seasons__C1814E1812BF9F89");

            entity.HasOne(d => d.Movie).WithMany(p => p.Seasons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seasons__MovieID__4F7CD00D");
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__Subscrip__755C22D7B7692B19");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC32454133");

            entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<WatchHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__WatchHis__4D7B4ADD0070BF5C");

            entity.Property(e => e.DateWatched).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Movie).WithMany(p => p.WatchHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WatchHist__Movie__46E78A0C");

            entity.HasOne(d => d.User).WithMany(p => p.WatchHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WatchHist__UserI__47DBAE45");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
