using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BISP_API.Models
{
    public class Image
    {
        [Key]
        public int ImgId { get; set; }
        public byte[] Img { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

    }
}
