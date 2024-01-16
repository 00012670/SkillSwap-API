using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BISP_API.Models
{
    public class Image
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("ImgCode")]
        [StringLength(50)]
        [Unicode(false)]
        public string Imgcode { get; set; }

        [Column("Image", TypeName = "image")]
        public byte[] Img { get; set; }

    }
}
