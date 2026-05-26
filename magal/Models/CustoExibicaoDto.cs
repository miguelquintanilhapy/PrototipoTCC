namespace magal.Models
{
    public class CustoExibicaoDto
    {
        public int id_custo { get; set; }
        public int id_projeto { get; set; }
        public int id_catalogo_custo { get; set; }
        public string nome { get; set; }
        public string categoria { get; set; }
        public decimal valor { get; set; }
        public string tipo { get; set; }
        public string unidade { get; set; }
    }
}