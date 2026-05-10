using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using magal.Data.Repositories;


namespace magal.Views
{
    public partial class FuncionarioView : UserControl
    {
        public FuncionarioView()
        {
            InitializeComponent();
            this.DataContext = new magal.ViewModels.FuncionarioViewModel();
        }
    }
}