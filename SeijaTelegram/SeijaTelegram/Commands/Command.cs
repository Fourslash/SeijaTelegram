using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Dynamic; 
using System.Reflection;

namespace SeijaTelegram.Commands
{
    class Command
    {
        public Command(string nm, Action<Message, string[]> act)
        {
            name = nm;
            action = act;
            LoadSettings();
        }
        public string name { get; set; }
        public List<string> keywords { get; set; }
        bool isMasterOnly { get; set; }
        Action<Message, string[]> action;

        public void Execute(Message msg, string[] args)
        {
            if (action != null)
            {
                try {
                    action(msg, args);
                } catch (Exception ex)
                {
                    Logger.Error(String.Format("Error on executing {0} \n{1}\n{2}", name, ex.Message, ex.StackTrace));
                }
            } else {
                Logger.Error(String.Format("There is no action for command {0}", name));
            }
        }

        private void LoadSettings()
        {
            try
            {
                var json = System.IO.File.ReadAllText(string.Format(@".\commands\{0}.json", name));
                JsonConvert.PopulateObject(json, this);
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                keywords = new List<string>()
                {
                    name.ToLower()
                };
                isMasterOnly = true;
            }
        }
    }
}
