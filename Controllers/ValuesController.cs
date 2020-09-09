using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebAppTest01.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public static int acumulado { get; set; } = 0;

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
    }
}
