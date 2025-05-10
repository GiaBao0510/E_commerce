using System.ComponentModel.DataAnnotations;

namespace E_commerce.Core.Entities
{
    public class _ProductType
    {
        public sbyte protyle_id {set; get;}
        [MaxLength(50)]
        public string protyle_name {set; get;}
        [MaxLength(50)]
        public string? alias_name {get; set;}
        [MaxLength(255)]
        public string? details {get;set;}

    }
}