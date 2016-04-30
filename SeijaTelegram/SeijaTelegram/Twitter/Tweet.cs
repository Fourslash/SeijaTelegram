using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace SeijaTelegram.Twitter
{
    class Tweet
    {
        const string DATE_FORMAT = "ddd MMM dd HH:mm:ss '+0000' yyyy";
        DateTime createdAt;
        public string created_at
        {
            get { return createdAt.ToString(); }
            set
            {
                createdAt = DateTime.ParseExact(value, DATE_FORMAT, CultureInfo.InvariantCulture);
            }
        }
        public long id { get; set; }
        public string text { get; set; }
    }
}
