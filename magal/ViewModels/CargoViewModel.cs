using System;
using System.Collections.Generic;
using System.Text;
using magal.Data.Repositories;
using magal.Models;
using System.Collections.ObjectModel;

namespace magal.ViewModels
{
    public class CargoViewModel
    {
        public ObservableCollection<Cargo> Cargos { get; set; }

        public CargoViewModel()
        {
            CarregarCargos();
        }

        private void CarregarCargos()
        {
            var repo = new CargoRepository();

            Cargos = new ObservableCollection<Cargo>(repo.ListarTodos());
        }
    }
}
