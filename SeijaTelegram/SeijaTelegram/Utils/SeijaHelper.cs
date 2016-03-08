using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeijaTelegram.Main;

namespace SeijaTelegram.Utils
{

    class SeijaHelper
    {
        public static bool isSeijaAsked(string msg)
        {
            if (msg.IndexOf(String.Format("@{0}", Brain.selfConsciousness.Username)) == 0)
                return true;
            for (var i=0; i< Brain.settings.botAliases.Count; i++ )
            {
                if (msg.IndexOf(Brain.settings.botAliases[i]) == 0)
                    return true;
            }
            return false;
        }

        public static bool isQuestion(string msg)
        {
            char lastSymbol = msg[msg.Length - 1];
            bool isQuestion = (lastSymbol == '?');
            return isQuestion;
        }
    }
}
