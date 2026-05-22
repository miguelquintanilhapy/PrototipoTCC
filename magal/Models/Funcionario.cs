using magal.Models;
using System;
using System.Collections.Generic;
using System.Text;
namespace magal.Models
{
    public class Funcionario : BaseModel
    {
        public int id_funcionario { get; set; }

        public int id_cargo { get; set; }

        public string nome { get; set; }

        public string nivel { get; set; }

        public string tipo_vinculo { get; set; }

        public string status { get; set; }

        public Cargo Cargo { get; set; }

        public override string ToString()
        {
            return nome;
        }

        public override bool Equals(object obj)
        {
            if (obj is Funcionario outro)
            {
                return this.id_funcionario == outro.id_funcionario;
            }

            return false;
        }

        public override int GetHashCode() => id_funcionario.GetHashCode();

        public string Iniciais
        {
            get
            {
                if (string.IsNullOrWhiteSpace(nome))
                    return "??";

                var partes = nome.Trim().Split(' ');

                if (partes.Length == 1)
                    return partes[0]
                        .Substring(0, Math.Min(2, partes[0].Length))
                        .ToUpper();

                return (
                    partes[0][0].ToString() +
                    partes[partes.Length - 1][0].ToString()
                ).ToUpper();
            }
        }
        public decimal custo_hora
        {
            get
            {
                if (Cargo == null)
                    return 0;

                switch (nivel)
                {
                    case "Júnior":
                        return Cargo.custo_medio_hora / 1.75m;

                    case "Pleno":
                        return Cargo.custo_medio_hora;

                    case "Sênior":
                        return Cargo.custo_medio_hora * 1.5m;

                    case "Especialista":
                        return Cargo.custo_medio_hora * 2m;

                    default:
                        return 0;
                }
            }
        }
    }
}