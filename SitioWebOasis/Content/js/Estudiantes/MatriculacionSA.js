$(document).ready(function () {
    var popOverSettings = {
        container: 'body',
        placement: 'left',
        toggle: 'popover',
        html: true,
        trigger: "hover"
    }
    $("button[rel=popover]").popover(popOverSettings);

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
    //MUESTRA ESTADISTICAS DEL ESTUDIANTE
    $('#btnEstadistica,#btnEstadisticaPA').on('click', function () {
        var idPeriodo = ($(this).attr("id") == "btnEstadistica") ? "" : $('#ddlLstPeriodosEstudiante option:selected').val();
        $.ajax({
            type: "POST",
            url: "/Estudiantes/MostrarEstadisticas/",
            data: '{ strCodPeriodoA: "' + idPeriodo + '" }',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (xhr, ajaxOptions, thrownError) {
                alert(thrownError);
            }
        }).success(function (data) {
            if (data.strEstadistica.length != 0) {
                //Load Charts and the corechart and barchart packages.
                google.charts.load('current', { 'packages': ['corechart'] });
                //Draw the pie chart and bar chart when Charts is loaded.
                google.charts.setOnLoadCallback(drawChart);
                var dataArray = [['Nombre',
                                    'Parcial 1', { role: 'annotation' },
                                    'Parcial 2', { role: 'annotation' },
                                    'Parcial 3', { role: 'annotation' },
                                    'Evaluación Final', { role: 'annotation' },
                                    'Evaluación Recuperación', { role: 'annotation' },
                                    'Equivalencia', { role: 'annotation' }]];

                for (i in JSON.constructor(data).strEstadistica) {
                    
                    if (JSON.parse(JSON.constructor(data).strEstadistica[i]).Parcial1 != 0) {
                        MostrarDialogo();
                        dataArray.push([JSON.parse(JSON.constructor(data).strEstadistica[i]).Materia,
                                        parseInt(JSON.parse(JSON.constructor(data).strEstadistica[i]).Parcial1), JSON.parse(JSON.constructor(data).strEstadistica[i]).Parcial1,
                                        parseInt(JSON.parse(JSON.constructor(data).strEstadistica[i]).Parcial2), JSON.parse(JSON.constructor(data).strEstadistica[i]).Parcial2,
                                        parseInt(JSON.parse(JSON.constructor(data).strEstadistica[i]).Parcial3), JSON.parse(JSON.constructor(data).strEstadistica[i]).Parcial3,
                                        parseInt(JSON.parse(JSON.constructor(data).strEstadistica[i]).ExamenPrincipal), JSON.parse(JSON.constructor(data).strEstadistica[i]).ExamenPrincipal,
                                        parseInt(JSON.parse(JSON.constructor(data).strEstadistica[i]).ExamenSuspension), (JSON.parse(JSON.constructor(data).strEstadistica[i]).ExamenSuspension) == 0 ? '' : (JSON.parse(JSON.constructor(data).strEstadistica[i]).ExamenSuspension),
                                        0, JSON.parse(JSON.constructor(data).strEstadistica[i]).Equivalencia])
                    } else {
                        $.alert({
                            title: 'Notificación',
                            content: 'Periodo Académico sin registros disponibles, favor consulte en secretaria de carrera.'
                        })
                    }
                }
                
                function drawChart() {
                    var data = new google.visualization.arrayToDataTable(dataArray);
                    var options = {
                        title: ($('#txtPeriodoA').text() == "") ? 'EVALUACIONES DEL PERIODO ACADÉMICO:' + " " + $('#ddlLstPeriodosEstudiante option:selected').text() : 'EVALUACIONES DEL' + $('#txtPeriodoA').text().toUpperCase(),
                        width: 920,
                        height: 400,
                        legend: { position: 'top', maxLines: 3 },
                        bar: { groupWidth: '75%' },
                        isStacked: true,
                        hAxis: {
                            title: 'Notas',
                            minValue: 0,
                            ticks: [0, 8, 18, 28, 40]
                        },
                        vAxis: { title: 'Materias' },
                        colors: ['#337AB7', '#337AB7', '#337AB7', '#F56421', '#777', '#000']
                    };
                    var barchart = new google.visualization.BarChart(document.getElementById('consolidado'));
                    barchart.draw(data, options);
                }
            } else {
                $.alert({
                    title: 'Notificación',
                    content: 'Periodo Académico sin registros disponibles, favor consulte en secretaria de carrera.'
                })
            }
        })
    })
    //ABRE EL DIALOGO DE LA ESTADISTICAS
    function MostrarDialogo() {
        $("#dialog").dialog({
            title: "-- COMPORTAMIENTO ACADÉMICO --",
            position: { my: "center", att: "center" },
            hide: "puff",
            width: "auto",
            buttons: { "Cerrar": function () { $(this).dialog("close"); } },
            modal: true,
            resizable: false,
            show: {
                effect: "blind",
                duration: 800
            },
            hide: {
                effect: "explode",
                duration: 800
            }
        }).prev("#dialog,.ui-dialog-titlebar").css("background", "#6FBC85");
    }
})