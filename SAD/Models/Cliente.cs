namespace SAD.Models
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string CpfCnpj { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Contato { get; set; } = string.Empty;
    }
}