namespace KrxMusicStation.Data
{
    public partial class Album
    {
        public long IdAlbum { get; set; }
        public string StrAlbum { get; set; }
        public string StrMusicBrainzAlbumId { get; set; }
        public string StrReleaseGroupMbid { get; set; }
        public string StrArtistDisp { get; set; }
        public string StrArtistSort { get; set; }
        public string StrGenres { get; set; }
        public long? IYear { get; set; }
        public long BCompilation { get; set; }
        public string StrMoods { get; set; }
        public string StrStyles { get; set; }
        public string StrThemes { get; set; }
        public string StrReview { get; set; }
        public string StrImage { get; set; }
        public string StrLabel { get; set; }
        public string StrType { get; set; }
        public double FRating { get; set; }
        public long IVotes { get; set; }
        public long IUserrating { get; set; }
        public string LastScraped { get; set; }
        public long BScrapedMbid { get; set; }
        public string StrReleaseType { get; set; }
        public long IdInfoSetting { get; set; }
    }
}
