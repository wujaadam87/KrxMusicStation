namespace KrxMusicStation.Logic
{
    public interface ISimpleLogger
    {
        void StartLog();
        void AppendLine(string contents);
    }
}
