using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookHub_Group5.Models.Domain
{
    public class books
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int bookid { get; set; }
        public string booktitle { get; set; }
        public string author { get; set; }
        public string genre { get; set; }
        public int publicationyear { get; set; }
        public string description { get; set; }
        public byte[] coverimage {  get; set; }
        public byte[] bookfile { get; set; }
        public Decimal price { get; set; }


    }
}
