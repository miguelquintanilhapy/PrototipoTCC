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
            set
            {
                _projetoAtual = value;
                OnPropertyChanged();
                // Sempre que um projeto novo é atribuído, precisamos ouvir o orçamento dele
                AssinarEventosOrcamento();
            }
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
            _isUpdating = true;

            var p = new Projeto
            {
                Orcamento = new Orcamento { margem_percentual = 20, percentual_impostos = 15 },
                Tarefas = new ObservableCollection<Tarefa>(),
                id_usuario = 1, 
                nome = "",
                status = "Rascunho",
                tipo = "Serviço",
                data_criacao = DateTime.Now
            };

            CustosExtras.Clear();
            ProjetoAtual = p;

            _isUpdating = false;
            AtualizarFinanceiro();
        }

        private void AssinarEventosOrcamento()
        {
            if (ProjetoAtual?.Orcamento != null)
            {
                // garante que se o usuário mudar a % na tela, o cálculo rode
                ProjetoAtual.Orcamento.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(Orcamento.margem_percentual) ||
                        e.PropertyName == nameof(Orcamento.percentual_impostos))
                    {
                        AtualizarFinanceiro();
                    }
                };
            }
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

                if (ProjetoAtual.Orcamento.margem_percentual < 0) ProjetoAtual.Orcamento.margem_percentual = 0;
                if (ProjetoAtual.Orcamento.percentual_impostos < 0) ProjetoAtual.Orcamento.percentual_impostos = 0;

                // executa a lógica de cálculo definida na Model
                ProjetoAtual.Orcamento.CalcularTotal(
                    ProjetoAtual.Tarefas.ToList(),
                    CustosExtras.ToList()
                );

                // notifica a View de que os campos de resultado mudaram
                ProjetoAtual.Orcamento.OnPropertyChanged("valor_total");
                ProjetoAtual.Orcamento.OnPropertyChanged("valor_impostos");
                ProjetoAtual.Orcamento.OnPropertyChanged("lucro_estimado");
                ProjetoAtual.Orcamento.OnPropertyChanged("valor_final");
                ProjetoAtual.Orcamento.OnPropertyChanged("custo_base");
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

        public void CarregarProjetoParaEdicao(Projeto projetoDoBanco)
        {
            if (projetoDoBanco == null) return;
            _isUpdating = true;

            try
            {
                // atribui o projeto e garante que o Orçamento notifique a VM
                this.ProjetoAtual = projetoDoBanco;
                AssinarEventosOrcamento();

                // sincroniza o cliente (para o ComboBox selecionar o item certo)
                if (this.ProjetoAtual.id_cliente > 0)
                {
                    this.ProjetoAtual.Cliente = Clientes.FirstOrDefault(c => c.id_cliente == projetoDoBanco.id_cliente);
                }

                // limpa e recarrega a lista de Custos Extras da ViewModel
                this.CustosExtras.Clear();
                if (projetoDoBanco.Custos != null)
                {
                    foreach (var c in projetoDoBanco.Custos)
                    {
                        // Assina o evento para que se você mudar o valor do custo na edição, o total atualize
                        c.PropertyChanged += (s, e) => {
                            if (e.PropertyName == nameof(Custo.valor)) AtualizarFinanceiro();
                        };
                        this.CustosExtras.Add(c);
                    }
                }

                foreach (var t in this.ProjetoAtual.Tarefas)
                {
                    // vincula o objeto funcionário da lista da ViewModel à tarefa
                    t.Funcionario = Funcionarios.FirstOrDefault(f => f.id_funcionario == t.id_funcionario);

                    // assiona o evento para que mudanças nas horas ou funcionário recalculem o preço
                    t.PropertyChanged += (s, e) => {
                        if (e.PropertyName == nameof(Tarefa.Funcionario) || e.PropertyName == nameof(Tarefa.horas_estimadas))
                            AtualizarFinanceiro();
                    };
                }

                _isUpdating = false;
                AtualizarFinanceiro();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar edição: " + ex.Message);
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
                    ProjetoAtual.id_cliente = ProjetoAtual.Cliente.id_cliente;

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
                FileName = $"Proposta_{ProjetoAtual.nome}_{DateTime.Now:yyyyMMdd}"
            };

            if (sfd.ShowDialog() == true)
            {
                new PdfService().GerarPropostaTecnica(ProjetoAtual, CustosExtras.ToList(), sfd.FileName);
                return true;
            }
            return false;
        }
    }
}