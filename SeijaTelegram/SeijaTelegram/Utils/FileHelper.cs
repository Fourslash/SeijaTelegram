using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SeijaTelegram.Utils
{
    class FileHelper
    {
        public static FileStream getStream(string filepath)
        {
            try
            {
                return new FileStream(filepath, FileMode.Open);                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }
    }
}
