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
    public class CargoViewModel : BaseModel
    {
        private readonly CargoRepository _repository;
        private Cargo _cargoSelecionado;
        private string _filtroTexto;

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

        public Cargo CargoSelecionado
        {
            get => _cargoSelecionado;
            set { _cargoSelecionado = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Cargo> Cargos { get; } = new ObservableCollection<Cargo>();
        public ICollectionView CargosView { get; private set; }

        public RelayCommand ExcluirCommand { get; }
        public RelayCommand AtualizarCommand { get; }
        public RelayCommand CriarCommand { get; }
        public RelayCommand EditarCommand { get; }

        public CargoViewModel()
        {
            _repository = new CargoRepository();

            CargosView = CollectionViewSource.GetDefaultView(Cargos);
            CargosView.Filter = FiltroDeCargos;

            // Inicialização dos Comandos
            ExcluirCommand = new RelayCommand(p => ExecutarExclusao(p as Cargo));
            AtualizarCommand = new RelayCommand(_ => CarregarCargos());
            CriarCommand = new RelayCommand(_ => ExecutarCriar());
            EditarCommand = new RelayCommand(p => ExecutarEdicao(p as Cargo));

            CarregarCargos();
        }

        private bool FiltroDeCargos(object obj)
        {
            if (string.IsNullOrWhiteSpace(FiltroTexto)) return true;
            var cargo = obj as Cargo;
            if (cargo == null) return false;

            var busca = FiltroTexto.ToLower().Trim();

            return (cargo.nome?.ToLower().Contains(busca) ?? false) ||
                   (cargo.nivel?.ToLower().Contains(busca) ?? false);
        }

        private void CarregarCargos()
        {
            try
            {
                FiltroTexto = string.Empty;
                var lista = _repository.ListarTodos();

                Cargos.Clear();
                foreach (var c in lista)
                    Cargos.Add(c);

                CargosView?.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar cargos: {ex.Message}", "Aero Concepts",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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

        private void ExecutarCriar()
        {
            var dialog = new magal.Views.CadastrarCargoDialog();
            dialog.Owner = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
            if (dialog.ShowDialog() == true)
                CarregarCargos();
        }

        private void ExecutarEdicao(Cargo cargo)
        {
            if (cargo == null) return;
            var dialog = new magal.Views.EditarCargoDialog(cargo);
            dialog.Owner = Application.Current.Windows.OfType<magal.MainWindow>().FirstOrDefault();
            if (dialog.ShowDialog() == true)
                CarregarCargos();
        }
    }
}