$(document).ready(function () {

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