$(document).ready(function () {
    //  Mascaras al los campos Telefono Celular / Telefono Fijo del formulario de estudiantes
    $('#txtTelefonoCelular').mask('(999) 999-9999');
    $('#txtTelefonoFijo').mask('(999) 999-999');

    $("#frmDtaPersonalEstudiante").validate({
        rules: {
            ddlEstadoCivil: { aFunction: true },
            ddlSexo: { aFunction: true },
            ddlEtnia: { aFunction: true },
            ddlTipoSangre: { aFunction: true },
            ddlGenero: { aFunction: true },
            ddl_FNPais: { aFunction: true },
            ddl_FNProvincias: { aFunction: true },
            ddl_FNCiudades: { aFunction: true },
            ddl_FNParroquias: { aFunction: true },
            ddl_DUPais: { aFunction: true },
            ddl_DUProvincias: { aFunction: true },
            ddl_DUCiudades: { aFunction: true },
            ddl_DUParroquias: { aFunction: true },
            txtDirCallePrincipal: "required",
            txtDirNumeroCasa: "required",
            txtDirCalleSecundaria: "required",
            txtDirReferencia: "required",
            txtTelefonoCelular: "required",
            txtCorreoAlternativo: "required email"
        },

        messages: {
            ddlEstadoCivil: "Campo requerido",
            ddlSexo: "Campo requerido",
            ddlEtnia: "Campo requerido",
            ddlTipoSangre: "Campo requerido",
            ddlGenero: "Campo requerido",
            ddl_FNPais: "Campo requerido",
            ddl_FNProvincias: "Campo requerido",
            ddl_FNCiudades: "Campo requerido",
            ddl_FNParroquias: "Campo requerido",
            ddl_DUPais: "Campo requerido",
            ddl_DUProvincias: "Campo requerido",
            ddl_DUCiudades: "Campo requerido",
            ddl_DUParroquias: "Campo requerido",
            txtDirCallePrincipal: "Campo requerido",
            txtDirNumeroCasa: "Campo requerido",
            txtDirCalleSecundaria: "Campo requerido",
            txtDirReferencia: "Campo requerido",
            txtTelefonoCelular: "Campo requerido",
            txtCorreoAlternativo: {
                required: "Campo requerido",
                email: "Favor ingrese una cuenta de correo valida"
            }
        },

        highlight: function (element) {
            $(element).closest('.form-group').addClass('has-error');
        },
        unhighlight: function (element) {
            $(element).closest('.form-group').removeClass('has-error');
        },
        errorElement: 'span',
        errorClass: 'help-block',
        errorPlacement: function (error, element) {
            if (element.parent('.input-group').length) {
                error.insertAfter(element.parent());
            } else {
                error.insertAfter(element);
            }
        }
    });

    $.validator.addMethod("aFunction",
        function (value, element) {
            if (value == "-1")
                return false;
            else
                return true;
        }, "Campo requerido - metodo" );
})

