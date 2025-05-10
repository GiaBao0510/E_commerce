using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Core.Entities
{
    public class _Supplier
    {
        public sbyte sup_id {get; set;}
        [MaxLength(50)]
        public string? sup_name {get; set;}
        [MaxLength(100)]
        public string? email {get; set;}
        [MaxLength(10)]
        public string? phone_num {get; set;}
        [MaxLength(255)]
        public string? address {get; set;}
        [MaxLength(100)]
        public string? contact_person {get; set;}
        [MaxLength(255)]
        public string? detail {get; set;}
        [MaxLength(20)]
        public string? tax_code {get; set;}
    }
}