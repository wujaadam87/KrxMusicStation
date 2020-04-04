using KrxMusicStation.Data;
using KrxMusicStation.KrxMusicStation.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace KrxMusicStation.Pages
{
    public class IndexModel : PageModel
    {
        private readonly KrxMusicStationDBContext _context;

        public IndexModel(KrxMusicStationDBContext context)
        {
            _context = context;
        }

        public PaginatedList<Song> Songs { get; set; }
        public IQueryable<Album> Albums { get; set; }

        public string ArtistSort { get; set; }
        public string SongSort { get; set; }
        public string AlbumSort { get; set; }
        public string DurationSort { get; set; }

        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public async Task OnGetAsync(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            SongSort = sortOrder == "Song" ? "song_desc" : "Song";
            AlbumSort = sortOrder == "Album" ? "album_desc" : "Album";
            ArtistSort = sortOrder == "Artist" ? "artist_desc" : "Artist";
            DurationSort = sortOrder == "Duration" ? "duration_desc" : "Duration";

            if (searchString != null)
                pageIndex = 1;
            else
                searchString = currentFilter;

            CurrentFilter = searchString;

            Albums = from a in _context.Albums select a;
            var songs = from s in _context.Songs select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                var bySongOrArtist = songs.Where(s => s.StrArtistDisp.ToUpper().Contains(searchString.ToUpper())
                                       || s.StrTitle.ToUpper().Contains(searchString.ToUpper()));
                var byAlbum = from s in _context.Songs
                              join a in _context.Albums on s.IdAlbum equals a.IdAlbum
                              where a.StrAlbum.ToUpper() == searchString.ToUpper()
                              || a.StrArtistDisp.ToUpper() == searchString.ToUpper()
                              select s;
                songs = byAlbum.Concat(bySongOrArtist).Distinct();
            }

            switch (sortOrder)
            {
                case "Song":
                    songs = songs.OrderBy(s => s.StrTitle);
                    break;
                case "name_desc":
                    songs = songs.OrderByDescending(s => s.StrTitle);
                    break;
                case "Artist":
                    songs = songs.OrderBy(s => s.StrArtistDisp);
                    break;
                case "artist_desc":
                    songs = songs.OrderByDescending(s => s.StrArtistDisp);
                    break;
                case "Album":
                    songs = from s in _context.Songs
                            join a in _context.Albums on s.IdAlbum equals a.IdAlbum
                            orderby a.StrAlbum
                            select s;
                    break;
                case "album_desc":
                    songs = from s in _context.Songs
                            join a in _context.Albums on s.IdAlbum equals a.IdAlbum
                            orderby a.StrAlbum descending
                            select s;
                    break;
                case "Duration":
                    songs = songs.OrderBy(s => s.IDuration);
                    break;
                case "duration_desc":
                    songs = songs.OrderByDescending(s => s.IDuration);
                    break;
                default:
                    songs = songs.OrderBy(s => s.IdSong);
                    break;
            }

            int pageSize = 50;
            Songs = await PaginatedList<Song>.CreateAsync(
                songs.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
