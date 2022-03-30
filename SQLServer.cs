using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VtasInternetEmail
{
    internal class SQLServer
    {
        public decimal getSalesStore(String server, int idStore, String date)
        {
            decimal sales = 0;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = server;
            builder.UserID = "sa";
            builder.Password = "Sa@p0$d3$";
            builder.InitialCatalog = "SalesAudit";
            builder.ConnectTimeout = 30;

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                String sql = "select id_cajero_vfp, sum(total_vfp) from trn_venta_forma_pago_vfp " +
                    "where date_operacion_vfp = '" + date + "' and id_cajero_vfp in ('777','888') group by id_cajero_vfp";
                using (SqlCommand command =  new SqlCommand(sql,connection))
                {
                    try
                    {
                        connection.Open();
                        Console.WriteLine("Connected to server " + (idStore+1000) );
                        Log.writeLog("Connected to server " + (idStore + 1000));
                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {                                                                          
                                sales += reader.GetDecimal(1);                                
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        sales = 0.0001M;                        
                        Console.WriteLine("Error connection to server {0}: {1}",idStore, ex.Message);
                        Log.writeLog($"Error connection to server {idStore}: {ex.Message}");
                    }                                        
                }
            }
            return sales;
        }
        
        public List<InfoVtasInternet> getSalesInternet(String date)
        {
            List<InfoVtasInternet> sales = new List<InfoVtasInternet>();
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "10.128.10.59";
            builder.UserID = "SapPsi";
            builder.Password = "Pa$$w0rd";
            builder.InitialCatalog = "SalesAuditSears";

            using(SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                String sql = "SELECT tienda_dt AS Unidad, SUM(cantidad_di) AS Cantidad, SUM(importe_bruto_di) AS Importe " +
                    "FROM delivery_tran_dt, delivery_item_di WHERE tienda_di=tienda_dt AND terminal_di=terminal_dt " +
                    "AND transaccion_di=transaccion_dt AND fecha_di=CONVERT(VARCHAR,fecha_dt,112) AND CONVERT(VARCHAR,fecha_dt,112)='" +
                    date + "' GROUP BY tienda_dt";

                using(SqlCommand command = new SqlCommand(sql, connection))
                {
                    try
                    {                        
                        connection.Open();
                        Console.WriteLine("Connected to server 10.128.10.59");
                        Log.writeLog("Connected to server 10.128.10.59");
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                sales.Add(new InfoVtasInternet()
                                {
                                    unidad = reader.GetInt32(0),
                                    cantidad = reader.GetInt32(1),
                                    importe = Math.Round(reader.GetDecimal(2),2)
                                });
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Error connection to server 59: {1}", ex.Message);
                        Log.writeLog($"Error connection to server 59: {ex.Message}");
                        return null;
                    }
                }
            }            
            return sales;
        }

        public List<Sanborn> getSanborns()
        {
            List<Sanborn> list = new List<Sanborn>();

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "10.128.10.24";
            builder.UserID = "SapPsi";
            builder.Password = "kTIX3wxO8?";
            builder.InitialCatalog = "Db_Util";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                string sql = "SELECT Id_Store, Desc_Store, Ip_Sap " +
                    "FROM Ctg_Store WHERE Id_Company = 1 AND Id_Store NOT IN (359,360,881,3000)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();
                        Console.WriteLine("Connected to server 10.128.10.24");
                        Log.writeLog("Connected to server 10.128.10.24");
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new Sanborn()
                                {
                                    idSanborn = reader.GetInt32(0),
                                    nameSanborn = reader.GetString(1).Trim(),
                                    ipSanborn = reader.GetString(2).Trim()
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error connection to server 24: {1}", ex.Message);
                        Log.writeLog($"Error connection to server 24: {ex.Message}");
                        return null;
                    }
                    
                }
            }
            return list;
        }

    }    
}
