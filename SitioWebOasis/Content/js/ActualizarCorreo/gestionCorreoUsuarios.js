﻿$(document).ready(function () {

    $('#btnUpdCtaCorreo').on('click', function (event) {
        $.confirm({
            theme: 'bootstrap',
            title: 'Actualizar cuenta de correo',
            content: 'Compañero usuario la cuenta de correo ' + $('#ctaMailAcceso').val() + ' sera actualizada en el sistema académico la cual le va permitir acceder a los servicios que este sistema ofrece',
            buttons: {
                formSubmit: {
                    text: 'Acepto',
                    btnClass: 'btn-blue',
                    action: function () {
                        $.ajax({type: 'POST',
                            url: '/ActualizarCuentaCorreo/UpdCtaCorreo',
                            data: '{strNumCedula: "' + $('#strNumCedula').val() + '", ctaMailAcceso: "' + $('#ctaMailAcceso').val() + '"}',
                            contentType: "application/json; charset=utf-8",
                            dataType: 'json',
                            success: function (data) {
                                //  Cierro la ventana GIF Proceso
                                HoldOn.close();

                                if (data.ban == true) {
                                    $.confirm({
                                        theme: 'bootstrap',
                                        title: 'Actualizar cuenta de correo',
                                        content: 'Cuenta de correo actualizada correctamente, favor volver a ingresar al sistema académico',
                                        buttons:{
                                            confirm: {
                                                text: 'Aceptar',
                                                action: function () {
                                                    $.redirect("/Account/SignOut/", "POST")
                                                }
                                            }
                                        }
                                    })
                                } else {
                                    $('#msmUpdCtaCorreo').removeAttr("hidden");
                                    $('#msmUpdCtaCorreo').attr("class", "alert alert-danger alert-dismissable");
                                    $('#msmUpdCtaCorreo').html("<a href='#' class='close'>×</a><strong>" + data.mensaje + "</strong>");
                                }
                            }
                        });
                    }
                },

                cancel:{
                    text: 'no acepto',
                    action: function () {
                            $.alert('Si necesita mas informacion, favor consulte en secretaria de su carrera');
                        }
                }
            }
        });
    })

})