
//
//  CLASE QUE GESTIONA INFORMACION DE NOTA DE UN ESTUDIANTE
//
var EvaluacionAcumulativa = function()
{
    this.No;
    this.NombreEstudiante;
    this.Total;
    this.bytAsistencia;
    this.bytNota1;
    this.bytNota2;
    this.bytNota3;
    this.bytNumMat;
    this.sintCodMatricula;
    this.strCodMateria;
    this.strCodNivel;
    this.strCodParalelo;
    this.strCodPeriodo;
    this.strCodigo;
    this.strObservaciones;
    this.parcialActivo;
    this.banEstado = 0;

    this.lstMatricula = ["1ra", "2da", "3ra"];
}

EvaluacionAcumulativa.prototype.setDtaEvaluacion = function (dtaEvaluacion, parcialActivo){
    this.No = dtaEvaluacion.No;
    this.NombreEstudiante = dtaEvaluacion.NombreEstudiante;
    this.Total = dtaEvaluacion.Total;
    this.bytAsistencia = dtaEvaluacion.bytAsistencia;
    this.bytNota1 = dtaEvaluacion.bytNota1;
    this.bytNota2 = dtaEvaluacion.bytNota2;
    this.bytNota3 = dtaEvaluacion.bytNota3;
    this.bytNumMat = dtaEvaluacion.bytNumMat;
    this.sintCodMatricula = dtaEvaluacion.sintCodMatricula;
    this.strCodMateria = dtaEvaluacion.strCodMateria;
    this.strCodNivel = dtaEvaluacion.strCodNivel;
    this.strCodParalelo = dtaEvaluacion.strCodParalelo;
    this.strCodPeriodo = dtaEvaluacion.strCodPeriodo;
    this.strCodigo = dtaEvaluacion.strCodigo;
    this.strObservaciones = dtaEvaluacion.strObservaciones;
    this.parcialActivo = parcialActivo;
    this.banEstado = 0;
}


EvaluacionAcumulativa.prototype.calculoPromedio = function(){
    return( this.acumulado() / parseFloat( this.parcialActivo ) );
}


EvaluacionAcumulativa.prototype.acumulado = function(){
    return Math.ceil(parseFloat(this.bytNota1) + parseFloat(this.bytNota2) + parseFloat(this.bytNota3));
}


EvaluacionAcumulativa.prototype.getEstadoEvaluacion = function () {
    var rst = "---";
    var acumulado = this.acumulado();

    if (this.parcialActivo == "3") {
        switch (true) {
            //  Exonerado
            case (acumulado >= 25 && this.bytAsistencia >= 70):
                rst = "<span class='label label-success'>EXONERADO</span>";
            break;

            //  Evaluacion Final
            case (acumulado >= 12 && acumulado < 25 && this.bytAsistencia >= 70):
                rst = "<span class='label label-default'>EVALUACION FINAL</span>";
            break;

            //  Reprobado
            case (acumulado < 12 && this.bytAsistencia >= 70):
                rst = "<span class='label label-danger'>REPROBADO</span>";
            break;

            //  Reprovado - Reprobado por faltas
            case (this.bytAsistencia < 70):
                rst = "<span class='label label-danger'>REPROBADO - FALTAS</span>";
            break;
        }
    }

    return rst;
}


EvaluacionAcumulativa.prototype.getNumMatricula = function () {
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