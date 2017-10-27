$(document).ready(function () {
    $('.btn-danger, .btn-success').on('click', function () {
        var strCodAsignatura = $(this).parent().parent().parent().attr('id');
        var strCodTipoArchivo = $(this).context.innerText;
        showLoadingProcess();

        $.ajax({
            type: "POST",
            url: "/Docentes/impresionNominaEstudiantes/" + strCodAsignatura + "/" + strCodTipoArchivo,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (xhr, ajaxOptions, thrownError) {
                alert(thrownError);
            }
        }).success(function (data) {
            if (data.fileName != "") {
                //  Oculto el mensaje de error
                $('#msmLstAsignaturas').attr("hidden");

                $.redirect( "/Docentes/DownloadFile",
                            { file: data.fileName },
                            "POST" )
            } else {
                //  Mostrar mensaje de estado de la transaccion
                getMensajeTransaccion(false, data.MessageGestion);
            }

            //  Cierro la ventana GIF Proceso
            HoldOn.close();
        })


        function showLoadingProcess() {
            HoldOn.open({
                theme: 'sk-dot',
                message: "<h4>GENERANDO ARCHIVO ...</h4>"
            });
        }


        function getMensajeTransaccion(banEstado, mensaje) {
            //  Muestro mensaje de gestion de informacion
            $('#msmLstAsignaturas').removeAttr("hidden");

            if (banEstado == false) {
                $('#msmLstAsignaturas').attr("class", "alert alert-danger fade in");
                $('#msmLstAsignaturas').html("<button class='close' data-dismiss='alert'>×</button> <i class='fa fa-exclamation-circle' aria-hidden='true'></i> <strong>" + mensaje + "</strong>");
            }
        }

    })
})