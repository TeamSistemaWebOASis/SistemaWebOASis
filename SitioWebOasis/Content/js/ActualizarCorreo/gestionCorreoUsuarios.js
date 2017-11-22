$(document).ready(function () {

    //$('#btnUpdCtaCorreo').on('click', function (event) {
    //    $.confirm({
    //        title: 'Confirmar',
    //        content: 'Compañero usuario, al actualizar la cuenta de correo usted .........',
    //        buttons: {
    //            confirm: function () {
    //                return true;
    //            },
    //            cancel: function () {
    //                return false;
    //            },
    //        }
    //    });
    //})

    $('#frmUpdCtaCorreo').submit(function (event) {
        
        $.confirm({
            title: 'Confirmar',
            content: 'Compañero usuario, al actualizar su cuenta de correo ' + $('#ctaMailAcceso').val(),
            buttons: {
                formSubmit: {
                    text: 'Acepto',
                    btnClass: 'btn-blue',
                    action: function () {
                        return true;
                    }
                },

                cancel: {
                    text: 'no',
                    action: function () {
                        $.alert({
                            title: 'DTIC - ESPOCH',
                            content: 'Cualquier duda, consulte en la secretaria de su carrera',
                        });

                        return false;
                    }
                },
            }
        });

    })




})