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

        public void CarregarProjetoParaEdicao(Projeto projetoDoBanco)
        {
            if (projetoDoBanco == null) return;
            _isUpdating = true;
            try
            {
                this.ProjetoAtual = projetoDoBanco;
                AssinarEventosOrcamento();

                if (this.ProjetoAtual.id_cliente > 0)
                    this.ProjetoAtual.Cliente = Clientes.FirstOrDefault(c => c.id_cliente == projetoDoBanco.id_cliente);

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

                foreach (var t in this.ProjetoAtual.Tarefas)
                {
                    t.Funcionario = Funcionarios.FirstOrDefault(f => f.id_funcionario == t.id_funcionario);
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
                MessageBox.Show("Erro ao carregar edição: " + ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally { _isUpdating = false; }
        }

        private void AssinarEventosOrcamento()
        {
            if (ProjetoAtual?.Orcamento != null)
            {
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
                MessageBox.Show("Erro ao carregar dados iniciais: " + ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AdicionarTarefa()
        {
            var novaTarefa = new Tarefa { descricao = "", horas_estimadas = 0 };
            novaTarefa.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(Tarefa.Funcionario) || e.PropertyName == nameof(Tarefa.horas_estimadas))
                    AtualizarFinanceiro();
            };
            ProjetoAtual.Tarefas.Add(novaTarefa);
            AtualizarFinanceiro();
        }

        private void DeletarTarefa(Tarefa tarefa)
        {
            if (tarefa != null && ProjetoAtual.Tarefas.Contains(tarefa))
            {
                var result = MessageBox.Show($"Remover a tarefa '{tarefa.descricao}'?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    ProjetoAtual.Tarefas.Remove(tarefa);
                    AtualizarFinanceiro();
                }
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
                var result = MessageBox.Show($"Remover o custo '{custo.nome}'?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    CustosExtras.Remove(custo);
                    AtualizarFinanceiro();
                }
            }
        }

        private void AtualizarFinanceiro()
        {
            if (_isUpdating || ProjetoAtual?.Orcamento == null) return;
            try
            {
                _isUpdating = true;
                ProjetoAtual.Orcamento.CalcularTotal(ProjetoAtual.Tarefas.ToList(), CustosExtras.ToList());
                ProjetoAtual.Orcamento.OnPropertyChanged("valor_total");
                ProjetoAtual.Orcamento.OnPropertyChanged("valor_final");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro no cálculo: " + ex.Message);
            }
            finally { _isUpdating = false; }
        }

        private void ExecutarFluxoFinal()
        {
            if (_processando) return;

            // 1. Validação de Nome
            if (string.IsNullOrWhiteSpace(ProjetoAtual.nome))
            {
                MessageBox.Show("Não é possível gerar a proposta. O campo 'Nome do Projeto' deve ser preenchido.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Validação de Cliente
            if (ProjetoAtual.Cliente == null)
            {
                MessageBox.Show("Por favor, selecione um cliente antes de finalizar a proposta.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // --- NOVA VALIDAÇÃO DE ITENS VAZIOS ---
            // Verifica se a lista de tarefas E a lista de custos extras estão vazias
            if ((ProjetoAtual.Tarefas == null || ProjetoAtual.Tarefas.Count == 0) &&
                (CustosExtras == null || CustosExtras.Count == 0))
            {
                MessageBox.Show("A proposta não contém nenhum item (Tarefa ou Custo). Adicione pelo menos um item para continuar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // ---------------------------------------

            // 3. Confirmação Final
            var confirm = MessageBox.Show("Deseja salvar os dados e gerar o PDF da proposta agora?", "Confirmar Proposta", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                _processando = true;
                OnPropertyChanged(nameof(BotaoAtivo));

                if (SalvarNoBancoSilencioso())
                {
                    if (GerarRelatorioPdf())
                    {
                        MessageBox.Show("Proposta finalizada e salva com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Erro crítico ao salvar: " + ex.Message, "Aviso de Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        private bool GerarRelatorioPdf()
        {
            var sfd = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = $"Proposta_{ProjetoAtual.nome}" };

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    new PdfService().GerarPropostaTecnica(ProjetoAtual, CustosExtras.ToList(), sfd.FileName);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao gerar PDF: " + ex.Message, "Aviso de Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            return false;
        }
    }
}