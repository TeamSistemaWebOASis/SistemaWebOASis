$(document).ready(function () {
    $('#ddlLstPeriodosEstudiante').change("click", function () {
        var dtaAsignatura = $(this).val().split("|");

        HoldOn.open({
            theme: 'sk-dot',
            message: "<h4>Cargando ...</h4>"
        });

        $.redirect( "/Docentes/EvaluacionAsignatura/" + dtaAsignatura["1"] + "/" + dtaAsignatura["0"] + "/" + dtaAsignatura["2"],
                    "GET")
    })
})