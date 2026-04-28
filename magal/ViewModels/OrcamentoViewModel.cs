using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using magal.Models;
using magal.Data.Repositories;
using magal.Services;
using System.Windows.Input;

namespace magal.ViewModels
{
    public class OrcamentoViewModel : BaseModel
    {
        private Projeto _projetoAtual;
        private bool _processando = false;

        public Projeto ProjetoAtual
        {
            get => _projetoAtual;
            set { _projetoAtual = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Cliente> Clientes { get; set; }
        public ObservableCollection<Funcionario> Funcionarios { get; set; }
        public ObservableCollection<Custo> CustosExtras { get; set; }

        public List<string> CategoriasCustos { get; } = new List<string>
        {
            "Equipamentos", "Licenças de Software", "Energia Elétrica",
            "Transporte/Deslocamento", "Manutenção", "Aluguel/Estrutura", "EPIs/Ferramentas"
        };

        public bool BotaoAtivo => !_processando;

        public RelayCommand AdicionarTarefaCommand { get; }
        public RelayCommand DeletarTarefaCommand { get; } 
        public RelayCommand AdicionarCustoCommand { get; }
        public RelayCommand DeletarCustoCommand { get; } 
        public RelayCommand GerarPdfCommand { get; }

        public OrcamentoViewModel()
        {
            NovoProjeto();
            CarregarDadosIniciais();

           
            AdicionarTarefaCommand = new RelayCommand(_ => AdicionarTarefa());
            DeletarTarefaCommand = new RelayCommand(param => DeletarTarefa(param as Tarefa));

            AdicionarCustoCommand = new RelayCommand(_ => AdicionarCustoExtra());
            DeletarCustoCommand = new RelayCommand(param => DeletarCustoExtra(param as Custo));

            GerarPdfCommand = new RelayCommand(_ => ExecutarFluxoFinal());
        }

        private void NovoProjeto()
        {
            ProjetoAtual = new Projeto
            {
                Orcamento = new Orcamento { MargemPercentual = 20, PercentualImpostos = 15 },
                Tarefas = new ObservableCollection<Tarefa>(),
                UsuarioId = 1,
                Nome = "Novo Orçamento Aero"
            };

            CustosExtras = new ObservableCollection<Custo>();
            ProjetoAtual.Orcamento.PropertyChanged += (s, e) => AtualizarFinanceiro();
            OnPropertyChanged(nameof(CustosExtras));
        }

        private void AdicionarTarefa()
        {
            var novaTarefa = new Tarefa { Descricao = "Nova Atividade Técnica", HorasEstimadas = 0 };
            novaTarefa.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(Tarefa.CustoReal) || e.PropertyName == nameof(Tarefa.Funcionario))
                    AtualizarFinanceiro();
            };

            ProjetoAtual.Tarefas.Add(novaTarefa);
            AtualizarFinanceiro();
        }

        private void DeletarTarefa(Tarefa tarefa)
        {
            if (tarefa != null && ProjetoAtual.Tarefas.Contains(tarefa))
            {
                ProjetoAtual.Tarefas.Remove(tarefa);
                AtualizarFinanceiro();
            }
        }

        private void AdicionarCustoExtra()
        {
            var novoCusto = new Custo { Nome = "Novo Item", Valor = 0, Categoria = "Equipamentos", Tipo = "Direto" };
            novoCusto.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(Custo.Valor)) AtualizarFinanceiro();
            };

            CustosExtras.Add(novoCusto);
            AtualizarFinanceiro();
        }

        private void DeletarCustoExtra(Custo custo)
        {
            if (custo != null && CustosExtras.Contains(custo))
            {
                CustosExtras.Remove(custo);
                AtualizarFinanceiro();
            }
        }

        private void AtualizarFinanceiro()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (ProjetoAtual?.Orcamento != null)
                {
                    ProjetoAtual.Orcamento.CalcularTotal(
                        ProjetoAtual.Tarefas.ToList(),
                        CustosExtras.ToList()
                    );
                    OnPropertyChanged(nameof(ProjetoAtual));
                }
            }));
        }

        private void ExecutarFluxoFinal()
        {
            if (_processando) return;

            if (string.IsNullOrWhiteSpace(ProjetoAtual.Nome) || ProjetoAtual.Cliente == null)
            {
                MessageBox.Show("Preencha o Nome do Projeto e o Cliente.", "Aero Concepts");
                return;
            }

            try
            {
                _processando = true;
                OnPropertyChanged(nameof(BotaoAtivo));

                if (SalvarNoBancoSilencioso())
                {
                    if (GerarRelatorioPdf())
                    {
                        MessageBox.Show("Proposta finalizada com sucesso!", "Sucesso");
                        NovoProjeto();
                    }
                }
            }
            finally
            {
                _processando = false;
                OnPropertyChanged(nameof(BotaoAtivo));
            }
        }

        private bool SalvarNoBancoSilencioso()
        {
            try
            {
                var repo = new ProjetoRepository();
                repo.SalvarProjetoCompleto(ProjetoAtual, CustosExtras.ToList());
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar: " + ex.Message);
                return false;
            }
        }

        private bool GerarRelatorioPdf()
        {
            var sfd = new SaveFileDialog
            {
                Filter = "PDF|*.pdf",
                FileName = $"Proposta_{ProjetoAtual.Nome}"
            };

            if (sfd.ShowDialog() == true)
            {
                new PdfService().GerarPropostaTecnica(ProjetoAtual, CustosExtras.ToList(), sfd.FileName);
                return true;
            }
            return false;
        }

        private void CarregarDadosIniciais()
        {
            try
            {
                Clientes = new ObservableCollection<Cliente>(new ClienteRepository().ListarTodos());
                Funcionarios = new ObservableCollection<Funcionario>(new FuncionarioRepository().ListarTodos());
            }
            catch { /* Log error */ }
        }
    }
}