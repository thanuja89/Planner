using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Planner.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduledTaskController : ControllerBase
    {
        // GET: api/ScheduledTask
        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/ScheduledTask/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ScheduledTask
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/ScheduledTask/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
