using KrxMusicStation.KrxMusicStation.Data;
using System;

namespace KrxMusicStation.Data
{
    public class KodiSqliteData : ISongData
    {
        private readonly KrxMusicStationDBContext dbContext;

        public KodiSqliteData(KrxMusicStationDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public string GetAlbumName(int songId)
        {
            throw new NotImplementedException();
        }

        public int GetBitrate(int songId)
        {
            throw new NotImplementedException();
        }

        public string GetFullPath(int songId)
        {
            throw new NotImplementedException();
        }
    }
}
