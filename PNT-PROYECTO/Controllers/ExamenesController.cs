﻿using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PNT_PROYECTO.Data;
using PNT_PROYECTO.Models;
using SQLitePCL;

namespace PNT_PROYECTO.Controllers
{
    public class ExamenesController : Controller
    {
        private readonly PNT_PROYECTOContext _context;

        public ExamenesController(PNT_PROYECTOContext context)
        {
            _context = context;
        }

        // GET: Examenes
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var stockContext = _context.Examen.Include(j => j.Profe);
            return View(await stockContext.ToListAsync());
        }

        // GET: Examenes/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Examen == null)
            {
                return NotFound();
            }

            var examen = await _context.Examen
                .Include(j => j.Materiales)
                .Include(j => j.Profe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examen == null)
            {
                return NotFound();
            }

            
            return View(examen);
        }

        // GET: Examenes/Create
        [Authorize(Roles ="ADMIN,ADJUNTO")]
        public IActionResult Create()
        {
            ViewData["Legajo"] = new SelectList(_context.Profesor, "Legajo", "NombreApellido");
            ExamenViewModel modelo = new ExamenViewModel(); 
            modelo.Materiales = new SelectList(_context.Material, "Id", "Titulo");
            return View(modelo);
        }

        // POST: Examenes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN,ADJUNTO")]
        public async Task<IActionResult> Create(ExamenViewModel examenVM)
        {
            Examen examen = new Examen();
            if (ModelState.IsValid)
            {
                
                examen.Fecha = examenVM.Fecha;
                examen.Titulo = examenVM.Titulo;
                examen.ProfeId = examenVM.ProfeId;
                
                ICollection<Material> matAux = new List<Material>();

                foreach (Int32 materialId in examenVM.MaterialesSeleccionados) {
                    matAux.Add(await _context.Material.FindAsync(materialId));
                }
                
                examen.Materiales = matAux;

                _context.Add(examen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Legajo"] = new SelectList(_context.Profesor, "Legajo", "NombreApellido", examen.ProfeId);
            return View(examenVM);
        }

        // GET: Examenes/Edit/5
        [Authorize(Roles = "ADMIN,ADJUNTO")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Examen == null)
            {
                return NotFound();
            }

            var examen = await _context.Examen
                .Include(j => j.Materiales)
                .Include(j=> j.Profe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examen == null)
            {
                return NotFound();
            }
            ExamenViewModel modelo = new ExamenViewModel();
            modelo.ProfeId = examen.ProfeId;
            modelo.Id = examen.Id;
            modelo.Titulo = examen.Titulo;
            modelo.Fecha = examen.Fecha;
            modelo.Profe = examen.Profe;

            modelo.MaterialesSeleccionados = new List<Int32>();
            foreach (Material m in examen.Materiales)
            {
                modelo.MaterialesSeleccionados.Add(m.Id);
            }
            modelo.Materiales = new SelectList(_context.Material, "Id", "Titulo");

            ViewData["Legajo"] = new SelectList(_context.Profesor, "Legajo", "NombreApellido");
            return View(modelo);
        }

        // POST: Examenes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN,ADJUNTO")]
        public async Task<IActionResult> Edit(int id, ExamenViewModel examenVM)
        {
            if (id != examenVM.Id)
            {
                return NotFound();
            }
            var examen = await _context.Examen
                .Include(j => j.Materiales)
                .Include(j => j.Profe)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ModelState.IsValid)
            {
                examen.Fecha = examenVM.Fecha;
                examen.Titulo = examenVM.Titulo;
                examen.ProfeId = examenVM.ProfeId;
                examen.Profe = examenVM.Profe;

                foreach(var i in examen.Materiales.ToList()) 
                {
                    examen.Materiales.Remove(i);
                }

                await _context.SaveChangesAsync();

                foreach (Int32 materialId in examenVM.MaterialesSeleccionados)
                {
                    examen.Materiales.Add(await _context.Material.FindAsync(materialId));
                }
                
                try
                {
                    _context.Add(examen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamenExists(examen.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["Legajo"] = new SelectList(_context.Profesor, "Legajo", "NombreApellido", examen.ProfeId);
            return View(examenVM);
        }

        // GET: Examenes/Delete/5
        [Authorize(Roles = "ADMIN,ADJUNTO")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Examen == null)
            {
                return NotFound();
            }

            var examen = await _context.Examen
                .Include(x => x.Materiales)
                .Include(x => x.Profe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examen == null)
            {
                return NotFound();
            }

            return View(examen);
        }

        // POST: Examenes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN,ADJUNTO")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Examen == null)
            {
                return Problem("Entity set 'PNT_PROYECTOContext.Examen'  is null.");
            }
            var examen = await _context.Examen.FindAsync(id);
            if (examen != null)
            {
                _context.Examen.Remove(examen);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamenExists(int id)
        {
          return _context.Examen.Any(e => e.Id == id);
        }
    }
}
