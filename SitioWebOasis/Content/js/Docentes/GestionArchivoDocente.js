$(document).ready(function () {
    // Hacemos la lógica que cuando nuestro SELECT cambia de valor 
    $("#cbCarrerasPA").change(function () {
        var codCarrera = $(this).val();
        //Desbloqueamos el Selecect de los periodos
        $("#cbPeriodosDPA").prop('enable', true);
        $.ajax({
            type: "POST",
            url: "/Docentes/GetPeriodosCarreraDocente/",
            data: '{ strCodCarrera: "' + codCarrera + '" }',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (xhr, ajaxOptions, thrownError) {
                alert(thrownError);
            }
        }).success(function (data) {
            if (data != "") {
                llenarComboBoxPA("#cbPeriodosDPA", data);
            }
        })
    });
    $(document).on('click', '#asignaturasDocente tr', function () {
        showLoadingProcess();
        $.ajax({
            type: "POST",
            url: "/Docentes/descargaActasPeriodosAnteriores",
            data: '{idCarrera: "' + $('#cbCarrerasPA').val() + '", idPeriodoAcademico:"' + $('#cbPeriodosDPA').val() + '", idAsignatura:"' + $(this).attr('id') + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).complete(function (data) {
            //  Cierro la ventana GIF Proceso
            HoldOn.close();
            if (data.responseJSON.fileName != "none" && data.responseJSON.fileName != "") {
                $.redirect("/Docentes/DownloadEvaluacionesFiles")
            } else {
                //  Si existe error, muestro el mensaje
                $('#messageError').removeAttr("hidden");
                $('#messageError').html("<a href='' class='close'>×</a><strong>FALLO !!!</strong> Favor vuelva a intentarlo");
            }
        })

    });
    function showLoadingProcess() {
        HoldOn.open({
            theme: 'sk-bounce',
            message: "<h4>PROCESANDO ...</h4>"
        });
    }
    function llenarComboBoxPA(dropDownId, list) {
        if (list != null && list.length > 0) {
            $(dropDownId).empty();
            $(dropDownId).removeAttr("disabled");
            $(dropDownId).append('<option value="">-- SELECCIONE UN PERIODO --</option>');
            $.each(list, function () {
                $(dropDownId).append($("<option></option>").val(this['Value']).html(this['Text']));
            });
        }
    }
    $("#cbPeriodosDPA").change(function () {
        var codPeriodo = $("#cbPeriodosDPA").val();
        var codCarrera = $("#cbCarrerasPA").val();
        $.ajax({
            type: "POST",
            url: "/Docentes/GetMateriasDocentePA/",
            data: '{ strCodCarrera: "' + codCarrera + '" , strCodPeriodo: "' + codPeriodo + '"}',
            contentType: "application/json; charset=utf-8",
            error: function (xhr, ajaxOptions, thrownError) {
                alert(thrownError);
            }
        }).success(function (data) {
            if (data != null) {
                $("#asignaturasDocente").find("tr:gt(0)").remove();
                $('#asignaturasDocente').append(data)
            }
        })

    });
    //METODO PARA IMPRIMIR LOS HORARIO DE CLASES
    $('#btnExport_HC_PDF,#btnExport_HC_XLS').on("click", function (e) {
        showLoadingProcess();
        var tipoFile = (this.id == 'btnExport_HC_PDF') ? "PDF" : (this.id == 'btnExport_HC_XLS') ? "Excel" : " ";
        $.ajax({
            type: "POST",
            url: "/Docentes/CrearFileHorarioAcademico/",
            data: '{idTypeFile:"' + tipoFile + '",strHorario:"Clase"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (xhr, ajaxOptions, thrownError) {
                alert(thrownError);
            }
        }).complete(function (data) {
            //  Cierro la ventana GIF Proceso
            HoldOn.close();
            if (data.responseJSON.fileName != "none" && data.responseJSON.fileName != "") {
                $.redirect("/Docentes/DownloadFile",{ file: data.responseJSON.fileName },
                                "POST")
            } else {
                //  Si existe error, muestro el mensaje
                $('#messageError').removeAttr("hidden");
                $('#messageError').html("<a href='' class='close'>×</a><strong>FALLO !!!</strong> Favor vuelva a intentarlo");
            }
        })
        
    });
    //METODO PARA IMPRIMIR LOS HORARIO DE EXAMENES 
    $('#btnExport_HE_PDF,#btnExport_HE_XLS').on("click", function (e) {
        showLoadingProcess();
        var tipoFile = (this.id == 'btnExport_HE_PDF') ? "PDF" : (this.id == 'btnExport_HE_XLS') ? "Excel" : " ";
        $.ajax({
            type: "POST",
            url: "/Docentes/CrearFileHorarioAcademico/",
            data: '{idTypeFile:"' + tipoFile + '",strHorario:"Examen"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (xhr, ajaxOptions, thrownError) {
                alert(thrownError);
            }
        }).complete(function (data) {
            //  Cierro la ventana GIF Proceso
            HoldOn.close();
            if (data.responseJSON.fileName != "none" && data.responseJSON.fileName != "") {
                $.redirect("/Docentes/DownloadFile", { file: data.responseJSON.fileName },
                                "POST")
            } else {
                //  Si existe error, muestro el mensaje
                $('#messageError').removeAttr("hidden");
                $('#messageError').html("<a href='' class='close'>×</a><strong>FALLO !!!</strong> Favor vuelva a intentarlo");
            }
        })

    });
   
})