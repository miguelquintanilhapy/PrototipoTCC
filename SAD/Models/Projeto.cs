using System;

namespace SAD.Models
{
    public class Projeto
    {
        public int IdProjeto { get; set; }
        public int IdUsuario { get; set; }
        public int IdCliente { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Status { get; set; } = "Rascunho";
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataConclusaoPrevista { get; set; }
    }
}