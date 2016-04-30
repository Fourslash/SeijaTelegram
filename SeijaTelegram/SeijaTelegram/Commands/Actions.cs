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

        public static void AngelHalo(Message msg, string[] args)
        {
            const string GBF_TWITTER_ID = "1549889018";
            string result = "";
            try {
                var tweets = Brain.twitterConnector.LoadTweets(GBF_TWITTER_ID);
                var parser = new GranblueParser(tweets);
                var info = parser.GetHaloInfo();
                var argList = new List<string>(args);
                var groupArg = argList.Find(str => str.StartsWith("-g="));
                if (groupArg != null)
                {
                    var group = groupArg[3];
                    GroupInfo groupInfo = info.Groups.First(g => new List<string>(g.GroupNames).Contains(group.ToString()));
                    if (groupInfo == null)
                        result = string.Format("Cant find group with key {0}", group);
                    else
                    {
                        var newGroups = new List<GroupInfo>();
                        newGroups.Add(groupInfo);
                        info.Groups = newGroups.ToArray();
                        result = info.ToString();
                    }
                }
                else {
                    result = info.ToString();
                }
            } catch (Exception e)
            {
                result = e.Message;
            } finally
            {
                Brain.SendMessage(result, msg.Chat.Id).Wait();
            }
        }

        public static void ClearPoll(Message msg, string[] args)
        {
            PollSingleton.Instance = null;
            var markup = new ReplyKeyboardHide();
            markup.HideKeyboard = true;
            Brain.SendMessage("Clearing poll", msg.Chat.Id, markup: markup).Wait();
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
