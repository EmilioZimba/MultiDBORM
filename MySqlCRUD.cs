using Mapeamento;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGE.DAL
{
    public class MySqlCRUD
    {
       
        //TODO: Inserir com parametros ao invez de plicas('') para evitar o sql injection
        
        public string inserir(string tabela, string campos, string valores)
        {
            return "INSERT INTO " + tabela + " (" + campos + ") VALUES (" + valores + ") ";
        }

        /// <summary>
        /// Gera um MySqlCommand Stored Procedure de Insert
        /// </summary>
        /// <param name="tabela">Representa a tabela por inserir</param>
        /// <param name="vcv">Lista de VinculoCampoValor</param>
        /// <param name="lstparametros">representa as variaveis armazenadas no banco de dados</param>
        /// <returns>MySqlCommand com os parametros preenchidos</returns>
        public ParametroSQL inserir(string tabela, List<MySqlParameter> vcv, string lstparametros = "")
        {
            string sql = "";
            string parametros = "";
            

            foreach (var item in vcv)
            {
                sql = sql + ", " + item.ParameterName;
                parametros = parametros + ", " + "?"+item.ParameterName;
                //param.Add(new MySqlParameter() { ParameterName = item.ParameterName, Value = item.Value });
            }

            sql = sql.Substring(2, sql.Length - 2);
            parametros = parametros.Substring(2, parametros.Length - 2);

            if (lstparametros == "")
            {
                sql = "INSERT INTO " + tabela + "(" + sql + ")" + " VALUES (" + parametros + ")";
            }
            else
            {
                sql = "INSERT INTO " + tabela + "(" + sql + ")" + " VALUES (" + parametros + " " + lstparametros +")";
            }



#if DEBUG
            Debug.WriteLine(sql);
#endif

            return new ParametroSQL() { parametros = vcv, SQL = sql };
        }

        public string actualizar(string tabela, string valores, string condicao)
        {
            return "UPDATE " + tabela + " SET " + valores + " WHERE " + condicao;
        }

        public string remover(string tabela, string condicao)
        {
            return "DELETE FROM " + tabela + " WHERE " + condicao;
        }

        public string seleccionar(string campos, string tabelas, string condicao = "")
        {
            string strSql = "SELECT " + campos + " FROM " + tabelas;
            if (condicao != "")
                strSql = strSql + " WHERE " + condicao;
            return strSql;
        }

        public string seleccionarTodos(string tabela)
        {
            return "SELECT * FROM " + tabela;
        }

        
    }
}
