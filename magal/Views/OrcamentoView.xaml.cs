using magal.Models;
using magal.ViewModels;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace magal.Views
{
    public partial class OrcamentoView : UserControl
    {
        public OrcamentoViewModel ViewModel => this.DataContext as OrcamentoViewModel;

        #region Construtores

        // Construtor para Novo Orçamento
        public OrcamentoView()
        {
            InitializeComponent();

            var vm = new OrcamentoViewModel();
            this.DataContext = vm;

            // Registra o evento para escutar o Loading
            vm.PropertyChanged += ViewModel_PropertyChanged;
        }

        // Construtor para Edição (Vindo do Histórico)
        // Construtor para Edição (Vindo do Histórico)
        public OrcamentoView(Projeto projetoParaEditar)
        {
            InitializeComponent();

            // 1. Já deixa a propriedade visual em Visible por segurança
            if (LoadingOverlay != null)
            {
                LoadingOverlay.Visibility = Visibility.Visible;
            }

            var vm = new OrcamentoViewModel();
            this.DataContext = vm;

            // 2. Registra o evento para escutar as mudanças do IsLoading
            vm.PropertyChanged += ViewModel_PropertyChanged;

            // 3. Força a ViewModel a saber que a tela começou carregando
            vm.IsLoading = true;

            // 4. Espera a tela estar TOTALMENTE carregada e visível na MainWindow antes de processar
            this.Loaded += (s, e) =>
            {
                // Executa em segundo plano para não congelar a interface enquanto monta as listas
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new System.Action(() =>
                {
                    vm.CarregarProjetoParaEdicao(projetoParaEditar);
                }));
            };
        }

        #endregion

        #region Controle do Carregamento (Idêntico ao Histórico)

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OrcamentoViewModel.IsLoading) && sender is OrcamentoViewModel vm)
            {
                Dispatcher.Invoke(() =>
                {
                    if (vm.IsLoading)
                    {
                        if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Collapsed;
                    }
                });
            }
        }

        #endregion

        #region Validações

        private void ValidarEntradaSemNegativo(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "-")
            {
                MessageBox.Show("Não é permitido valores negativos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Handled = true;
                return;
            }

            Regex regex = new Regex("[^0-9,]+");
            bool temCaractereInvalido = regex.IsMatch(e.Text);

            if (temCaractereInvalido)
            {
                MessageBox.Show("Este campo aceita apenas números positivos.", "Entrada Inválida", MessageBoxButton.OK, MessageBoxImage.Information);
                e.Handled = true;
            }
        }

        #endregion
    }
}