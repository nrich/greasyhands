using System;

namespace GreasyHands.Search
{
    public class Result
    {
        public int IssueID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Uri Link { get; set; }
        public int Size { get; set; }
        public DateTime Published { get; set; }
    }
}
