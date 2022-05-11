using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackendOperacionesFroward.Entities.Models
{

    public class User : Auth
    {
        [Key]
        public int? IdUser { get; set; }

        public Client? Client { get; set; }

        public string Permissions { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        private string _RUT;
        public string RUT
        {
            get { return _RUT; }
            set => _RUT = DataManagement.PrepareRUT(value);
        }

        public string Telephone { get; set; }

        public string Email { get; set; }

        public DateTime? LastEntry { get; set; }

        public bool? Active { get; set; }

    }
}
