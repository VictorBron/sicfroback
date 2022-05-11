using System.ComponentModel.DataAnnotations;

namespace BackendOperacionesFroward.Entities.Models
{
    public class Client : Auth
    {
        [Key]
        public int? IdClient { get; set; }

        private string _RUT;
        public string RUT
        {
            get { return _RUT; }
            set => _RUT = DataManagement.PrepareRUT(value);
        }

        public string OT { get; set; }

        public string Name { get; set; }

        public string? Giro { get; set; }

        public string Direction { get; set; }

        public string City { get; set; }

        public string Telephone { get; set; }

        public string Email { get; set; }

    }
}
