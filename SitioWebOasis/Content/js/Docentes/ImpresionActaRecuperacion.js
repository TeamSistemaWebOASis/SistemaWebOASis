$(document).ready(function () {
    $.jgrid.defaults.width = '100%';
    $.jgrid.defaults.responsive = true;
    $.jgrid.defaults.styleUI = 'Bootstrap';

    var dtaEvRecuperacion = $('#dtaJsonEvRecuperacion').val();
    var grdEvRecuperacion = $("#grdEvRecuperacion");
    var lstEvaluacionRecuperacion = new Array();

    //  Objeto con informacion de dtaEvRecuperacion
    cargarDatosEvRecuperacion($('#dtaJsonEvRecuperacion').val());

    function cargarDatosEvRecuperacion(dtaEvRecuperacion) {
        var dtaEvaluacionRecuperacion = (dtaEvRecuperacion.length != 0)
                                            ? eval(dtaEvRecuperacion)
                                            : new Array();

        if (dtaEvaluacionRecuperacion.length > 0) {
            var nef = dtaEvaluacionRecuperacion.length;
            for (var x = 0; x < nef; x++) {
                var objEvaluacionRecuperacion = new EvaluacionRecuperacion();
                objEvaluacionRecuperacion.setDtaEvaluacionRecuperacion(dtaEvaluacionRecuperacion[x]);
                lstEvaluacionRecuperacion.push(objEvaluacionRecuperacion);
            }
        }
    }

    grdEvRecuperacion.jqGrid({
        url: 'data.json',
        editurl: 'clientArray',
        datatype: "jsonstring",
        colModel: [{ name: 'No', index: 'No', label: 'No', align: 'center', width: '30', sortable: false },
                    { name: "sintCodMatricula", key: true, hidden: true },
                    { name: 'strCodigo', label: "Codigo", align: "center", width: "60", sortable: true },
                    { name: 'NombreCompleto', label: "Nombre estudiante", width: '300', align: 'left', sortable: true },
                    { name: 'bytNumMat', label: 'Matrícula', width: '80', align: 'center', sortable: true },
                    { name: 'bytAcumulado', label: 'Total evaluación formativa', width: '150', align: 'center', sortable: false },

                    { name: 'bytNota', label: 'Nota evaluación recuperación', width: '170', align: 'center', sortable: true },

                    { name: 'Total', label: 'Total', width: '120', align: 'center', sortable: true },
                    { name: 'erAcumulado', label: 'Estado', width: '120', align: 'center', sortable: true },
                    { name: 'strObservaciones', label: 'Observación', width: '170', align: 'center', sortable: false }],

        loadonce: true,
        datastr: $("#dtaJsonEvRecuperacion").val(),
        autowidth: true,
        height: "auto",
        shrinkToFit: true,
        ignoreCase: true,
        rowNum: 100,

        loadComplete: function (data) {
            //  Resaltar contenido en columnas en la pagina
            updContenidoColumnasGrid();
        }
    });


    $("#grdEvRecuperacion").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [{ startColumnName: 'Total', numberOfColumns: 2, titleText: '<b>TOTALIZADO</b>' }]
    });


    function updContenidoColumnasGrid() {
        var rowIds = $('#grdEvRecuperacion').jqGrid('getDataIDs');
        var DataIds = $('#grdEvRecuperacion').jqGrid('getRowData');
        var numRegEF = lstEvaluacionRecuperacion.length;

        for (i = 0; i < rowIds.length; i++) {
            for (j = 0; j < numRegEF; j++) {
                if (rowIds[i] == lstEvaluacionRecuperacion[j].sintCodMatricula) {
                    //  En funcion al parcial activo resalto el color de la columna 
                    $("#grdEvRecuperacion").jqGrid('setCell',
                                            rowIds[i],
                                            'bytNota',
                                            "",
                                            { 'background-color': '#fcf8e3' });

                    $("#grdEvRecuperacion").jqGrid('setRowData', rowIds[i], { bytNumMat: lstEvaluacionRecuperacion[j].getNumMatricula() });
                    $("#grdEvRecuperacion").jqGrid('setRowData', rowIds[i], { erAcumulado: lstEvaluacionRecuperacion[j].getEstadoEvaluacionRecuperacion() });

                    j = numRegEF;
                }
            }
        }
    }


    $('#pER_pdf, #pER_xls, #pER_blc').on('click', function () {
        showLoadingProcess();
        imprimirActaEvRecuperacion($(this).attr("id"))
    })


    function imprimirActaEvRecuperacion( idActa ) {
        showLoadingProcess();

        $.ajax({
            type: "POST",
            url: "/Docentes/impresionActas",
            data: '{idActa: "' + idActa + '", idAsignatura: "' + $('#ddlLstPeriodosEstudiante').val() + '"}',
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


    function showLoadingProcess() {
        HoldOn.open({
            theme: 'sk-dot',
            message: "<h4>PROCESANDO INFORMACIÓN ...</h4>"
        });
    }

})