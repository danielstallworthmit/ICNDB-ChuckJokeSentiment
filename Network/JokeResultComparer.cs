using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    class JokeResultComparer : IComparer<JokeResult>
    {
        public int Compare(JokeResult jr1, JokeResult jr2)
        {
            return -jr1.AverageSentimentScore.CompareTo(jr2.AverageSentimentScore);
        }
    }
}
