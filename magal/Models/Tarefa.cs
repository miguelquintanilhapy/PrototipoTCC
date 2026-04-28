using magal.Models;

public class Tarefa : BaseModel
{
    public int Id { get; set; }
    public int ProjetoId { get; set; }

    private string _descricao;
    public string Descricao
    {
        get => _descricao;
        set { _descricao = value; OnPropertyChanged(); }
    }

    private int _funcionarioId;
    public int FuncionarioId
    {
        get => _funcionarioId;
        set { _funcionarioId = value; OnPropertyChanged(); }
    }

    private Funcionario _funcionario;
    public Funcionario Funcionario
    {
        get => _funcionario;
        set
        {
            _funcionario = value;
            if (_funcionario != null) _funcionarioId = _funcionario.Id;

            OnPropertyChanged();
            // Notifica o cálculo do subtotal
            OnPropertyChanged(nameof(CustoReal));
        }
    }

    private decimal _horasEstimadas;
    public decimal HorasEstimadas
    {
        get => _horasEstimadas;
        set
        {
            _horasEstimadas = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CustoReal));
        }
    }

    public decimal HorasReais { get; set; } 

    public decimal CustoReal
    {
        get
        {
            decimal valorHora = Funcionario?.Cargo?.CustoMedioHora ?? Funcionario?.CustoHora ?? 0;
            return HorasEstimadas * valorHora;
        }
    }

    private string _status = "Pendente";
    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }
}