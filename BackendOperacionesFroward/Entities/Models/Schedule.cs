using System;
using System.ComponentModel.DataAnnotations;

namespace BackendOperacionesFroward.Entities.Models
{
    public class Schedule : Auth
    {
        [Key]
        public int? IdSchedule { get; set; }
        public Client? Client { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DayFrom { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DayTo { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
        public TimeSpan? HourFrom { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
        public TimeSpan? HourTo { get; set; }

        public string? Comment { get; set; }

    }
}
