using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SeijaTelegram.Main
{
    class Heart
    {
        public static void StartBeating()
        {
            Brain.LoadMemory();
            Task.Run(HeartBeats);
            Logger.Success("Child process started");
            while (true) ;
        }

        static async Task HeartBeats()
        {
            var offset = 0;
            while (true)
            {
                offset = Brain.CheckNews(offset);
            }
        }
    }
}
