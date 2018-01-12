$(document).ready(function () {
    $.jgrid.defaults.width = 'auto';
    $.jgrid.defaults.responsive = true;
    $.jgrid.defaults.styleUI = 'Bootstrap';

    var grdEvAcumulativa = $('#dtaJsonEvAcumulativa').val();
    var grid = $("#grdEvAcumulativa");
    var lastsel;
    var lstEvaluaciones = new Array();
    var parcialActivo = "bytNota" + $("#dtaParcialActivo").val();
    var selIRow = 1;
    var rowIds;
    var gn1 = ($('#dtaParcialActivo').val() == "1") ? true : false;
    var gn2 = ($('#dtaParcialActivo').val() == "2") ? true : false;
    var gn3 = ($('#dtaParcialActivo').val() == "3") ? true : false;

    var blnCambiosEvAc = false;
    var banControlImpresion = false;

    //  Objeto con informacion de dtaEvAcumulada
    cargarDatosEvAcumulativa($('#dtaJsonEvAcumulativa').val());

    //  Gestion de notas 
    grid.jqGrid({
        url: 'data.json',
        editurl: 'clientArray',
        datatype: "jsonstring",
        colModel: [ { name: "No", index: "No", label: "No", align: "center", width: "20", sortable: false },
			        { name: "sintCodMatricula", key: true, hidden: true },
                    { name: "strCodigo", label: "Codigo", align: "center", width: "30", sortable: true, sorttype: "number" },
			        { name: "NombreEstudiante", label: "Nombre estudiante", align: "left", width: "200", sortable: true, sorttype: "string" },
			        { name: "bytNumMat", label: "Matricula", align: "center", width: "50", sortable: true, sorttype: "number" },

			        { name: "bytNota1", index: "bytNota1", label: "Nota uno (1)", align: "center", width: "50", editable: gn1, edittype: "text", editoptions: { size: 1, maxlength: 2, dataInit: soloNumero }, editrules: { custom: true, custom_func: validarNotaSobre8, integer: true }, sortable: true, sorttype: "number", formatter: { integer: { thousandsSeparator: " ", defaultValue: 0 } } },
                    { name: "bytNota2", index: "bytNota2", label: "Nota dos (2)", align: "center", width: "50", editable: gn2, edittype: "text", editoptions: { size: 1, maxlength: 2, dataInit: soloNumero }, editrules: { custom: true, custom_func: validarNotaSobre10 }, sortable: true, sorttype: "number", formatter: { integer: { thousandsSeparator: " ", defaultValue: 0 } } },
                    { name: "bytNota3", index: "bytNota3", label: "Nota tres (3)", align: "center", width: "50", editable: gn3, edittype: "text", editoptions: { size: 2, maxlength: 2, dataInit: soloNumero }, editrules: { custom: true, custom_func: validarNotaSobre10 }, sortable: true, sorttype: "number", formatter: { integer: { thousandsSeparator: " ", defaultValue: 0 } } },

			        { name: "Total", label: "Total", align: "center", width: "50", sortable: true, sorttype: "number" },
			        { name: "bytAsistencia", label: "Asistencia(%)", align: "center", width: "50", editable: gn3, edittype: "text", editoptions: { size: 2, maxlength: 3, dataInit: soloNumero }, editrules: { custom: true, custom_func: validarAsistencia }, sortable: true, sorttype: "number", formatter: { integer: { thousandsSeparator: " ", defaultValue: 0 } } },
			        { name: "ucAcumulado", label: "Estado", width: "70", align: "center", sortable: true, sorttype: "string" },
			        { name: "strObservaciones", label: "Observación", align: "left", width: "100", sortable: false }],

        loadonce: true,
        datastr: $("#dtaJsonEvAcumulativa").val(),
        viewrecords: true,
        autowidth: true,
        height:"auto",
        shrinkToFit: true,
        ignoreCase: true,
        rowNum: 100,
        onSelectRow: editRegistroNota,
        loadComplete: function (data) {
            $(this).jqGrid('setGridParam', 'rowNum', data.length);

            //  Resaltar contenido en columnas en la pagina
            updContenidoColumnasGrid();
        }
    });


    var clickedCell;
    var banUpdRow = false;
    $('#grdEvAcumulativa td').on('click', function (e) {
        clickedCell = this;
    });


    function editRegistroNota(id, status, e) 
    {
        if (id !== lastsel) {
            //  Cierro edicion de la ultima fila gestionada
            if (lastsel != undefined) {
                $('#grdEvAcumulativa').jqGrid('restoreRow', lastsel);
            }

            //
            //  Recalculo Acumulado - si el docente edito la nota recalcula el acumulado total, 
            //  cumplimiento y si esta en el ultimo parcial la equivalencia
            //  
            $("#grdEvAcumulativa").jqGrid("editRow", id, {
                keys: true,
                focusField: 4,
                oneditfunc: function () {
                    $('input, textarea', clickedCell).select();
                },
                aftersavefunc: function (id) {
                    //  Registro la informacion gestionada en el JSON
                    guardarDtaEvaluacion(id);

                    //  Actualizo contenido de la fila
                    updDtaEvaluacion(id);

                    //  Obtengo el identificador de la siguiente registro de notas a gestionar
                    var idNextRow = getIdNextRow(id);
                    $('#grdEvAcumulativa').jqGrid('setSelection', idNextRow, true);
                    $("#" + idNextRow + "_" + parcialActivo).select();
                }
            });

            lastsel = id;
        }
    }


    function validarNotaSobre8(value, colname) {
        if (isNaN(parseInt(value)) || value < 0 || value > 8){
            return [false,
                    "Nota de parcial '" + $("#dtaParcialActivo").val() + "' fuera de rango(0 - 8), la calificación es sobre '8' puntos"];
        }else{
            return [true, ""];
        }   
    }


    function validarNotaSobre10(value, colname)
    {
        if (isNaN(parseInt(value)) || value < 0 || value > 10)
            return [false, "Nota de parcial '" + $("#dtaParcialActivo").val() + "' fuera de rango, la calificación es sobre '10' puntos"];
        else
            return [true, ""];
    }


    function validarAsistencia(value, colname)
    {
        if (value < 0 || value > 100) {
            return [false, "Porciento de asistencia fuera de rango (0, 100)"];
        } else {
            return [true, ""];
        }
    }


    function getIdNextRow(idActualRow) {
        var num = rowIds.length;
        var idNextRow = idActualRow;

        for (var x = 0; x < num; x++) {
            if (rowIds[x] == idActualRow && x < num) {
                idNextRow = rowIds[x + 1];
                break;
            }
        }

        return idNextRow;
    }


    function cargarDatosEvAcumulativa(dtaEvAcumulada)
    {
        var dtaEvaluaciones = eval(dtaEvAcumulada);

        if (dtaEvaluaciones.length > 0) {
            var nee = dtaEvaluaciones.length;
            for (var x = 0; x < nee; x++) {
                var objEvaluaciones = new EvaluacionAcumulativa();
                objEvaluaciones.setDtaEvaluacion(   dtaEvaluaciones[x],
                                                    $("#dtaParcialActivo").val());

                lstEvaluaciones.push(objEvaluaciones);
            }
        }
    }


    function updDtaEvaluacion(id)
    {
        numReg = lstEvaluaciones.length;
        for (var x = 0; x < numReg; x++) {
            if (lstEvaluaciones[x].sintCodMatricula == id) {
                $("#grdEvAcumulativa").jqGrid('setRowData', id, { Total: lstEvaluaciones[x].acumulado() });
                $("#grdEvAcumulativa").jqGrid('setRowData', id, { ucAcumulado: lstEvaluaciones[x].getEstadoEvaluacion() });

                break;
            }
        }

        $("#grdEvAcumulativa").jqGrid(  'setCell',
                                        id,
                                        parcialActivo,
                                        "",
                                        {   'background-color': '#dff0d8',
                                            'background-image': 'none',
                                            'text-align': 'center',
                                            'font-size': 'medium',
                                            'font-weight': 'bold'
                                        });

        $("#grdEvAcumulativa").jqGrid('setCell',
                                        id,
                                        'Total',
                                        "",
                                        {   'background-color': '#dff0d8',
                                            'background-image': 'none',
                                            'text-align': 'center',
                                            'font-size': 'medium',
                                            'font-weight': 'bold'
                                        });

        if ($("#dtaParcialActivo").val() == 3) {

            $("#grdEvAcumulativa").jqGrid('setCell',
                                        id,
                                        'bytAsistencia',
                                        "",
                                        {   'background-color': '#dff0d8',
                                            'background-image': 'none',
                                            'text-align': 'center',
                                            'font-size': 'medium',
                                            'font-weight': 'bold'
                                        });

        }

        return true;
    }


    function updContenidoColumnasGrid()
    {
        var rowIds = $('#grdEvAcumulativa').jqGrid('getDataIDs');
        var DataIds = $('#grdEvAcumulativa').jqGrid('getRowData');
        var numRegEA = lstEvaluaciones.length;

        for (i = 0; i < rowIds.length; i++) {
            for (j = 0; j < numRegEA; j++) {
                if (rowIds[i] == lstEvaluaciones[j].sintCodMatricula) {
                    //  En funcion al parcial activo resalto el color de la columna 
                    $("#grdEvAcumulativa").jqGrid(  'setCell',
                                                    rowIds[i],
                                                    parcialActivo,
                                                    "",
                                                    { 'background-color': '#fcf8e3' });

                    $("#grdEvAcumulativa").jqGrid('setRowData', rowIds[i], { bytNumMat: lstEvaluaciones[j].getNumMatricula() });
                    $("#grdEvAcumulativa").jqGrid('setRowData', rowIds[i], { ucAcumulado: lstEvaluaciones[j].getEstadoEvaluacion() });

                    j = numRegEA;
                }
            }
        }
    }


    //$("#grdEvAcumulativa").jqGrid('setGroupHeaders', {
    //    useColSpanStyle: true,
    //    groupHeaders: [ { startColumnName: 'bytNota1', numberOfColumns: 3, titleText: '<b>EVALUACIÓN ACUMULATIVA</b>' },
    //                    { startColumnName: 'Total', numberOfColumns: 3, titleText: '<b>TOTALIZADOS</b>' }]
    //});


    function guardarDtaEvaluacion(id)
    {
        numReg = lstEvaluaciones.length;
        for (var x = 0; x < numReg; x++) {
            if (lstEvaluaciones[x].sintCodMatricula == id) {
                var dtaNota = $("#grdEvAcumulativa").jqGrid("getCell", id, "bytNota" + $('#dtaParcialActivo').val());
                var dtaAsistencia = $("#grdEvAcumulativa").jqGrid("getCell", id, "bytAsistencia");

                lstEvaluaciones[x]["bytNota" + $('#dtaParcialActivo').val()] = (dtaNota == "") ? 0 : dtaNota;
                lstEvaluaciones[x].bytAsistencia = dtaAsistencia;
                lstEvaluaciones[x].banEstado = 1;

                blnCambiosEvAc = true;

                return true;
            }
        }

        return false;
    }


    function soloNumero(element)
    {
        $(element).keypress(function (e) {
            if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
                return false;
            }
        });
    }


    $('#btnGuardarEvAcumulativa').on('click', function(){
        //  Muestro mensaje de proceso
        showLoadingProcess('Guardar información');

        if (blnCambiosEvAc == true) {
            $.ajax({
                type: "POST",
                url: "/Docentes/registrarEvaluacion/" + $("#nivel").val() + "/" + $("#codAsignatura").val() + "/" + $("#paralelo").val() + "/" + $('#dtaParcialActivo').val(),
                data: JSON.stringify(lstEvaluaciones),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log(xhr.responseText);

                    //  Mostrar mensaje de estado de la transaccion
                    getMensajeTransaccion(false, "Error, favor vuelva a intentarlo, si el problema persiste consulte en la secretaria de su carrara");

                    //  Cierro la ventana GIF Proceso
                    HoldOn.close();

                    //  Regreso a su valor original la variable de control de cambios en el grid de notas
                    blnCambiosEvAc = false;

                    //  limpio el ultimo registro de notas gestionado
                    lastsel = 0;
                }
            }).success(function (data) {
                if (data.dtaEvAcumulativaUpd != "false") {
                    //  Vacio la lista de notas
                    lstEvaluaciones = new Array()

                    //  Actualizo el grid con la informacion gestionada a nivel BD
                    cargarDatosEvAcumulativa(data.dtaEvAcumulativaUpd);

                    //  Actualizo el grid de notas
                    updContenidoColumnasGrid();

                    //  Regreso a su valor original la variable de control de cambios en el grid de notas
                    blnCambiosEvAc = false;

                    //  Mostrar mensaje de estado de la transaccion
                    getMensajeTransaccion(true, data.MessageGestion);

                    //  limpio el ultimo registro de notas gestionado
                    lastsel = 0;

                    $('#grdEvAcumulativa').trigger('reloadGrid');
                } else {
                    //  Mostrar mensaje de estado de la transaccion
                    getMensajeTransaccion(false, data.MessageGestion);
                }
                
                //  Cierro la ventana GIF Proceso
                HoldOn.close();
            })
        } else {
            //  Cierro la ventana GIF Proceso
            HoldOn.close();
        }
    })


    function getMensajeTransaccion( banEstado, mensaje )
    {
        //  Muestro mensaje de gestion de informacion
        $('#msmGrdEvAcumulativa').removeAttr("hidden");

        if (banEstado == true) {
            $('#msmGrdEvAcumulativa').attr("class", "alert alert-success fade in");
            $('#msmGrdEvAcumulativa').html("<button class='close' data-dismiss='alert'>×</button> <i class='fa fa-check' aria-hidden='true'></i> <strong>" + mensaje + "</strong>");
        } else if( banEstado == false){
            $('#msmGrdEvAcumulativa').attr("class", "alert alert-danger fade in");
            $('#msmGrdEvAcumulativa').html("<button class='close' data-dismiss='alert'>×</button> <i class='fa fa-exclamation-circle' aria-hidden='true'></i> <strong>"+ mensaje +"</strong>");
        }

    }


    function showLoadingProcess(mensaje) {
        var msg = (mensaje.length == 0) ? 'GUARDANDO INFORMACION ...'
                                        : mensaje;

        HoldOn.open({
            theme: 'sk-dot',
            message: "<h4>" + msg + "</h4>"
        });
    }


    //  Control de impresion de actas
    $('#pEA_pdf, #pEA_xls, #pEA_blc').on('click', function () {
        $("#opImpEvAcumulada").val($(this).attr("id"));

        if (banControlImpresion == false) {
            if (blnCambiosEvAc == true) {
                $('#btnGuardarEvAcumulativa').trigger("click");
            }

            //  Creo y envio el codigo de autenticacion 
            controlImpresion();
        } else if (banControlImpresion == true) {
            imprimirActaEvAcumulada();
        }
        
    })


    function controlImpresion() {
        //  Control de impresion
        $.confirm({
            icon: 'glyphicon glyphicon-alert',
            columnClass: 'col-md-6 col-md-offset-3',
            title: 'Control de impresión',
            content: '<form action="#" class="formName">' +
                    '   <div class="alert alert-warning">' +
                    '       <p>Compañero docente, le recordamos que luego de ejecutar esta acción' +
                    '       <strong> USTED DA POR FINALIZADA LA "GESTIÓN DE NOTAS DE LA EVALUACIÓN DE ACUMULATIVA DE LA ASIGNATURA ' + $('#ddlLstPeriodosEstudiante :selected').text() + '" </strong>.</p>' +
                    '       <p>Luego de esta acción ninguna nota podrá ser gestionada desde este módulo web del sistema académico.</p>' +
                    '   </div>' +

                    '   <div class="alert alert-info">' +
                    '       Por su seguridad y para continuar con la ejecución de esta tarea se enviará un "código de impresión" a su cuenta de correo electrónico institucional <strong>' + $('#dtaCtaUsuario').val() + '</strong>' +
                    '   </div>' +
                    '</form>',

            escapeKey: 'cancelar',
            buttons: {
                formSubmit: {
                    text: 'Enviar código de impresión',
                    btnClass: 'btn-blue',
                    action: function () {
                        showLoadingProcess('Enviando código de impresión ...');
                        enviarCodigoImpresion();
                    }
                },
                cancelar: function () {
                    //close
                },
            },

        });
    }


    function enviarCodigoImpresion()
    {
        $.ajax({
            type: "POST",
            url: "/Docentes/EnviarCorreoValidacionImpresion",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (xhr, ajaxOptions, thrownError) {
                console.log(xhr.responseText);

                //  Mostrar mensaje de estado de la transaccion
                getMensajeTransaccion(false, "Error, favor vuelva a intentarlo, si el problema persiste consulte en la secretaria de su carrara");

                //  Cierro la ventana GIF Proceso
                HoldOn.close();
            }
        }).complete(function (data) {
            if (data.responseJSON.banEnviocodAutenticacion == true) {
                //  Cierro la ventana GIF Proceso
                HoldOn.close();

                //  Muestro la ventana de ingreso de codigo de impresion
                frmValidacionCodigoImpresion()
            } else {
                //  Si existe error, muestro el mensaje
                $('#messageError').removeAttr("hidden");
                $('#messageError').html("<a href='' class='close'>×</a><strong>" + data.responseJSON.errorMessage + "</strong>, Favor vuelva a intentarlo");
            }
        })

    }


    function frmValidacionCodigoImpresion() {
        $.confirm({
            icon: 'glyphicon glyphicon-alert',
            columnClass: 'col-md-6 col-md-offset-3',
            title: 'Control de impresión',
            content: '<form action="#" class="formName">' +
                    '   <div class="form-group">' +
                    '       <input id="dtaNumConfirmacion" maxlength="4" type="text" placeholder="código de impresión" class="name form-control" required />' +
                    '   </div>' +
                    '</form>' +
                    '<script>' +
                    '   $(document).ready(function(){' +
                    '       $("#dtaNumConfirmacion").keypress(function (event) {' +
                    '           var $this = $(this);' +
                    '           if (((event.which < 48 || event.which > 57) && (event.which != 0 && event.which != 8))) {' +
                    '               event.preventDefault();' +
                    '           }' +
                    '       })' +
                    '   })' +
                    '</script>',

            escapeKey: 'cancelar',
            buttons: {
                formSubmit: {
                    text: 'Validar e imprimir',
                    btnClass: 'btn-blue',
                    action: function () {
                        validarCodigoImpresion($("#opImpEvAcumulada").val(), this.$content.find('#dtaNumConfirmacion').val())
                    }
                },
                cancelar: function () {
                    //  Close
                },
            },
        });
    }


    function validarSoloNumero() {
        alert("You pressed a key inside the input field");
    }


    function validarCodigoImpresion(opImpresion, numConfirmacion)
    {
        showLoadingProcess('Verificando código de impresión ...');

        $.ajax({
            type: "POST",
            url: "/Docentes/ValidarCodigoImpresion",
            data: '{strCodImpresion: "' + numConfirmacion + '", idActa: "' + opImpresion + '", idAsignatura: "' + $('#ddlLstPeriodosEstudiante').val() + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            error: function (xhr, ajaxOptions, thrownError) {
                console.log(xhr.responseText);

                //  Mostrar mensaje de estado de la transaccion
                getMensajeTransaccion(false, "Error, favor vuelva a intentarlo, si el problema persiste consulte en la secretaria de su carrara");

                //  Cierro la ventana GIF Proceso
                HoldOn.close();
            }

        }).complete(function (data) {
            if (data.responseJSON.fileName != "none" && data.responseJSON.fileName != "" && data.responseJSON.fileName != undefined) {
                showLoadingProcess('Generando archivo ...');

                //  Cambio el color del boton
                $('#btnEA, #btnEAF').attr("class", 'btn btn-warning btn-md');

                //  Oculto el mensaje de error
                $('#messageError').attr("hidden");

                //  Cambio el grid de gestion de nota de evaluacion acumulativa a modo solo lectura
                grdEvAcumulativaSoloLectura();

                //  Actualizo la bandera de impresion
                banControlImpresion = true;

                //  Actualizo mensaje de estado de evaluacion
                $('#msgEstadoEvaluacion').attr('class', 'text-info pull-right')
                $('#msgEstadoEvaluacion')[0].innerHTML = "<strong>Gestión de notas 'finalizada'</strong>"

                //  Elimino el boton de guardar
                $('#btnGuardarEvAcumulativa').remove();

                //  Cierro la ventana GIF Proceso
                HoldOn.close();

                $.redirect("/Docentes/DownloadFile", { file: data.responseJSON.fileName }, "POST")
            } else {
                //  Si existe error, muestro el mensaje
                alert(data.responseJSON.errorMessage);

                //  Cierro la ventana GIF Proceso
                HoldOn.close();

                frmValidacionCodigoImpresion();
            }
        })
    }


    $('#btnCerrarNC').click(function () {
        //  Cierro el formulario de ingreso de codigo de impresion
        $.unblockUI();
    })


    function grdEvAcumulativaSoloLectura() {
        $('#grdEvAcumulativa').setColProp('bytNota1', { editable: 'False' });
        $('#grdEvAcumulativa').setColProp('bytNota2', { editable: 'False' });
        $('#grdEvAcumulativa').setColProp('bytNota3', { editable: 'False' });
        $('#grdEvAcumulativa').setColProp('bytAsistencia', { editable: 'False' });
    }


    function imprimirActaEvAcumulada()
    {
        showLoadingProcess('PROCESANDO INFORMACIÓN ...');

        $.ajax({
            type: "POST",
            url: "/Docentes/impresionActas",
            data: '{idActa: "' + $("#opImpEvAcumulada").val() + '", idAsignatura: "' + $('#ddlLstPeriodosEstudiante').val() + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (xhr, ajaxOptions, thrownError) {
                console.log(xhr.responseText);

                //  Mostrar mensaje de estado de la transaccion
                getMensajeTransaccion(false, "Error, favor vuelva a intentarlo, si el problema persiste consulte en la secretaria de su carrara");

                //  Cierro la ventana GIF Proceso
                HoldOn.close();
            }
        }).complete(function (data) {
            //  Oculto el mensaje de error
            $('#messageError').attr("hidden");

            //  Cierro la ventana GIF Proceso
            HoldOn.close();

            if (data.responseJSON.fileName != "none" && data.responseJSON.fileName != "") {
                $.redirect("/Docentes/DownloadFile",
                            {   file: data.responseJSON.fileName },
                                "POST")
            } else {
                //  Si existe error, muestro el mensaje
                $('#messageError').removeAttr("hidden");
                $('#messageError').html("<a href='#' class='close'>×</a><strong>FALLO !!!</strong> Favor vuelva a intentarlo");
            }
        })
    }
})