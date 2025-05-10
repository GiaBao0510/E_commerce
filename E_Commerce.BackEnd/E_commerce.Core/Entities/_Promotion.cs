using System.ComponentModel.DataAnnotations;

namespace E_commerce.Core.Entities
{
    public class _Promotion
    {
        public int promo_id {get; set;}
        [MaxLength(50)]
        public string promo_name {get; set;}
        public int discount {get; set;}
        public DateTime start_time {get; set;} = DateTime.Now;
        public DateTime end_time {get; set;}
    }
}