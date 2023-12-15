namespace BookHub_Group5.Models
{
    public class LibraryBooks
    {
        public int SaleId { get; set; }
        public int BookId { get; set; }
        public string UserEmail { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal Price { get; set; }
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int PublicationYear { get; set; }
        public string Description { get; set; }
        public byte[] CoverImage { get; set; }
        public string CoverImageBase64 { get; set; }

        public byte[] BookFile { get; set; }
        public string BookFileBase64 { get; set; }
    }
}
