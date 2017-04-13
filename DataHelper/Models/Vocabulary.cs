using System;
using System.Collections.Generic;

namespace DataHelper.Models
{
    public class Vocabulary:IComparable<Vocabulary>
    {
        public string Word { get; set; }
        public string LowerWord { get; set; }

        public int Count { get; set; }

        public List<PrevWord> Prev { get; set; }

        public int CompareTo(Vocabulary comparePart)
        {

            if (comparePart == null)
                return 1;

            else
                return this.Count.CompareTo(comparePart.Count);
        }

     }

    public class PrevWord:IComparable<PrevWord>
    {
        public string Word { get; set; }

        public string LowerWord { get; set; }
        public int Count { get; set; }

        public int CompareTo(PrevWord comparePart)
        {
 
            if (comparePart == null)
                return 1;

            else
                return this.Count.CompareTo(comparePart.Count);
        }


    }

}
