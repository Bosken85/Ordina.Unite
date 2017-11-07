using Ordina.Unite.Course.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Ordina.Unite.Course.Domain.Exceptions;

namespace Ordina.Unite.Course.Service
{
    internal class CourseRepository : ICourseRepository
    {
        private const string CourseCollectionName = "courses";

        private readonly IReliableStateManager _stateManager;
        private readonly CancellationToken _cancellationToken;

        public CourseRepository(IReliableStateManager stateManager, CancellationToken cancellationToken)
        {
            _stateManager = stateManager;
            _cancellationToken = cancellationToken;
        }

        public async Task<IEnumerable<Domain.Course>> Get()
        {
            var result = new List<Domain.Course>();
            var courses = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Course.Domain.Course>>(CourseCollectionName);
            using (var tx = _stateManager.CreateTransaction())
            {
                var courseCollection = await courses.CreateEnumerableAsync(tx, EnumerationMode.Unordered);
                using (var enumerator = courseCollection.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(_cancellationToken))
                    {
                        KeyValuePair<Guid, Domain.Course> current = enumerator.Current;
                        result.Add(current.Value);
                    }
                }
            }
            return result;
        }

        public async Task<Domain.Course> Get(Guid id)
        {
            var courses = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Course.Domain.Course>>(CourseCollectionName);
            using (var tx = _stateManager.CreateTransaction())
            {
                var course = await courses.TryGetValueAsync(tx, id);
                if (!course.HasValue)
                    throw new CourseNotFoundException();

                return course.Value;
            }
        }

        public async Task<Domain.Course> Add(Domain.Course course)
        {
            var courses = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Course.Domain.Course>>(CourseCollectionName);
            using (var tx = _stateManager.CreateTransaction())
            {
                if(course.Id == Guid.Empty) course.Id = Guid.NewGuid();
                await courses.AddOrUpdateAsync(tx, course.Id, course, (id, value) => course);
                await tx.CommitAsync();
            }
            return course;
        }

        public async Task<Domain.Course> Update(Guid id, Domain.Course course)
        {
            var courses = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Course.Domain.Course>>(CourseCollectionName);
            using (var tx = _stateManager.CreateTransaction())
            {
                var result = await courses.TryGetValueAsync(tx, id, LockMode.Update);
                if (!result.HasValue)
                    throw new CourseNotFoundException();

                await courses.SetAsync(tx, id, course);
                await tx.CommitAsync();
            }
            return course;
        }

        public async Task Remove(Guid id)
        {
            var courses = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Course.Domain.Course>>(CourseCollectionName);
            using (var tx = _stateManager.CreateTransaction())
            {
                await courses.TryRemoveAsync(tx, id);
                await tx.CommitAsync();
            }
        }

        public async Task ReserveSeat(Guid id)
        {
            var courses = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Course.Domain.Course>>(CourseCollectionName);
            using (var tx = _stateManager.CreateTransaction())
            {
                var course = await courses.TryGetValueAsync(tx, id, LockMode.Update);

                if (!course.HasValue)
                    throw new CourseNotFoundException();

                if (course.Value.AvailableSeats <= 0)
                    throw new NoAvailableSeatsException();

                var newCourse = course.Value.Clone();
                newCourse.AvailableSeats--;

                await courses.SetAsync(tx, newCourse.Id, newCourse);
                await tx.CommitAsync();
            }
        }

        public async Task CancelSeat(Guid id)
        {
            var courses = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Course.Domain.Course>>(CourseCollectionName);
            using (var tx = _stateManager.CreateTransaction())
            {
                var course = await courses.TryGetValueAsync(tx, id, LockMode.Update);

                if (!course.HasValue)
                    throw new CourseNotFoundException();

                var newCourse = course.Value.Clone();
                newCourse.AvailableSeats++;

                await courses.SetAsync(tx, newCourse.Id, newCourse);
                await tx.CommitAsync();
            }
        }
    }
}
