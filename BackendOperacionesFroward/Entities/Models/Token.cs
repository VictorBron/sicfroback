using System;
using System.ComponentModel.DataAnnotations;

namespace BackendOperacionesFroward.Entities.Models
{
    public class Token
    {
        [Key]
        public int? IdToken { get; set; }

        public int? IdUser { get; set; }

        public DateTime? RequestedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public string TokenCode { get; set; }

    }
}
