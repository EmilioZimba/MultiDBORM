
using MySql.Data.MySqlClient;
using System.Collections.Generic;
namespace Mapeamento
{
    /// <summary>
    /// Faz o vinculo entre o parametro e a instrucao SQL
    /// </summary>
    public class ParametroSQL
    {
        public List<MySqlParameter> parametros { get; set; }
        public string SQL { get; set; }
        /// <summary>
        /// Representa o tipo de instrucao
        /// true se existirem parametros
        /// false se nao existirem
        /// por defeito e true
        /// Uma instrucao pode ser simples ou seja sem nenhum parametro (por exemplo: select, definicao de variaveis...)
        /// </summary>
        public bool Tipo { get; set; }

        public ParametroSQL(bool tipo = true)
        {
            this.Tipo = tipo;
        }
    }
}