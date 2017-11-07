using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Ordina.Unite.Course.Domain;
using UserActor.Interfaces;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors;
using Ordina.Unite.Course.Domain.Exceptions;

namespace Ordina.Unite.Api.Controllers
{
    [Route("api/users/{userId}/[controller]")]
    public class EnrollementsController : Controller
    {
        private readonly ICourseService _courseService;

        public EnrollementsController()
        {
            _courseService = ServiceProxy.Create<ICourseService>(new Uri("fabric:/Ordina.Unite/Ordina.Unite.Course.Service"),
                new ServicePartitionKey(0));
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid userId)
        {
            try
            {
                IUserActor actor = GetActor(userId);
                IEnumerable<Guid> enrollements = await actor.GetEnrollements();
                return Ok(enrollements);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(Guid userId, [FromBody] Guid courseId)
        {
            try
            {
                IUserActor actor = GetActor(userId);

                if (await actor.IsEnrolled(courseId))
                    return StatusCode((int)HttpStatusCode.Conflict, "Already enrolled in the course");

                await _courseService.ReserveSeat(courseId);
                await actor.Enroll(courseId);
                return Ok();
            }
            catch (AggregateException aex)
            {
                if (aex.InnerExceptions.OfType<CourseNotFoundException>().Any())
                    return NotFound("Course not found");

                if (aex.InnerExceptions.OfType<NoAvailableSeatsException>().Any())
                    return BadRequest("There are no more seats available");

                return StatusCode(500);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpDelete("{courseId}")]
        public async Task<IActionResult> Delete(Guid userId, [FromRoute] Guid courseId)
        {
            try
            {
                IUserActor actor = GetActor(userId);
                await _courseService.CancelSeat(courseId);
                await actor.Disenroll(courseId);
                return NoContent();
            }
            catch (AggregateException aex)
            {
                if (aex.InnerExceptions.OfType<CourseNotFoundException>().Any())
                    return NotFound("Course not found");

                if (aex.InnerExceptions.OfType<NoAvailableSeatsException>().Any())
                    return BadRequest("There are no more seats available");

                return StatusCode(500);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        private static IUserActor GetActor(Guid userId)
        {
            return ActorProxy.Create<IUserActor>(new ActorId(userId), new Uri("fabric:/Ordina.Unite/UserActorService"));
        }
    }
}
