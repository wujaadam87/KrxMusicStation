using KrxMusicStation.Data;
using KrxMusicStation.KrxMusicStation.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace KrxMusicStation.Logic
{
    public class PlaylistAccess : IPlaylistService
    {
        private readonly IConfiguration config;
        private readonly KrxMusicStationDBContext dbContext;

        public PlaylistAccess(IConfiguration config, 
                                KrxMusicStationDBContext dbContext)
        {
            this.config = config;
            this.dbContext = dbContext;
        }

        public bool SavePlaylist(string playlistName, IList<Song> songs, out string resultMessage)
        {
            resultMessage = "Playlist saved";
            string targetPath = config["PlayListsFolder"] + playlistName + ".m3u";
            File.WriteAllText(targetPath, "#EXTCPlayListM3U::M3U" + Environment.NewLine, Encoding.UTF8);

            foreach (var s in songs)
            {
                AppendTrack(targetPath, s);
            }

            return true;
        }

        private void AppendTrack(string plPath, Song song)
        {
            string shortenedInfo = GetShortenedInfo(song);
            File.AppendAllText(plPath, 
                $"#EXTINF:{song.IDuration},{shortenedInfo}{Environment.NewLine}{GetSongFullPath(song)}{Environment.NewLine}",
                Encoding.UTF8);
        }

        private string GetShortenedInfo(Song song)
        {
            string artist = String.IsNullOrWhiteSpace(song.StrArtistDisp) ?
                            "Unknown" :
                            song.StrArtistDisp;
            string songInfo = $"{artist}/{song.StrTitle}";
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

            songInfo = myTI.ToTitleCase(songInfo)
                .Replace(" ", "");

            return songInfo;
        }

        private string GetSongFullPath(Song song)
        {
            string root = (from p in dbContext.Paths
                          join s in dbContext.Songs on p.IdPath equals s.IdPath
                          where s.IdSong == song.IdSong
                          select p.StrPath).Single();

            return root + song.StrFileName;
        }
    }
}
