﻿using System.ComponentModel.DataAnnotations;

namespace PNT_PROYECTO.Models
{
    public class Examen
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        public string? Titulo { get; set; }
        public ICollection<Material>? Materiales { get; set; }
        public Profesor? Profe { get; set; }
        [Required]
        public int ProfeId { get; set; }


    }
}
