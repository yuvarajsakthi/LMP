using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.Common
{
    public class ByteArrayDTO
    {
        [Required]
        public byte[] Data { get; set; } = Array.Empty<byte>();
    }
}
