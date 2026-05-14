using System;
using System.Windows;
using System.Linq;
using magal.Models;
using magal.Data.Repositories;

namespace magal.ViewModels
{
    public class GerenciamentoViewModel : BaseModel
    {
        // Repositórios
        private readonly FuncionarioRepository _funcionarioRepo;
        private readonly ClienteRepository _clienteRepo;
        private readonly CargoRepository _cargoRepo;
        private readonly CustoRepository _custoRepo;

        private string _totalFuncionarios;
        public string TotalFuncionarios
        {
            get => _totalFuncionarios;
            set { _totalFuncionarios = value; OnPropertyChanged(); }
        }

        private string _totalClientes;
        public string TotalClientes
        {
            get => _totalClientes;
            set { _totalClientes = value; OnPropertyChanged(); }
        }

        private string _totalCargos;
        public string TotalCargos
        {
            get => _totalCargos;
            set { _totalCargos = value; OnPropertyChanged(); }
        }

        private string _totalCustos;
        public string TotalCustos
        {
            get => _totalCustos;
            set { _totalCustos = value; OnPropertyChanged(); }
        }

        public GerenciamentoViewModel()
        {
            // Inicialização dos repositórios
            _funcionarioRepo = new FuncionarioRepository();
            _clienteRepo = new ClienteRepository();
            _cargoRepo = new CargoRepository();
            _custoRepo = new CustoRepository();

            CarregarIndicadores();
        }

        public void CarregarIndicadores()
        {
            try
            {

                var funcionarios = _funcionarioRepo.ListarTodos();
                TotalFuncionarios = (funcionarios?.Count ?? 0).ToString("D2");

                var clientes = _clienteRepo.ListarTodos();
                TotalClientes = (clientes?.Count ?? 0).ToString("D2");

                var cargos = _cargoRepo.ListarTodos();
                TotalCargos = (cargos?.Count ?? 0).ToString("D2");

                var custos = _custoRepo.ListarTodos();
                TotalCustos = (custos?.Count ?? 0).ToString("D2");
            }
            catch (Exception ex)
            {
                // Fallback amigável para a interface
                TotalFuncionarios = "00";
                TotalClientes = "00";
                TotalCargos = "00";
                TotalCustos = "00";

                // Log para depuração no console do Visual Studio
                System.Diagnostics.Debug.WriteLine($"[Erro GerenciamentoVM]: {ex.Message}");
            }
        }
    }
}