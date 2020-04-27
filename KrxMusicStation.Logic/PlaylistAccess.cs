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

        public static void ClearupTempMusic(IConfiguration config)
        {
            var logger = new SimpleLogger(config);
            logger.StartLog();
            logger.AppendLine("start clearup");
            //production env is on linux, dev on windows but without the music library, skip for dev
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return;

            var tempFolder = config["TempMusicFolder"];

            var songsOnPlaylists = new List<string>();
            var tempSongs = Directory.GetFiles(tempFolder);
            var playlists = Directory.GetFiles(config["PlayListsFolder"]);

            logger.AppendLine($"{tempSongs.Count()} tempsongs found");
            logger.AppendLine($"{playlists.Count()} playlists found");

            foreach (var playlist in playlists)
            {
                var songs = GetSongPathFromPlaylist(playlist);
                songsOnPlaylists.AddRange(songs);
                logger.AppendLine($"collecting tracks from {playlist}, {songs.Count()} tracks found");
            }

            var tempSongsOnPlaylists = songsOnPlaylists.Distinct().Where(p => p.Contains(tempFolder));
            var songsToDelete = tempSongs.Except(tempSongsOnPlaylists);

            logger.AppendLine($"{songsToDelete.Count()} tracks to delete found");
            foreach (var toDelete in songsToDelete)
            {
                logger.AppendLine($"deleting: {toDelete}");
                File.Delete(toDelete);
            }
            logger.AppendLine("end clearup");
        }

        private static IEnumerable<string> GetSongPathFromPlaylist(string playlistPath)
        {
            foreach (var line in File.ReadAllLines(playlistPath).Skip(1))
            {
                if (!line.Contains("#EXT"))
                    yield return line;
            }
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
