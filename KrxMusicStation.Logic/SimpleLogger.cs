using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace KrxMusicStation.Logic
{
    public class SimpleLogger : ISimpleLogger
    {
        private readonly IConfiguration config;

        public SimpleLogger(IConfiguration config)
        {
            this.config = config;
        }

        public void AppendLine(string contents)
        {
            File.AppendAllText(config["LogPath"], $"{DateTime.Now.ToLongTimeString()}\t{contents}{Environment.NewLine}");
        }

        public void StartLog()
        {
            File.WriteAllText(config["LogPath"], $"{DateTime.Now.ToShortDateString()}\tLog started{Environment.NewLine}");
        }
    }
}
