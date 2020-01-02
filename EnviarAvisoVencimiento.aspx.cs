using AppCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;

public partial class Forward_EnviarAvisoVencimiento : System.Web.UI.Page
{
    string idProducto;
    string operaciones;
    string reenvio;
    string folioReenvio;
    string folios;
    string fechaVencimiento;
    int index = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        idProducto = Request.Form["idProducto"].ToString();
        operaciones = Request.Form["Operaciones"].ToString();
        fechaVencimiento = Request.Form["fechaVencimiento"].ToString();
        enviarAvisoVencimiento();

        
        Response.Write("OK");
        Response.End();

    }

    public bool enviarAvisoVencimiento()
    {
        string[] operacionesList = operaciones.Split(',');
        foreach (string operacion in operacionesList)
        {                   
            string[] detalleOperacionList = operacion.Split('|');
            string rut = detalleOperacionList[0].ToString();
            string folio = detalleOperacionList[1].ToString();
            string nombre = detalleOperacionList[2].ToString();
            string fechaVenc= detalleOperacionList[3].ToString();
            string fechaIni = detalleOperacionList[4].ToString();

            #region EnvioPorUsuario 
            folios = folios + "," + folio;

            if (index < operacionesList.Count() - 1)
            {
                if (operacionesList[index].ToString().Substring(0, operacionesList[index].ToString().IndexOf("|")) != operacionesList[index + 1].ToString().Substring(0, operacionesList[index + 1].ToString().IndexOf("|")))
                {
                    folios = folios.Substring(1);
                    string[] foliosList = folios.Split(',');
            
                    string tablahtml = "<font color='0e016d'>" +
                        "<TABLE bgcolor='#484848' cellspacing='1' cellpadding='0' border='0' style='border-collapse: separate'>" +
                            "<TR bgcolor='#959595'>" +
                                "<TD>N° Contrato</TD>" +
                                "<TD>Tipo Contrato</TD>" +
                                "<TD>Modalidad</TD>" +
                                "<TD>Fecha Operación</TD>" +
                                "<TD>Fecha Vencimiento</TD>" +
                                "<TD>Posición Normal</TD>" +
                                "<TD>Precio Forward</TD>" +
                                "<TD>USD Observado</TD>" +
                                "<TD>Resultado</TD>" +
                            "</TR>";
                    
                    foreach (string folioaux in foliosList)
                    {
                        tablahtml += obtieneDetalleVencimiento(operacionesList, folioaux);
                    }
                    tablahtml += "</table></font>";

                    Confirmacion conf = new Confirmacion();
                    conf.EnviarCorreoAvisoVencimiento(idProducto, folios,tablahtml, fechaVenc, nombre,rut);
                    folios = "";
                }
                index++;
            }
            else
            {
                folios = folios.Substring(1);
                string[] foliosList = folios.Split(',');
                string tablahtml = "<font color='0e016d'>" +
                       "<TABLE bgcolor='#484848' cellspacing='1' cellpadding='0' border='0' style='border-collapse: separate'>" +
                           "<TR bgcolor='#959595'>" +
                               "<TD>N° Contrato</TD>" +
                               "<TD>Tipo Contrato</TD>" +
                               "<TD>Modalidad</TD>" +
                               "<TD>Fecha Operación</TD>" +
                               "<TD>Fecha Vencimiento</TD>" +
                               "<TD>Posición Normal</TD>" +
                               "<TD>Precio Forward</TD>" +
                               "<TD>USD Observado</TD>" +
                               "<TD>Resultado</TD>" +
                           "</TR>";
                foreach (string folioaux in foliosList)
                {
                    tablahtml += obtieneDetalleVencimiento(operacionesList, folioaux);
                }
                tablahtml += "</table></font>";

                Confirmacion conf = new Confirmacion();
                conf.EnviarCorreoAvisoVencimiento(idProducto, folios, tablahtml, fechaVenc, nombre,rut);
                folios = "";
            }
            #endregion
        }
        return true;
    }

  

   
   public string obtieneDetalleVencimiento(string[] operaciones, string folioCrear)
    {
        try
        {
            string tablahtml = "";

            foreach (string operacion in operaciones)
            {
                string[] detalleOperacionList = operacion.Split('|');

                if (folioCrear == detalleOperacionList[1].ToString())
                {
                    string rut = detalleOperacionList[0].ToString();
                    string folio = detalleOperacionList[1].ToString();
                    string nombre = detalleOperacionList[2].ToString();
                    string fechaVenc = detalleOperacionList[3].ToString();
                    string fechaIni = detalleOperacionList[4].ToString();
                    string modalidad = detalleOperacionList[5].ToString();
                    string tipoMov = detalleOperacionList[6].ToString();
                    string posicionNominal = detalleOperacionList[7].ToString();
                    string precioForward = detalleOperacionList[8].ToString();
                    string CodMonPrinc = detalleOperacionList[9].ToString();
                    //string test2 = detalleOperacionList[10].ToString();

                    DataTable tcobtenido = this.ObtenerValores(fechaVenc);

                    for (int i = 0; i < tcobtenido.Rows.Count; i++)
                    {
                        tcobtenido.Rows[i]["Dolar"].ToString();
                        tcobtenido.Rows[i]["Euro"].ToString();
                        tcobtenido.Rows[i]["Uf"].ToString();
                    }

                    double tipmoneda = 0;
                    string tipmonshow = "";
                    if (CodMonPrinc == "USD" || CodMonPrinc == "usd" || CodMonPrinc == "Usd")
                    {
                        //tipmonshow = reemplazarSeparadorMiles(tcobtenido.Rows[0]["Dolar"].ToString());
                        tipmonshow = tcobtenido.Rows[0]["Dolar"].ToString();
                        tipmoneda = Convert.ToDouble(tipmonshow);
                        if (tipmonshow == "" || tipmonshow == null)
                        {
                            tipmonshow = "0";
                        }
                    }
                    else if (CodMonPrinc == "EUR" || CodMonPrinc == "Eur" || CodMonPrinc == "eur")
                    {
                        //tipmonshow = reemplazarSeparadorMiles(tcobtenido.Rows[0]["Euro"].ToString());
                        tipmonshow = tcobtenido.Rows[0]["Euro"].ToString();
                        if (tipmonshow == "" || tipmonshow == null)
                        {
                            tipmonshow = "0";
                        }
                        tipmoneda = Convert.ToDouble(tipmonshow);
                    }
                    else if (CodMonPrinc == "UF" || CodMonPrinc == "Uf" || CodMonPrinc == "uf")
                    {
                        //tipmonshow = reemplazarSeparadorMiles(tcobtenido.Rows[0]["Uf"].ToString());
                        tipmonshow = tcobtenido.Rows[0]["Uf"].ToString();
                        if (tipmonshow == "" || tipmonshow == null)
                        {
                            tipmonshow = "0";
                        }
                        tipmoneda = Convert.ToDouble(tipmonshow);
                    }

                    double montPri = Convert.ToDouble(posicionNominal.Replace(".",","));
                    //double montTC = Convert.ToDouble(precioForward.Replace(".",","));
                    double montTC = Convert.ToDouble(precioForward);
                    double montSecun = montPri * montTC;
                    double resulta2 = 0;
                    if (tipoMov == "Compra" || tipoMov == "COMPRA" || tipoMov == "compra")
                    {
                        resulta2 = (montPri * tipmoneda) - montSecun;
                    }
                    else
                    {
                        resulta2 = montSecun - (montPri * tipmoneda);
                    }

                    double resultabla = Convert.ToDouble(resulta2);

                    string rrestabla = String.Format("{0:C}", resultabla);

                    if (tipmonshow == "0")
                    {
                        tipmonshow = "-";
                    }

                    tablahtml += "<TR bgcolor='#ffffff' align='center'>" +
                                       "<TD>" + folio + "</TD>" +
                                       "<TD>" + tipoMov + "</TD>" +
                                       "<TD>" + modalidad + "</TD>" +
                                       "<TD>" + fechaIni + "</TD>" +
                                       "<TD>" + fechaVenc + "</TD>" +
                                       "<TD>" + posicionNominal + "</TD>" +
                                       "<TD>" + reemplazarSeparadorMiles(precioForward) + "</TD>" +
                                       "<TD>" + reemplazarSeparadorMiles(tipmonshow) + "</TD>" +
                                       "<TD>"+ reemplazarSeparadorMiles(rrestabla) + "</TD>" +
                                       //"<TD>" + resultabla + "</TD>" +

                                   "</TR>";
                }
            }
            return tablahtml;
        }
        catch(Exception ex)
        {
            return ex.ToString();
        }
    }

    public DataTable ObtenerValores(String fecha)
    {
        DataTable dt = new DataTable();
        String conexion_string = ConfigurationManager.ConnectionStrings["CadenaConexion"].ToString();
        Conexion conect = new Conexion(conexion_string);

        SqlParameter[] parameters = new SqlParameter[1];
        parameters[0] = conect.agregaParametros("@fecha", fecha);
        dt = conect.EjecutarSP_Parametros("SP_CO_OBTENER_VALORES", parameters);

        return dt;

    }

    public string reemplazarSeparadorMiles(String numero)
    {
        numero = numero.Replace(".", "_");
        numero = numero.Replace(",", ".");
        numero = numero.Replace("_", ",");
        return numero;

    }

}