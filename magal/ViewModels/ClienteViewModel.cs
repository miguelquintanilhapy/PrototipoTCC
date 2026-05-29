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

namespace magal.ViewModels
{
    public class ClienteViewModel : BaseModel
    {
        #region Atributos e Campos Privados

        private readonly ClienteRepository _repository;
        private readonly PdfService _pdfService;
        private Cliente _clienteSelecionado;
        private string _filtroTexto;

        #endregion

        #region Propriedades e Filtros

        public string FiltroTexto
        {
            get => _filtroTexto;
            set
            {
                _filtroTexto = value;
                OnPropertyChanged();
                ClientesView?.Refresh();
            }
        }

        public Cliente ClienteSelecionado
        {
            get => _clienteSelecionado;
            set { _clienteSelecionado = value; OnPropertyChanged(); }
        }

        #endregion

        #region Coleções e Visões de Dados

        public ObservableCollection<Cliente> Clientes { get; } = new ObservableCollection<Cliente>();
        public ICollectionView ClientesView { get; private set; }

        #endregion

        #region Comandos

        public RelayCommand ExcluirCommand { get; }
        public RelayCommand AtualizarCommand { get; }
        public RelayCommand CriarCommand { get; }
        public RelayCommand EditarCommand { get; }
        public RelayCommand ExportarPdfCommand { get; }

        #endregion

        #region Construtor

        public ClienteViewModel()
        {
            _repository = new ClienteRepository();
            _pdfService = new PdfService();

            ClientesView = CollectionViewSource.GetDefaultView(Clientes);
            ClientesView.Filter = FiltroDeClientes;

            ExcluirCommand = new RelayCommand(p => ExecutarExclusao(p as Cliente));
            AtualizarCommand = new RelayCommand(_ => CarregarClientes());
            CriarCommand = new RelayCommand(_ => ExecutarCriar());
            EditarCommand = new RelayCommand(p => ExecutarEdicao(p as Cliente));
            ExportarPdfCommand = new RelayCommand(_ => ExecutarExportacaoPdf());

            CarregarClientes();
        }

        #endregion

        #region Métodos Públicos

        public void CarregarClientes()
        {
            try
            {
                FiltroTexto = string.Empty;

                var lista = _repository.ListarTodos();
                Clientes.Clear();
                foreach (var c in lista)
                {
                    Clientes.Add(c);
                }

                ClientesView?.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar clientes: {ex.Message}", "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Métodos Auxiliares

        private bool FiltroDeClientes(object obj)
        {
            if (string.IsNullOrWhiteSpace(FiltroTexto)) return true;
            if (obj is not Cliente cli) return false;

            var busca = FiltroTexto.ToLower().Trim();

            return (cli.nome?.ToLower().Contains(busca) ?? false) ||
                   (cli.cpf_cnpj?.ToLower().Contains(busca) ?? false) ||
                   (cli.cidade?.ToLower().Contains(busca) ?? false) ||
                   (cli.estado?.ToLower().Contains(busca) ?? false) ||
                   (cli.tipo?.ToLower().Contains(busca) ?? false);
        }

        private void ExecutarExclusao(Cliente cliente)
        {
            if (cliente == null) return;

            var msg = $"Tem certeza que deseja excluir o cliente '{cliente.nome}'?";
            if (MessageBox.Show(msg, "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _repository.Excluir(cliente.id_cliente);
                    Clientes.Remove(cliente);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExecutarCriar()
        {
            var dialog = new magal.Views.CadastrarClienteDialog();
            dialog.Owner = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
            if (dialog.ShowDialog() == true)
                CarregarClientes();
        }

        private void ExecutarEdicao(Cliente cliente)
        {
            if (cliente == null) return;
            var dialog = new magal.Views.EditarClienteDialog(cliente);
            dialog.Owner = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
            if (dialog.ShowDialog() == true)
                CarregarClientes();
        }

        private void ExecutarExportacaoPdf()
        {
            var clientesFiltrados = ClientesView.Cast<Cliente>().ToList();

            if (!clientesFiltrados.Any())
            {
                MessageBox.Show("Não há registros na tabela para exportar.", "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string pastaDownloads = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Arquivos PDF (*.pdf)|*.pdf",
                FileName = $"Relatorio_Clientes_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
                InitialDirectory = System.IO.Directory.Exists(pastaDownloads) ? pastaDownloads : string.Empty,
                Title = "Salvar Relatório de Clientes"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _pdfService.GerarRelatorioTabelaClientes(clientesFiltrados, saveFileDialog.FileName);

                    MessageBox.Show("Relatório de clientes gerado com sucesso!", "Sucesso",
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