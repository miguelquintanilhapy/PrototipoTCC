using magal.Models;
using System;
using System.Collections.Generic;
using System.Text;
namespace magal.Models
{
    public class Funcionario : BaseModel
    {
        public int Id { get; set; }
        public int CargoId { get; set; }
        public string Nome { get; set; }
        public decimal CustoHora { get; set; }
        public string TipoVinculo { get; set; }
        public string Status { get; set; }
        public Cargo Cargo { get; set; }

        public override string ToString()
        {
            return Nome;
        }


        public string Iniciais
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Nome)) return "??";
                var partes = Nome.Trim().Split(' ');
                if (partes.Length == 1) return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();
                return (partes[0][0].ToString() + partes[partes.Length - 1][0].ToString()).ToUpper();
            }
        }
    }
}
