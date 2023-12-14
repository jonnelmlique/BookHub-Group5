namespace BookHub_Group5.Models
{
    public class BookDetailsViewModel
    {
        public int bookid { get; set; }
        public string booktitle { get; set; }
        public string author { get; set; }
        public string genre { get; set; }
        public int publicationyear { get; set; }
        public string description { get; set; }
        public byte[] coverimage { get; set; }
        public string CoverImageBase64 { get; set; }
        public byte[] bookfile { get; set; }
        public string BookFileBase64 { get; set; }

        public Decimal price { get; set; }
    }
}
