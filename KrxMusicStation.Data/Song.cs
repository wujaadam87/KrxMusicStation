namespace KrxMusicStation.Data
{
    public partial class Song
    {
        public long IdSong { get; set; }
        public long? IdAlbum { get; set; }
        public long? IdPath { get; set; }
        public string StrArtistDisp { get; set; }
        public string StrArtistSort { get; set; }
        public string StrGenres { get; set; }
        public string StrTitle { get; set; }
        public long? ITrack { get; set; }
        public long? IDuration { get; set; }
        public long? IYear { get; set; }
        public string StrFileName { get; set; }
        public string StrMusicBrainzTrackId { get; set; }
        public long? ITimesPlayed { get; set; }
        public long? IStartOffset { get; set; }
        public long? IEndOffset { get; set; }
        public string Lastplayed { get; set; }
        public double Rating { get; set; }
        public long Votes { get; set; }
        public long Userrating { get; set; }
        public string Comment { get; set; }
        public string Mood { get; set; }
        public string StrReplayGain { get; set; }
        public string DateAdded { get; set; }
    }
}
