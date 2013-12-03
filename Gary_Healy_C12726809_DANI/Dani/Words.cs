using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dani
{
    public class Words
    {
        //Properties
        private string word;
        public string Word
        {
            get { return word; }
            set { word = value; }
        }

        private List<FollowUp> fol;
        public List<FollowUp> Fol
        {
            get { return fol; }
            set { fol = value; }
        }

        //Constructor
        public Words()
        {
            this.word = Word;
            this.fol = new List<FollowUp>();
        }
    }
}
