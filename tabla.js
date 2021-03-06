//# sourceURL=tabla.js

var checkList = [];
var checkListCreados = [];
var sesion = getSessionId();
var botonMasivo;

var confTabla = ( function() {
	
	var perfil = getPerfilActual();
	var Rows = [];
	var cobrosDefinitivos = iniciarParametros("cobDef");
	var cobrosNoDefinitivos = iniciarParametros("cobNoDef");
	var cobrosVisitaCliente = iniciarParametros("cobAgendaVisita");
	var cobrosRetenciones = iniciarParametros("cobRetencion");
	
	function getRows(){
		var conexion = getSessionId();
		var res = getDatos(URL + '/CasoRESTService.svc/GetCasoListId/' + flujoACT + '/' + colaACT + '/' +conexion,'');
		Rows = res;
		return res;
	}
	
	function mostrarTablaCasos(){
		var res = getRows();
		
		updateCaseNumbers(flujoACT, colaACT);
		
		llenaTabla("tabla1",res);
		$(document).ready( function () {
			$('#tabla1').DataTable();
			if (stdForm) stdForm.unblockInput();
		});
	}
	
	function llenaTabla(tabla,lista){ 
		//recibe nombre de tabla y lista (json) de objetos
		var menus = getMenu(perfil);
		var tbody = "<tbody>";

		var diasA = Number($.session.get('DAmarillo') || 0);
		var diasR = Number($.session.get('DRojo'    ) || 0);

		var fechaIngreso;
		var auxFecha;
		var fecha;
		var table = document.getElementById(tabla);
		var controlListaDatos = 0;
		var checkBoxDisplay = " style='display:none'";


		for(i=0;i<lista.length;i++){
			
			var record = lista[i];
			record.DiasA = diasA;
			record.DiasR = diasR;
			var descripcion = "";
			var tbPago = getTBPagoByOnBase(lista[i].Id);
			var tbPagoPersona = getTBPagoPersonaByOnBase(lista[i].Id);
			var folio="";
			var fechacreacion="";

											//variables a usar//Javier Nuñez//09-27-2019
			var fechaAuxiliar = lista[i].Name;
			var ubicacion = fechaAuxiliar.indexOf("Solicitud:") + 11;
			fechaAuxiliar = fechaAuxiliar.substring(ubicacion,ubicacion + 9);	
															//Fin

			if(tbPago.length > 0){
				descripcion = "[Fecha Solicitud: " + formatoFechaToTabla(tbPago[0].idContrato) + "] ";
				folio =  tbPago[0].folio;
				fechacreacion = tbPago[0].fechaSolicitud;

			}else if(tbPagoPersona.length > 0){
				folio = tbPagoPersona[0].folio;
				descripcion = "[Rut: " + tbPagoPersona[0].idPersona + "] "
				+ "[Nombre: " + tbPagoPersona[0].nombrePersona + "] "
				+ "[Póliza: " + tbPagoPersona[0].idNegocio + "] "
				+ "[Pago: $" + formatoMiles(tbPagoPersona[0].montoTotal) + ".-] ";
			}
			
			fechaIngreso = (fecha ? interFechaCompleta(fecha) : "");

			contador = i + 1;
			
			tbody 	+='<tr data-AtrVencimiento='+lista[i].AtrVencimiento+'>'
			+'<td><input type="checkbox" id="cbox2'+ i +'" value="'+lista[i].Id+'" onchange=actualizarListaChecked('+ 'cbox2'+i +','+lista[i].Id+');></td>'					
			+ '<td align=center>' + folio + '</td>'
			+ '<td>' + stdForm.led(record) + '</td>'
			+  "<td><a alt='' class='trabajar' href='javascript:confTabla.trabajar(\""  + lista[i].Id + "*" + fechaAuxiliar + "*" + lista[i].EstadoCR + "\")'>"
									                                                                        //fechaIngreso
			+ descripcion + "</a></td>";

			checkListCreados.push("cbox2"+ i);									
                                          //Fecha creacion
			tbody +=  "<td align=center>" + fechaAuxiliar + "</td>"
			+  "<td><center>"
			+  "<a alt='Buscar' class='buscar'    href='javascript:confTabla.buscar(\""  + lista[i].Id + "\")'>"
			+  "<i class='fa fa-search fa-fw' title='Visualizar Caso'></i></a>"
			+  "<a alt='Buscar' class='historial' href='javascript:confTabla.historial(\""  + lista[i].Id + "\")'>"
			+  "<i class='fa fa-file-text fa-fw' title='Historial Caso'></i></a>"
										//Obtención de datos//Javier Nuñez//09-27-2019
			+  "<td align=right>" + fechaAuxiliar + "</td>"
										//Fin
			+  "</center></tr>";								


		}

		//******************************************************&*******************************************************
		//BLOQUE QUE GENRA BARRA SUPERIOR
		//BARRA CON SELECIONAR TODO Y BOTON DE ENVIO MASIVO
		var tableBarra = document.getElementById("tabla2");
		var boton = "";		
		var theadBarra = "<colgroup>"
		+"<col style='width: 2%'>"
		+"<col style='width: 50%'>"
		+"</colgroup><thead><tr><th>Todo&nbsp<input type='checkbox' id='cboxTodo' onchange=chequearTodosNinguno()></th>"
		var tbodyBarra = "<tbody></tbody>";

		// Realiza recie boleta/ factura comercial - Masivo
		debugger;
		var sesionOK = chequearSesion();				
		if(sesionOK == 1){
			var res = getDatos(URL + '/GetAdHocTask.svc/GetAdHocTaskList/'+ flujoACT + '/' + colaACT + '/' + sesion,'');
			var autorizaAdjMasivo = getAutorizaAdjMasivo(flujoACT,colaACT);
			var disableStr = '';
			if(autorizaAdjMasivo.ColaActual != 0)
			{
				boton += "<button class='btn btn-primary' id='idAdjDocumento' onclick="+"'javascript:showModalAdjuntar()'>"+autorizaAdjMasivo.NombreBoton+"</button>";
				disableStr = 'disabled';
			}
			for(var iterador=0; iterador < res.length; iterador++){

				if(getAutorizaBotonMasivo(""+res[iterador].IdTask) != 0){

					botonMasivo = res[iterador].IdTask;				   
					boton += "<button class=" + "'btn btn-primary'" + " id=" + "'" + res[iterador].IdTask + "' " + disableStr + " onClick="+"'javascript:btnEnviarConfirmacionMasiva(\"" + res[iterador].IdTask +"\")'>" + res[iterador].NameTask + "</button>";
					boton += "  ";			
				}	           
			}
		}

		theadBarra += "<th>" + boton + "</th></tr></thead>";

		//********************************************************&*****************************************************		

		tbody += "</tbody>";
		
		var thead =  '<colgroup>'
		+'<col style="width: 2%">'
		+'<col style="width: 2%">'
		+'<col style="width: 4%">'
		+'<col style="width:69%">'
		+'<col style="width:14%">'
		+'<col style="width:9%">'
		+'</colgroup>'
		+ "<thead><tr>";
		
		if( controlListaDatos == 0){
		  thead += "<th></th><th>Folio</th><th>Estado</th><th>Descripci&oacute;n</th><th>Fecha Creación</th><th>Ver caso</th>"; //2016-05-27 SM: se agregan nuevos nombres a las columnas	
		}
		else{
		  thead += "<th>Folio</th><th>Estado</th><th>Descripci&oacute;n</th><th>Tipificaci&oacute;n</th><th>UsuarioIngreso</th><th>Fecha Solicitud</th><th>Ver caso</th>"; //2016-05-27 SM: se agregan nuevos nombres a las columnas
		}		

		//nueva columna con Fecha de Ingreso// Javier Nuñez / 26-09-2019
		thead += "<th>Fecha Ingreso</th>"; 		
		
		thead += "</tr></thead>";
		
		table.innerHTML=thead+tbody;
		$('#tabla1').dataTable( {
			"iDisplayLength": 25
		});
		tableBarra.innerHTML=theadBarra+tbodyBarra;
		$('#tabla1').trigger('recordsloaded')
		return false;
	}
	
	function verificaRut() {
		
		$("#idRut").on('change', function(e) {

			var rut = $("#idRut").val();
			var punto=".";
			rut = rut.replace(punto,'');
			rut = rut.replace(punto,'');
			
			if (rut.toString().trim() != '' && rut.toString().indexOf('-') > 0) {
				var caracteres = new Array();
				var serie = new Array(2, 3, 4, 5, 6, 7);
				var dig = rut.toString().substr(rut.toString().length - 1, 1);
				rut = rut.toString().substr(0, rut.toString().length - 2);

				for (var i = 0; i < rut.length; i++) {
					caracteres[i] = parseInt(rut.charAt((rut.length - (i + 1))));
				}

				var sumatoria = 0;
				var k = 0;
				var resto = 0;

				for (var j = 0; j < caracteres.length; j++) {
					if (k == 6) {
						k = 0;
					}
					sumatoria += parseInt(caracteres[j]) * parseInt(serie[k]);
					k++;
				}

				resto = sumatoria % 11;
				dv = 11 - resto;

				if (dv == 10) {
					dv = "K";
				}
				else if (dv == 11) {
					dv = 0;
				}

				if (dv.toString().trim().toUpperCase() == dig.toString().trim().toUpperCase()){
		        	//alert("Válido");
		        	return 1;
		        }
		        else
		        {
		        	alert("Rut No Válido");
		        	var x = document.getElementById("idRut");
		        	x.value = '';
		        	
		        	return 0;
		        }
		    }
		    else {
		    	alert("Rut Mal ingresado");
		    	var x = document.getElementById("idRut");
		    	x.value = '';
		    	return 0;
		    }
		});
	}

	function verificarFolio(){
		$("#idFolio").on('change', function(e) {
			
			var numeros="0123456789";
			var texto = $("#idFolio").val();
			var control = 1;
			
			for(i=0; i<texto.length && control == 1 ; i++){

				if (numeros.indexOf(texto.charAt(i),0)!=-1){
					control = 1;

				}
				else
				{
					control = 0;
				}
			}

			if(control==0){
				alert("Folio no válido");
				var telefono = document.getElementById("idFolio");
				telefono.value = '';

			}

			
		});
	}

	function mostrarCaso(buscarCaso){
		
		var IDDOCUMENT = $.session.get('IDDOCUMENT');
		var res = getDatos( URLACS + '/ServiceAtencion.svc/getAtencionesByFlujo/' + flujoACT + '/' + IDDOCUMENT, '');
		
		if( res != "" && res != null && res.ATENCIONPAGINA != ""){
			$.session.set('flujo', flujoACT);
			callQueue('conectores/' + res.ATENCIONPAGINA,flujoACT,colaACT);
			return;
		}
		
		
		//2016-04-12_CBA: Consulta default
		$.session.set('flujo', flujoACT);
		queueOptions.modo='Consulta'; //Esta función siempre debe obtener el formulario sólo lectura;
		callConector(pathConector,flujoACT, colaACT, queueOptions);
		
	}

	function trabajarCaso(buscarCaso){
		if (!stdForm.isSessionValid()) return;
		if (typeof stdForm=='object') stdForm.blockInput({loader:'grid'});
		$.session.set('flujo', flujoACT);
		queueOptions.modo='Modificacion'; //Esta función siempre debe obtener el formulario sólo lectura;

		callConector(pathConector,flujoACT, colaACT, queueOptions);		  			
	}

	function mostrarHistorial(buscarCaso){
		$.session.set('flujo', flujoACT);
		$.session.set('cola', colaACT);
		$.session.set('IDDOCUMENT', buscarCaso);
		var win = window.open("historial.html", '_blank');
		win.focus();
	}

	function formattedDate(date) {
		var d = new Date(date || Date.now()),
		month = '' + (d.getMonth() + 1),
		day = '' + d.getDate(),
		year = d.getFullYear();

		if (month.length < 2) month = '0' + month;
		if (day.length < 2) day = '0' + day;

		return [month, day, year].join('/');
	}
	
	return{
		
		loadnormal : function() {

			mostrarTablaCasos();
			verificarFolio();
			verificaRut();

			window.scroll(0,0); //2016-05-02_CBA

		},
		trabajar: function(buscarCaso){

			buscarCaso = buscarCaso.split("*");

			$.session.set('IDDOCUMENT', buscarCaso[0]);
			$.session.set('FECHAINGRESOCOLA', buscarCaso[1]);
			$.session.set('EstadoCasoCola', buscarCaso[2]); //20-06-2016 SM: Se guarda en sesión el estado del Caso 
			trabajarCaso(buscarCaso);

		},
		buscar: function(buscarCaso){

			$.session.set('IDDOCUMENT', buscarCaso);
			mostrarCaso(buscarCaso);

		},
		historial: function(buscarCaso){

			$.session.set('IDDOCUMENT', buscarCaso);
			mostrarHistorial(buscarCaso);

		}
		,getRows : getRows
		,getLoadedRows: function(){
			return Rows;
		}

	};
	
})();


function actualizarListaChecked(idCheck,idOnbase)
{
	console.log(idCheck.checked == true)
	if(idCheck.checked == true)
	{
		checkList.push(idOnbase);
		console.log(checkList);
	}
	else
	{
		for(var i =0; i < checkList.length; i++)
		{
			if(checkList[i] == idOnbase)
			{
				checkList = removeA(checkList,idOnbase);
				console.log(checkList);
			}
		}
	}
}

function removeA(arr) {
	var what, a = arguments, L = a.length, ax;
	while (L > 1 && arr.length) {
		what = a[--L];
		while ((ax= arr.indexOf(what)) !== -1) {
			arr.splice(ax, 1);
		}
	}
	return arr;
}

function btnEnviarConfirmacionMasiva(idButon)
{
	var sesionOK = chequearSesion();
	if(checkList.length == 0){
		sendErrorMessage('Debe seleccionar almenos un caso para continuar.')
	}
	else{
		stdForm.blockInput();
		if(sesionOK == 1){
			var sesion = getSessionId();
			var user = getNombreUsuarioSesion();
			var urlQueue  = getUrlQueue(flujoACT)
			var res
			for(var i =0; i < checkList.length; i++)
			{
					res = getDatos(URL + '/ExecAdHocTask.svc/EjecutaAdHocTaskSub/' + flujoACT + '/' + colaACT + '/' + idButon + '/' + checkList[i] + '/' + user + '/' + $.session.get('pass_global'),''); // Servicio Nuevo que elimina la sesión como parámetro, e incluye como nuevos el nombre usuario y password
				}
				if(res.EjecutaAdHocTaskSubResult == "OK"){				
					callQueue(urlQueue,flujoACT,colaACT);
					stdForm.unblockInput();
				}
				else{
					sendErrorMessage("Error al ejecutar Tarea: " + res.EjecutaAdHocTaskSubResult);
					stdForm.unblockInput();
				}
			}
			else{
				sendErrorMessage("Sesion Finalizada");
				location.href = "login.html";
				stdForm.unblockInput();
			}
		}
	}

	function chequearTodosNinguno()
	{
		var checkAll = document.getElementById("cboxTodo");

		if(checkAll.checked == true)
		{
			for(var i = 0; i < checkListCreados.length; i++)
			{
				var vCheck = document.getElementById(checkListCreados[i]);			
				vCheck.checked = true;
				actualizarListaChecked(vCheck,vCheck.value);
			}
		}
		else
		{
			for(var i = 0; i < checkListCreados.length; i++)
			{
				var vCheck = document.getElementById(checkListCreados[i]);			
				vCheck.checked = false;
				actualizarListaChecked(vCheck,vCheck.value);
			}
		}
	}

	function showModalAdjuntar(){
		$('#adjAprobacionPago').modal('show');
	}

	function adjuntarDocsMasivos(){

		$("#idAdjuntar").on('click', function(e) {

			var autorizaAdjMasivo = getAutorizaAdjMasivo(flujoACT,colaACT);		
			var NumeroKeyword_CP = getValor(cpLC,'NKeyword_CP');
			var fic =  $("#idDocumento").val();
			var idsession = $.session.get('sessionID');
			fic = fic.split('\\');
			var nombreDoc = fic[fic.length-1];

			if(nombreDoc.length > 0){
				stdForm.blockInput();
				if(checkList.length > 0)
				{
					for(var i = 0; i < checkList.length; i++)
					{
						var RutIdContrato = getIdContratoRut(autorizaAdjMasivo.TipoComplemento,checkList[i]);

						var formData = new FormData($("#formulario")[0]);                                       
						$.ajax({
							url: URL + '/UploadArchivoGen.svc/UploadFileGen/' + autorizaAdjMasivo.NumeroDoc + '/' + NumeroKeyword_CP + '/' + checkList[i] + '/' + RutIdContrato + '/'+autorizaAdjMasivo.Descripcion+'/' + idsession + '/' + nombreDoc,
							type: "POST",
							data: formData,
							contentType: false,
							processData: false,
							success: function(responseText, textStatus)
							{
								if(responseText.WasSuccessful > 0 ){
									setDocumentoPorFlujo(responseText.WasSuccessful, flujoACT, colaACT);
									document.getElementById(botonMasivo).disabled = false;                         	                                                     
								}
								else
								{
									sendErrorMessage("No es posible subir el documento, numero idDoc: " + responseText.WasSuccessful);
								}
							},

							error : function(xhrequest, ErrorText, thrownError) {
								alert(thrownError );
								stdForm.unblockInput();
							}
						});
					}
				}
				else
				{
					sendErrorMessage('No ha seleccionado ningún elemento');
				}
				$('#adjAprobacionPago').modal('hide');
				stdForm.unblockInput();
			}
			else
			{
				sendErrorMessage("Debe seleccionar Documento !");
			}
		});

	}

/*
CREATE: NAITSIRHC REQUENA A
FUNCION QUE RETORNA UN NUMERO FORMATEADO {1.000,01}
VARIABLE DE ENTRADA {INT}
RETURN {INT}
FUNCION QUE PERMITE DAR FORMATO EN PESOS A UN VALOR NUMERICO
*/
function formatoMiles(x) {
	var parts = x.toString().split(".");
	parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");   

	var conversionMil = parts.join(".");    
	var quitarComasSeparadoras = conversionMil.replace(/,/g,"c");   
	var quitarPuntosDecimales = quitarComasSeparadoras.replace(".","p");    
	var setPuntosSeparadores = quitarPuntosDecimales.replace(/c/g,".");    
	var setComaDecimal = setPuntosSeparadores.replace("p",",");
	return setComaDecimal;
}

/*
CREATE: NAITSIRHC REQUENA A
FUNCION QUE RETORNA UN DATE CON FORMATO dd/mm/yyyy
VARIABLE DE ENTRADA {int} formato{yyyyMMdd} ej(20170505)
RETURN {string}
FUNCION QUE PERMITE DAR FORMATO A UNA FECHA (dd/mm/yyyy)
*/
function formatoFechaToTabla(fecha)
{
	var dateString  = ""+fecha;
	var year        = dateString.substring(0,4);
	var month       = dateString.substring(4,6);
	var day         = dateString.substring(6,8);

	return day+"-"+month+"-"+year;
}

confTabla.loadnormal();