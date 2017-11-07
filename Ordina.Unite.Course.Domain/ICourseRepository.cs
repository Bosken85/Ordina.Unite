using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ordina.Unite.Course.Domain
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> Get();
        Task<Course> Get(Guid id);
        Task<Course> Add(Course course);
        Task<Domain.Course> Update(Guid id, Course course);
        Task Remove(Guid id);
        Task ReserveSeat(Guid id);
        Task CancelSeat(Guid id);
    }
}