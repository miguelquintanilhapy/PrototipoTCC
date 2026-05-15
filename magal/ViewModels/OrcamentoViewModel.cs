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

        // Variáveis para controle de descarte e verificação
        private Projeto _projetoOriginal;
        private List<decimal> _custosValoresOriginais;

        public Projeto ProjetoAtual
        {
            get => _projetoAtual;
            set
            {
                _projetoAtual = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DataExpiracaoFormatada));
                AssinarEventosOrcamento();
            }
        }

        public ObservableCollection<Cliente> Clientes { get; } = new ObservableCollection<Cliente>();
        public ObservableCollection<Funcionario> Funcionarios { get; } = new ObservableCollection<Funcionario>();
        public ObservableCollection<Custo> CustosExtras { get; } = new ObservableCollection<Custo>();

        public List<string> CategoriasCustos { get; } = new List<string> { "Equipamentos", "Licenças de Software", "Energia Elétrica", "Transporte/Deslocamento", "Manutenção", "Aluguel/Estrutura", "EPIs/Ferramentas" };
        public List<string> OpcoesStatus { get; } = new List<string> { "Rascunho", "Orçado", "Aprovado", "Executando", "Concluído", "Cancelado" };
        public List<string> OpcoesTipo { get; } = new List<string> { "Serviço", "Produto", "Consultoria", "P&D" };

        public bool BotaoAtivo => !_processando;
        public RelayCommand AdicionarTarefaCommand { get; }
        public RelayCommand DeletarTarefaCommand { get; }
        public RelayCommand AdicionarCustoCommand { get; }
        public RelayCommand DeletarCustoCommand { get; }
        public RelayCommand GerarPdfCommand { get; }
        public RelayCommand DescartarCommand { get; }

        public OrcamentoViewModel()
        {
            AdicionarTarefaCommand = new RelayCommand(_ => AdicionarTarefa());
            DeletarTarefaCommand = new RelayCommand(param => DeletarTarefa(param as Tarefa));
            AdicionarCustoCommand = new RelayCommand(_ => AdicionarCustoExtra());
            DeletarCustoCommand = new RelayCommand(param => DeletarCustoExtra(param as Custo));
            GerarPdfCommand = new RelayCommand(_ => ExecutarFluxoFinal());
            DescartarCommand = new RelayCommand(_ => ExecutarDescarte());

            CarregarDadosIniciais();
            NovoProjeto();
        }

        // CORREÇÃO: Removido o "=" que sobrescrevia a data do projeto
        public string DataExpiracaoFormatada
        {
            get
            {
                if (ProjetoAtual?.Orcamento == null) return "-";

                // Apenas lê a data, não altera o objeto
                DateTime dataBase = ProjetoAtual.data_criacao;

                if (dataBase == DateTime.MinValue)
                    dataBase = DateTime.Now;

                return dataBase.AddDays(ProjetoAtual.Orcamento.validade_dias).ToString("dd/MM/yyyy");
            }
        }

        private bool TemAlteracoes()
        {
            if (_projetoOriginal == null) return !string.IsNullOrWhiteSpace(ProjetoAtual.nome) || ProjetoAtual.Tarefas.Count > 0;

            // Comparação rigorosa para detectar mudanças, incluindo a validade
            bool basicoAlterado = ProjetoAtual.nome != _projetoOriginal.nome ||
                                  ProjetoAtual.id_cliente != _projetoOriginal.id_cliente ||
                                  ProjetoAtual.status != _projetoOriginal.status ||
                                  ProjetoAtual.tipo != _projetoOriginal.tipo ||
                                  ProjetoAtual.Orcamento.margem_percentual != _projetoOriginal.Orcamento.margem_percentual ||
                                  ProjetoAtual.Orcamento.percentual_impostos != _projetoOriginal.Orcamento.percentual_impostos ||
                                  ProjetoAtual.Orcamento.validade_dias != _projetoOriginal.Orcamento.validade_dias;

            if (basicoAlterado) return true;
            if (ProjetoAtual.Tarefas.Count != _projetoOriginal.Tarefas.Count) return true;
            if (CustosExtras.Count != (_custosValoresOriginais?.Count ?? 0)) return true;

            return false;
        }

        private void ExecutarDescarte()
        {
            var result = MessageBox.Show("Deseja descartar todas as alterações e voltar ao histórico?", "Atenção",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var mainWindow = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
                mainWindow?.AbrirHistorico();
            }
        }

        public Visibility VisibilidadeBotaoDescartar =>
            (_projetoOriginal != null) ? Visibility.Visible : Visibility.Collapsed;

        public void CarregarProjetoParaEdicao(Projeto projetoDoBanco)
        {
            if (projetoDoBanco == null) return;

            _isUpdating = true;
            try
            {
                this.ProjetoAtual = projetoDoBanco;

                // BACKUP PARA COMPARAÇÃO (Deep Copy simples)
                _projetoOriginal = new Projeto
                {
                    nome = projetoDoBanco.nome,
                    id_cliente = projetoDoBanco.id_cliente,
                    status = projetoDoBanco.status,
                    tipo = projetoDoBanco.tipo,
                    data_criacao = projetoDoBanco.data_criacao,
                    Orcamento = new Orcamento
                    {
                        margem_percentual = projetoDoBanco.Orcamento.margem_percentual,
                        percentual_impostos = projetoDoBanco.Orcamento.percentual_impostos,
                        validade_dias = projetoDoBanco.Orcamento.validade_dias
                    },
                    Tarefas = new ObservableCollection<Tarefa>(projetoDoBanco.Tarefas.ToList())
                };

                _custosValoresOriginais = projetoDoBanco.Custos?.Select(c => c.valor).ToList() ?? new List<decimal>();

                OnPropertyChanged(nameof(VisibilidadeBotaoDescartar));
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar edição: " + ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                _isUpdating = false;
                AtualizarFinanceiro();
            }
        }

        private void ExecutarFluxoFinal()
        {
            if (_processando) return;

            if (!TemAlteracoes())
            {
                MessageBox.Show("Nenhuma alteração foi detectada no projeto.", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(ProjetoAtual.nome))
            {
                MessageBox.Show("O campo 'Nome do Projeto' deve ser preenchido.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ProjetoAtual.Cliente == null)
            {
                MessageBox.Show("Selecione um cliente antes de finalizar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show("Deseja salvar as alterações e gerar o PDF?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                _processando = true;
                OnPropertyChanged(nameof(BotaoAtivo));

                if (SalvarNoBancoSilencioso())
                {
                    if (GerarRelatorioPdf())
                    {
                        MessageBox.Show("Alterações salvas com sucesso!", "Sucesso", MessageBoxButton.OK);
                        var mainWindow = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
                        mainWindow?.AbrirHistorico();
                    }
                }
            }
            finally
            {
                _processando = false;
                OnPropertyChanged(nameof(BotaoAtivo));
            }
        }

        private void NovoProjeto()
        {
            _isUpdating = true;
            var p = new Projeto
            {
                Orcamento = new Orcamento { margem_percentual = 20, percentual_impostos = 15, validade_dias = 15 },
                Tarefas = new ObservableCollection<Tarefa>(),
                id_usuario = 1,
                nome = "",
                status = "Rascunho",
                tipo = "Serviço",
                data_criacao = DateTime.Now
            };

            ProjetoAtual = p;
            _projetoOriginal = null;
            CustosExtras.Clear();

            AdicionarTarefa();
            AdicionarCustoExtra();

            _isUpdating = false;
            AtualizarFinanceiro();
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

                    // Notifica a interface para atualizar a data escrita por extenso
                    if (e.PropertyName == nameof(Orcamento.validade_dias))
                    {
                        OnPropertyChanged(nameof(DataExpiracaoFormatada));
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

                if (ProjetoAtual != null && _projetoOriginal == null)
                {
                    if (ProjetoAtual.Cliente == null && Clientes.Count > 0)
                    {
                        ProjetoAtual.Cliente = Clientes[0];
                        ProjetoAtual.id_cliente = Clientes[0].id_cliente;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados iniciais: " + ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AdicionarTarefa()
        {
            var primeiroFunc = Funcionarios.FirstOrDefault();
            var novaTarefa = new Tarefa
            {
                descricao = "",
                horas_estimadas = 0,
                Funcionario = primeiroFunc,
                id_funcionario = primeiroFunc?.id_funcionario ?? 0
            };

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
                string identificador = string.IsNullOrWhiteSpace(tarefa.descricao)
                    ? "esta tarefa"
                    : $"a tarefa '{tarefa.descricao}'";

                var result = MessageBox.Show($"Deseja remover {identificador}?", "Atenção",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

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
                if (e.PropertyName == nameof(Custo.valor))
                    AtualizarFinanceiro();
            };
            CustosExtras.Add(novoCusto);
            AtualizarFinanceiro();
        }

        private void DeletarCustoExtra(Custo custo)
        {
            if (custo != null && CustosExtras.Contains(custo))
            {
                string identificador = string.IsNullOrWhiteSpace(custo.nome)
                    ? "este item"
                    : $"o custo '{custo.nome}'";

                var result = MessageBox.Show($"Deseja remover {identificador}?", "Atenção",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    CustosExtras.Remove(custo);
                    AtualizarFinanceiro();
                }
            }
        }

        private void AtualizarFinanceiro()
        {
            if (ProjetoAtual?.Orcamento == null || _isUpdating) return;

            try
            {
                ProjetoAtual.Orcamento.CalcularTotal(
                    ProjetoAtual.Tarefas.ToList(),
                    CustosExtras.ToList()
                );

                // Força a UI a revalidar os campos financeiros
                OnPropertyChanged(nameof(ProjetoAtual));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro no cálculo: " + ex.Message);
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