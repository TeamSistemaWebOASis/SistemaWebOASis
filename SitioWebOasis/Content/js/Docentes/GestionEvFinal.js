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
    

    //  Objeto con informacion de dtaEvAcumulacionFinal
    cargarDatosEvFinal($('#dtaJsonEvFinal').val());


    function cargarDatosEvFinal(dtaEvFinal) {
        var dtaEvaluacionFinal = eval(dtaEvFinal);
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
        url: 'Docentes/',
        datatype: "json",
        colModel: [ { name: 'No', index: 'No', label: 'No', align: 'center', width:'30', sortable: false },
                    { name: 'strCodigo', key: true, hidden: true },
                    { name: 'NombreCompleto', label: "Nombre estudiante", width: '300', align: 'left', sortable: false },
                    { name: 'bytNumMat', label: 'Matrícula', width: '80', align: 'center', sortable: false },
                    { name: 'bytAcumulado', label: 'Total acumulado', width: '120', align: 'center', sortable: false },
                    { name: 'bytAsistencia', label: 'Total asistencia(%)', width: '120', align: 'center', sortable: false },

                    { name: 'bytNota', label: 'Nota evaluación final', width: '120', align: 'center', editable: true, edittype: 'text', editoptions: { size: 1, maxlength: 2, dataInit: soloNumero }, editrules: { custom: true, custom_func: validarNotaEvFinal }, sortable: false, formatter: { integer: { thousandsSeparator: " ", defaultValue: '0' } } },

                    { name: 'Total', label: 'Total Ev. final', width: '120', align: 'center', sortable: false },
                    { name: 'efAcumulado', label: 'Estado', width: '120', align: 'center' },
                    { name: 'strObservaciones', label: 'Observación', width: '170', align: 'center', sortable: false }],
        datatype: "jsonstring",
        datastr: $("#dtaJsonEvFinal").val(),
        viewrecords: true,
        height: "100%",
        ignoreCase: true,
        rowNum: 100,
        onSelectRow: function (id, status, e) {
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
                    aftersavefunc: function (id) {
                        //  Registro la informacion gestionada en el JSON
                        guardarDtaEvaluacionFinal(id);

                        //  Actualizo contenido de la fila
                        updDtaEvaluacionFinal(id);

                        //  Obtengo el identificador de la siguiente registro de notas a gestionar
                        var idNextRow = getIdNextRow(id);
                        if (id != idNextRow) {
                            $('#grdEvFinal').jqGrid('setSelection', idNextRow, true);
                        }
                    }
                });

                lastsel = id;
            } else {
                //  Si el registro del estudiante es de EXONERADO - REPROBADO
                $('#grdEvFinal').jqGrid('setSelection', id, false);
            }
        },

        loadComplete: function (data) {
            //  Resaltar contenido en columnas en la pagina
            updContenidoColumnasGrid();
        }
    });


    function editarFila(id)
    {
        var ban = true;

        numReg = lstEvaluacionFinal.length;
        for (var x = 0; x < numReg; x++) {
            if (lstEvaluacionFinal[x].strCodigo == id) {
                ban = lstEvaluacionFinal[x].esExoneradoReprobado();
            }
        }

        return ban;
    }


    $("#grdEvFinal").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [ { startColumnName: 'bytAcumulado', numberOfColumns: 2, titleText: '<b>TOTALIZADO EVALUACIÓN FORMATIVA</b>' },
                        { startColumnName: 'Total', numberOfColumns: 2, titleText: '<b>TOTALIZADO</b>' }]
    });


    function guardarDtaEvaluacionFinal(id) {
        var ban = false;
        var numReg = lstEvaluacionFinal.length;
        for (var x = 0; x < numReg; x++) {
            if (lstEvaluacionFinal[x].strCodigo == id) {
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
        rowIds = $('#grdEvFinal').jqGrid('getDataIDs');

        for (i = 0; i <= rowIds.length - 1 ; i++) {
            //  En funcion al parcial activo resalto el color de la columna 
            $("#grdEvFinal").jqGrid('setCell',
                                    rowIds[i],
                                    'bytNota',
                                    "",
                                    { 'background-color': '#fcf8e3' });

            $("#grdEvFinal").jqGrid('setRowData', rowIds[i], { bytNumMat: lstEvaluacionFinal[i].getNumMatricula() });
            $("#grdEvFinal").jqGrid('setRowData', rowIds[i], { efAcumulado: lstEvaluacionFinal[i].getEstadoEvaluacionFinal() });
        }
    }


    function updDtaEvaluacionFinal(id) {
        numReg = lstEvaluacionFinal.length;
        for (var x = 0; x < numReg; x++) {
            if (lstEvaluacionFinal[x].strCodigo == id) {
                $("#grdEvFinal").jqGrid('setRowData', id, { Total: lstEvaluacionFinal[x].getTotalEvFinal() });
                $("#grdEvFinal").jqGrid('setRowData', id, { efAcumulado: lstEvaluacionFinal[x].getEstadoEvaluacionFinal() });

                break;
            }
        }

        $("#grdEvFinal").jqGrid(    'setCell',
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


    function showLoadingProcess() {
        HoldOn.open({
            theme: 'sk-dot',
            message: "<h4>GUARDANDO INFORMACION ...</h4>"
        });
    }


    function validarNotaEvFinal(value, colname) {
        if (value < 0 || value > 12)
            return [false, "Nota fuera de rango (0, 10)"];
        else
            return [true, ""];
    }

    
    $('#btnGuardarEvFinal').on('click', function(){
        //  Muestro mensaje de proceso
        showLoadingProcess();

        //  Verifico si el Grid de notas a cambiado
        if (blnCambiosEvFinal == true){
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
                //  Muestro mensaje de gestion de informacion
                $('#msmGrdEvFinal').removeAttr("hidden");

                //  Actualizo el grid con la informacion gestionada a nivel BD
                cargarDatosEvFinal(data);

                //  Actualizo el grid de notas
                updContenidoColumnasGrid();

                //  Regreso a su valor original la variable de control de cambios en el grid de notas
                blnCambiosEvFinal = false;

                //  Cierro la ventana GIF Proceso
                HoldOn.close();
            })
        } else {
            //  Cierro la ventana GIF Proceso
            HoldOn.close();
        }
    })


    //  Control de impresion de actas
    $('#pEF_pdf, #pEF_xls, #pEF_blc').on('click', function () {
        $("#opImpEvFinal").val($(this).attr("id"));

        //  Muestro ventana de autenticacion a dos factores
        $.blockUI({ message: $('#loginForm') });
    })


    $('#btnValidarImprimir').click(function () {
        if ($('#dtaNumConfirmacion').val() == "987") {
            $.unblockUI();
            showLoadingProcess();
            var opImpresion = $("#opImpEvFinal").val();

            //  Ejecuto el proceso de registro de informacion
            $('#btnGuardarEvFinal').trigger("click");

            $.ajax({
                type: "POST",
                url: "/Docentes/impresionActas",
                data: '{idActa: "' + opImpresion + '", idAsignatura: "' + $('#ddlLstPeriodosEstudiante').val() + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            }).complete(function (data) {
                //  Cambio el color del boton
                $('#btnEF, #btnEFF').attr("class", 'btn btn-warning btn-md');

                //  Oculto el mensaje de error
                $('#messageError').attr("hidden");

                $("opImpEvFinal").attr("value", "");

                if (data.responseJSON.fileName != "none" && data.responseJSON.fileName != "") {
                    //  Cambio el Grid a modo "soloLectura"
                    grdEvFinalSoloLectura();

                    $.redirect("/Docentes/DownloadFile",
                                { file: data.responseJSON.fileName },
                                    "POST")
                } else {
                    //  Si existe error, muestro el mensaje
                    $('#messageError').removeAttr("hidden");
                    $('#messageError').html("<a href='' class='close'>×</a><strong>FALLO !!!</strong> Favor vuelva a intentarlo");
                }

                //  Cierro la ventana GIF Proceso
                HoldOn.close();
            })
        } else {
            alert('NUMERO DE CONFIRMACION NO VALIDO, favor vuelva a ingresarlo');
        }
    })



    function grdEvFinalSoloLectura()
    {
        rowIds = $('#grdEvFinal').jqGrid('getDataIDs');

        for (i = 0; i <= rowIds.length - 1 ; i++) {
            var rowEF = $("#grdEvFinal").jqGrid('getRowData',
                                                rowIds[i]);

            rowEF.setColProp('Arrived', { editable: false });
        }
    }


})