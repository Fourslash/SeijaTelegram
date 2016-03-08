using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeijaTelegram.Main;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SeijaTelegram.Commands
{
    class Actions
    {
        public static void Hug(Message msg, string[] args)
        {
            if (msg.From.Id == Brain.settings.masterId)
            {
                Brain.SendMessage("/(✿◕⁀◕)\\", msg.Chat.Id).Wait();
            }
            else {
                Brain.SendMessage(string.Format("( ° ͜ʖ͡°)╭∩╮ Hey {0}, here's a \"hug\" for you. ( ° ͜ʖ͡°)╭∩╮", msg.From.FirstName), msg.Chat.Id).Wait();
            }
        }

        public static void Poll(Message msg, string[] args)
        {
            if (args.Length < 2)
                return;
            if (PollSingleton.Instance.status != PollStatuses.notStarted)
            {
                Brain.SendMessage("There can be only one vote in time!", msg.Chat.Id).Wait();
            }
            else {
                var argList = new List<string>(String.Join(" ", args).Split('|'));
                var question = argList[0];
                var markup = new ReplyKeyboardMarkup();
                markup.Keyboard = argList.GetRange(1, argList.Count - 1).Select(str => str.Split()).ToArray();
                markup.OneTimeKeyboard = true;
                var pollMesage = Brain.SendMessage(question, msg.Chat.Id, markup: markup).Result;
                PollSingleton.Instance.Init(question, argList, pollMesage);         
            }
        }
    }
}
