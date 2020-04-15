using KrxMusicStation.Data;
using KrxMusicStation.KrxMusicStation.Data;
using KrxMusicStation.Logic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KrxMusicStation.Pages
{
    public class IndexModel : PageModel
    {
        private readonly KrxMusicStationDBContext context;
        private readonly IPlaylistService playlistService;

        public IndexModel(KrxMusicStationDBContext context,
                            IPlaylistService playlistService)
        {
            this.context = context;
            this.playlistService = playlistService;
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
            await InitializePageWith(sortOrder, currentFilter, searchString, pageIndex);
        }

        public async Task OnPostAsync(string sortOrder,
            string currentFilter, string searchString, int? pageIndex, string playListName)
        {
            await InitializePageWith(sortOrder, currentFilter, searchString, pageIndex);

            playlistService.SavePlaylist(playListName, Songs, out string message);

            RedirectToPage("./Index");
        }

        private async Task InitializePageWith(string sortOrder,
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

            Albums = from a in context.Albums select a;
            var songs = from s in context.Songs select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                var bySongOrArtist = songs.Where(s => s.StrArtistDisp.ToUpper().Contains(searchString.ToUpper())
                                       || s.StrTitle.ToUpper().Contains(searchString.ToUpper()));
                var byAlbum = from s in context.Songs
                              join a in context.Albums on s.IdAlbum equals a.IdAlbum
                              where a.StrAlbum.ToUpper().Contains(searchString.ToUpper())
                              || a.StrArtistDisp.ToUpper().Contains(searchString.ToUpper())
                              select s;
                var byPath = from s in context.Songs
                              join p in context.Paths on s.IdPath equals p.IdPath
                              where p.StrPath.ToUpper().Contains(searchString.ToUpper())
                              select s;
                songs = byAlbum.Concat(bySongOrArtist)
                                .Concat(byPath)
                                .Distinct();
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
                    songs = from s in context.Songs
                            join a in context.Albums on s.IdAlbum equals a.IdAlbum
                            orderby a.StrAlbum
                            select s;
                    break;
                case "album_desc":
                    songs = from s in context.Songs
                            join a in context.Albums on s.IdAlbum equals a.IdAlbum
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
