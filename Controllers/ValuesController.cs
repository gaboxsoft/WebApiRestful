using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using AutoMapper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace WebAppTest01.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly  IConfiguration configuration;
        public static int acumulado { get; set; } = 0;

        public ValuesController(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[]{"Value1","Value2", "Value3","Value 4" };
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            acumulado += id;
            return $"el valor leido es {id} y el acumulado es:{acumulado}";
        }


        [HttpGet("Config/{id}", Name="Config")]
        public ActionResult<string> Get(string id)
        {
            var s =  configuration[id];
            return s;
        }
    }
}
