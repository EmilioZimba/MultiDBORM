using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Configuration;
using System.Diagnostics;
using System.Data.SqlClient;
using Mapeamento;
using System.Collections.Generic;

namespace SGE.DAL
{
    public class Conexao : IDisposable
    {
        public MySqlConnection con;
        public MySqlCommand cmd;
        private MySqlTransaction trans;
        //private MySqlDataAdapter adpt;
        private DataTable dt = new DataTable();

        //Construtor
        public Conexao()
        {

            this.con = new MySqlConnection(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString.ToString());
            this.cmd = new MySqlCommand();

            try
            {
                this.con.Open();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //Destrutor
        public void Dispose()
        {
            this.con.Close();
        }

        //ExecuteNonQuery
        public bool Executar(String strSql)
        {
            try
            {
                cmd.Connection = con;
                cmd.CommandText = strSql;
                Debug.WriteLine(strSql);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 1062:
                        Debug.WriteLine("Registo existente na Base de Dados!");
                        break;
                    default:
                        Debug.WriteLine(ex.Message.ToString());
                        break;
                }
                return false;
            }
        }
        
        public bool ExecutarTransacao(string[] sqlStr)
        {
            try
            {
                trans = con.BeginTransaction();
                cmd = con.CreateCommand();
                cmd.Transaction = trans;

                foreach (var item in sqlStr)
                {
                    Debug.WriteLine(item);
                    cmd.CommandText = item;
                    cmd.ExecuteNonQuery();
                }
                trans.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                try
                {
                    trans.Rollback();
                    return false;
                }
                catch (Exception ex2)
                {
                    Debug.WriteLine(ex2.Message);
                    return false;
                }
            }
        }

        public bool ExecutarTransacao(List<ParametroSQL> instrucoes)
        {

            
            try
            {
                trans = con.BeginTransaction();
                cmd = con.CreateCommand();
                cmd.Transaction = trans;

                foreach (var item in instrucoes)
                {
#if DEBUG
                    Debug.WriteLine(item);
#endif
                    cmd.CommandText = item.SQL;
                    if (item.Tipo)
                    {
                        cmd.Parameters.Add(item.parametros);
                    }

                    cmd.ExecuteNonQuery();
                }
                trans.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                try
                {
                    trans.Rollback();
                    return false;
                }
                catch (Exception ex2)
                {
                    Debug.WriteLine(ex2.Message);
                    return false;
                }
            }
        }
        
        public long GetLastInsertedID()
        {
            try
            {
                cmd.Connection = con;
                return cmd.LastInsertedId;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }

        }

        public bool IniciarTransacao()
        {
            try
            {
                trans = con.BeginTransaction();
                cmd = con.CreateCommand();
                cmd.Transaction = trans;
                Debug.WriteLine("INICIALIZANDO A TRANSACAO");
                return true;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("Ocorreu um erro na tentativa de iniciar a transacao\n{0}", ex.Message);
#endif
                return false;
            }
        }

        public bool ExecutarTransacao(ParametroSQL instrucoes)
        {
            try
            {
                cmd.CommandText = instrucoes.SQL;
                cmd.Parameters.Clear();
                if (instrucoes.Tipo)
                {
                    foreach (var item in instrucoes.parametros)
                    {
                        cmd.Parameters.AddWithValue(item.ParameterName, item.Value);
                        
                    }
                }

                cmd.Prepare();

                cmd.ExecuteNonQuery();
                
                return true;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("Ocorreu um erro na tentativa de executar a transacao\n{0}", ex.Message);
#endif
                FinalizarTransacao(true);

                return false;
            }

        }

        /// <summary>
        /// Finalizar a tranzacao se:
        ///  *for a ultima instrucao
        ///  *a instrucao ter falhado
        ///  NB: a conexao invoca automaticamente este metodo assim que ocorre um erro
        /// </summary>
        /// <param name="rollBack"> true se for pra saltar para o rollBack sem fazer o commit</param>
        /// <example>
        /// uso externo: boll resposta = cnx.finalizarTransacao();
        /// ou boll resposta = cnx.finalizarTransacao(false);
        /// uso interno: boll resposta = cnx.finalizarTransacao(true);
        /// </example>
        /// <returns>true or false</returns>
        /// 
        public bool FinalizarTransacao(bool rollBack = false)
        {

            if (rollBack)
            {
                try
                {
                    trans.Rollback();
                    // o retorno aqui significa que conseguiu fazer o rollBack 
                    return true;
                }
                catch (Exception ex2)
                {
#if DEBUG
                    Debug.WriteLine("Ocorreu um erro na tentativa de fazer o rollBack da transacao\n{0}", ex2.Message);
#endif
                    return false;
                }
            }
            
            try
            {
                trans.Commit();
                return true;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("Ocorreu um erro na tentativa de finalizar a transacao\n{0}", ex.Message);
#endif
                try
                {
                    trans.Rollback();
                    return false;
                }
                catch (Exception ex2)
                {
#if DEBUG
                    Debug.WriteLine("Ocorreu um erro na tentativa de fazer o rollBack da transacao\n{0}", ex2.Message);
#endif
                    return false;
                }

            }

        }

        //Execute Reader
        public MySqlDataReader Ler(String strSql)
        {
            try
            {
                cmd.Connection = con;
                cmd.CommandText = strSql;
                return cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        //Existem dois (2) metodos seleccionar cuja diferença está no tipo de retorno para entender isto recorre-se a conceitos de sobrecarga
        //METODO 1 seleccionar
        //public MySqlDataReader seleccionar(string campos, string tabelas, string condicao = "") {

        //    string strSql = "SELECT " + campos + " FROM " + tabelas + "";

        //    if (condicao != "") {
        //        strSql = strSql + " WHERE " + condicao;
        //    }

        //    return this.ler(strSql);
        //}

        //public DataTable preencherDataTable(string campos, string tabelas, string condicao = "") {

        //    string strSql = "SELECT " + campos + " FROM " + tabelas + "";

        //    if (condicao != "") {
        //        strSql = strSql + " WHERE " + condicao;
        //    }

        //    adpt = new MySqlDataAdapter(strSql, con);
        //    adpt.Fill(dt);

        //    return dt;
        //}

        //public bool inserir(string tabela, string campos, string valores) {
        //    return this.executar("INSERT INTO " + tabela + " (" + campos + ") VALUES (" + valores + ") ");
        //}

        //public bool actualizar(string tabela, string valores, string condicao) {
        //    return this.executar("UPDATE " + tabela + " SET " + valores + " WHERE " + condicao + "");
        //}

        //public bool remover(string tabela, string condicao) {
        //    return this.executar("DELETE FROM " + tabela + " WHERE " + condicao + "");
        //}


    }
}
