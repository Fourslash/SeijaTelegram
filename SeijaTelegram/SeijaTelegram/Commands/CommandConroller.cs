using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using SeijaTelegram.Utils;
using SeijaTelegram.Main;

namespace SeijaTelegram.Commands
{
    class CommandConroller
    {
        List<Command> commands;
        XmlDictionary simpleCommands;
        public CommandConroller(string simpleCommandPath = @"./SimpleCommands.ext")
        {
            commands = new List<Command>()
            {
                new Command("Hug", Actions.Hug),
                new Command("Poll", Actions.Poll),
            };
            simpleCommands = new XmlDictionary(simpleCommandPath);
        }

        public bool TryExecute(string command, string[] args, Message msg)
        {
            var complexCommand = commands.Find(cmd => cmd.keywords.Contains(command));
            if (complexCommand != null)
            {
                Logger.Log(String.Format("Proessing command {0}", command));
                complexCommand.Execute(msg, args);
                return true;
            } else
            {
                var keys = new List<string>(simpleCommands.Pairs.Keys);
                var matchedKey = keys.Find(key => key == command);
                if (matchedKey != null)
                {
                    Brain.SendMessage(simpleCommands.Pairs[matchedKey], msg.Chat.Id).Wait();
                    return true;
                }
            }
            return false;
        }

        
    }
}
