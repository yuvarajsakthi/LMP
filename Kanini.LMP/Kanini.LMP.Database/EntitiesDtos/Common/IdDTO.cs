using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.Common
{
    public class IdDTO
    {
        [Required]
        public int Id { get; set; }
    }
}