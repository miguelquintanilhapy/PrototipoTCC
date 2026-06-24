using magal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace magal.Models
{
    public class Usuario : BaseModel
    {
        public int id_usuario { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string senha { get; set; }
        public string status { get; set; } //  "Ativo/Inativo"
        public string nivel { get; set; } // "Administrador" ou "Operador"
    }
}