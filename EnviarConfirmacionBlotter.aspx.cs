using AppCode;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Forward_EnviarConfirmacionBlotter : System.Web.UI.Page
{
    string idProducto;
    string operaciones;
    string reenvio;
    string folioReenvio;

        protected void Page_Load(object sender, EventArgs e)
        {
            idProducto = Request.Form["idProducto"].ToString();
            operaciones = Request.Form["Operaciones"].ToString();
            reenvio = Request.Form["Reenvio"].ToString();

            if (reenvio == "SI")
            {
                eliminarOperacionParaReenviar();
            }

            enviarOperacionesConfirmacion();
          
            Response.Write("OK");
            Response.End();

        }

        public bool enviarOperacionesConfirmacion()
        {
            string[] operacionesList = operaciones.Split(',');
            foreach (string operacion in operacionesList)
            {
           
                string[] detalleOperacionList = operacion.Split('|');
                string fechaInicio = detalleOperacionList[0].ToString();
                string folioOperacion = detalleOperacionList[1].ToString();
                string fechaVencimiento = detalleOperacionList[2].ToString();
                string rut = detalleOperacionList[3].ToString();
                string secuencia = detalleOperacionList[4].ToString();
                string nombreCliente = detalleOperacionList[5].ToString();
                string tipoMovimiento = detalleOperacionList[6].ToString();
                string monedaPrincipal = detalleOperacionList[7].ToString();
                string montoPrincipal = detalleOperacionList[8].ToString();

                string monedaSecundario = detalleOperacionList[9].ToString();
                string tcCierreForward = detalleOperacionList[10].ToString();
                string montoSecundario = detalleOperacionList[11].ToString();
                string cumplimiento = detalleOperacionList[12].ToString();
                string agente = detalleOperacionList[13].ToString();


            Confirmacion conf = new Confirmacion();
            String resultadoCreacion = "";

            resultadoCreacion = conf.crearOperacionConfirmacionBlotter(idProducto,fechaInicio,folioOperacion,fechaVencimiento,rut
                                                      ,secuencia,nombreCliente,tipoMovimiento,monedaPrincipal,montoPrincipal, monedaSecundario
                                                      , tcCierreForward, montoSecundario, cumplimiento, agente, folioOperacion, folioOperacion, folioOperacion, folioOperacion
                                                      ,folioOperacion, folioOperacion, folioOperacion, folioOperacion, folioOperacion
                                                      );


                generadorPDF(idProducto, fechaInicio, folioOperacion, fechaVencimiento, rut
                                                      , secuencia, nombreCliente, tipoMovimiento, monedaPrincipal, montoPrincipal, monedaSecundario
                                                      , tcCierreForward, montoSecundario, cumplimiento, agente, folioOperacion, folioOperacion, folioOperacion, folioOperacion
                                                      , folioOperacion, folioOperacion, folioOperacion, folioOperacion, folioOperacion);

                string writer = Server.MapPath("~/Archivos/");



                string folioOp = folioOperacion + "|" + resultadoCreacion;
                conf.EnviarCorreoConfirmacion(idProducto, folioOp, writer, nombreCliente, rut);


                #region EnvioPorUsuario 
                //folios = folios + "," + folio;

                //if (index < operacionesList.Count() - 1)
                //{
                //    if (operacionesList[index].ToString().Substring(0, operacionesList[index].ToString().IndexOf("|")) != operacionesList[index + 1].ToString().Substring(0, operacionesList[index + 1].ToString().IndexOf("|")))
                //    {
                //        folios = folios.Substring(1);
                //        string[] foliosList = folios.Split(',');
                //        foreach (string folioaux in foliosList)
                //        {
                //            generadorPDFSinVariables(folioaux);
                //        }

                //        string writer = Server.MapPath("~/Archivos/"); 

                //        conf.EnviarCorreoConfirmacion(idProducto, folios, writer);
                //        folios = "";
                //    }
                //    index++;
                //}
                //else
                //{
                //    folios = folios.Substring(1);
                //    string[] foliosList = folios.Split(',');
                //    foreach (string folioaux in foliosList)
                //    {
                //        generadorPDFSinVariables(folioaux);
                //    }

                //    string writer = Server.MapPath("~/Archivos/"); 

                //    conf.EnviarCorreoConfirmacion(idProducto, folios, writer);
                //    folios = "";
                //}
                #endregion


            }
            return true;
        }

        public bool eliminarOperacionParaReenviar()
        {
            string[] operacionesList = operaciones.Split(',');

            foreach (string operacion in operacionesList)
            {
                string[] detalleOperacionList = operacion.Split('|');
                string rut = detalleOperacionList[0].ToString();
                string folio = detalleOperacionList[1].ToString();
                folioReenvio = folio;
                string origen = detalleOperacionList[2].ToString();
                Confirmacion conf = new Confirmacion();
                conf.EliminaOperacionPorReenvio(idProducto, folio);
            }
            return true;
        }

        public bool generadorPDF(String idProducto
                                                     , String fechaInicio
                                                     , String Folio
                                                     , String fechaVencimiento
                                                     , String rut
                                                     , String secuencia
                                                     , String nombreCliente
                                                     , String tipoMovimiento
                                                     , String monedaPrincipal
                                                     , String montoPrincipal
                                                     , String monedaSecundario
                                                     , String tcCierreForward
                                                     , String montoSecundario
                                                     , String cumplimiento
                                                     , String agente
                                                     , String montoLiquidacion
                                                     , String margen
                                                     , String cartera
                                                     , String vehiculo
                                                     , String folioAsociado
                                                     , String comentario
                                                     , String fixingDate
                                                     , String fechaAnticipo
                                                     , String tasaAnticipo)
        {
            try
            {
                Confirmacion conf = new Confirmacion();
                DataTable dt = new DataTable();
               
                LblNumeroOP.Text = Folio;
                lblFechaInicio.Text = fechaInicio;
                //OBTIENE VALORES REFERENCIALES
                DataTable dtvalores = new DataTable();
                dtvalores = conf.ObtenerValores(fechaInicio.Substring(0,10));
                string uf = "";
                string dolar = "";
                string euro = "";
                foreach (DataRow row2 in dtvalores.Rows)
                {
                    dolar = reemplazarSeparadorMiles(row2["dolar"].ToString());
                    euro = reemplazarSeparadorMiles(row2["euro"].ToString());
                    uf = reemplazarSeparadorMiles(row2["uf"].ToString());
                }
                lblUF.Text = uf.ToString();//row["FechaInicio"].ToString();
                lblPrecioReferencialMercado.Text = dolar.ToString();//row["FechaInicio"].ToString();


                string tipop = tipoMovimiento;

                if (tipop == "COMPRA" || tipop == "compra" || tipop == "Compra")
                {
                    lblComprador.Text = "Credicorp Capital S.A. Corredores de Bolsa"; //row["ejecutivo"].ToString();
                    lblVendedor.Text = nombreCliente;
                }
                else if (tipop == "VENTA" || tipop == "venta" || tipop == "Venta")
                {
                    lblVendedor.Text = "Credicorp Capital S.A. Corredores de Bolsa"; //row["ejecutivo"].ToString();
                    lblComprador.Text = nombreCliente;
                }
                else
                {
                    lblComprador.Text = "Nombre no reconocido"; //row["ejecutivo"].ToString();
                    lblVendedor.Text = "Nombre no reconocido";//row["RazonSocial"].ToString();
                }

                //Para diferenciar le puse OperadorI
                lblOperadorI.Text = "";
                //Este segundo operador sería OperadorII
                lblOperadorII.Text = agente;
                lblTipoOperacion.Text = tipoMovimiento;
                lblMonto.Text = reemplazarSeparadorMiles(montoPrincipal);
                lblTasa.Text = "N/A";
                lblPrecioFuturo.Text = reemplazarSeparadorMiles(tcCierreForward);
                lblMontoFinal.Text = reemplazarSeparadorMiles(montoSecundario);
                lblPlazo.Text = "-";
                lblFechaVencimiento.Text = fechaVencimiento;
                lblModalidadPago.Text = cumplimiento;
                lblFechaValuta.Text = fechaVencimiento;

                string tipmoneda = monedaPrincipal;
                if (tipmoneda == "USD")
                {
                    lblValorReferencialSalida.Text = "Dólar observado";
                }
                else if (tipmoneda == "UF")
                {
                    lblValorReferencialSalida.Text = "Unidad de fomento";
                }
                else
                {
                    lblValorReferencialSalida.Text = "Tipo de moneda no reconocida";
                }
                //queda como fijo como bloomberg  por orden de Hector Nuñez por reunion del 19-07-19
                lblFixing.Text = "Bloomberg";//fixingDate;//row["FechaInicio"].ToString();

                lblNombreEmpresaFirma.Text = nombreCliente;


                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                this.Page.RenderControl(hw);
                StringReader sr = new StringReader(sw.ToString());
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                string ruta = Server.MapPath("~/Archivos/" + Folio + ".pdf");
                FileStream streaming = new FileStream(ruta, FileMode.Create);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, streaming);

                iTextSharp.text.Image addLogo = default(iTextSharp.text.Image);
                addLogo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/img/logo-credicorp.png"));
                addLogo.SetAbsolutePosition(150f, 680f);

               
                pdfDoc.Open();
                pdfDoc.Add(addLogo);
              
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
              
            return true;
        }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return false;
            }
        }


    public string reemplazarSeparadorMiles(String numero)
    {
        numero = numero.Replace(".", "_");
        numero = numero.Replace(",", ".");
        numero = numero.Replace("_", ",");
        return numero;

    }

}

