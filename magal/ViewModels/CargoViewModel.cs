using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using magal.Models;
using magal.Data.Repositories;
using magal.Services; // Adicionado para enxergar o PdfService
using Microsoft.Win32; // Adicionado para usar o SaveFileDialog
using System.Collections.Generic;

namespace magal.ViewModels
{
    /// <summary>
    /// ViewModel responsável por gerenciar a tela de cargos,
    /// controlando filtros de busca, listagem e ações de criação, edição e exclusão.
    /// </summary>
    public class CargoViewModel : BaseModel
    {
        #region Atributos e Campos Privados

        private readonly CargoRepository _repository;
        private readonly PdfService _pdfService; // Adicionado o campo do serviço de PDF
        private Cargo _cargoSelecionado;
        private string _filtroTexto;

        #endregion

        #region Propriedades e Filtros

        /// <summary>
        /// Obtém ou define o texto de busca utilizado para filtrar os cargos na tela em tempo real.
        /// </summary>
        public string FiltroTexto
        {
            get => _filtroTexto;
            set
            {
                _filtroTexto = value;
                OnPropertyChanged();
                CargosView?.Refresh();
            }
        }

        /// <summary>
        /// Obtém ou define o cargo atualmente selecionado na listagem (DataGrid).
        /// </summary>
        public Cargo CargoSelecionado
        {
            get => _cargoSelecionado;
            set { _cargoSelecionado = value; OnPropertyChanged(); }
        }

        #endregion

        #region Coleções e Visões de Dados

        /// <summary>
        /// Lista observável de cargos carregados do banco de dados.
        /// </summary>
        public ObservableCollection<Cargo> Cargos { get; } = new ObservableCollection<Cargo>();

        /// <summary>
        /// Visão customizada da coleção de cargos que permite a aplicação de filtros em tempo real sem perder a lista original.
        /// </summary>
        public ICollectionView CargosView { get; private set; }

        #endregion

        #region Comandos disparados pela View

        /// <summary>
        /// Comando para deletar um cargo do banco de dados e da listagem.
        /// </summary>
        public RelayCommand ExcluirCommand { get; }

        /// <summary>
        /// Comando para recarregar a lista de cargos a partir do banco de dados.
        /// </summary>
        public RelayCommand AtualizarCommand { get; }

        /// <summary>
        /// Comando para abrir a caixa de diálogo de cadastro de um novo cargo.
        /// </summary>
        public RelayCommand CriarCommand { get; }

        /// <summary>
        /// Comando para abrir a caixa de diálogo de edição do cargo selecionado.
        /// </summary>
        public RelayCommand EditarCommand { get; }

        /// <summary>
        /// NOVO: Comando para exportar a listagem atual de cargos em formato PDF.
        /// </summary>
        public RelayCommand ExportarPdfCommand { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="CargoViewModel"/>, configurando os repositórios, comandos e filtros.
        /// </summary>
        public CargoViewModel()
        {
            _repository = new CargoRepository();
            _pdfService = new PdfService(); // Instanciando o serviço de geração de PDFs

            // Configuração do mecanismo de filtragem do WPF
            CargosView = CollectionViewSource.GetDefaultView(Cargos);
            CargosView.Filter = FiltroDeCargos;

            // Inicialização dos Comandos mapeados para os botões da tela
            ExcluirCommand = new RelayCommand(p => ExecutarExclusao(p as Cargo));
            AtualizarCommand = new RelayCommand(_ => CarregarCargos());
            CriarCommand = new RelayCommand(_ => ExecutarCriar());
            EditarCommand = new RelayCommand(p => ExecutarEdicao(p as Cargo));

            // NOVO: Inicializando o comando de exportação de PDF para cargos
            ExportarPdfCommand = new RelayCommand(_ => ExecutarExportacaoPdf());

            CarregarCargos();
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Busca a lista atualizada de cargos do banco de dados e limpa os filtros da tela.
        /// </summary>
        public void CarregarCargos()
        {
            try
            {
                FiltroTexto = string.Empty;
                var lista = _repository.ListarTodos();

                Cargos.Clear();
                foreach (var c in lista)
                {
                    Cargos.Add(c);
                }

                CargosView?.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar cargos: {ex.Message}", "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Métodos Auxiliares / Privados

        /// <summary>
        /// Avalia se um cargo deve ser exibido no DataGrid com base no texto inserido no campo de busca.
        /// </summary>
        /// <param name="obj">O objeto de cargo encapsulado pela view.</param>
        /// <returns><c>true</c> se o cargo corresponder aos critérios de busca; caso contrário, <c>false</c>.</returns>
        private bool FiltroDeCargos(object obj)
        {
            if (string.IsNullOrWhiteSpace(FiltroTexto)) return true;
            if (obj is not Cargo cargo) return false;

            var busca = FiltroTexto.ToLower().Trim();

            return (cargo.nome?.ToLower().Contains(busca) ?? false);
        }

        /// <summary>
        /// Solicita a confirmação do usuário e executa a exclusão do cargo no banco de dados e na memória.
        /// </summary>
        private void ExecutarExclusao(Cargo cargo)
        {
            if (cargo == null) return;

            var msg = $"Tem certeza que deseja excluir o cargo '{cargo.nome}'?";
            if (MessageBox.Show(msg, "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _repository.Excluir(cargo.id_cargo);
                    Cargos.Remove(cargo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Abre a janela modal de cadastro de cargos e recarrega a lista caso o registro seja concluído.
        /// </summary>
        private void ExecutarCriar()
        {
            var dialog = new magal.Views.CadastrarCargoDialog();
            dialog.Owner = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
            if (dialog.ShowDialog() == true)
                CarregarCargos();
        }

        /// <summary>
        /// Abre a janela modal de edição preenchida com os dados do cargo selecionado e atualiza a lista se houver salvamento.
        /// </summary>
        private void ExecutarEdicao(Cargo cargo)
        {
            if (cargo == null) return;
            var dialog = new magal.Views.EditarCargoDialog(cargo);
            dialog.Owner = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
            if (dialog.ShowDialog() == true)
                CarregarCargos();
        }

        /// <summary>
        /// NOVO: Abre a caixa de diálogo para salvar o arquivo e dispara a geração do PDF com os cargos filtrados na tela.
        /// </summary>
        private void ExecutarExportacaoPdf()
        {
            // Captura apenas os registros de cargos que estão atualmente passando pelo filtro do DataGrid
            var cargosFiltrados = CargosView.Cast<Cargo>().ToList();

            if (!cargosFiltrados.Any())
            {
                MessageBox.Show("Não há registros na tabela para exportar.", "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Pega o caminho real da pasta de Downloads do usuário logado no Windows
            string pastaDownloads = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Arquivos PDF (*.pdf)|*.pdf",
                FileName = $"Relatorio_Cargos_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
                InitialDirectory = System.IO.Directory.Exists(pastaDownloads) ? pastaDownloads : string.Empty, // Abre direto em Downloads
                Title = "Salvar Relatório de Cargos"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Chama o novo método criado no PdfService para gerar o relatório real de cargos
                    _pdfService.GerarRelatorioTabelaCargos(cargosFiltrados, saveFileDialog.FileName);

                    MessageBox.Show("Relatório de cargos gerado com sucesso!", "Sucesso",
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