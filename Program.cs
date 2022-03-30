using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VtasInternetEmail
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int countOK = 0;
            int countError = 0;
            String stringOK = "";
            String stringError = "";
            String bodyMail = "";
            var yesterday = DateTime.Today.AddDays(-1);
            String date = yesterday.ToString("yyyyMMdd");

                        
            List<InfoVtasInternet> vtsInternet = new SQLServer().getSalesInternet(date);
            List<Sanborn> ipsSanborn = new SQLServer().getSanborns();

            bodyMail += "<p>INICIA REVISION DE VENTAS INTERNET<br>";
            Log.writeLog("INICIA REVISION DE VENTAS INTERNET");
            bodyMail += " * FECHA DE OPERACION: <strong>" + date + "</strong> *</p>";
            Log.writeLog(" * FECHA DE OPERACION: " + date + " *");

            if (vtsInternet != null)
            {
                bodyMail += "<p>Revisando " + vtsInternet.Count + " unidades con ventas internet</p>";
                Log.writeLog("Revisando " + vtsInternet.Count + " unidades con ventas internet");
            }
            else
            {
                bodyMail += "<b style = 'Color:#8d3c3c;'>Sin conexion al servidor de Ventas Internet ip 10.128.10.59</b>";
            }

            if (ipsSanborn == null)
            {
                bodyMail += "<b style = 'Color:#8d3c3c;'>Sin conexion al servidor de IPs de tiendas 10.128.10.24</b>";
            }            

            if (vtsInternet != null && ipsSanborn != null)
            {
                foreach (InfoVtasInternet info in vtsInternet)
                {
                    foreach (Sanborn sanborn in ipsSanborn)
                    {
                        if (info.unidad == (sanborn.idSanborn + 1000))
                        {
                            decimal vta = new SQLServer().getSalesStore(sanborn.ipSanborn, sanborn.idSanborn, date);
                            Log.writeLog($"Unidad {info.unidad}: ServerInt ${info.importe} VS ServerSap ${vta}");
                            if (vta == 0.0001M)
                            {
                                stringError += "<b style='Color:#B90606;'>Sin conexion a unidad " + info.unidad + " ip " + sanborn.ipSanborn + "</b><br>";
                                countError++;
                                break;
                            }

                            if (vta == info.importe)
                            {
                                stringOK += "Unidad " + info.unidad + " ServerInternet $" + info.importe + " VS ServerTienda $" + vta + "<br>";
                                countOK++;
                            }
                            else
                            {
                                stringError += "<b style='Color:#8d3c3c;'>Revisar unidad  " + info.unidad + " ServerInternet $" + info.importe + " VS ServerTienda $" + vta + "</b><br>";
                                countError++;
                            }
                            break;
                        }
                    }
                }
                bodyMail += "<p>Unidades correctas: <strong>" + countOK + "</strong><br>";
                bodyMail += "Unidades con error: <strong>" + countError + "</strong></p>";

                if (countError > 0)
                    bodyMail += "<p><strong>Revisar las siguientes unidades</strong></p><p style='font-family:Consolas;font-size:14px;'>" + stringError + "</p>";
                bodyMail += "<br><p style='font-family:Consolas;font-size:14px;'>******************** <strong>UNIDADES CORRECTAS</strong> ********************<br>" + stringOK + "</p>";
            }
                        
            if(vtsInternet.Count > 0)
            {                
                new Email().sendEmail(bodyMail);
            }
            else
            {
                Log.writeLog("No se envia email ya que no hay unidades con ventas internet");
            }
        }
    }
}
