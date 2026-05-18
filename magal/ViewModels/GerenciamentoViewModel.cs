using System;
using System.Windows;
using System.Linq;
using magal.Models;
using magal.Data.Repositories;

namespace magal.ViewModels
{
    /// <summary>
    /// ViewModel responsável por gerenciar a tela de visão geral e indicadores do sistema,
    /// consolidando dados volumétricos de funcionários, clientes, cargos e custos.
    /// </summary>
    public class GerenciamentoViewModel : BaseModel
    {
        #region Atributos e Campos Privados

        private readonly FuncionarioRepository _funcionarioRepo;
        private readonly ClienteRepository _clienteRepo;
        private readonly CargoRepository _cargoRepo;
        private readonly CustoRepository _custoRepo;

        private string _totalFuncionarios;
        private string _totalClientes;
        private string _totalCargos;
        private string _totalCustos;

        #endregion

        #region Propriedades de Indicadores (KPIs)

        /// <summary>
        /// Obtém ou define o total de funcionários cadastrados formatado com dois dígitos.
        /// </summary>
        public string TotalFuncionarios
        {
            get => _totalFuncionarios;
            set { _totalFuncionarios = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Obtém ou define o total de clientes cadastrados formatado com dois dígitos.
        /// </summary>
        public string TotalClientes
        {
            get => _totalClientes;
            set { _totalClientes = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Obtém ou define o total de cargos cadastrados formatado com dois dígitos.
        /// </summary>
        public string TotalCargos
        {
            get => _totalCargos;
            set { _totalCargos = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Obtém ou define o total de custos cadastrados formatado com dois dígitos.
        /// </summary>
        public string TotalCustos
        {
            get => _totalCustos;
            set { _totalCustos = value; OnPropertyChanged(); }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="GerenciamentoViewModel"/>, configurando os repositórios e carregando os indicadores iniciais.
        /// </summary>
        public GerenciamentoViewModel()
        {
            // Inicialização dos repositórios
            _funcionarioRepo = new FuncionarioRepository();
            _clienteRepo = new ClienteRepository();
            _cargoRepo = new CargoRepository();
            _custoRepo = new CustoRepository();

            CarregarIndicadores();
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Consulta os repositórios para obter a contagem atualizada de cada entidade e atualiza as propriedades visíveis na tela.
        /// </summary>
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

        #endregion
    }
}