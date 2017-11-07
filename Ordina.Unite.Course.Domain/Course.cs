using System;

namespace Ordina.Unite.Course.Domain
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Info { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int AvailableSeats { get; set; }

        public Course Clone()
        {
            return new Course
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                Info = this.Info,
                Start = this.Start,
                End = this.End,
                AvailableSeats = this.AvailableSeats
            };
        }
    }
}
