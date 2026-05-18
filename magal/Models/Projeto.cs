using System;
using System.Collections.ObjectModel;

namespace magal.Models
{
    public class Projeto : BaseModel
    {
        public int id_projeto { get; set; }
        public int id_usuario { get; set; }
        public int id_cliente { get; set; }
        public Cliente Cliente { get; set; }
        public string nome { get; set; }
        public string tipo { get; set; }   // "Produto/Serviço"
        public string status { get; set; } // "Rascunho/Orçado/Aprovado/Executando/Concluído"
        public DateTime data_criacao { get; set; } = DateTime.Now;
        public DateTime? data_conclusao_prevista { get; set; }
        public DateTime DataExpiracao => data_criacao.AddDays(Orcamento?.validade_dias ?? 0);
        public bool EstaVencido => status == "Em Aberto" && DataExpiracao < DateTime.Today;

        // Objetos de navegação e coleções
        public Orcamento Orcamento { get; set; } = new Orcamento();
        public ObservableCollection<Tarefa> Tarefas { get; set; } = new ObservableCollection<Tarefa>();
        public ObservableCollection<Custo> Custos { get; set; } = new ObservableCollection<Custo>();
    }
}