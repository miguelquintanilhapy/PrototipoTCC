using magal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace magal.Models
{
    public class Cliente : BaseModel
    {
        public int id_cliente { get; set; }
        public string nome { get; set; }
        public string tipo { get; set; }
        public string cpf_cnpj { get; set; }
        public string cidade { get; set; }
        public string estado { get; set; }
        public string contato { get; set; }
    }
}