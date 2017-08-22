$(document).ready(function () {
    var ban = true;

    //  LUGAR DE NACIMIENTO
    //  Seleccionamos todos los elementos que forman parte del area Fecha de nacimiento - ubicacion
    $('#fchNacimiento select').change(function () {
        var value = 0;
        //  Obtenemos el identificador del elemento
        var id = $(this).attr("id");

        //  Obtenemos el valor del elemento
        if ($(this).val() != "") {
            value = $(this).val();
        }

        $.ajax({
            type: "POST",
            url: "/Estudiantes/GetDPA",
            data: '{ idType: "' + id + '", idDPA: "' + value + '" }',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function () {
                switch(id){
                    case "ddl_FNPais":
                        $('#ddl_FNProvincias, #ddl_FNCiudades, #ddl_FNParroquias').attr("disabled", "disabled");
                        break;

                    case "ddl_FNProvincias":
                        $('#ddl_FNCiudades, #ddl_FNParroquias').attr("disabled", "disabled");
                    break;

                    case "ddl_FNCiudades":
                        $('#ddl_FNParroquias').attr("disabled", "disabled");
                    break;
                }
            },

            success: function (response) {
                var dtaDPA = response;
                switch ( id ) {
                    case "ddl_FNPais":
                        llenarComboBox("#ddl_FNProvincias", dtaDPA);
                    break;

                    case "ddl_FNProvincias":
                        llenarComboBox("#ddl_FNCiudades", dtaDPA);
                    break;

                    case "ddl_FNCiudades":
                        llenarComboBox("#ddl_FNParroquias", dtaDPA);
                    break;
                }
            }
        })

    })

    
    function llenarComboBox(dropDownId, list) {
        if (list != null && list.length > 0) {
            $(dropDownId).empty();
            $(dropDownId).removeAttr("disabled");
            $.each(list, function () {
                $(dropDownId).append($("<option></option>").val(this['Value']).html(this['Text']));
                if (this['Value'] == "-2") {
                    ban = false;
                    disableComboBoxDtaUbicacion(dropDownId, this['Value'], this['Text']);
                }
            });
        }
    }

    //  DIRECCION ESTUDIANTE
    //  Seleccionamos todos los elementos que forman parte del area DIRECCION
    $('#estDireccion select').change(function () {
        var value = 0;
        //  Obtenemos el identificador del elemento
        var id = $(this).attr("id");

        //  Obtenemos el valor del elemento
        if ($(this).val() != "") {
            value = $(this).val();
        }

        $.ajax({
            type: "POST",
            url: "/Estudiantes/GetDPA",
            data: '{ idType: "' + id + '", idDPA: "' + value + '" }',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function () {
                switch (id) {
                    case "ddl_DUPais":
                        $('#ddl_DUProvincias, #ddl_DUCiudades, #ddl_DUParroquias').attr("disabled", "disabled");
                        break;

                    case "ddl_DUProvincias":
                        $('#ddl_DUCiudades, #ddl_DUParroquias').attr("disabled", "disabled");
                        break;

                    case "ddl_DUCiudades":
                        $('#ddl_DUParroquias').attr("disabled", "disabled");
                        break;
                }
            },

            success: function (response) {
                var dtaDPA = response;
                switch (id) {
                    case "ddl_DUPais":
                        llenarComboBoxDU("#ddl_DUProvincias", dtaDPA);
                        break;

                    case "ddl_DUProvincias":
                        llenarComboBoxDU("#ddl_DUCiudades", dtaDPA);
                        break;

                    case "ddl_DUCiudades":
                        llenarComboBoxDU("#ddl_DUParroquias", dtaDPA);
                        break;
                }
            }
        })
    })


    function llenarComboBoxDU(dropDownId, list) {
        if (list != null && list.length > 0) {
            $(dropDownId).empty();
            $(dropDownId).removeAttr("disabled");
            $.each(list, function () {
                $(dropDownId).append($("<option></option>").val(this['Value']).html(this['Text']));
                if (this['Value'] == "-2") {
                    ban = false;
                    disableComboBoxDtaUbicacion(dropDownId, this['Value'], this['Text']);
                }
            });
        }
    }


    function disableComboBoxDtaUbicacion(dropDownId, value, text) {
        switch (dropDownId) {
            //  PAIS
            case "#ddl_FNPais":
                $('#ddl_FNProvincias, #ddl_FNCiudades, #ddl_FNParroquias').empty();
                $('#ddl_FNProvincias, #ddl_FNCiudades, #ddl_FNParroquias').append($("<option></option>").val(value).html(text));
                $('#ddl_FNProvincias, #ddl_FNCiudades, #ddl_FNParroquias').attr("disabled", "disabled");
            break;

            case "#ddl_DUPais":
                $('#ddl_DUProvincias, #ddl_DUCiudades, #ddl_DUParroquias').empty();
                $('#ddl_DUProvincias, #ddl_DUCiudades, #ddl_DUParroquias').append($("<option></option>").val(value).html(text));
                $('#ddl_DUProvincias, #ddl_DUCiudades, #ddl_DUParroquias').attr("disabled", "disabled");
            break;
            
            //  PROVINCIAS
            case "#ddl_FNProvincias":
                $('#ddl_FNProvincias, #ddl_FNCiudades, #ddl_FNParroquias').empty();
                $('#ddl_FNProvincias, #ddl_FNCiudades, #ddl_FNParroquias').append($("<option></option>").val(value).html(text));
                $('#ddl_FNProvincias, #ddl_FNCiudades, #ddl_FNParroquias').attr("disabled", "disabled");
            break;

            case "#ddl_DUProvincias":
                $('#ddl_DUProvincias, #ddl_DUCiudades, #ddl_DUParroquias').empty();
                $('#ddl_DUProvincias, #ddl_DUCiudades, #ddl_DUParroquias').append($("<option></option>").val(value).html(text));
                $('#ddl_DUProvincias, #ddl_DUCiudades, #ddl_DUParroquias').attr("disabled", "disabled");
            break;

            //  CIUDADES
            case "#ddl_FNCiudades":
                $('#ddl_FNCiudades, #ddl_FNParroquias').empty();
                $('#ddl_FNCiudades, #ddl_FNParroquias').append($("<option></option>").val(value).html(text));
                $('#ddl_FNCiudades, #ddl_FNParroquias').attr("disabled", "disabled");
            break;

            case "#ddl_DUCiudades":
                $('#ddl_DUCiudades, #ddl_DUParroquias').empty();
                $('#ddl_DUCiudades, #ddl_DUParroquias').append($("<option></option>").val(value).html(text));
                $('#ddl_DUCiudades, #ddl_DUParroquias').attr("disabled", "disabled");
            break;
        }
    }

})