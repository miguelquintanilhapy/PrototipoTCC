using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using magal.Models;
using magal.Data.Repositories;
using magal.Services;
using Microsoft.Win32;
using System.Collections.Generic;

namespace magal.ViewModels
{
    /// <summary>
    /// ViewModel responsável por gerenciar a tela de catálogo de custos,
    /// controlando filtros de busca, listagem e ações de lançamento, edição e exclusão.
    /// </summary>
    public class CustoViewModel : BaseModel
    {
        #region Atributos e Campos Privados

        // CORRIGIDO: Agora aponta para o repositório correto do catálogo global
        private readonly CatalogoCustoRepository _repository;
        private readonly PdfService _pdfService;
        private CatalogoCusto _custoSelecionado;
        private string _filtroTexto;

        #endregion

        #region Propriedades e Filtros

        /// <summary>
        /// Obtém ou define o texto de busca utilizado para filtrar os custos por nome ou categoria em tempo real.
        /// </summary>
        public string FiltroTexto
        {
            get => _filtroTexto;
            set
            {
                _filtroTexto = value;
                OnPropertyChanged();
                CustosView?.Refresh();
            }
        }

        /// <summary>
        /// Obtém ou define o custo do catálogo atualmente selecionado na listagem (DataGrid).
        /// </summary>
        public CatalogoCusto CustoSelecionado
        {
            get => _custoSelecionado;
            set { _custoSelecionado = value; OnPropertyChanged(); }
        }

        #endregion

        #region Coleções e Visões de Dados

        /// <summary>
        /// Lista observável de itens do catálogo de custos carregados do banco de dados.
        /// </summary>
        public ObservableCollection<CatalogoCusto> Custos { get; } = new ObservableCollection<CatalogoCusto>();

        /// <summary>
        /// Visão customizada da coleção que permite a aplicação de filtros em tempo real sem perder a lista original.
        /// </summary>
        public ICollectionView CustosView { get; private set; }

        #endregion

        #region Comandos disparados pela View

        /// <summary>
        /// Comando para deletar um custo do banco de dados e da listagem.
        /// </summary>
        public RelayCommand ExcluirCommand { get; }

        /// <summary>
        /// Comando para recarregar a lista de custos a partir do banco de dados.
        /// </summary>
        public RelayCommand AtualizarCommand { get; }

        /// <summary>
        /// Comando para abrir a caixa de diálogo de lançamento de um novo custo no catálogo.
        /// </summary>
        public RelayCommand CriarCommand { get; }

        /// <summary>
        /// Comando para abrir a caixa de diálogo de edição do custo selecionado.
        /// </summary>
        public RelayCommand EditarCommand { get; }

        /// <summary>
        /// Comando para exportar a listagem atual de custos em formato PDF.
        /// </summary>
        public RelayCommand ExportarPdfCommand { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="CustoViewModel"/>, configurando os repositórios, comandos e filtros.
        /// </summary>
        public CustoViewModel()
        {
            // CORRIGIDO: Inicializa o repositório do catálogo para bater com os métodos chamados
            _repository = new CatalogoCustoRepository();
            _pdfService = new PdfService();

            // Configuração do mecanismo de filtragem do WPF
            CustosView = CollectionViewSource.GetDefaultView(Custos);
            CustosView.Filter = FiltroDeCustos;

            // Inicialização dos Comandos mapeados para os botões da tela
            ExcluirCommand = new RelayCommand(p => ExecutarExclusao(p as CatalogoCusto));
            AtualizarCommand = new RelayCommand(_ => CarregarCustos());
            CriarCommand = new RelayCommand(_ => ExecutarCriar());
            EditarCommand = new RelayCommand(p => ExecutarEdicao(p as CatalogoCusto));
            ExportarPdfCommand = new RelayCommand(_ => ExecutarExportacaoPdf());

            CarregarCustos();
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Busca a lista atualizada de itens do catálogo do banco de dados e limpa os filtros da tela.
        /// </summary>
        public void CarregarCustos()
        {
            try
            {
                FiltroTexto = string.Empty;

                // CORRETO: Agora o _repository possui a definição de ListarTodos()
                var lista = _repository.ListarTodos();

                Custos.Clear();
                foreach (var c in lista)
                {
                    Custos.Add(c);
                }

                CustosView?.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar catálogo de custos: {ex.Message}", "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Métodos Auxiliares / Privados

        /// <summary>
        /// Avalia se um custo deve ser exibido no DataGrid com base no texto inserido no campo de busca (Nome ou Categoria).
        /// </summary>
        private bool FiltroDeCustos(object obj)
        {
            if (string.IsNullOrWhiteSpace(FiltroTexto)) return true;
            if (obj is not CatalogoCusto custo) return false;

            var busca = FiltroTexto.ToLower().Trim();

            return (custo.nome?.ToLower().Contains(busca) ?? false) ||
                   (custo.categoria?.ToLower().Contains(busca) ?? false);
        }

        /// <summary>
        /// Solicita a confirmação do usuário e executa a exclusão do custo no banco de dados e na memória.
        /// </summary>
        private void ExecutarExclusao(CatalogoCusto custo)
        {
            if (custo == null) return;

            var msg = $"Tem certeza que deseja excluir o custo '{custo.nome}' no valor de {custo.valor:C} do catálogo?";
            if (MessageBox.Show(msg, "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    // CORRETO: Agora o _repository possui a definição de Excluir() recebendo o ID do catálogo
                    _repository.Excluir(custo.id_catalogo_custo);
                    Custos.Remove(custo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir custo: {ex.Message}", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Abre a janela modal de lançamento de custos e recarrega a lista caso o registro seja concluído.
        /// </summary>
        private void ExecutarCriar()
        {
            var dialog = new magal.Views.CadastrarCustoDialog();
            dialog.Owner = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
            if (dialog.ShowDialog() == true)
                CarregarCustos();
        }

        /// <summary>
        /// Abre a janela modal de edição preenchida com os dados do custo selecionado e atualiza a lista se houver salvamento.
        /// </summary>
        private void ExecutarEdicao(CatalogoCusto custo)
        {
            if (custo == null) return;
            var dialog = new magal.Views.EditarCustoDialog(custo);
            dialog.Owner = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
            if (dialog.ShowDialog() == true)
                CarregarCustos();
        }

        /// <summary>
        /// Abre a caixa de diálogo para salvar o arquivo e dispara a geração do PDF com os custos filtrados na tela.
        /// </summary>
        private void ExecutarExportacaoPdf()
        {
            var custosFiltrados = CustosView.Cast<CatalogoCusto>().ToList();

            if (!custosFiltrados.Any())
            {
                MessageBox.Show("Não há registros na tabela para exportar.", "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string pastaDownloads = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Arquivos PDF (*.pdf)|*.pdf",
                FileName = $"Relatorio_Catalogo_Custos_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
                InitialDirectory = System.IO.Directory.Exists(pastaDownloads) ? pastaDownloads : string.Empty,
                Title = "Salvar Relatório do Catálogo de Custos"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _pdfService.GerarRelatorioTabelaCustos(custosFiltrados, saveFileDialog.FileName);

                    MessageBox.Show("Relatório do catálogo de custos gerado com sucesso!", "Sucesso",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao exportar PDF: {ex.Message}", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion
    }
}