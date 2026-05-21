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
    /// <summary>
    /// ViewModel responsável por gerenciar a elaçaboração e edição de orçamentos de projetos,
    /// controlando custos diretos, alocação de equipe (tarefas), margens, impostos e exportação em PDF.
    /// </summary>
    public class OrcamentoViewModel : BaseModel
    {
        #region Atributos e Campos Privados

        private Projeto _projetoAtual;
        private bool _processando = false;
        private bool _isUpdating = false;

        // Variáveis para controle de descarte e verificação de concorrência/mudanças
        private Projeto _projetoOriginal;
        private List<decimal> _custosValoresOriginais;
        private decimal _valorFinalOriginal;
        private decimal _totalHorasOriginal;

        #endregion

        #region Propriedades e Filtros

        /// <summary>
        /// Obtém ou define o projeto que está sendo orçado ou editado atualmente na tela.
        /// </summary>
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

        /// <summary>
        /// Obtém a string contendo a data limite de validade da proposta comercial calculada por extenso.
        /// </summary>
        public string DataExpiracaoFormatada
        {
            get
            {
                if (ProjetoAtual?.Orcamento == null) return "-";

                DateTime dataBase = ProjetoAtual.data_criacao;

                if (dataBase == DateTime.MinValue)
                    dataBase = DateTime.Now;

                return dataBase.AddDays(ProjetoAtual.Orcamento.validade_dias).ToString("dd/MM/yyyy");
            }
        }

        /// <summary>
        /// Obtém o estado de permissão do botão de execução (Bloqueia reentrância caso o fluxo esteja processando).
        /// </summary>
        public bool BotaoAtivo => !_processando;

        /// <summary>
        /// Determina o comportamento visual do botão de descarte (Exibido apenas em modo de edição).
        /// </summary>
        public Visibility VisibilidadeBotaoDescartar =>
            (_projetoOriginal != null) ? Visibility.Visible : Visibility.Collapsed;

        #endregion

        #region Coleções Estáticas e Listas Auxiliares

        /// <summary>
        /// Fonte de dados com todos os clientes elegíveis para vinculação ao projeto.
        /// </summary>
        public ObservableCollection<Cliente> Clientes { get; } = new ObservableCollection<Cliente>();

        /// <summary>
        /// Fonte de dados contendo os funcionários ativos para vinculação nas tarefas operacionais.
        /// </summary>
        public ObservableCollection<Funcionario> Funcionarios { get; } = new ObservableCollection<Funcionario>();

        /// <summary>
        /// Lista de custos adicionais e despesas diretas associadas ao projeto atual.
        /// </summary>
        public ObservableCollection<Custo> CustosExtras { get; } = new ObservableCollection<Custo>();

        /// <summary>
        /// Categorias fixas de despesas de infraestrutura e projetos.
        /// </summary>
        public List<string> CategoriasCustos { get; } = new List<string> { "Equipamentos", "Licenças de Software", "Energia Elétrica", "Transporte/Deslocamento", "Manutenção", "Aluguel/Estrutura", "EPIs/Ferramentas" };

        /// <summary>
        /// Lista de controle de fluxo de estados físicos do projeto.
        /// </summary>
        public List<string> OpcoesStatus { get; } = new List<string> { "Rascunho", "Orçado", "Aprovado", "Executando", "Concluído", "Cancelado" };

        /// <summary>
        /// Opções de enquadramento técnico do projeto.
        /// </summary>
        public List<string> OpcoesTipo { get; } = new List<string> { "Serviço", "Produto", "Consultoria", "P&D" };

        #endregion

        #region Comandos disparados pela View

        /// <summary>
        /// Comando para inserir uma nova linha de tarefa na matriz técnica do projeto.
        /// </summary>
        public RelayCommand AdicionarTarefaCommand { get; }

        /// <summary>
        /// Comando para expurgar uma linha de tarefa técnica do escopo físico do projeto.
        /// </summary>
        public RelayCommand DeletarTarefaCommand { get; }

        /// <summary>
        /// Comando para adicionar uma nova linha de despesa na tabela de custos extras.
        /// </summary>
        public RelayCommand AdicionarCustoCommand { get; }

        /// <summary>
        /// Comando para deletar uma despesa extra da lista de custos do projeto.
        /// </summary>
        public RelayCommand DeletarCustoCommand { get; }

        /// <summary>
        /// Comando mestre para salvar o projeto de forma persistente e gerar a proposta comercial consolidada.
        /// </summary>
        public RelayCommand GerarPdfCommand { get; }

        /// <summary>
        /// Comando para anular alterações feitas em modo de edição e retroceder a visualização.
        /// </summary>
        public RelayCommand DescartarCommand { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="OrcamentoViewModel"/>, mapeando as ações e gerando um novo template limpo.
        /// </summary>
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

        #endregion

        #region Métodos Públicos (Controle de Estado Interno)

        /// <summary>
        /// Transpõe e injeta uma instância de projeto vinda do banco de dados para a tela de edição, gerando backups de descarte.
        /// </summary>
        /// <param name="projetoDoBanco">O objeto persistido do projeto que será mapeado para a tela.</param>
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
                _valorFinalOriginal = projetoDoBanco.Orcamento?.valor_final ?? 0;
                _totalHorasOriginal = projetoDoBanco.Tarefas?.Sum(t => t.horas_estimadas) ?? 0;

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

        #endregion

        #region Métodos Auxiliares / Privados

        /// <summary>
        /// Compara minuciosamente o estado atual do formulário com o backup original para verificar se há dados modificados.
        /// </summary>
        /// <returns><c>true</c> se houver alterações detectadas; caso contrário, <c>false</c>.</returns>
        private bool TemAlteracoes()
        {
            if (_projetoOriginal == null) return !string.IsNullOrWhiteSpace(ProjetoAtual.nome) || ProjetoAtual.Tarefas.Count > 0;

            // 1. Verifica se dados textuais ou configurações do orçamento mudaram
            bool basicoAlterado = ProjetoAtual.nome != _projetoOriginal.nome ||
                                  ProjetoAtual.id_cliente != _projetoOriginal.id_cliente ||
                                  ProjetoAtual.status != _projetoOriginal.status ||
                                  ProjetoAtual.tipo != _projetoOriginal.tipo ||
                                  ProjetoAtual.Orcamento.margem_percentual != _projetoOriginal.Orcamento.margem_percentual ||
                                  ProjetoAtual.Orcamento.percentual_impostos != _projetoOriginal.Orcamento.percentual_impostos ||
                                  ProjetoAtual.Orcamento.validade_dias != _projetoOriginal.Orcamento.validade_dias;

            if (basicoAlterado) return true;

            // 2. Verifica se a quantidade de linhas de tarefas ou custos mudou
            if (ProjetoAtual.Tarefas.Count != _projetoOriginal.Tarefas.Count) return true;
            if (CustosExtras.Count != (_custosValoresOriginais?.Count ?? 0)) return true;

            // 3. Verifica se os valores internos (horas ou dinheiro) mudaram
            decimal totalHorasAtual = ProjetoAtual.Tarefas?.Sum(t => t.horas_estimadas) ?? 0;
            if (totalHorasAtual != _totalHorasOriginal) return true;

            if (ProjetoAtual.Orcamento.valor_final != _valorFinalOriginal) return true;

            // Se tudo for igual, nenhuma alteração real foi feita
            return false;
        }

        /// <summary>
        /// Cancela as edições correntes e força a janela a retroceder para a tela de histórico.
        /// </summary>
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

        /// <summary>
        /// Executa a validação estrutural de segurança, salva os dados fisicamente e aciona o motor de relatórios em PDF.
        /// </summary>
        /// <summary>
        /// Executa a validação estrutural de segurança, salva os dados fisicamente no banco de dados 
        /// e oferece a opção de gerar a proposta comercial consolidada em PDF.
        /// </summary>
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

            var confirm = MessageBox.Show("Deseja salvar as alterações deste projeto?", "Confirmar Salvamento", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                _processando = true;
                OnPropertyChanged(nameof(BotaoAtivo));

                //Salva sempre no banco de dados primeiro de forma silenciosa
                if (SalvarNoBancoSilencioso())
                {
                    // Pergunta ao usuário de forma amigável se ele quer o PDF agora
                    var respostaPdf = MessageBox.Show(
                        "Alterações gravadas no banco de dados com sucesso!\n\nDeseja gerar o relatório em PDF desta proposta?",
                        "Gerar PDF Opcional",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (respostaPdf == MessageBoxResult.Yes)
                    {
                        // Tenta gerar o PDF. Se ele gerar ou cancelar o arquivo, o projeto já está salvo de qualquer forma.
                        GerarRelatorioPdf();
                    }

                    // Sucesso absoluto e redirecionamento para a tela de histórico
                    MessageBox.Show("Projeto salvo com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                    var mainWindow = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
                    mainWindow?.AbrirHistorico();
                }
            }
            finally
            {
                _processando = false;
                OnPropertyChanged(nameof(BotaoAtivo));
            }
        }

        /// <summary>
        /// Instancia um novo formulário com valores padrão para a criação de um orçamento limpo.
        /// </summary>
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

        /// <summary>
        /// Assina o ouvinte de eventos de propriedade do modelo interno de orçamento para recalcular o fluxo financeiro em tempo real.
        /// </summary>
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

                    if (e.PropertyName == nameof(Orcamento.validade_dias))
                    {
                        OnPropertyChanged(nameof(DataExpiracaoFormatada));
                    }
                };
            }
        }

        /// <summary>
        /// Popula as coleções iniciais de clientes e funcionários carregando os dados do banco de dados.
        /// </summary>
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

        /// <summary>
        /// Insere uma nova linha técnica de tarefas e aciona as escutas de alteração de horas e valores.
        /// </summary>
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

        /// <summary>
        /// Remove com validação e confirmação em tela uma tarefa vinculada ao escopo do projeto.
        /// </summary>
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

        /// <summary>
        /// Insere uma nova linha de despesas na grade de custos diretos e externos.
        /// </summary>
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

        /// <summary>
        /// Remove um custo extra associado da lista de insumos com confirmação em tela.
        /// </summary>
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

        /// <summary>
        /// Dispara o motor matemático de cálculo unificado de precificação das entidades acopladas ao orçamento.
        /// </summary>
        private void AtualizarFinanceiro()
        {
            if (ProjetoAtual?.Orcamento == null || _isUpdating) return;

            try
            {
                ProjetoAtual.Orcamento.CalcularTotal(
                    ProjetoAtual.Tarefas.ToList(),
                    CustosExtras.ToList()
                );

                // Força a UI a revalidar e renderizar as propriedades financeiras atualizadas
                OnPropertyChanged(nameof(ProjetoAtual));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro no cálculo: " + ex.Message);
            }
        }

        /// <summary>
        /// Grava o estado atual do projeto de forma atômica no banco de dados sem exibir mensagens de sucesso intermediárias.
        /// </summary>
        /// <returns><c>true</c> se a operação no banco for concluída com sucesso; caso contrário, <c>false</c>.</returns>
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

        /// <summary>
        /// Invoca a caixa de diálogo do sistema operacional para gravação física do relatório de proposta comercial em PDF.
        /// </summary>
        /// <returns><c>true</c> se o arquivo for compilado e gravado; caso contrário, <c>false</c>.</returns>
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

        #endregion
    }
}