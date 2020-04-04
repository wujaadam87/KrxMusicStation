using KrxMusicStation.Data;
using Microsoft.EntityFrameworkCore;

namespace KrxMusicStation.KrxMusicStation.Data
{
    public partial class KrxMusicStationDBContext : DbContext
    {
        public KrxMusicStationDBContext()
        {
        }

        public KrxMusicStationDBContext(DbContextOptions<KrxMusicStationDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Album> Albums { get; set; }
        public virtual DbSet<Bitrate> Bitrates { get; set; }
        public virtual DbSet<Path> Paths { get; set; }
        public virtual DbSet<Song> Songs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Album>(entity =>
            {
                entity.HasKey(e => e.IdAlbum);

                entity.ToTable("album");

                entity.HasIndex(e => e.BCompilation)
                    .HasName("idxAlbum_1");

                entity.HasIndex(e => e.IdInfoSetting)
                    .HasName("idxAlbum_3");

                entity.HasIndex(e => e.StrAlbum)
                    .HasName("idxAlbum");

                entity.HasIndex(e => e.StrMusicBrainzAlbumId)
                    .HasName("idxAlbum_2")
                    .IsUnique();

                entity.Property(e => e.IdAlbum)
                    .HasColumnName("idAlbum")
                    .ValueGeneratedNever();

                entity.Property(e => e.BCompilation)
                    .HasColumnName("bCompilation")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.BScrapedMbid).HasColumnName("bScrapedMBID");

                entity.Property(e => e.FRating)
                    .HasColumnName("fRating")
                    .HasColumnType("FLOAT");

                entity.Property(e => e.IUserrating).HasColumnName("iUserrating");

                entity.Property(e => e.IVotes).HasColumnName("iVotes");

                entity.Property(e => e.IYear).HasColumnName("iYear");

                entity.Property(e => e.IdInfoSetting).HasColumnName("idInfoSetting");

                entity.Property(e => e.LastScraped)
                    .HasColumnName("lastScraped")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.StrAlbum)
                    .HasColumnName("strAlbum")
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.StrArtistDisp).HasColumnName("strArtistDisp");

                entity.Property(e => e.StrArtistSort).HasColumnName("strArtistSort");

                entity.Property(e => e.StrGenres).HasColumnName("strGenres");

                entity.Property(e => e.StrImage).HasColumnName("strImage");

                entity.Property(e => e.StrLabel).HasColumnName("strLabel");

                entity.Property(e => e.StrMoods).HasColumnName("strMoods");

                entity.Property(e => e.StrMusicBrainzAlbumId).HasColumnName("strMusicBrainzAlbumID");

                entity.Property(e => e.StrReleaseGroupMbid).HasColumnName("strReleaseGroupMBID");

                entity.Property(e => e.StrReleaseType).HasColumnName("strReleaseType");

                entity.Property(e => e.StrReview).HasColumnName("strReview");

                entity.Property(e => e.StrStyles).HasColumnName("strStyles");

                entity.Property(e => e.StrThemes).HasColumnName("strThemes");

                entity.Property(e => e.StrType).HasColumnName("strType");
            });

            modelBuilder.Entity<Bitrate>(entity =>
            {
                entity.HasKey(e => e.IdSong);

                entity.ToTable("bitrate");

                entity.Property(e => e.IdSong)
                    .HasColumnName("idSong")
                    .ValueGeneratedNever();

                entity.Property(e => e.Bitrate1).HasColumnName("bitrate");
            });

            modelBuilder.Entity<Path>(entity =>
            {
                entity.HasKey(e => e.IdPath);

                entity.ToTable("path");

                entity.HasIndex(e => e.StrPath)
                    .HasName("idxPath");

                entity.Property(e => e.IdPath)
                    .HasColumnName("idPath")
                    .ValueGeneratedNever();

                entity.Property(e => e.StrHash).HasColumnName("strHash");

                entity.Property(e => e.StrPath)
                    .HasColumnName("strPath")
                    .HasColumnType("varchar(512)");
            });

            modelBuilder.Entity<Song>(entity =>
            {
                entity.HasKey(e => e.IdSong);

                entity.ToTable("song");

                entity.HasIndex(e => e.ITimesPlayed)
                    .HasName("idxSong1");

                entity.HasIndex(e => e.IdAlbum)
                    .HasName("idxSong3");

                entity.HasIndex(e => e.Lastplayed)
                    .HasName("idxSong2");

                entity.HasIndex(e => e.StrTitle)
                    .HasName("idxSong");

                entity.HasIndex(e => new { e.IdPath, e.StrFileName })
                    .HasName("idxSong6");

                entity.HasIndex(e => new { e.IdAlbum, e.ITrack, e.StrMusicBrainzTrackId })
                    .HasName("idxSong7")
                    .IsUnique();

                entity.Property(e => e.IdSong)
                    .HasColumnName("idSong")
                    .ValueGeneratedNever();

                entity.Property(e => e.Comment).HasColumnName("comment");

                entity.Property(e => e.DateAdded).HasColumnName("dateAdded");

                entity.Property(e => e.IDuration).HasColumnName("iDuration");

                entity.Property(e => e.IEndOffset).HasColumnName("iEndOffset");

                entity.Property(e => e.IStartOffset).HasColumnName("iStartOffset");

                entity.Property(e => e.ITimesPlayed).HasColumnName("iTimesPlayed");

                entity.Property(e => e.ITrack).HasColumnName("iTrack");

                entity.Property(e => e.IYear).HasColumnName("iYear");

                entity.Property(e => e.IdAlbum).HasColumnName("idAlbum");

                entity.Property(e => e.IdPath).HasColumnName("idPath");

                entity.Property(e => e.Lastplayed)
                    .HasColumnName("lastplayed")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Mood).HasColumnName("mood");

                entity.Property(e => e.Rating)
                    .HasColumnName("rating")
                    .HasColumnType("FLOAT");

                entity.Property(e => e.StrArtistDisp).HasColumnName("strArtistDisp");

                entity.Property(e => e.StrArtistSort).HasColumnName("strArtistSort");

                entity.Property(e => e.StrFileName).HasColumnName("strFileName");

                entity.Property(e => e.StrGenres).HasColumnName("strGenres");

                entity.Property(e => e.StrMusicBrainzTrackId).HasColumnName("strMusicBrainzTrackID");

                entity.Property(e => e.StrReplayGain).HasColumnName("strReplayGain");

                entity.Property(e => e.StrTitle)
                    .HasColumnName("strTitle")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Userrating).HasColumnName("userrating");

                entity.Property(e => e.Votes).HasColumnName("votes");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
