using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppBooks.Contexts;
using WebAppBooks.Entities;

namespace WebAppBooks.Controllers
{
    [Route("[Controller]")]
    [ApiController]

    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        public AutoresController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Autor>>> Get()
        {
            var autores =  await context.Autores.ToListAsync();
            return autores;
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<Autor>> Get(int Id)
        {
            var autorDTOSimple = await context.Autores.FirstOrDefaultAsync(x => x.Id == Id);
                
        }


    }
}
