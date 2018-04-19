$(document).ready(function () {
    var popOverSettings = { container: 'body',
                            placement: 'left',
                            toggle: 'popover', 
                            html: true }

    $('#btnMatriculacionSA').on('click', function () {
        var numCedula = $('#numIdentificacion').val();
        if (numCedula != '') {
            window.open('http://academicoseg.espoch.edu.ec/SilverlightDiscapacitadosWCF.Web/Loginwithout.aspx?cedula=' + numCedula, '_blank');
        } else {
            alert('USUARIO NO REGISTRADO');
            window.location('/Account/SignOut');
        }
    })
    //IMPRIME CERTIFICADO DE MATRICULACION DEL ESTUDIANTE
    $('#btnImpresion,.btn btn-success').on('click', function () {
        var idPeriodo = $(this).parent().parent().parent().attr('id');
        showLoadingProcess();
        $.ajax({
            type: "POST",
            url: "/Estudiantes/ImpresionArchivoMatricula/",
            data: '{ strCodPeriodo: "' + idPeriodo + '" }',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (xhr, ajaxOptions, thrownError) {
                alert(thrownError);
            }
        }).success(function (data) {
            if (data.fileName != "") {
                //  Oculto el mensaje de error
                $('#msmLstAsignaturas').attr("hidden");
                $.redirect("/Estudiantes/DownloadFile",
                            { file: data.fileName },
                            "POST")
            } else {
                //  Mostrar mensaje de estado de la transaccion
                getMensajeTransaccion(false, data.MessageGestion);
            }

            //  Cierro la ventana GIF Proceso
            HoldOn.close();
        })
        
    })

    function showLoadingProcess() {
        HoldOn.open({
            theme: 'sk-bounce',
            message: "<h4>GENERANDO ARCHIVO ...</h4>"
        });
    }
    function getMensajeTransaccion(banEstado, mensaje) {
        //  Muestro mensaje de gestion de informacion
        $('#messageError').removeAttr("hidden");

        if (banEstado == false) {
            $('#messageError').attr("class", "alert alert-danger fade in");
            $('#messageError').html("<button class='close' data-dismiss='alert'>×</button> <i class='fa fa-exclamation-circle' aria-hidden='true'></i> <strong>" + mensaje + "</strong>");
        }
    }

})