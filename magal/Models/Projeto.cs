using System;
using System.Collections.ObjectModel;

namespace magal.Models
{
    public class Projeto : BaseModel
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int ClienteId { get; set; } // FK no banco

        // --- ESTA É A LINHA QUE ESTÁ FALTANDO ---
        public Cliente Cliente { get; set; }
        // ----------------------------------------

        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string Status { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataConclusaoPrevista { get; set; }

        public Orcamento Orcamento { get; set; } = new Orcamento();
        public ObservableCollection<Tarefa> Tarefas { get; set; } = new ObservableCollection<Tarefa>();
        public ObservableCollection<Custo> Custos { get; set; } = new ObservableCollection<Custo>();
    }
}