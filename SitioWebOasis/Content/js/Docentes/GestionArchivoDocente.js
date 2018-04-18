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
    //DESCARGA ACTAS COMPRIMIDAS DE PERIODOS ANTERIORES 
    $(document).on('click', '.btn-info', function () {
        showLoadingProcess();
        $.ajax({
            type: "POST",
            url: "/Docentes/descargaActasPeriodosAnteriores",
            data: '{idCarrera: "' + $('#cbCarrerasPA').val() + '", idPeriodoAcademico:"' + $('#cbPeriodosDPA').val() + '", idAsignatura:"' + $(this).parent().parent().parent().attr("id") + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).complete(function (data) {
            //  Cierro la ventana GIF Proceso
            HoldOn.close();
            if (data.responseJSON.fileName != "none" && data.responseJSON.fileName != "") {
                $.redirect("/Docentes/DownloadEvaluacionesFiles", { strNombreArchivos: data.responseJSON.fileName },
                                "POST")
            } else {
                $.alert({
                    title: 'Notificación',
                    content: 'Asignatura sin registros disponibles, favor consulte en secretaria de carrera'
                })
            }
        })

    });
    //HABRE DIALOGO PARA PRESENTAR ESTADISTICAS
    $(document).on('click', '.btn-success', function () {
        var codigoMateria = $(this).parent().parent().parent().attr("id");
        var idFila = $(this).parents("tr").find("td").eq(0).html();
        $.ajax({
            type: "POST",
            url: "/Docentes/MostrarEstadisticas/",
            data: '{strIdCarrera: "' + $('#cbCarrerasPA').val() + '", strIdPeriodoAcademico:"' + $('#cbPeriodosDPA').val() + '", strIdAsignatura:"' + $(this).parent().parent().parent().attr("id") + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).complete(function (data) {
            if (data.responseJSON.fileName != "none" && data.responseJSON.fileName != "") {
                var datos = JSON.parse(data.responseJSON.strEstadistica);
                var darPrincipal = datos.DarPrincipal;

                var aprobados = (datos.PrincipalAprovados) + (datos.SuspensionAprovados);
                var reprovados = (datos.PrincipalReprovados) + (datos.SuspensionReprovados);
                if (aprobados != 0 || reprovados != 0 || datos.Exonerados != 0 || datos.PerdidosFalta != 0 || datos.NotaBaja) {
                    $("#dialog,.ui-dialog-title").css("color", "#f3f3f3");
                    MostarDialogo();
                    google.charts.load('current', { 'packages': ['corechart'] });
                    google.charts.setOnLoadCallback(drawChart);
                    function drawChart() {
                        var data = google.visualization.arrayToDataTable([
                          ['acumulado', 'Porcentaje'],
                          ['Exonerados', datos.Exonerados],
                          ['Aprobados', aprobados],
                          ['Reprobados', reprovados],
                          ['ReprobadosFalta', datos.PerdidosFalta],
                          ['Reprobados(Nota<6)', datos.NotaBaja]
                        ]);
                        var materia = $('#asignaturasDocente tr:eq(' + idFila + ') td:eq(1)').text();
                        var options = {
                            title: materia + ' / ' + $('#cbCarrerasPA option:selected').text() + ' / ' + $('#cbPeriodosDPA option:selected').text(),
                            is3D: true,
                            colors: ['#4ba84b', '#6024d2', '#e60909', '#3f78a5', '#fc9826', '#87da1d']
                        };
                        var chart = new google.visualization.PieChart(document.getElementById('acumulado'));
                        chart.draw(data, options);
                    }
                } else {
                    $.alert({
                        title: 'Notificación',
                        content: 'Asignatura sin registros disponibles, favor consulte en secretaria de carrera'
                    })
                }
            } else {
                $.alert({
                    title: 'Notificación',
                    content: 'Asignatura sin registros disponibles, favor consulte en secretaria de carrera'
                })
            }
        })
    });
    //ABRE EL DIALOGO DE LA ESTADISTICAS
    function MostarDialogo() {
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
        }).prev("#dialog,.ui-dialog-titlebar").css("background", "#3f78a5");
    }
    //PROCESO GIF CARGANDO
    function showLoadingProcess() {
        HoldOn.open({
            theme: 'sk-bounce',
            message: "<h4>PROCESANDO ...</h4>"
        });
    }
    //LLENA COMBOBOX DE DE LOS PERIODOS ANTERIORES
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
    //CAMBIA EL COMBOBOX DE LOS PERIOSOS ANTERIORES
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
        });
    });
    //METODO PARA IMPRIMIR LOS HORARIO DE CLASES
    $('#btnExport_HC_PDF,#btnExport_HC_XLS').on("click", function (e) {
        showLoadingProcess();
        var tipoFile = (this.id == 'btnExport_HC_PDF') ? "PDF" : (this.id == 'btnExport_HC_XLS') ? "Excel" : " ";
        var horario = $('#cmbListaAsignaturaDocente').val();
        if (horario != "")//Verifico si desea imprimir todas las asignatura y concateno  al tipo de archivo
            tipoFile = tipoFile + "|" + horario;
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
                $.redirect("/Docentes/DownloadFile", { file: data.responseJSON.fileName },
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

    //SELECCIONA SOLO CARRERA
    $('#cmbListaAsignaturaDocente').change("click", function () {
        var dtaAsignatura = $(this).val();
        if (dtaAsignatura != "") {
            $.ajax({
                type: "POST",
                url: "/Docentes/HorarioDocenteAsignatura/",
                data: '{strCodMateria:"' + dtaAsignatura + '"}',
                contentType: "application/json; charset=utf-8",
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(thrownError);
                }
            }).success(function (data) {
                if (data != null) {
                    $("#ticket-table").find("tr:gt(0)").remove();
                    $('#ticket-table').html(data);
                }
            });
        } else {
            $.redirect("/Docentes/HorarioDocente/" + " ", "GET")
        }

    })
})