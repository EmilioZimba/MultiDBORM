using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapeamento
{
    public enum TipoDado
    {
        DataHora,
        Data,
        Hora,
        NumeroTelefone,
        Moeda,
        Texto,
        Email,
        Decimal,
        Inteiro
    }

    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = false)]
    public class ColunaAttribute: Attribute
    {
        public string Nome { get; private set; }
        public double Tamanho { get; set; }
        public string Alias { get; set; }
        public bool PK { get; set; }
        public bool FK { get; set; }
        public TipoDado Tipo { get; set; }
        public bool Nulo { get; set; }
        public bool herda { get; set; }
        public bool carrega { get; set; }

        public ColunaAttribute(string nome)
        {
            this.Nome = nome;
            Alias = nome;
            this.Tipo = TipoDado.Texto;
            this.Nulo = true;
            this.herda = false;
            this.carrega = true;
            
            //condicao para auto-definicao da chave primaria
            if (nome == "cod" || nome == "id")
            {
                PK = true;
                this.Tipo = TipoDado.Inteiro;
            }
            else
            {
                PK = false;
            }

            //condicao para auto-definicao da chave estrangeira
            if (nome == "cod_" + nome || nome == "id_" + nome || nome == "cod" + nome || nome == "id" + nome)
            {
                FK = true;
                this.Tipo = TipoDado.Inteiro;
                this.Nulo = false;
                this.herda = true;
            }
            else
            {
                FK = false;
            }
        }
    }
}
