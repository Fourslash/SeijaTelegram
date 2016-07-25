using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using SeijaTelegram.Commands;
using SeijaTelegram.Utils;
using System.IO;
using System.Security.Cryptography;


namespace SeijaTelegram.Main
{
    class Brain
    {
        public static Settings settings { get; set; }
        public static Twitter.TwitterConnector twitterConnector;
            
        public static User selfConsciousness;
        static Api bot;
        static CommandConroller commandController;

        public static void LoadMemory()
        {
            settings = Settings.Load(@"./settings.json");
            bot = new Api(settings.apiKey);
            selfConsciousness = bot.GetMe().Result;
            commandController = new CommandConroller();
            twitterConnector = new Twitter.TwitterConnector();
        }

        public static int CheckNews(int offset)
        {           
            var updates = bot.GetUpdates(offset).Result;
            int newOffset = offset;
            foreach (var update in updates)
            {
                if (update.Message != null)
                {
                    var message = update.Message;

                    var timeElapsed = DateTime.UtcNow - message.Date;
                    if (timeElapsed.TotalMinutes <= 5)
                    {
                        if (update.Message.Type == MessageType.TextMessage)
                        {

                            ProcessTextMessage(message);
                        }
                    }
                }
                newOffset = update.Id + 1;
            }
            return newOffset;
        }

        public async static Task<Message> SendMessage(string text, long destination, int replyId = 0, ReplyMarkup markup = null)
        {
            await bot.SendChatAction(destination, ChatAction.Typing);
            await Task.Delay(text.Length * 10);
            return bot.SendTextMessage(destination, text, replyToMessageId: replyId, replyMarkup:markup).Result;
        }

        public async static void SendFile(long destination, string filePath, string fileName, string caption = "", int replyId = 0)
        {
            try {
                using (FileStream file = Utils.FileHelper.getStream(filePath)) {
                    await bot.SendPhoto(destination, new FileToSend(fileName, file), caption, replyId);                    
                }
            } catch (Exception ex)
            {
                Logger.Error(ex);
            }   
        }
        static void ProcessTextMessage(Message msg)
        {
            var isProcessed = false;
            if (!isProcessed)
                isProcessed = ParseVote(msg);
            if (!isProcessed)
                isProcessed = ParseCommand(msg);
            if (!isProcessed)
                isProcessed = ParseQuestion(msg);
        }

        static bool ParseQuestion(Message msg)
        {
            if (SeijaHelper.isQuestion(msg.Text) && SeijaHelper.isSeijaAsked(msg.Text))
            {
                var rng = new Random();
                var answer = rng.Next(3);
                string filename = "./files/";
                switch(answer)
                {
                    case 0: filename = "SeijaNO.jpg"; break;
                    case 1: filename = "SeijaYES.jpg"; break;
                    case 2: filename = "SeijaMEH.jpg"; break;
                }
                Brain.SendFile(
                    msg.Chat.Id,
                    "./files/" + filename,
                    filename,
                    replyId: msg.MessageId
                );
                return true;
            }
            return false;
        }

        static bool ParseVote(Message msg)
        {
            if (PollSingleton.Instance != null)
            {
                return PollSingleton.Instance.addAnswer(msg);
            }
            return false;
        }

        static bool ParseCommand(Message msg)
        {
            string commandString = null;
            if (msg.Text.IndexOf(String.Format(@"@{0}", selfConsciousness.Username)) == 0)
            {
                commandString = msg.Text.Substring(String.Format(@"@{0}", selfConsciousness.Username).Length);
            }
            else if (msg.Text[0] == '/')
            {
                commandString = msg.Text.Substring(1);
            }
            if (commandString != null)
            {
                commandString = commandString
                    .Replace(String.Format(@"@{0}", selfConsciousness.Username), "")
                    .Trim();
                var arr = commandString.Split(' ');
                if (arr.Length != 0)
                {
                    var args = new List<string>(arr)
                        .GetRange(1, arr.Length - 1)
                        .ToArray();
                    var command = arr[0].ToLower();
                    return commandController.TryExecute(command, args, msg);
                }
            }
            return false;
        }
       
    }
}
