using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using magal.Models;
using magal.Data.Repositories;

namespace magal.ViewModels
{
    public class FuncionarioViewModel : BaseModel
    {
        private readonly FuncionarioRepository _repository;
        private Funcionario _funcionarioSelecionado;
        private string _filtroTexto;

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

        public Funcionario FuncionarioSelecionado
        {
            get => _funcionarioSelecionado;
            set { _funcionarioSelecionado = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Funcionario> Funcionarios { get; } = new ObservableCollection<Funcionario>();
        public ICollectionView FuncionariosView { get; private set; }

        public RelayCommand ExcluirCommand { get; }
        public RelayCommand AtualizarCommand { get; }

        public RelayCommand CriarCommand { get; }



        public FuncionarioViewModel()
        {
            _repository = new FuncionarioRepository();

            FuncionariosView = CollectionViewSource.GetDefaultView(Funcionarios);
            FuncionariosView.Filter = FiltroDeFuncionarios;

            ExcluirCommand = new RelayCommand(p => ExecutarExclusao(p as Funcionario));
            AtualizarCommand = new RelayCommand(_ => CarregarFuncionarios());
            CriarCommand = new RelayCommand(_ => ExecutarCriar());

            CarregarFuncionarios();
        }

        private bool FiltroDeFuncionarios(object obj)
        {
            if (string.IsNullOrWhiteSpace(FiltroTexto)) return true;
            var func = obj as Funcionario;
            if (func == null) return false;

            var busca = FiltroTexto.ToLower().Trim();

            return (func.nome?.ToLower().Contains(busca) ?? false) ||
                   (func.tipo_vinculo?.ToLower().Contains(busca) ?? false) ||
                   (func.status?.ToLower().Contains(busca) ?? false);
        }

        private void CarregarFuncionarios()
        {
            try
            {
                var lista = _repository.ListarTodos();
                Funcionarios.Clear();
                foreach (var f in lista)
                    Funcionarios.Add(f);

                FuncionariosView?.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar funcionários: {ex.Message}", "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void ExecutarCriar()
        {
            var dialog = new magal.Views.CadastrarFuncionarioDialog();
            dialog.Owner = Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
                CarregarFuncionarios();
        }
    }
}