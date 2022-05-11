using System;
using System.ComponentModel.DataAnnotations;

namespace BackendOperacionesFroward.Entities.Models
{
    public class History
    {

        [Key]
        public int IdHistory { get; set; }

        public DateTime Date { get; set; }

        public int IdUser { get; set; }

        public string Action { get; set; }

        public string Table { get; set; }

        public int Object { get; set; }

    }
}
