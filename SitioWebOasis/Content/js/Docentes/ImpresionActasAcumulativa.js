$(document).ready(function () {
    $.jgrid.defaults.width = '100%';
    $.jgrid.defaults.responsive = true;
    $.jgrid.defaults.styleUI = 'Bootstrap';

    var lstEvaluaciones = new Array();
    var grdEvAcumulativa = $('#dtaJsonEvAcumulativa').val();
    var grid = $("#grdEvAcumulativa");
    var parcialActivo = "bytNota" + $("#dtaParcialActivo").val();
    
    //  Objeto con informacion de dtaEvAcumulada
    cargarDatosEvAcumulativa($('#dtaJsonEvAcumulativa').val());


    function cargarDatosEvAcumulativa(dtaEvAcumulada) {
        var dtaEvaluaciones = eval(dtaEvAcumulada);

        if (dtaEvaluaciones.length > 0) {
            var nee = dtaEvaluaciones.length;
            for (var x = 0; x < nee; x++) {
                var objEvaluaciones = new EvaluacionAcumulativa();
                objEvaluaciones.setDtaEvaluacion(dtaEvaluaciones[x],
                                                    $("#dtaParcialActivo").val());

                lstEvaluaciones.push(objEvaluaciones);
            }
        }
    }


    //  Gestion de notas 
    grid.jqGrid({
        url: 'data.json',
        editurl: 'clientArray',
        datatype: "jsonstring",
        colModel: [ { name: "No", index: "No", label: "No", align: "center", width: "30", sortable: true },
			        { name: "sintCodMatricula", key: true, hidden: true },
                    { name: "strCodigo", label: "Codigo", align: "center", width: "60", sortable: true, sorttype: "number", sorttype: "number" },
			        { name: "NombreEstudiante", label: "Nombre estudiante", align: "left", width: "300", sortable: true },
			        { name: "bytNumMat", label: "Matricula", align: "center", width: "60", sortable: true, sorttype: "number" },

			        { name: "bytNota1", index: "bytNota1", label: "Nota uno (1)", align: "center", width: "90", sortable: true, sorttype: "number" },
                    { name: "bytNota2", index: "bytNota2", label: "Nota dos (2)", align: "center", width: "90", sortable: true, sorttype: "number" },
                    { name: "bytNota3", index: "bytNota3", label: "Nota tres (3)", align: "center", width: "90", sortable: true, sorttype: "number" },

			        { name: "Total", label: "Total", align: "center", width: "60", sortable: true, sorttype: "number" },
			        { name: "bytAsistencia", label: "Asistencia (%)", align: "center", width: "100", sortable: true, sorttype: "number" },
			        { name: "ucAcumulado", label: "Estado", width: "125", align: "center" },
			        { name: "strObservaciones", label: "Observación", align: "left", width: "100", sortable: false }],

        loadonce: true,
        datastr: $("#dtaJsonEvAcumulativa").val(),
        autowidth: true,
        height:"auto",
        shrinkToFit: true,
        ignoreCase: true,
        rowNum: 100,
        loadComplete: function (data) {
            $(this).jqGrid('setGridParam', 'rowNum', data.length);

            //  Resaltar contenido en columnas en la pagina
            updContenidoColumnasGrid();
        }
    });


    function updContenidoColumnasGrid() {
        var rowIds = $('#grdEvAcumulativa').jqGrid('getDataIDs');
        var DataIds = $('#grdEvAcumulativa').jqGrid('getRowData');
        var numRegEA = lstEvaluaciones.length;

        for (i = 0; i < rowIds.length; i++) {
            for (j = 0; j < numRegEA; j++) {
                if (rowIds[i] == lstEvaluaciones[j].sintCodMatricula) {
                    //  En funcion al parcial activo resalto el color de la columna 
                    $("#grdEvAcumulativa").jqGrid(  'setCell',
                                                    rowIds[i],
                                                    'bytNota',
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


    //  Control de impresion de actas
    $('#pEA_pdf, #pEA_xls, #pEA_blc').on('click', function () {
        showLoadingProcess();
        
        $.ajax({
            type: "POST",
            url: "/Docentes/impresionActas",
            data: '{idActa: "' + $(this).attr("id") + '", idAsignatura: "' + $('#ddlLstPeriodosEstudiante').val() + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).complete(function (data) {
            //  Cambio el color del boton
            $('#btnEA, #btnEAF').attr("class", 'btn btn-warning btn-md');

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
                $('#messageError').html("<a href='' class='close'>×</a><strong>FALLO !!!</strong> Favor vuelva a intentarlo");
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