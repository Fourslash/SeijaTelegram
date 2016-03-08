using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SeijaTelegram.Main
{
    class Settings
    {
        public long masterId { get; set; }
        public long botId { get; set; }
        public long homeGroupId { get; set; }
        public string apiKey { get; set; }
        public List<string> botAliases { get; set; }

        public static Settings Load(string pathToFile)
        {
            try
            {
                var json = System.IO.File.ReadAllText(pathToFile);
                return JsonConvert.DeserializeObject<Settings>(json);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new Settings();
            }          
        }
    }
      
}
