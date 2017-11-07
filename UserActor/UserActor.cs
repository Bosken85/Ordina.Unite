using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using UserActor.Interfaces;

namespace UserActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class UserActor : Actor, IUserActor
    {
        /// <summary>
        /// Initializes a new instance of UserActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public UserActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task<IEnumerable<Guid>> GetEnrollements()
        {
            List<Guid> courseIds = await StateManager.GetOrAddStateAsync<List<Guid>>("enrollements", new List<Guid>());
            return courseIds;
        }

        public async Task<bool> IsEnrolled(Guid courseId)
        {
            List<Guid> courseIds = await StateManager.GetOrAddStateAsync<List<Guid>>("enrollements", new List<Guid>());
            return courseIds.Contains(courseId);
        }

        public async Task Enroll(Guid courseId)
        {
            List<Guid> courseIds = await StateManager.GetOrAddStateAsync<List<Guid>>("enrollements", new List<Guid>());

            if (!courseIds.Contains(courseId))
                courseIds.Add(courseId);

            await StateManager.SaveStateAsync();
        }

        public async Task Disenroll(Guid courseId)
        {
            List<Guid> courseIds = await StateManager.GetOrAddStateAsync<List<Guid>>("enrollements", new List<Guid>());

            if (courseIds.Contains(courseId))
                courseIds.Remove(courseId);

            await StateManager.SaveStateAsync();
        }
    }
}
