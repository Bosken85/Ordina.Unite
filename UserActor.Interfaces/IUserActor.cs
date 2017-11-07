using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace UserActor.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IUserActor : IActor
    {
        Task<IEnumerable<Guid>> GetEnrollements();
        Task<bool> IsEnrolled(Guid courseId);
        Task Enroll(Guid courseId);
        Task Disenroll(Guid courseId);
    }
}
