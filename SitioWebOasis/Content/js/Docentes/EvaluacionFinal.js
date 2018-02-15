  
//
//  CLASE QUE GESTIONA INFORMACION DE EVALUACION FINAL DE UN ESTUDIANTE
//
var EvaluacionFinal = function () {
    this.No;
    this.NombreCompleto;
    this.Total;
    this.bytAsistencia;
    this.bytAcumulado;
    this.bytNota;
    this.bytNumMat;
    this.sintCodMatricula;
    this.strCodEquivalencia;
    this.strCodMateria;
    this.strCodNivel;
    this.strCodParalelo;
    this.strCodPeriodo;
    this.strCodigo;
    this.strObservaciones;
    this.strCodTipoExamen;
    this.banEstado = 0;

    this.lstMatricula = ["1ra", "2da", "3ra"];
}

EvaluacionFinal.prototype.setDtaEvaluacionFinal = function (dtaEvFinal) {
    this.No = dtaEvFinal.No;
    this.NombreCompleto = dtaEvFinal.NombreCompleto;
    this.Total = dtaEvFinal.Total;
    this.bytAcumulado = dtaEvFinal.bytAcumulado;
    this.bytAsistencia = dtaEvFinal.bytAsistencia;
    this.bytNota = dtaEvFinal.bytNota;
    this.bytNumMat = dtaEvFinal.bytNumMat;
    this.sintCodMatricula = dtaEvFinal.sintCodMatricula;
    this.strCodEquivalencia = dtaEvFinal.strCodEquiv;
    this.strCodMateria = dtaEvFinal.strCodMateria;
    this.strCodNivel = dtaEvFinal.strCodNivel;
    this.strCodParalelo = dtaEvFinal.strCodParalelo;
    this.strCodPeriodo = dtaEvFinal.strCodPeriodo;
    this.strCodigo = dtaEvFinal.strCodigo;
    this.strObservaciones = dtaEvFinal.strObservaciones;
    this.strCodTipoExamen = dtaEvFinal.strCodTipoExamen;

    this.banEstado = 0;
}


EvaluacionFinal.prototype.getEstadoEvaluacionFinal = function () {
    var rst = false;

    switch (true) {

        //  Exonerado
        case (this.Total >= 28 && this.strCodEquivalencia == "E" && this.bytAsistencia >= 70):
            rst = "<span class='label label-success'>EXONERADO</span>";
        break;

        //  Aprobado
        case (this.Total >= 28 && this.strCodEquivalencia != "E" && this.bytAsistencia >= 70):
            rst = "<span class='label label-success'>APROBADO</span>";
        break;

        //  Ev. Recuperacion
        case (this.Total >= 16 && this.Total < 28 && this.bytAsistencia >= 70):
            rst = "<span class='label label-warning'>Ev. RECUPERACION</span>";
        break;

        //  Reprueba
        case (this.Total < 16 && this.strCodEquivalencia == "R" && this.bytAsistencia > 70):
            rst = "<span class='label label-danger'>REPRUEBA</span>";
        break;

        //  Reprueba - FALTAS
        case (this.bytAsistencia < 70):
            rst = "<span class='label label-danger'>REPRUEBA - FALTAS</span>";
        break;
    }

    return rst;
}


EvaluacionFinal.prototype.getNumMatricula = function () {
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


EvaluacionFinal.prototype.getTotalEvFinal = function () {
    this.Total = parseInt( this.bytAcumulado ) + parseInt( this.bytNota );
    return this.Total;
}


EvaluacionFinal.prototype.esExoneradoReprobado = function () {
    return (this.strCodEquivalencia == "E" || this.bytAsistencia < 70 ) ? false : true;
}