using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SeniorCare.Domain.Entities;
using SeniorCare.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SeniorCare.Web.Controllers
{
    public class CitasController : Controller
    {
        private readonly SeniorCareDbContext _db;
        public CitasController(SeniorCareDbContext db) => _db = db;

        // LISTA + FILTROS
        // GET: /Citas?q=ana&desde=2025-08-01&hasta=2025-08-31
        public async Task<IActionResult> Index(string q, DateTime? desde, DateTime? hasta)
        {
            var query = _db.Citas.Include(c => c.Paciente).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(c =>
                    c.Proposito.Contains(q) ||
                    c.Paciente.Nombre.Contains(q) ||
                    c.Paciente.Cedula.Contains(q));
            }

            if (desde.HasValue) query = query.Where(c => c.Fecha >= desde.Value.Date);
            if (hasta.HasValue) query = query.Where(c => c.Fecha <= hasta.Value.Date);

            var lista = await query
                .OrderBy(c => c.Fecha).ThenBy(c => c.Hora)
                .ToListAsync();

            ViewData["q"] = q;
            ViewData["desde"] = desde?.ToString("yyyy-MM-dd");
            ViewData["hasta"] = hasta?.ToString("yyyy-MM-dd");
            return View(lista);
        }

        // CREATE
        public IActionResult Create()
        {
            ViewBag.Pacientes = new SelectList(_db.Pacientes.OrderBy(p => p.Nombre), "Id", "Nombre");
            return View(new Cita { Fecha = DateTime.Today, Hora = new TimeSpan(9, 0, 0) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cita model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Pacientes = new SelectList(_db.Pacientes.OrderBy(p => p.Nombre), "Id", "Nombre", model.PacienteId);
                return View(model);
            }

            model.Id = Guid.NewGuid();
            _db.Citas.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // EDIT
        public async Task<IActionResult> Edit(Guid id)
        {
            var cita = await _db.Citas.FindAsync(id);
            if (cita is null) return NotFound();

            ViewBag.Pacientes = new SelectList(_db.Pacientes.OrderBy(p => p.Nombre), "Id", "Nombre", cita.PacienteId);
            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Cita model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewBag.Pacientes = new SelectList(_db.Pacientes.OrderBy(p => p.Nombre), "Id", "Nombre", model.PacienteId);
                return View(model);
            }

            _db.Entry(model).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // DETAILS
        public async Task<IActionResult> Details(Guid id)
        {
            var cita = await _db.Citas.Include(c => c.Paciente).FirstOrDefaultAsync(c => c.Id == id);
            if (cita is null) return NotFound();
            return View(cita);
        }

        // DELETE
        public async Task<IActionResult> Delete(Guid id)
        {
            var cita = await _db.Citas.Include(c => c.Paciente).FirstOrDefaultAsync(c => c.Id == id);
            if (cita is null) return NotFound();
            return View(cita);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cita = await _db.Citas.FindAsync(id);
            if (cita != null)
            {
                _db.Citas.Remove(cita);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ====== CALENDARIO ======

        // Vista del calendario
        // GET: /Citas/Calendario
        public IActionResult Calendario() => View();

        // Endpoint JSON consumido por FullCalendar
        // GET: /Citas/Eventos?pacienteId=...
        [HttpGet]
        public IActionResult Eventos(Guid? pacienteId)
        {
            var q = _db.Citas.Include(c => c.Paciente).AsQueryable();

            if (pacienteId.HasValue)
                q = q.Where(c => c.PacienteId == pacienteId.Value);

            var eventos = q.Select(c => new
            {
                id = c.Id,
                title = c.Paciente.Nombre + " — " + c.Proposito,
                start = c.Fecha.Date.Add(c.Hora),                 // inicio = Fecha + Hora
                end = c.Fecha.Date.Add(c.Hora).AddMinutes(30),    // fin = +30 min (ajústalo)
                url = Url.Action("Details", "Citas", new { id = c.Id })
            }).ToList();

            return Json(eventos);
        }
    }
}
