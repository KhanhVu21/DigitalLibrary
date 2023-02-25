namespace DigitalLibary.Service.Common.FormatApi
{
    public class CustomApiAnalystUserAndBook
    {
        public CustomApiAnalystUserAndBook()
        {

        }

        public UserAnalyst userAnalyst { get; set; }
        public TotalserAnalyst totalser { get; set; }
        public BookBorrowAnalyst bookBorrowAnalyst { get; set; }
        public BookBackAnalyst bookBackAnalyst { get; set; }
    }
    public class UserAnalyst
    {
        public int NumberUserCurrentMonth { get; set; }
        public int NumberUserLastMonth { get; set; }
        public int CurrentMonth { get; set; }
        public int LastMonth { get; set; }
        public double percentDifference { get; set; }
    }
    public class TotalserAnalyst
    {
        public int TotalUser { get; set; }
    }
    public class BookBorrowAnalyst
    {
        public int TotalBorrowBookCurrentMonth { get; set; }
        public int TotalBorrowBookLastMonth { get; set; }
        public int CurrentMonth { get; set; }
        public int LastMonth { get; set; }
        public double percentDifference { get; set; }
    }
    public class BookBackAnalyst
    {
        public int TotalBookBackCurrentMonth { get; set; }
        public int TotalBookBackLastMonth { get; set; }
        public int CurrentMonth { get; set; }
        public int LastMonth { get; set; }
        public double percentDifference { get; set; }
    }
}
