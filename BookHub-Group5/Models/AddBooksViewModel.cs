using System.ComponentModel.DataAnnotations;

namespace BookHub_Group5.Models
{
    public class AddBooksViewModel
    {
        [Required]
        public string booktitle { get; set; }

        [Required]
        public string author { get; set; }

        [Required]
        public string genre { get; set; }

        [Required]
        public int publicationYear { get; set; }

        public string description { get; set; }

        [Required]
        public IFormFile CoverImage { get; set; }

        [Required]
        public IFormFile BookFile { get; set; }

        [Required]
        public Decimal price { get; set; }
    }
}