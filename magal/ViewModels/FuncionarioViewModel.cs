using magal.Data.Repositories;
using magal.Models;
using magal.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace magal.ViewModels
{
    /// <summary>
    /// ViewModel responsável por gerenciar a tela de funcionários,
    /// controlando filtros de busca, listagem e ações de criação, edição e exclusão.
    /// </summary>
    public class FuncionarioViewModel : BaseModel
    {
        #region Atributos e Campos Privados

        private readonly FuncionarioRepository _repository;
        private readonly PdfService _pdfService;
        private Funcionario _funcionarioSelecionado;
        private string _filtroTexto;

        #endregion

        #region Propriedades e Filtros

        /// <summary>
        /// Obtém ou define o texto de busca utilizado para filtrar os funcionários na tela em tempo real.
        /// </summary>
        public string FiltroTexto
        {
            get => _filtroTexto;
            set
            {
                _filtroTexto = value;
                OnPropertyChanged();
                FuncionariosView?.Refresh();
            }
        }

        /// <summary>
        /// Obtém ou define o funcionário atualmente selecionado na listagem (DataGrid).
        /// </summary>
        public Funcionario FuncionarioSelecionado
        {
            get => _funcionarioSelecionado;
            set { _funcionarioSelecionado = value; OnPropertyChanged(); }
        }

        #endregion

        #region Coleções e Visões de Dados

        /// <summary>
        /// Lista observável de funcionários carregados do banco de dados.
        /// </summary>
        public ObservableCollection<Funcionario> Funcionarios { get; } = new ObservableCollection<Funcionario>();

        /// <summary>
        /// Visão customizada da coleção de funcionários que permite a aplicação de filtros em tempo real sem perder a lista original.
        /// </summary>
        public ICollectionView FuncionariosView { get; private set; }

        #endregion

        #region Comandos disparados pela View

        /// <summary>
        /// Comando para deletar um funcionário do banco de dados e da listagem.
        /// </summary>
        public RelayCommand ExcluirCommand { get; }

        /// <summary>
        /// Comando para recarregar a lista de funcionários a partir do banco de dados.
        /// </summary>
        public RelayCommand AtualizarCommand { get; }

        /// <summary>
        /// Comando para abrir a caixa de diálogo de cadastro de um novo funcionário.
        /// </summary>
        public RelayCommand CriarCommand { get; }

        /// <summary>
        /// Comando para abrir a caixa de diálogo de edição do funcionário selecionado.
        /// </summary>
        public RelayCommand EditarCommand { get; }

        /// <summary>
        /// Comando para exportar a listagem atual de funcionários em formato PDF.
        /// </summary>
        public RelayCommand ExportarPdfCommand { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="FuncionarioViewModel"/>, configurando os repositórios, comandos e filtros.
        /// </summary>
        public FuncionarioViewModel()
        {
            _repository = new FuncionarioRepository();
            _pdfService = new PdfService();

            // Configuração do mecanismo de filtragem do WPF
            FuncionariosView = CollectionViewSource.GetDefaultView(Funcionarios);
            FuncionariosView.Filter = FiltroDeFuncionarios;

            // Inicialização dos comandos mapeados para os botões da tela
            ExcluirCommand = new RelayCommand(p => ExecutarExclusao(p as Funcionario));
            AtualizarCommand = new RelayCommand(_ => CarregarFuncionarios());
            CriarCommand = new RelayCommand(_ => ExecutarCriar());
            EditarCommand = new RelayCommand(p => ExecutarEdicao(p as Funcionario));
            ExportarPdfCommand = new RelayCommand(_ => ExecutarExportacaoPdf());

            CarregarFuncionarios();
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Busca a lista atualizada de funcionários do banco de dados e limpa os filtros da tela.
        /// </summary>
        public void CarregarFuncionarios()
        {
            try
            {
                FiltroTexto = string.Empty;

                var lista = _repository.ListarTodos();
                Funcionarios.Clear();
                foreach (var f in lista)
                {
                    Funcionarios.Add(f);
                }

                FuncionariosView?.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar funcionários: {ex.Message}", "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Métodos Auxiliares / Privados

        /// <summary>
        /// Avalia se um funcionário deve ser exibido no DataGrid com base no texto inserido no campo de busca.
        /// </summary>
        private bool FiltroDeFuncionarios(object obj)
        {
            if (string.IsNullOrWhiteSpace(FiltroTexto)) return true;
            if (obj is not Funcionario func) return false;

            var busca = FiltroTexto.ToLower().Trim();

            return (func.nome?.ToLower().Contains(busca) ?? false) ||
                   (func.tipo_vinculo?.ToLower().Contains(busca) ?? false) ||
                   (func.status?.ToLower().Contains(busca) ?? false);
        }

        /// <summary>
        /// Solicita a confirmação do usuário e executa a exclusão do funcionário no banco de dados e na memória.
        /// </summary>
        private void ExecutarExclusao(Funcionario funcionario)
        {
            if (funcionario == null) return;

            var msg = $"Tem certeza que deseja excluir '{funcionario.nome}'?";
            if (MessageBox.Show(msg, "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _repository.Excluir(funcionario.id_funcionario);
                    Funcionarios.Remove(funcionario);

                    // Força a atualização da contagem no rodapé após remover do ObservableCollection
                    FuncionariosView?.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir funcionário: {ex.Message}", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Abre a janela de cadastro de funcionários e recarrega a lista caso o registro seja concluído.
        /// </summary>
        private void ExecutarCriar()
        {
            var dialog = new magal.Views.CadastrarFuncionarioDialog();
            dialog.Owner = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
            if (dialog.ShowDialog() == true)
                CarregarFuncionarios();
        }

        /// <summary>
        /// Abre a janela de edição preenchida com os dados do funcionário selecionado e atualiza a lista se houver salvamento.
        /// </summary>
        private void ExecutarEdicao(Funcionario funcionario)
        {
            if (funcionario == null) return;
            var dialog = new magal.Views.EditarFuncionarioDialog(funcionario);
            dialog.Owner = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
            if (dialog.ShowDialog() == true)
                CarregarFuncionarios();
        }

        /// <summary>
        /// Abre a caixa de diálogo para salvar o arquivo e dispara a geração do PDF com os dados filtrados na tela.
        /// </summary>
        private void ExecutarExportacaoPdf()
        {
            var funcionariosFiltrados = FuncionariosView.Cast<Funcionario>().ToList();

            if (!funcionariosFiltrados.Any())
            {
                MessageBox.Show("Não há registros na tabela para exportar.", "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string pastaDownloads = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Arquivos PDF (*.pdf)|*.pdf",
                FileName = $"Relatorio_Funcionarios_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
                InitialDirectory = System.IO.Directory.Exists(pastaDownloads) ? pastaDownloads : string.Empty,
                Title = "Salvar Relatório de Funcionários"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _pdfService.GerarRelatorioTabelaFuncionarios(funcionariosFiltrados, saveFileDialog.FileName);

                    MessageBox.Show("Relatório gerado com sucesso!", "Sucesso",
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