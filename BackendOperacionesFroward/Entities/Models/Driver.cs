using System.ComponentModel.DataAnnotations;

namespace BackendOperacionesFroward.Entities.Models
{
    public class Driver : Auth
    {
        [Key]
        public int? IdDriver { get; set; }

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

    }
}
