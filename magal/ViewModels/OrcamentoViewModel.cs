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
        private bool _isUpdating = false;

        public Projeto ProjetoAtual
        {
            get => _projetoAtual;
            set { _projetoAtual = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Cliente> Clientes { get; } = new ObservableCollection<Cliente>();
        public ObservableCollection<Funcionario> Funcionarios { get; } = new ObservableCollection<Funcionario>();
        public ObservableCollection<Custo> CustosExtras { get; } = new ObservableCollection<Custo>();

        public List<string> CategoriasCustos { get; } = new List<string>
        {
            "Equipamentos", "Licenças de Software", "Energia Elétrica",
            "Transporte/Deslocamento", "Manutenção", "Aluguel/Estrutura", "EPIs/Ferramentas"
        };

        public List<string> OpcoesStatus { get; } = new List<string>
        {
            "Rascunho", "Orçado", "Aprovado", "Executando", "Concluído", "Cancelado"
        };

        public List<string> OpcoesTipo { get; } = new List<string>
        {
            "Serviço", "Produto", "Consultoria", "P&D"
        };

        public bool BotaoAtivo => !_processando;
        public RelayCommand AdicionarTarefaCommand { get; }
        public RelayCommand DeletarTarefaCommand { get; }
        public RelayCommand AdicionarCustoCommand { get; }
        public RelayCommand DeletarCustoCommand { get; }
        public RelayCommand GerarPdfCommand { get; }

        public OrcamentoViewModel()
        {
            AdicionarTarefaCommand = new RelayCommand(_ => AdicionarTarefa());
            DeletarTarefaCommand = new RelayCommand(param => DeletarTarefa(param as Tarefa));
            AdicionarCustoCommand = new RelayCommand(_ => AdicionarCustoExtra());
            DeletarCustoCommand = new RelayCommand(param => DeletarCustoExtra(param as Custo));
            GerarPdfCommand = new RelayCommand(_ => ExecutarFluxoFinal());

            NovoProjeto();
            CarregarDadosIniciais();
        }

        private void NovoProjeto()
        {
            ProjetoAtual = new Projeto
            {
                Orcamento = new Orcamento { margem_percentual = 20, percentual_impostos = 15 },
                Tarefas = new ObservableCollection<Tarefa>(),
                id_usuario = 1,
                nome = ""
            };

            CustosExtras.Clear();
            OnPropertyChanged(nameof(ProjetoAtual));
        }

        private void CarregarDadosIniciais()
        {
            try
            {
                var listaClientes = new ClienteRepository().ListarTodos();
                var listaFuncionarios = new FuncionarioRepository().ListarTodos();

                Clientes.Clear();
                foreach (var c in listaClientes) Clientes.Add(c);

                Funcionarios.Clear();
                foreach (var f in listaFuncionarios) Funcionarios.Add(f);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados: " + ex.Message);
            }
        }

        private void AdicionarTarefa()
        {
            var novaTarefa = new Tarefa { descricao = "", horas_estimadas = 0 };

            novaTarefa.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(Tarefa.Funcionario) ||
                    e.PropertyName == nameof(Tarefa.horas_estimadas))
                {
                    AtualizarFinanceiro();
                }
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
            var novoCusto = new Custo { nome = "", valor = 0, categoria = "Equipamentos", tipo = "Direto" };

            novoCusto.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(Custo.valor)) AtualizarFinanceiro();
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
            if (_isUpdating || ProjetoAtual?.Orcamento == null) return;

            try
            {
                _isUpdating = true;

                // --- TRAVA DE VALORES NEGATIVOS ---
                // Limpa as tarefas
                foreach (var t in ProjetoAtual.Tarefas)
                {
                    if (t.horas_estimadas < 0) t.horas_estimadas = 0;
                }

                // Limpa os custos extras
                foreach (var c in CustosExtras)
                {
                    if (c.valor < 0) c.valor = 0;
                }

                // Limpa os impostos e margem no orçamento
                if (ProjetoAtual.Orcamento.margem_percentual < 0) ProjetoAtual.Orcamento.margem_percentual = 0;
                if (ProjetoAtual.Orcamento.percentual_impostos < 0) ProjetoAtual.Orcamento.percentual_impostos = 0;
                // ----------------------------------

                ProjetoAtual.Orcamento.CalcularTotal(
                    ProjetoAtual.Tarefas.ToList(),
                    CustosExtras.ToList()
                );

                ProjetoAtual.Orcamento.OnPropertyChanged("valor_total");
                ProjetoAtual.Orcamento.OnPropertyChanged("valor_impostos");
                ProjetoAtual.Orcamento.OnPropertyChanged("lucro_estimado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro no cálculo: " + ex.Message);
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void ExecutarFluxoFinal()
        {
            if (_processando) return;

            if (string.IsNullOrWhiteSpace(ProjetoAtual.nome) || (ProjetoAtual.id_cliente == 0 && ProjetoAtual.Cliente == null))
            {
                MessageBox.Show("Preencha o Nome do Projeto e selecione um Cliente.", "Aero Concepts");
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
                        MessageBox.Show("Proposta finalizada e salva com sucesso!", "Sucesso");
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
                if (ProjetoAtual.Cliente != null)
                {
                    ProjetoAtual.id_cliente = ProjetoAtual.Cliente.id_cliente;
                }

                var repo = new ProjetoRepository();
                repo.SalvarProjetoCompleto(ProjetoAtual, CustosExtras.ToList());
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar no banco: " + ex.Message, "Erro");
                return false;
            }
        }

        private bool GerarRelatorioPdf()
        {
            var sfd = new SaveFileDialog
            {
                Filter = "PDF|*.pdf",
                FileName = $"Proposta_{ProjetoAtual.nome}"
            };

            if (sfd.ShowDialog() == true)
            {
                new PdfService().GerarPropostaTecnica(ProjetoAtual, CustosExtras.ToList(), sfd.FileName);
                return true;
            }
            return false;
        }

        public void CarregarProjetoParaEdicao(Projeto projetoDoBanco)
        {
            if (projetoDoBanco == null) return;
            _isUpdating = true;

            try
            {
                this.ProjetoAtual.id_projeto = projetoDoBanco.id_projeto;
                this.ProjetoAtual.nome = projetoDoBanco.nome;
                this.ProjetoAtual.id_cliente = projetoDoBanco.id_cliente;
                this.ProjetoAtual.tipo = projetoDoBanco.tipo;
                this.ProjetoAtual.status = projetoDoBanco.status;

                if (projetoDoBanco.Orcamento != null)
                {
                    this.ProjetoAtual.Orcamento = projetoDoBanco.Orcamento;
                }

                this.ProjetoAtual.Cliente = Clientes.FirstOrDefault(c => c.id_cliente == projetoDoBanco.id_cliente);

                this.ProjetoAtual.Tarefas.Clear();
                foreach (var t in projetoDoBanco.Tarefas)
                {
                    t.Funcionario = Funcionarios.FirstOrDefault(f => f.id_funcionario == t.id_funcionario);
                    // Garante que o evento de mudança de propriedade seja assinado ao carregar do banco
                    t.PropertyChanged += (s, e) => {
                        if (e.PropertyName == nameof(Tarefa.Funcionario) || e.PropertyName == nameof(Tarefa.horas_estimadas))
                            AtualizarFinanceiro();
                    };
                    this.ProjetoAtual.Tarefas.Add(t);
                }

                this.CustosExtras.Clear();
                if (projetoDoBanco.Custos != null)
                {
                    foreach (var c in projetoDoBanco.Custos)
                    {
                        c.PropertyChanged += (s, e) => {
                            if (e.PropertyName == nameof(Custo.valor)) AtualizarFinanceiro();
                        };
                        this.CustosExtras.Add(c);
                    }
                }

                _isUpdating = false;
                AtualizarFinanceiro();
            }
            finally
            {
                _isUpdating = false;
            }
            OnPropertyChanged(nameof(ProjetoAtual));
        }
    }
}