using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using magal.ViewModels;

namespace magal.Views
{
    /// <summary>
    /// Lógica de interação para CargoView.xaml
    /// </summary>
    public partial class CargoView : UserControl
    {
        #region Construtores

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="CargoView"/>.
        /// Configura os componentes visuais e vincula a ViewModel ao DataContext.
        /// </summary>
        public CargoView()
        {
            InitializeComponent();

            // Instancia e vincula a ViewModel ao contexto de dados da View
            this.DataContext = new CargoViewModel();
        }

        #endregion

        #region Eventos Disparados pela View

        /// <summary>
        /// Localiza a MainWindow ativa na aplicação e solicita o retorno 
        /// para a tela de menu/gerenciamento principal.
        /// </summary>
        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();

            if (mainWindow != null)
            {
                mainWindow.AbrirGerenciamento();
            }
        }

        #endregion
    }
}