using GestorErrores;
using SitioWebOasis.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitioWebOasis.Models
{
    public class EvaluacionesDocenteModel: DatosCarrera
    {
        public string strCodAsignatura{get; set;}

        public string strCodNivel { get; set;}

        public string strCodParalelo { get; set;}

        public string strParcialActivo {
            get {   EvaluacionActiva pa = new EvaluacionActiva();
                    return pa.getDtaEvaluacionActiva();
            }
        }
        

        private DatosAcademicosDocente _dad;


        public EvaluacionesDocenteModel(){
            this._dad = new DatosAcademicosDocente(this.UsuarioActual.CarreraActual.Codigo.ToString());
        }

        
        public EvaluacionAcumulativaModel evAcumulativaModel { get; set; }


        public EvaluacionFinalModel evFinalModel { get; set;}


        public EvaluacionRecuperacionModel evRecuperacionModel { get; set; }


        public string getNombreAsignatura( string strCodAsignatura, string strCodNivel, string strCodParalelo)
        {
            return this._dad.getNombreAsignatura(strCodAsignatura, strCodNivel, strCodParalelo);
        }


        public List<System.Web.Mvc.SelectListItem> getLstAsignaturasDocente(string strCodAsignatura = "")
        {
            return this._dad.getLstAsignaturasDocente(strCodAsignatura);
        }



    }
}