using magal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace magal.Models
{
    public class Usuario : BaseModel
    {
        public int Id { get; set; } // id_usuario
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Status { get; set; } // "Ativo/Inativo"
    }
}