namespace Products1.API.Controllers
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Products1.Domain;

    [Authorize]
    public class UbicationsController : ApiController
    {
        private DataContext db = new DataContext();

        // GET: api/Ubications
        public IQueryable<Ubication> GetUbications()
        {
            return db.Ubications;
        }

        // GET: api/Ubications/5
        [ResponseType(typeof(Ubication))]
        public async Task<IHttpActionResult> GetUbication(int id)
        {
            Ubication ubication = await db.Ubications.FindAsync(id);
            if (ubication == null)
            {
                return NotFound();
            }

            return Ok(ubication);
        }

        // PUT: api/Ubications/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUbication(int id, Ubication ubication)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ubication.UbicationId)
            {
                return BadRequest();
            }

            db.Entry(ubication).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UbicationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Ubications
        [ResponseType(typeof(Ubication))]
        public async Task<IHttpActionResult> PostUbication(Ubication ubication)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Ubications.Add(ubication);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = ubication.UbicationId }, ubication);
        }

        // DELETE: api/Ubications/5
        [ResponseType(typeof(Ubication))]
        public async Task<IHttpActionResult> DeleteUbication(int id)
        {
            Ubication ubication = await db.Ubications.FindAsync(id);
            if (ubication == null)
            {
                return NotFound();
            }

            db.Ubications.Remove(ubication);
            await db.SaveChangesAsync();

            return Ok(ubication);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UbicationExists(int id)
        {
            return db.Ubications.Count(e => e.UbicationId == id) > 0;
        }
    }
}