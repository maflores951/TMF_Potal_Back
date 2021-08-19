using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginBase.Models;
using LoginBase.Models.Menu;

namespace LoginBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolModFunsController : ControllerBase
    {
        private readonly DataContext _context;

        public RolModFunsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/RolModFuns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolModFun>>> GetRolModFuns()
        {
            return await _context.RolModFuns.ToListAsync();
        }

        // GET: api/RolModFuns/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RolModFun>> GetRolModFun(int id)
        {
            var rolModFun = await _context.RolModFuns.FindAsync(id);

            if (rolModFun == null)
            {
                return NotFound();
            }

            return rolModFun;
        }

        // PUT: api/RolModFuns/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRolModFun(int id, RolModFun rolModFun)
        {
            if (id != rolModFun.RolModFunId)
            {
                return BadRequest();
            }

            _context.Entry(rolModFun).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolModFunExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RolModFuns
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<RolModFun>> PostRolModFun(RolModFun rolModFun)
        {
            _context.RolModFuns.Add(rolModFun);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRolModFun", new { id = rolModFun.RolModFunId }, rolModFun);
        }

        // DELETE: api/RolModFuns/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RolModFun>> DeleteRolModFun(int id)
        {
            var rolModFun = await _context.RolModFuns.FindAsync(id);
            if (rolModFun == null)
            {
                return NotFound();
            }

            _context.RolModFuns.Remove(rolModFun);
            await _context.SaveChangesAsync();

            return rolModFun;
        }

        private bool RolModFunExists(int id)
        {
            return _context.RolModFuns.Any(e => e.RolModFunId == id);
        }
    }
}
