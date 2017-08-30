
//
//  CLASE QUE GESTIONA INFORMACION DE EVALUACION FINAL DE UN ESTUDIANTE
//
var EvaluacionRecuperacion = function () {
    this.No;
    this.NombreCompleto;
    this.Total;
    this.bytAcumulado;
    this.bytAsistencia;
    this.bytNota;
    this.bytNumMat;
    this.sintCodMatricula;
    this.strCodEquivalencia;
    this.strCodMateria;
    this.strCodNivel;
    this.strCodParalelo;
    this.strCodPeriodo;
    this.strCodigo;
    this.strCodTipoExamen;
    this.strObservaciones;
    this.banEstado = 0;

    this.lstMatricula = ["1ra", "2da", "3ra"];
}

EvaluacionRecuperacion.prototype.setDtaEvaluacionRecuperacion = function (dtaEvRecuperacion) {
    this.No = dtaEvRecuperacion.No;
    this.NombreCompleto = dtaEvRecuperacion.NombreCompleto;
    this.Total = dtaEvRecuperacion.Total;
    this.bytAcumulado = dtaEvRecuperacion.bytAcumulado;
    this.bytAsistencia = dtaEvRecuperacion.bytAsistencia;
    this.bytNota = dtaEvRecuperacion.bytNota;
    this.bytNumMat = dtaEvRecuperacion.bytNumMat;
    this.sintCodMatricula = dtaEvRecuperacion.sintCodMatricula;
    this.strCodEquivalencia = dtaEvRecuperacion.strCodEquiv;
    this.strCodMateria = dtaEvRecuperacion.strCodMateria;
    this.strCodNivel = dtaEvRecuperacion.strCodNivel;
    this.strCodParalelo = dtaEvRecuperacion.strCodParalelo;
    this.strCodPeriodo = dtaEvRecuperacion.strCodPeriodo;
    this.strCodigo = dtaEvRecuperacion.strCodigo;
    this.strObservaciones = dtaEvRecuperacion.strObservaciones;
    this.strCodTipoExamen = dtaEvRecuperacion.strCodTipoExamen;

    this.banEstado = 0;
}


EvaluacionRecuperacion.prototype.getEstadoEvaluacionRecuperacion = function () {
    var rst = false;

    switch (true) {
        //  Aprobado
        case (this.Total >= 28 && this.strCodEquivalencia != "E" && this.bytAsistencia >= 70):
            rst = "<span class='label label-success'>APROBADO</span>";
        break;

        //  Reprueba
        case (this.Total < 15 && this.strCodEquivalencia == "R"):
            rst = "<span class='label label-danger'>REPRUEBA</span>";
        break;
    }

    return rst;
}


EvaluacionRecuperacion.prototype.getNumMatricula = function () {
    var numMatricula = " -- ";

    if (this.bytNumMat != undefined) {
        switch (this.bytNumMat) {
            case 1: numMatricula = this.lstMatricula[0]; break;
            case 2: numMatricula = this.lstMatricula[1]; break;
            case 3: numMatricula = this.lstMatricula[2]; break;
        }
    }

    return numMatricula;
}


EvaluacionRecuperacion.prototype.getTotalEvRecuperacion = function () {
    this.Total = parseInt(this.bytAcumulado) + parseInt(this.bytNota);
    return this.Total;
}