using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BackendOperacionesFroward.Entities.Models
{
    public class Auth
    {
        public DateTime? Created { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? Modified { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
