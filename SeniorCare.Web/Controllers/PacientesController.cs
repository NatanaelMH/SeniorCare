using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorCare.Domain.Entities;
using SeniorCare.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SeniorCare.Web.Controllers
{
    public class PacientesController : Controller
    {
        private readonly SeniorCareDbContext _db;
        public PacientesController(SeniorCareDbContext db) => _db = db;

        // GET: /Pacientes?q=texto
        public async Task<IActionResult> Index(string q)
        {
            var query = _db.Pacientes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(p =>
                    p.Nombre.Contains(q) ||
                    p.Telefono.Contains(q) ||
                    p.Correo.Contains(q) ||
                    p.Cedula.Contains(q));
            }
            var lista = await query.OrderBy(p => p.Nombre).ToListAsync();
            ViewData["q"] = q;
            return View(lista);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Paciente model)
        {
            if (!ModelState.IsValid) return View(model);
            model.Id = Guid.NewGuid();
            _db.Pacientes.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var paciente = await _db.Pacientes.FindAsync(id);
            if (paciente is null) return NotFound();
            return View(paciente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Paciente model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            _db.Entry(model).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var paciente = await _db.Pacientes.FirstOrDefaultAsync(p => p.Id == id);
            if (paciente is null) return NotFound();
            return View(paciente);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var paciente = await _db.Pacientes.FirstOrDefaultAsync(p => p.Id == id);
            if (paciente is null) return NotFound();
            return View(paciente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var paciente = await _db.Pacientes.FindAsync(id);
            if (paciente != null)
            {
                _db.Pacientes.Remove(paciente);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
