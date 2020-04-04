namespace KrxMusicStation.Data
{
    public interface ISongData
    {
        public string GetFullPath(int songId);
        public string GetAlbumName(int songId);
        public int GetBitrate(int songId);
    }
}
