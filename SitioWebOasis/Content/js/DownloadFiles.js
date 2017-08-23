$(document).ready(function () {

    //  Exportar horario academico en formato PDF
    $('#btnExport_HA_PDF').click(function () {
        //  Muestra GIF Proceso
        showLoadingProcess();

        //  Crea y Descarga el archivo
        CreateDownloadFileHorarios( "createFileHorarioAcademico", "PDF" );
    })

    //  Exportar horario academico en formato EXCEL
    $('#btnExport_HA_XLS').click(function () {
        //  Muestra GIF Proceso
        showLoadingProcess();

        //  Crea y Descarga el archivo
        CreateDownloadFileHorarios( "createFileHorarioAcademico", "Excel" );
    })


    //  Exportar horario de examenes a formato PDF
    $('#btnExport_HE_PDF').click(function () {
        //  Muestra GIF Proceso
        showLoadingProcess();

        //  Crea y Descarga el archivo
        CreateDownloadFileHorarios( "createFileHorarioExamenes", "PDF" );
    })

    //  Exportar horario de examenes en formato Excel
    $('#btnExport_HE_XLS').click(function () {
        //  Muestra GIF Proceso
        showLoadingProcess();

        //  Crea y Descarga el archivo
        CreateDownloadFileHorarios("createFileHorarioExamenes", "Excel");
    })


    function showLoadingProcess()
    {
        HoldOn.open({   theme: 'sk-dot',
                        message: "<h4>Cargando ...</h4>" });
    }


    function CreateDownloadFileHorarios( typeAction, idTypeFile) {
        $.ajax({    type: "POST",
                    url: "/Estudiantes/" + typeAction,
                    data: (typeAction == 'createFileHorarioAcademico')  ? '{ idCurso: "' + $('#ddlLstCursoParalelo').val() + '", idTypeFile: "' + idTypeFile + '" }'
                                                                        : '{ idTypeFile: "' + idTypeFile + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json"
        }).complete(function (data) {
            //  Oculto el mensaje de error
            $('#messageError').attr("hidden");

            //  Cierro la ventana GIF Proceso
            HoldOn.close();

            if (data.responseJSON.fileName != "none" && data.responseJSON.fileName != "") {
                $.redirect( "DownloadFile",
                            {   file: data.responseJSON.fileName },
                                "POST")
            } else {
                //  Si existe error, muestro el mensaje
                $('#messageError').removeAttr("hidden");
                $('#messageError').html("<a href='' class='close'>×</a><strong>FALLO !!!</strong> Favor vuelva a intentarlo");
            }
        })
    }

})