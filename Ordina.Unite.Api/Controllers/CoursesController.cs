using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Ordina.Unite.Api.Models;
using Ordina.Unite.Course.Domain;

namespace Ordina.Unite.Api.Controllers
{
    [Route("api/[controller]")]
    public class CoursesController : Controller
    {
        private readonly ICourseService _courseService;

        public CoursesController()
        {
            _courseService = ServiceProxy.Create<ICourseService>(new Uri("fabric:/Ordina.Unite/Ordina.Unite.Course.Service"), 
                new ServicePartitionKey(0));
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var courses = await _courseService.GetAll();
                var result = courses.Select(x => new ApiCourse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Info = x.Info,
                    Start = x.Start,
                    End = x.End,
                    AvailableSeats = x.AvailableSeats
                });
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var course = await _courseService.Get(id);
                if (course == null) return NotFound();
                return Ok(course);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ApiCourse apiCourse)
        {
            try
            {
                var course = new Course.Domain.Course
                {
                    Id = apiCourse.Id,
                    Name = apiCourse.Name,
                    Description = apiCourse.Description,
                    Info = apiCourse.Info,
                    Start = apiCourse.Start,
                    End = apiCourse.End,
                    AvailableSeats = apiCourse.AvailableSeats
                };
                course = await _courseService.Add(course);
                apiCourse.Id = course.Id;
                return CreatedAtAction("Get", new { course.Id },  apiCourse);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody]ApiCourse apiCourse)
        {
            try
            {
                var course = new Course.Domain.Course
                {
                    Id = apiCourse.Id,
                    Name = apiCourse.Name,
                    Description = apiCourse.Description,
                    Info = apiCourse.Info,
                    Start = apiCourse.Start,
                    End = apiCourse.End,
                    AvailableSeats = apiCourse.AvailableSeats
                };
                await _courseService.Add(course);
                return Ok(apiCourse);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _courseService.Remove(id);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            
        }
    }
}
