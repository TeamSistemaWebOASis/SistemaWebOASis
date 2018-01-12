$(document).ready(function () {
    $.jgrid.defaults.width = '100%';
    $.jgrid.defaults.responsive = true;
    $.jgrid.defaults.styleUI = 'Bootstrap';

    var lstEvaluacionFinal = new Array();
    var grdEvFinal = $("#grdEvFinal");

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
        url: 'data.json',
        editurl: 'clientArray',
        datatype: "jsonstring",
        colModel: [{ name: 'No', index: 'No', label: 'No', align: 'center', width: '30', sortable: false },
                    { name: "sintCodMatricula", key: true, hidden: true },
                    { name: "strCodigo", label: "Codigo", align: "center", width: "60", sortable: true, sorttype: "number" },
                    { name: 'NombreCompleto', label: "Nombre estudiante", width: '300', align: 'left', sortable: true },
                    { name: 'bytNumMat', label: 'Matrícula', width: '80', align: 'center', sortable: true, sorttype: "number" },
                    { name: 'bytAcumulado', label: 'Total acumulado', width: '120', align: 'center', sortable: true, sorttype: "number" },
                    { name: 'bytAsistencia', label: 'Total asistencia(%)', width: '120', align: 'center', sortable: true, sorttype: "number" },

                    { name: 'bytNota', label: 'Nota evaluación final', width: '120', align: 'center', sortable: true, sorttype: "number" },

                    { name: 'Total', label: 'Total Ev. final', width: '120', align: 'center', sortable: true, sorttype: "number" },
                    { name: 'efAcumulado', label: 'Estado', width: '120', align: 'center', sortable: true, sorttype: "string" },
                    { name: 'strObservaciones', label: 'Observación', width: '100%', align: 'center', sortable: true }],

        loadonce: true,
        datastr: $("#dtaJsonEvFinal").val(),
        autowidth: true,
        height: "auto",
        shrinkToFit: true,
        ignoreCase: true,
        rowNum: 100,
        loadComplete: function (data) {
            //  Resaltar contenido en columnas en la pagina
            updContenidoColumnasEvFinal();
        }
    });

    $("#grdEvFinal").jqGrid('setGroupHeaders', {
        useColSpanStyle: true,
        groupHeaders: [ { startColumnName: 'bytAcumulado', numberOfColumns: 2, titleText: '<b>TOTALIZADO EVALUACIÓN FORMATIVA</b>' },
                        { startColumnName: 'Total', numberOfColumns: 2, titleText: '<b>TOTALIZADO</b>' }]
    });


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

    $('#pEF_pdf, #pEF_xls, #pEF_blc').on('click', function () {
        showLoadingProcess();

        $.ajax({
            type: "POST",
            url: "/Docentes/impresionActas",
            data: '{idActa: "' + $(this).attr("id") + '", idAsignatura: "' + $('#ddlLstPeriodosEstudiante').val() + '"}',
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
    })


    function showLoadingProcess() {
        HoldOn.open({
            theme: 'sk-dot',
            message: "<h4>PROCESANDO INFORMACIÓN ...</h4>"
        });
    }

})