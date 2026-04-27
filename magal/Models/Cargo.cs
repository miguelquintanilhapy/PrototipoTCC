using magal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace magal.Models
{
    public class Cargo : BaseModel
    {
        public int Id { get; set; } // id_cargo
        public string Nome { get; set; }
        public string Nivel { get; set; } // Jr/Pl/Sr
        public decimal CustoMedioHora { get; set; } // custo_medio_hora
        public string Descricao { get; set; }
    }
}