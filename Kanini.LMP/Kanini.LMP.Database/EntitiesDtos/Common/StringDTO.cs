using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.Common
{
    public class StringDTO
    {
        [Required]
        public string Value { get; set; } = string.Empty;
    }
}
