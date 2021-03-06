﻿$(document).ready(function () {
    $('#strNumCedula').keypress(function(e) {
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    })

    $('#btnValidarCedula').on('click', function () {
        showLoadingProcess();
        $.redirect("/ActualizarCuentaCorreo/ValidarNumeroCedula",
                    { strNumCedula: $('#strNumCedula').val() },
                    "POST");
    })

    function showLoadingProcess() {
        HoldOn.open({
            theme: 'sk-dot',
            message: "<h4>VALIDANDO USUARIO...</h4>"
        });
    }
})