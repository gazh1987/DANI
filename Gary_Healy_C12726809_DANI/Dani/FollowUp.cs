using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dani
{
    public class FollowUp
    {
        //Properties
        private string word;
        public string Word
        {
            get { return word; }
            set { word = value; }
        }

        private int count;
        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        //Constructor
        public FollowUp()
        {
            this.count = Count;
            this.word = Word;
        }
    }
}
