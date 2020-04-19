using KrxMusicStation.Data;
using KrxMusicStation.KrxMusicStation.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

            bool extractFlag = false;

            foreach (var s in songs)
            {
                AppendTrack(targetPath, s, out bool extracted);
                if (extracted)
                    extractFlag = true;
            }

            if (extractFlag)
            {
                //below code fixes strange behaviour of kodi / yamaha:
                //when new files are created and attempt is made to play them
                //from playlist on yamaha - it tells "cannot play"
                //if the same playlist is copied however, it plays back without any problem
                File.Copy(targetPath, $"{config["PlayListsFolder"]}{playlistName} 2.m3u", true);
                File.Delete(targetPath);
            }

            return true;
        }

        private void AppendTrack(string plPath, Song song, out bool extracted)
        {
            string shortenedInfo = GetShortenedInfo(song);
            string path = GetSongFullPath(song);
            extracted = false;

            if (IsPartOfFullAlbumFile(song))
            {
                string initPath = path;
                ExtractTrack(song, ref path);
                if (initPath != path)
                    extracted = true;
            }

            File.AppendAllText(plPath, 
                $"#EXTINF:0,{shortenedInfo}{Environment.NewLine}{path}{Environment.NewLine}",
                Encoding.UTF8);
        }

        /// <summary>
        /// Extracts a track from the whole-album-file, ffmpeg must be installed on the machine
        /// </summary>
        private void ExtractTrack(Song song, ref string sourcePath)
        {
            //production env is on linux, dev on windows but without the music library, skip for dev
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return;

            string startAt = TimeSpan.FromMilliseconds((double)song.IStartOffset)
                                            .ToString(@"%h\:mm\:ss\.fff");
            string duration = TimeSpan.FromMilliseconds((double)(song.IEndOffset - song.IStartOffset))
                                            .ToString(@"%h\:mm\:ss\.fff"); ;
            string targetFileName = GetShortenedInfo(song).Replace("/", "-");
            string extension = System.IO.Path.GetExtension(sourcePath);
            string targetPath = $"{config["TempMusicFolder"]}{targetFileName}{extension}";

            //assure there's no conflict with existing file (might be used in some other playlist)
            int i = 1;
            while (File.Exists(targetPath))
                targetPath = $"{config["TempMusicFolder"]}{targetFileName}{i++}{extension}";

            string args = $"-c \" ffmpeg -ss {startAt} -i '{sourcePath}' -vn -c copy -t {duration} '{targetPath}' \"";

            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/bin/bash", Arguments = args, };
            Process proc = new Process() { StartInfo = startInfo, };
            proc.Start();
            proc.WaitForExit();

            if (File.Exists(targetPath))
                sourcePath = targetPath;
        }

        private bool IsPartOfFullAlbumFile(Song song)
        {
            return !(song.IEndOffset == 0 & song.IStartOffset == 0);
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
