using magal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace magal.Models
{
    public class Cliente : BaseModel
    {
        public int Id { get; set; } 
        public string Nome { get; set; }
        public string Tipo { get; set; } 
        public string CpfCnpj { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Contato { get; set; }
    }
}
