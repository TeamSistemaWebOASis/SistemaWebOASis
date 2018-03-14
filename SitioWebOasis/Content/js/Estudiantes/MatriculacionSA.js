$(document).ready(function () {

    $('#btnMatriculacionSA').on('click', function () {
        var numCedula = $('#numIdentificacion').val();
        if (numCedula != '') {
            window.open('http://academicoseg.espoch.edu.ec/SilverlightDiscapacitadosWCF.Web/Loginwithout.aspx?cedula=' + numCedula, '_blank');
        } else {
            alert('USUARIO NO REGISTRADO');
            window.location('/Account/SignOut');
        }
    })

})