using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mapeamento
{
    public class Gerar : IDisposable
    {

        public Gerar()
        {
#if DEBUG
            Debug.WriteLine("Inicializando a Classe Gerar");
#endif
        }

        //TODO: Marcar como obsoleto
        public string CamposComPK<T>(T item) where T : new()
        {
            //TODO: Fazer stored procedures

            string campos=null;

            foreach (var property in item.GetType().GetProperties())
            {
                var columnMapping = property.GetCustomAttributes(false)
                    .FirstOrDefault(a => a.GetType() == typeof(ColunaAttribute));

                if (columnMapping != null)
                {
                    var mapsto = columnMapping as ColunaAttribute;

                    campos = campos + ", " + mapsto.Nome;
                    Debug.WriteLine(mapsto.Nome);

                }
            }

            //Remove a primeira virgula e a ultima virgula
            campos = campos.Substring(2, campos.Length - 2);
#if DEBUG
            Debug.WriteLine("String campos");
            Debug.WriteLine(campos);
#endif
            return campos;
        }

        //TODO: Marcar como obsoleto
        public string CamposSemPK<T>(T item) where T : new()
        {
            //TODO: Fazer stored procedures

            string campos = null;

            foreach (var property in item.GetType().GetProperties())
            {
                var columnMapping = property.GetCustomAttributes(false)
                    .FirstOrDefault(a => a.GetType() == typeof(ColunaAttribute));

                if (columnMapping != null)
                {
                    var mapsto = columnMapping as ColunaAttribute;

                    if (mapsto.PK == true)
                    {

                    }
                    else
                    {
                        campos = campos + ", " + mapsto.Nome;
                    }
                }
            }

            //Remove a primeira virgula e a ultima virgula
            campos = campos.Substring(2, campos.Length - 2);
#if DEBUG
            Debug.WriteLine("String campos");
            Debug.WriteLine(campos);
#endif
            return campos;
        }

        //TODO: Marcar como obsoleto
        public string ValoresComPK<T>(T item) where T : new()
        {
            // TODO: Converter a data para um formato adequado (aqui foi aplicada uma solucao provisoria)
            // TODO: Tratar os dados para o formato adequado

            string valores = null;

            foreach (var i in item.GetType().GetProperties())
            {
                var columnMapping = i.GetCustomAttributes(false)
                    .FirstOrDefault(a => a.GetType() == typeof(ColunaAttribute));

                var mapsto = columnMapping as ColunaAttribute;

                if (mapsto.herda == false)
                {
                    valores = valores + ", " + item.GetType().GetProperty(i.Name).GetValue(item, null);
                }
            }


            //Remove a primeira virgula e a ultima virgula
            valores = valores.Substring(2, valores.Length - 2);
#if DEBUG
            Debug.WriteLine("String valores");
            Debug.WriteLine(valores);
#endif
            return valores;
        }

        //TODO: Marcar como obsoleto
        public string ValoresSemPK<T>(T item) where T : new()
        {
            // TODO: Converter a data para um formato adequado
            // TODO: Tratar os dados para o formato adequado

            string valores = null;
            foreach (var property in item.GetType().GetProperties())
            {

                var columnMapping = property.GetCustomAttributes(false)
                    .FirstOrDefault(a => a.GetType() == typeof(ColunaAttribute));

                if (columnMapping != null)
                {
                    var mapsto = columnMapping as ColunaAttribute;

                    if (!mapsto.PK)
                    {
                        if (!mapsto.herda)
                        {
                            if (mapsto.Tipo == TipoDado.Texto)
                            {
                                valores = valores + ", '" + item.GetType().GetProperty(property.Name).GetValue(item, null) + "'";
                            }
                            else if(mapsto.Tipo == TipoDado.Inteiro)
                            {
                                valores = valores + ", " + item.GetType().GetProperty(property.Name).GetValue(item, null);
                            }
                            else if(mapsto.Tipo == TipoDado.Data)
                            {
                                DateTime dt = Convert.ToDateTime(item.GetType().GetProperty(property.Name).GetValue(item, null));
                                valores = valores + ", '" + dt.ToString("yyyy/MM/dd").Substring(0,10) + "'";
                            }
                            else
                            {
                                valores = valores + ", " + item.GetType().GetProperty(property.Name).GetValue(item, null);
                            }
                        }
                    }
                }
            }


            //Remove a primeira virgula e a ultima virgula
            valores = valores.Substring(2, valores.Length - 2);
#if DEBUG
            Debug.WriteLine("String valores");
            Debug.WriteLine(valores);
#endif
            return valores;
        }

        /**
         * Para o uso exclusivo da instrucao update cuja clausula e a chave primaria nao combinada
         * */
        //TODO: Marcar como obsoleto
        public string CamposComValores<T>(T item) where T : new()
        {
            //TODO: Fazer stored procedures
 
            string camposComValores = null;
            string lastString = null;

            foreach (var property in item.GetType().GetProperties())
            {

                var columnMapping = property.GetCustomAttributes(false)
                    .FirstOrDefault(a => a.GetType() == typeof(ColunaAttribute));

                if (columnMapping != null)
                {
                    var mapsto = columnMapping as ColunaAttribute;

                    if (mapsto.PK == true)
                    {
                        lastString = " WHERE " + mapsto.Nome + "=" + item.GetType().GetProperty(property.Name).GetValue(item, null);
                    }
                    else
                    {
                        camposComValores = camposComValores + ", " + mapsto.Nome + "=" + item.GetType().GetProperty(property.Name).GetValue(item, null);
                    }
                }

            }

            //Remove a primeira virgula e a ultima virgula
            camposComValores = camposComValores.Substring(2, camposComValores.Length - 2);

            camposComValores = camposComValores + lastString;

            
#if DEBUG
            Debug.WriteLine("String campos com valores");
            Debug.WriteLine(camposComValores);
#endif
            return camposComValores;
        }

        public List<MySqlParameter> ValCampos<T>(T item) where T : new()
        {

            List<MySqlParameter> r = new List<MySqlParameter>();

            foreach (var property in item.GetType().GetProperties())
            {

                var columnMapping = property.GetCustomAttributes(false)
                    .FirstOrDefault(a => a.GetType() == typeof(ColunaAttribute));

                if (columnMapping != null)
                {
                    var mapsto = columnMapping as ColunaAttribute;

                    if (mapsto.PK == true)
                    {
                        r.Add(new MySqlParameter() { ParameterName = mapsto.Nome, Value = null });
                    }
                    else if(!mapsto.carrega)
                    {
#if DEBUG
                        Debug.WriteLine("Saltando o: {0} por pertencer outra entidade",mapsto.Nome);
#endif
                    }
                    else
                    {

                        try
                        {
                            r.Add(new MySqlParameter
                            {
                                ParameterName = mapsto.Nome,
                                Value = item.GetType().GetProperty(property.Name).GetValue(item, null).ToString()
                            });
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Debug.WriteLine("Gerrar Erro: {0}", ex.Message.ToString());
#endif
                        }
                    }
                }
            }
#if DEBUG
            Debug.WriteLine("String campos com valores");
            foreach (var it in r)
            {
                Debug.WriteLine("{0} = {1}", it.ParameterName, it.Value);
            }
#endif
            return r;
        }

        public string Tabela(Type item)
        {
            TabelaAttribute tbl = (TabelaAttribute)Attribute.GetCustomAttribute(item, typeof(TabelaAttribute));
            return tbl.Tabela;
        }
        public void Dispose()
        {
#if DEBUG
            Debug.WriteLine("Terminando a Classe Gerar");
#endif
        }
    }

}
