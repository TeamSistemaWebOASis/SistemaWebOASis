//  GESTION DE INFORMACION DE EVALUACION FINAL
$(document).ready(function () {
    $.jgrid.defaults.width = '100%';
    $.jgrid.defaults.responsive = true;
    $.jgrid.defaults.styleUI = 'Bootstrap';

    var dtaEvRecuperacion = $('#dtaJsonEvRecuperacion').val();
    var grdEvRecuperacion = $("#grdEvRecuperacion");
    var lstEvaluacionRecuperacion = new Array();

    var lastsel;
    var selIRow = 1;
    var rowIds;

    var blnCambiosEvRecuperacion = false;

    //  Objeto con informacion de dtaEvAcumulacionFinal
    cargarDatosEvRecuperacion($('#dtaJsonEvRecuperacion').val());


    function cargarDatosEvRecuperacion(dtaEvRecuperacion) {
        var dtaEvaluacionRecuperacion = eval(dtaEvRecuperacion);
        if (dtaEvaluacionRecuperacion.length > 0) {
            var nef = dtaEvaluacionRecuperacion.length;
            for (var x = 0; x < nef; x++) {
                var objEvaluacionRecuperacion = new EvaluacionRecuperacion();
                objEvaluacionRecuperacion.setDtaEvaluacionRecuperacion(dtaEvaluacionRecuperacion[x]);
                lstEvaluacionRecuperacion.push(objEvaluacionRecuperacion);
            }
        }
    }


    //  Gestion de notas 
    grdEvRecuperacion.jqGrid({
        url: 'Docentes/',
        datatype: "json",
        colModel: [{ name: 'No', index: 'No', label: 'No', align: 'center', width: '30', sortable: false },
                    { name: 'strCodigo', key: true, hidden: true },
                    { name: 'NombreCompleto', label: "Nombre estudiante", width: '300', align: 'left', sortable: false },
                    { name: 'bytNumMat', label: 'Matrícula', width: '80', align: 'center', sortable: false },
                    { name: 'bytAcumulado', label: 'Total evaluación formativa', width: '150', align: 'center', sortable: false },
                    
                    { name: 'bytNota', label: 'Nota evaluación recuperación', width: '170', align: 'center', editable: true, edittype: 'text', editoptions: { size: 1, maxlength: 2, dataInit: soloNumero }, editrules: { custom: true, custom_func: validarNota }, sortable: false, formatter: { integer: { thousandsSeparator: " ", defaultValue: '0' } } },

                    { name: 'Total', label: 'Total', width: '120', align: 'center', sortable: false },
                    { name: 'erAcumulado', label: 'Estado', width: '120', align: 'center' },
                    { name: 'strObservaciones', label: 'Observación', width: '170', align: 'center', sortable: false }],
        datatype: "jsonstring",
        datastr: $("#dtaJsonEvRecuperacion").val(),
        viewrecords: true,
        height: "100%",
        ignoreCase: true,
        rowNum: 100,
        onSelectRow: function (id, status, e) {
            if (id !== lastsel) {
                //  Cierro edicion de la ultima fila gestionada
                if (lastsel != undefined) {
                    $('#grdEvRecuperacion').jqGrid('restoreRow', lastsel);
                }

                //
                //  Recalculo Acumulado - si el docente edito la nota recalcula el acumulado total, 
                //  cumplimiento y si esta en el ultimo parcial la equivalencia
                //  
                $("#grdEvRecuperacion").jqGrid("editRow", id, {
                    keys: true,
                    focusField: 4,
                    aftersavefunc: function (id) {
                        //  Registro la informacion gestionada en el JSON
                        guardarDtaEvRecuperacion(id);

                        //  Actualizo contenido de la fila
                        updDtaEvRecuperacion(id);

                        //  Obtengo el identificador de la siguiente registro de notas a gestionar
                        var idNextRow = getIdNextRow(id);
                        if (id != idNextRow) {
                            $('#grdEvRecuperacion').jqGrid('setSelection', idNextRow, true);
                        }
                    }
                });

                lastsel = id;
            } else {
                //  Si el registro del estudiante es de EXONERADO - REPROBADO
                $('#grdEvRecuperacion').jqGrid('setSelection', id, false);
            }
        },

        loadComplete: function (data) {
            //  Resaltar contenido en columnas en la pagina
            updContenidoColumnasGrid();
        }
    });


    $("#grdEvRecuperacion").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'Total', numberOfColumns: 2, titleText: '<b>TOTALIZADO</b>' }]
    });


    function guardarDtaEvRecuperacion(id) {
        numReg = lstEvaluacionRecuperacion.length;
        for (var x = 0; x < numReg; x++) {
            if (lstEvaluacionRecuperacion[x].strCodigo == id) {
                var dtaNota = $("#grdEvRecuperacion").jqGrid("getCell", id, "bytNota");
                lstEvaluacionRecuperacion[x]["bytNota"] = dtaNota;
                lstEvaluacionRecuperacion[x].banEstado = 1;

                //  Registro un cambio
                blnCambiosEvRecuperacion = true;

                return true;
            }
        }

        return false;
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


    function soloNumero(element) {
        $(element).keypress(function (e) {
            if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
                return false;
            }
        });
    }


    function updContenidoColumnasGrid() {
        rowIds = $('#grdEvRecuperacion').jqGrid('getDataIDs');

        for (i = 0; i <= rowIds.length - 1 ; i++) {
            //  En funcion al parcial activo resalto el color de la columna 
            $("#grdEvRecuperacion").jqGrid('setCell',
                                    rowIds[i],
                                    'bytNota',
                                    "",
                                    { 'background-color': '#fcf8e3' });

            $("#grdEvRecuperacion").jqGrid('setRowData', rowIds[i], { bytNumMat: lstEvaluacionRecuperacion[i].getNumMatricula() });
            $("#grdEvRecuperacion").jqGrid('setRowData', rowIds[i], { erAcumulado: lstEvaluacionRecuperacion[i].getEstadoEvaluacionRecuperacion() });
        }
    }


    function updDtaEvRecuperacion(id) {
        numReg = lstEvaluacionRecuperacion.length;
        for (var x = 0; x < numReg; x++) {
            if (lstEvaluacionRecuperacion[x].strCodigo == id) {
                $("#grdEvRecuperacion").jqGrid('setRowData', id, { Total: lstEvaluacionRecuperacion[x].getTotalEvRecuperacion() });
                $("#grdEvRecuperacion").jqGrid('setRowData', id, { erAcumulado: lstEvaluacionRecuperacion[x].getEstadoEvaluacionRecuperacion() });

                break;
            }
        }

        $("#grdEvRecuperacion").jqGrid( 'setCell',
                                        id,
                                        'bytNota',
                                        "",
                                        {   'background-color': '#dff0d8',
                                            'background-image': 'none',
                                            'text-align': 'center',
                                            'font-size': 'medium',
                                            'font-weight': 'bold'
                                        });

        return true;
    }


    function validarNota(value, colname) {
        if (value < 0 || value > 20)
            return [false, "Nota fuera de rango (0, 20)"];
        else
            return [true, ""];
    }


    $('#btnGuardarEvRecuperacion').on('click', function () {
        //  Muestro mensaje de proceso
        $.blockUI({ message: '<h3>Procesando información, favor espere un momento ...</h3>' });

        if (blnCambiosEvRecuperacion == true) {
            $.ajax({
                type: "POST",
                url: "/Docentes/registrarEvaluacionRecuperacion/" + $("#nivel").val() + "/" + $("#codAsignatura").val() + "/" + $("#paralelo").val() + "/" + $('#dtaParcialActivo').val(),
                data: JSON.stringify(lstEvaluacionRecuperacion),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(thrownError);
                }
            }).success(function (data) {
                //  Muestro mensaje de gestion de informacion
                $('#msmGrdEvRecuperacion').removeAttr("hidden");

                //  Actualizo el grid con la informacion gestionada a nivel BD
                cargarDatosEvRecuperacion(data);

                //  Actualizo el grid de notas
                updContenidoColumnasGrid();

                //  Regreso a su valor original la variable de control de cambios en el grid de notas
                blnCambiosEvRecuperacion = false;

                //  Cierro la ventana GIF Proceso
                $.unblockUI();
            })
        } else {
            //  Cierro la ventana GIF Proceso
            $.unblockUI();
        }
    })


    //  Control de impresion de actas
    $('#pER_pdf, #pER_xls, #pER_blc').on('click', function () {
        $("#opImpEvRecuperacion").val($(this).attr("id"));

        //  Muestro ventana de autenticacion a dos factores
        $.blockUI({ message: $('#loginForm') });
    })


    $('#btnValidarImprimir').click(function () {
        if ($('#dtaNumConfirmacion').val() == "987") {
            $.unblockUI();

            //  Guardo los cambios registrados
            $('#btnGuardarEvRecuperacion').trigger("click");

            showLoadingProcess();

            var opImpresion = $("#opImpEvRecuperacion").val();
            
            $.ajax({
                type: "POST",
                url: "/Docentes/impresionActas",
                data: '{idActa: "' + opImpresion + '", idAsignatura: "' + $('#ddlLstPeriodosEstudiante').val() + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            }).complete(function (data) {
                //  Cambio el color del boton
                $('#btnER, #btnERF').attr("class", 'btn btn-warning btn-md');

                //  Oculto el mensaje de error
                $('#messageError').attr("hidden");

                //  Cierro la ventana GIF Proceso
                HoldOn.close();

                $("opImpEvRecuperacion").attr("value", "");

                if (data.responseJSON.fileName != "none" && data.responseJSON.fileName != "") {
                    //  Cambio el Grid a modo "soloLectura"
                    grdEvRecuperacionSoloLectura();

                    //  Elimino el boton de guardado de Ev recuperacion
                    $('#btnGuardarEvRecuperacion').remove();

                    $.redirect( "/Docentes/DownloadFile",
                                { file: data.responseJSON.fileName },
                                "POST")
                } else {
                    //  Si existe error, muestro el mensaje
                    $('#messageError').removeAttr("hidden");
                    $('#messageError').html("<a href='' class='close'>×</a><strong>FALLO !!!</strong> Favor vuelva a intentarlo");
                }
            })
        } else {
            alert('NUMERO DE CONFIRMACION NO VALIDO, favor vuelva a ingresarlo');
        }
    })


    function showLoadingProcess() {
        HoldOn.open({
            theme: 'sk-dot',
            message: "<h4>GUARDANDO INFORMACIÓN ...</h4>"
        });
    }


    function grdEvRecuperacionSoloLectura(){
        $('#grdEvRecuperacion').setColProp('bytNota', { editable: 'False' });
    }
})