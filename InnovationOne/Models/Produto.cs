using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InnovationOne.Models
{
    public class Produto
    {
        public int Id { get; set; }
        [Display(Name = "Descrição")]
        [Required(ErrorMessage ="O campo descrição é obrigatório.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O campo quantidade é obrigatório.")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "O campo categoria é obrigatório.")]
        public int CategoriaId { get; set; }

        public Categoria Categoria { get; set; }

    }
}
