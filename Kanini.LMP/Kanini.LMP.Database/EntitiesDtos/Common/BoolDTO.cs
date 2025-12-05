using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.Common
{
    public class BoolDTO
    {
        [Required]
        public bool Value { get; set; }
    }
}
