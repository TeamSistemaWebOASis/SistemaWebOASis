//  GESTION DE INFORMACION DE EVALUACION FINAL
$(document).ready(function () {
    $.jgrid.defaults.width = '100%';
    $.jgrid.defaults.responsive = true;
    $.jgrid.defaults.styleUI = 'Bootstrap';

    var dtaEvFinal = $('#dtaJsonEvFinal').val();
    var grdEvFinal = $("#grdEvFinal");
    var lstEvaluacionFinal = new Array();
    var blnCambiosEvFinal = false;

    var lastsel;
    var selIRow = 1;
    var rowIds;
    var codAutenticacion = null;
    var banControlImpresion = false;

    //  Objeto con informacion de dtaEvAcumulacionFinal
    cargarDatosEvFinal($('#dtaJsonEvFinal').val());

    function cargarDatosEvFinal(dtaEvFinal) {
        var dtaEvaluacionFinal = (dtaEvFinal.length != 0)
                                    ? eval(dtaEvFinal)
                                    : new Array();

        if (dtaEvaluacionFinal.length > 0) {
            var nef = dtaEvaluacionFinal.length;
            for (var x = 0; x < nef; x++) {
                var objEvaluacionFinal = new EvaluacionFinal();
                objEvaluacionFinal.setDtaEvaluacionFinal(dtaEvaluacionFinal[x]);
                lstEvaluacionFinal.push(objEvaluacionFinal);
            }
        }
    }


    //  Gestion de notas 
    grdEvFinal.jqGrid({
        url: 'data.json',
        editurl: 'clientArray',
        datatype: "jsonstring",
        colModel: [{ name: 'No', index: 'No', label: 'No', align: 'center', width: '30', sortable: false },
                    { name: "sintCodMatricula", key: true, hidden: true },
                    { name: "strCodigo", label: "Codigo", align: "center", width: "60", sortable: true, sorttype: "number" },
                    { name: 'NombreCompleto', label: "Nombre estudiante", width: '300', align: 'left', sortable: true },
                    { name: 'bytNumMat', label: 'Matrícula', width: '80', align: 'center', sortable: false, sorttype: "number" },
                    { name: 'bytAcumulado', label: 'Total acumulado', width: '120', align: 'center', sortable: false, sorttype: "number" },
                    { name: 'bytAsistencia', label: 'Total asistencia(%)', width: '120', align: 'center', sortable: false, sorttype: "number" },

                    { name: 'bytNota', label: 'Nota evaluación final', width: '120', align: 'center', editable: true, edittype: 'text', editoptions: { size: 1, maxlength: 2, dataInit: soloNumero }, editrules: { custom: true, custom_func: validarNotaEvFinal }, sortable: true, sorttype: "number", formatter: { integer: { thousandsSeparator: " ", defaultValue: '0' } } },

                    { name: 'Total', label: 'Total Ev. final', width: '120', align: 'center', sortable: true, sorttype: "number" },
                    { name: 'efAcumulado', label: 'Estado', width: '120', align: 'center' },
                    { name: 'strObservaciones', label: 'Observación', width: '170', align: 'center', sortable: false, sorttype: "string" }],

        loadonce: true,
        datastr: $("#dtaJsonEvFinal").val(),
        autowidth: true,
        height: "auto",
        shrinkToFit: true,
        ignoreCase: true,
        rowNum: 100,
        onSelectRow: editarRegistroEvFinal,
        loadComplete: function (data) {
            //  Resaltar contenido en columnas en la pagina
            updContenidoColumnasEvFinal();
        }
    });


    var clickedCell;
    var banUpdRow = false;
    $('#grdEvFinal td').on('click', function (e) {
        clickedCell = this;
    });


    function editarRegistroEvFinal(id, status, e) {
        if (id !== lastsel && editarFila(id)) {
            //  Cierro edicion de la ultima fila gestionada
            if (lastsel != undefined) {
                $('#grdEvFinal').jqGrid('restoreRow', lastsel);
            }

            //
            //  Recalculo Acumulado - si el docente edito la nota recalcula el acumulado total, 
            //  cumplimiento y si esta en el ultimo parcial la equivalencia
            //  
            $("#grdEvFinal").jqGrid("editRow", id, {
                keys: true,
                focusField: 4,
                oneditfunc: function () {
                    $('input, textarea', clickedCell).select();
                },
                aftersavefunc: function (id) {
                    //  Registro la informacion gestionada en el JSON
                    guardarDtaEvaluacionFinal(id);

                    //  Actualizo contenido de la fila
                    updDtaEvaluacionFinal(id);

                    //  Obtengo el identificador de la siguiente registro de notas a gestionar
                    var idNextRow = getIdNextRow(id);
                    $('#grdEvFinal').jqGrid('setSelection', idNextRow, true);
                    $("#" + idNextRow + "_bytNota").select();
                }
            });

            lastsel = id;
        } else {
            //  Si el registro del estudiante es de EXONERADO - REPROBADO
            $('#grdEvFinal').jqGrid('setSelection', id, false);
        }
    }


    function editarFila(id) {
        var ban = true;

        numReg = lstEvaluacionFinal.length;
        for (var x = 0; x < numReg; x++) {
            if (lstEvaluacionFinal[x].sintCodMatricula == id) {
                ban = lstEvaluacionFinal[x].esExoneradoReprobado();
            }
        }

        return ban;
    }


    $("#grdEvFinal").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'bytAcumulado', numberOfColumns: 2, titleText: '<b>TOTALIZADO EVALUACIÓN FORMATIVA</b>' },
                        { startColumnName: 'Total', numberOfColumns: 2, titleText: '<b>TOTALIZADO</b>' }]
    });


    function guardarDtaEvaluacionFinal(id) {
        var ban = false;
        var numReg = lstEvaluacionFinal.length;
        for (var x = 0; x < numReg; x++) {
            if (lstEvaluacionFinal[x].sintCodMatricula == id) {
                var dtaNota = $("#grdEvFinal").jqGrid("getCell", id, "bytNota");
                lstEvaluacionFinal[x]["bytNota"] = dtaNota;
                lstEvaluacionFinal[x].banEstado = 1;

                blnCambiosEvFinal = true;

                ban = true;
            }
        }

        return ban;
    }


    function getIdNextRow(idActualRow) {
        var rowIds = $('#grdEvFinal').jqGrid('getDataIDs');
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


    function soloNumero(element) {
        $(element).keypress(function (e) {
            if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
                return false;
            }
        });
    }


    function updContenidoColumnasEvFinal() {
        var rowIds = $('#grdEvFinal').jqGrid('getDataIDs');
        var DataIds = $('#grdEvFinal').jqGrid('getRowData');
        var numRegEF = lstEvaluacionFinal.length;

        for (i = 0; i < rowIds.length; i++) {
            for (j = 0; j < numRegEF; j++) {
                if (rowIds[i] == lstEvaluacionFinal[j].sintCodMatricula) {
                    //  En funcion al parcial activo resalto el color de la columna 
                    $("#grdEvFinal").jqGrid('setCell',
                                            rowIds[i],
                                            'bytNota',
                                            "",
                                            { 'background-color': '#fcf8e3' });

                    $("#grdEvFinal").jqGrid('setRowData', rowIds[i], { bytNumMat: lstEvaluacionFinal[j].getNumMatricula() });
                    $("#grdEvFinal").jqGrid('setRowData', rowIds[i], { efAcumulado: lstEvaluacionFinal[j].getEstadoEvaluacionFinal() });

                    j = numRegEF;
                }
            }
        }

    }


    function updDtaEvaluacionFinal(id) {
        numReg = lstEvaluacionFinal.length;
        for (var x = 0; x < numReg; x++) {
            if (lstEvaluacionFinal[x].sintCodMatricula == id) {
                $("#grdEvFinal").jqGrid('setRowData', id, { Total: lstEvaluacionFinal[x].getTotalEvFinal() });
                $("#grdEvFinal").jqGrid('setRowData', id, { efAcumulado: lstEvaluacionFinal[x].getEstadoEvaluacionFinal() });

                break;
            }
        }

        $("#grdEvFinal").jqGrid('setCell',
                                    id,
                                    'bytNota',
                                    "",
                                    {
                                        'background-color': '#dff0d8',
                                        'background-image': 'none',
                                        'text-align': 'center',
                                        'font-size': 'medium',
                                        'font-weight': 'bold'
                                    });


        $("#grdEvFinal").jqGrid('setCell',
                                    id,
                                    'Total',
                                    "",
                                    {
                                        'background-color': '#dff0d8',
                                        'background-image': 'none',
                                        'text-align': 'center',
                                        'font-size': 'medium',
                                        'font-weight': 'bold'
                                    });

        return true;
    }


    function showLoadingProcess(mensaje) {
        var msg = (mensaje.length == 0) ? 'GUARDANDO INFORMACION ...'
                                            : mensaje;

        HoldOn.open({
            theme: 'sk-dot',
            message: "<h4>" + mensaje + "</h4>"
        });
    }


    function validarNotaEvFinal(value, colname) {
        if (value < 0 || value > 12)
            return [false, "Nota de evaluación final fuera de rango, la calificación es sobre '12' puntos"];
        else
            return [true, ""];
    }


    $('#btnGuardarEvFinal').on('click', function () {
        //  Muestro mensaje de proceso
        showLoadingProcess('Guardando información');

        //  Verifico si el Grid de notas a cambiado
        if (blnCambiosEvFinal == true) {
            $.ajax({
                type: "POST",
                url: "/Docentes/registrarEvaluacionFinal/" + $("#nivel").val() + "/" + $("#codAsignatura").val() + "/" + $("#paralelo").val() + "/" + $('#dtaParcialActivo').val(),
                data: JSON.stringify(lstEvaluacionFinal),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(thrownError);
                }
            }).success(function (data) {
                if (data.dtaEvFinalUpd != "false") {
                    lstEvaluacionFinal = new Array();

                    //  Muestro mensaje de gestion de informacion
                    $('#msmGrdEvFinal').removeAttr("hidden");

                    //  Actualizo el grid con la informacion gestionada a nivel BD
                    cargarDatosEvFinal(data.dtaEvFinalUpd);

                    //  Actualizo el grid de notas
                    updContenidoColumnasEvFinal();

                    //  Regreso a su valor original la variable de control de cambios en el grid de notas
                    blnCambiosEvFinal = false;

                    //  Mostrar mensaje de estado de la transaccion
                    getMensajeTransaccion(true, data.MessageGestion);

                    //  limpio el ultimo registro de notas gestionado
                    lastsel = 0;

                    //  recargo el grid de notas
                    $('#grdEvFinal').trigger('reloadGrid');
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


    function getMensajeTransaccion(banEstado, mensaje) {
        //  Muestro mensaje de gestion de informacion
        $('#msmGrdEvFinal').removeAttr("hidden");

        if (banEstado == true) {
            $('#msmGrdEvFinal').attr("class", "alert alert-success fade in");
            $('#msmGrdEvFinal').html("<button class='close' data-dismiss='alert'>×</button> <i class='fa fa-check' aria-hidden='true'></i> <strong>" + mensaje + "</strong>");
        } else if (banEstado == false) {
            $('#msmGrdEvFinal').attr("class", "alert alert-danger fade in");
            $('#msmGrdEvFinal').html("<button class='close' data-dismiss='alert'>×</button> <i class='fa fa-exclamation-circle' aria-hidden='true'></i> <strong>" + mensaje + "</strong>");
        }

    }


    //  Control de impresion de actas
    $('#pEF_pdf, #pEF_xls, #pEF_blc').on('click', function () {
        $("#opImpEvFinal").val($(this).attr("id"));

        if (banControlImpresion == false) {
            if (blnCambiosEvFinal == true) {
                $('#btnGuardarEvFinal').trigger("click");
            }

            controlImpresion();
        } else if (banControlImpresion == false) {
            imprimirActaEvFinal();
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
                    '       <strong> USTED DA POR FINALIZADA LA "GESTIÓN DE NOTAS DE LA EVALUACIÓN DE RECUPERACIÓN DE LA ASIGNATURA ' + $('#ddlLstPeriodosEstudiante :selected').text() + '" </strong>.</p>' +
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
                },
                cancelar: function () {
                    //close
                },
            },

        });
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
                        var opImpresion = $("#opImpEvFinal").val();
                        var numConfirmacion = this.$content.find('#dtaNumConfirmacion').val();
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

                                return false;
                            }

                        }).complete(function (data) {
                            if (data.responseJSON.fileName != "none" && data.responseJSON.fileName != "" && data.responseJSON.fileName != undefined) {
                                //  Cambio el color del boton
                                $('#btnEF, #btnEFF').attr("class", 'btn btn-warning btn-md');

                                //  Oculto el mensaje de error
                                $('#messageError').attr("hidden");

                                //  Cierro la ventana GIF Proceso
                                HoldOn.close();

                                //  Cambio el grid de gestion de nota de evaluacion acumulativa a modo solo lectura
                                grdEvFinalSoloLectura();

                                //  Actualizo la bandera de impresión
                                banControlImpresion = true;

                                //  Elimino el boton de guardar
                                $('#btnGuardarEvFinal').remove();

                                //  Actualizo mensaje de estado de evaluacion
                                $('#msgEstadoEvaluacion').attr('class', 'text-info pull-right')
                                $('#msgEstadoEvaluacion')[0].innerHTML = "<strong>Gestión de notas 'finalizada'</strong>"

                                //  Descarga de archivo
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
                },
                cancelar: function () {
                    //close
                },
            },
        });
    }


    function grdEvFinalSoloLectura() {
        $('#grdEvFinal').setColProp('bytNota', { editable: 'False' });
    }


    function getCodAutenticacion() {
        var rst;

        $.ajax({
            type: "POST",
            url: "/Docentes/EnviarCorreoValidacionImpresion",
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).complete(function (data) {
            if (data.responseJSON.codAutenticacion != "false") {
                //  Ejecuto proceso de validacion de informacion
                procesoImpresionActa(data.responseJSON.codAutenticacion);
            }
        })

        return rst;
    }


    function procesoImpresionActa(codAutenticacionCorreo) {
        //  Muestro ventana de autenticacion a dos factores
        $.blockUI({ message: $('#loginForm') });

        codAutenticacion = codAutenticacionCorreo;
    }


    function imprimirActaEvFinal() {
        showLoadingProcess();

        $.ajax({
            type: "POST",
            url: "/Docentes/impresionActas",
            data: '{idActa: "' + $("#opImpEvFinal").val() + '", idAsignatura: "' + $('#ddlLstPeriodosEstudiante').val() + '"}',
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
                            { file: data.responseJSON.fileName },
                                "POST")
            } else {
                //  Si existe error, muestro el mensaje
                $('#messageError').removeAttr("hidden");
                $('#messageError').html("<a href='' class='close'>×</a><strong>Problemas al generar archivo</strong>, favor vuelva a intentarlo");
            }
        })
    }

})