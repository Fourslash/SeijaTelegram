using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Windows.Threading;
using SeijaTelegram.Main;

namespace SeijaTelegram.Commands
{
    public enum PollStatuses { notStarted, Started, Ended };

    class Result
    {
        public int Index;
        public int Count;
        public Result(int count, int index)
        {
            Index = index;
            Count = count;
        }
    }

    class PollSingleton
    {
        string Question;
        List<string> Variants;
        Dictionary<long, int> Answers = new Dictionary<long, int>();
        Message startMessage;
        System.Timers.Timer timer;
        public PollStatuses status = PollStatuses.notStarted;
        public void Init(string question, List<string> variants, Message message, int timeout = 2)
        {
            Question = question;
            Variants = variants;
            startMessage = message;
            timer = new System.Timers.Timer();
            timer.Elapsed +=onTimeEnd;
            timer.Interval = timeout * 60 * 1000;
            status = PollStatuses.Started;
            timer.Enabled = true;

        }

        public bool addAnswer(Message msg)
        {
            if (this.status == PollStatuses.Started)
            {
                var index = Variants.IndexOf(msg.Text);
                if (index != -1)
                {
                    Answers[msg.From.Id] = index;
                    return true;
                }
            }
            return false;
        }
        void onTimeEnd(object sender, EventArgs e)
        {
            timer.Enabled = false;
            string message;
            if (Answers.Count > 0)
            {
                var values = new List<int>(Answers.Values);
                var distinctResults = values.Distinct();

                List<Result> results = new List<Result>();
                foreach (var value in distinctResults)
                {
                    results.Add(new Result(values.Count(x => x == value), value));
                }
                results.Sort((x, y) => y.Count.CompareTo(x.Count));
                if (Answers.Count == 1)
                {
                    message = String.Format("Winner is \"{0}\" with only one vote.", Variants[results[0].Index], results[0].Count);
                } else {
                    if (results[0].Index == results[1].Index)
                    {
                        message = "Tie!";
                    }
                    else
                    {
                        message = String.Format("Winner is \"{0}\" with {1} votes!", Variants[results[0].Index], results[0].Count);
                    }
                }
            } else
            {
                message = "No one answered";
            }
            var markup = new ReplyKeyboardHide();
            markup.HideKeyboard = true;
            Brain.SendMessage(message, startMessage.Chat.Id, startMessage.MessageId, markup).Wait();
            status = PollStatuses.Ended;
            PollSingleton.instance = null;
        }


        static PollSingleton instance;
        public static PollSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PollSingleton();
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }
    }
}
