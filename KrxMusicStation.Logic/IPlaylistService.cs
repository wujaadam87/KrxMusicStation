using KrxMusicStation.Data;
using System.Collections.Generic;

namespace KrxMusicStation.Logic
{
    public interface IPlaylistService
    {
        public bool SavePlaylist(string playlistName, IList<Song> songs, out string resultMessage);
    }
}
