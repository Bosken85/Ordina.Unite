using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Ordina.Unite.Course.Domain
{
    public interface ICourseService : IService
    {
        Task<IEnumerable<Course>> GetAll();
        Task<Course> Get(Guid id);
        Task<Course> Add(Course course);
        Task<Domain.Course> Update(Guid id, Domain.Course course);
        Task Remove(Guid id);
        Task ReserveSeat(Guid id);
        Task CancelSeat(Guid id);
    }
}