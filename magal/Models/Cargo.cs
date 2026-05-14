using magal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace magal.Models
{
    public class Cargo : BaseModel
    {
        public int id_cargo { get; set; }
        public string nome { get; set; }
        public decimal custo_medio_hora { get; set; }
    }
}