using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Ordina.Unite.Course.Domain;

namespace Ordina.Unite.Course.Service
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class CourseService : StatefulService, ICourseService
    {
        private ICourseRepository _courseRepository;

        public CourseService(StatefulServiceContext context)
            : base(context)
        { }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            _courseRepository = new CourseRepository(this.StateManager, cancellationToken);
            //Initial Sample Data
            var serviceFabricCourse = new Domain.Course
            {
                Id = new Guid("947d6e8e-97b8-44fe-8aa9-c866486387ac"),
                Name = "Jumpstarting Microservices architecture with Service Fabric",
                Description = "A quick introduction on how to start implementing your application using a microservices architecture",
                AvailableSeats = 100,
                Start = new DateTime(2017, 11, 06, 17, 00, 00, DateTimeKind.Utc),
                End = new DateTime(2017, 11, 06, 18, 00, 00, DateTimeKind.Utc)
            };
            await _courseRepository.Add(serviceFabricCourse);
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                //Extension methode to register this service as a remoting listener service
                new ServiceReplicaListener(ctx => this.CreateServiceRemotingListener(ctx)) 
            };
        }

        public async Task<IEnumerable<Domain.Course>> GetAll()
        {
            return await _courseRepository.Get();
        }

        public async Task<Domain.Course> Get(Guid id)
        {
            return await _courseRepository.Get(id);
        }

        public async Task<Domain.Course> Add(Domain.Course course)
        {
            return await _courseRepository.Add(course);
        }

        public async Task Remove(Guid id)
        {
            await _courseRepository.Remove(id);
        }

        public async Task ReserveSeat(Guid id)
        {
            await _courseRepository.ReserveSeat(id);
        }

        public async Task CancelSeat(Guid id)
        {
            await _courseRepository.CancelSeat(id);
        }
    }
}
