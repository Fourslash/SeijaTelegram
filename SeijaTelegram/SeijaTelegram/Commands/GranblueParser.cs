using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;

using SeijaTelegram.Twitter;

namespace SeijaTelegram.Commands
{
    class HaloInfo
    {
        public DateTime HaloDate { get; set; }
        public GroupInfo[] Groups { get; set; }
        public override string ToString()
        {
            var res = string.Format(
                "Angel Halo schedule for {0}:\n ",
                HaloDate.ToString("d MMMM", CultureInfo.CreateSpecificCulture("en-US"))
            );
            res += String.Join("\n\n", Groups.Select(r => r.ToString(HaloDate)));
            return res;
        }
    }

    class GroupInfo
    {
        public string[] GroupNames { get; set; }
        public Record[] Recodrs { get; set; }
        public string ToString(DateTime HaloDate)
        {
            string result;
            result = string.Format("-----------[{0}]-----------\n", String.Join(" - ", GroupNames));
            result += String.Join("\n", Recodrs.Select(r => r.ToString(HaloDate)));
            return result;

        }
    }

    public class Record
    {
        public DateTime Date { get; set; }
        public bool Star { get; set; }
        public string ToString(DateTime HaloDate)
        {
            string result;
            if (HaloDate.Day == Date.Day)
                result = Date.ToString("H:mm");
            else
                result = Date.ToString("d MMMM H:mm");
            return Star ? result += " ★" : result;
        }
    }

    class GranblueParser
    {
        Tweet[] UnfilteredTweets { get; set; }
        Tweet[] HaloTweets { get; set; }
        private DateTime HaloDate { get; set; }

        private TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Yakutsk Standard Time");
        public GranblueParser(Tweet[] tweets)
        {
            UnfilteredTweets = tweets;
            HaloTweets = filterHaloTweets(tweets);
        }

        public HaloInfo GetHaloInfo()
        {
            if (HaloTweets.Length == 0)
                throw new Exception("Can't find any angel halo tweets in results");

            var tweet = HaloTweets[0];
            var result = new HaloInfo();
            HaloDate = getHaloDate(tweet);
            result.HaloDate = HaloDate;
            result.Groups = getGroupInfoArr(tweet);
            return result;
        }

        GroupInfo[] getGroupInfoArr(Tweet tweet)
        {
            var groupReg = new Regex(@"(グループ[A-D]\&amp;[E-H]\n\d+\S{1,2}\n\d+\S{1,2}\n\d+\S{1,2})", RegexOptions.Multiline);
            var matches = groupReg.Matches(tweet.text);
            List<GroupInfo> info = new List<GroupInfo>();
            foreach (Match match in matches)
            {
                info.Add(getGroupInfo(match.Value));
            }
            return info.ToArray();
        }

        GroupInfo getGroupInfo(string match)
        {
            var gInfo = new GroupInfo();
            var groupNameReg = new Regex(@"([A-H])");
            List<string> groupNames = new List<string>();
            var matches = groupNameReg.Matches(match);
            foreach (Match nameMatch in matches)
            {
                groupNames.Add(nameMatch.Value);
            }
            gInfo.GroupNames = groupNames.ToArray();
            gInfo.Recodrs = getRecords(match);
            return gInfo;
        }

        Record[] getRecords(string match)
        {
            var recordReg = new Regex(@"(\n(\d+)(時|★){1,2})");
            List<Record> records = new List<Record>();
            var matches = recordReg.Matches(match);
            foreach (Match recordMatch in matches)
            {
                records.Add(getRecord(recordMatch.Value));
            }

            return records.ToArray();
        }

        Record getRecord(string str)
        {

            var record = new Record();
            var hour = new Regex(@"\d+").Match(str).Value;
            record.Date = convertDate(HaloDate.AddHours(Convert.ToDouble(hour)));


            record.Star = str.Contains("★");
            return record;
        }


        DateTime convertDate(DateTime unformatetd)
        {
            var utc = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(
                    unformatetd,
                    DateTimeKind.Unspecified
                ), tz
            );
            return utc.ToLocalTime();
        }


        Tweet[] filterHaloTweets(Tweet[] tweets)
        {
            return tweets.Where(t => t.text.StartsWith("【グランブルーファンタジー】【時限クエ】")).ToArray();
        }

        DateTime getHaloDate(Tweet tweet)
        {
            var dateRegexp = new Regex(@"(\d+\/\d+)");
            var found = dateRegexp.Match(tweet.text);
            var dates = found.Value.Split('/');
            return new DateTime(
                DateTime.Now.Year,
                Convert.ToInt32(dates[0]),
                Convert.ToInt32(dates[1])
            );
        }
    }
}
