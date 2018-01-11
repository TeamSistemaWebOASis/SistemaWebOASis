using GestorErrores;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;




namespace SitioWebOasis.Library
{
    public class Asignatura: DatosCarrera
    {
        protected string _strCodCarrera { get; set; }

        protected string _strCodAsignatura { get; set; }

        protected string _strCodNivel{ get; set; }

        protected string _strCodParalelo{ get; set; }

        protected string _evaluacionActiva = string.Empty;

        private WSGestorDeReportesMatriculacion.dtstCursosDocente _dtstCursosDocente = new WSGestorDeReportesMatriculacion.dtstCursosDocente();

        protected EvaluacionActiva _evaluacion = null;

        private DataRow _drProximoParcial = default(DataRow);

        //  Gestiona si el acta ha sido impresa
        //  public bool estadoActa = true;

        public Asignatura()
        {
            if (!string.IsNullOrEmpty(this._strCodCarrera)){
                this.UsuarioActual.SetRolCarreraActual( Roles.Docentes,
                                                        this._strCodCarrera);
            }
            
            this._evaluacion = new EvaluacionActiva();
            this._dtstPeriodoVigente = this._dataPeriodoAcademicoVigente();
            this._dtstCursosDocente = this._dsAsignaturasDocente();
            this._evaluacionActiva = _evaluacion.getDataEvaluacionActiva();
            this._drProximoParcial = this._evaluacion.getProximoParcial();
        }


        //  Lista de asignaturas por carrera
        private WSGestorDeReportesMatriculacion.dtstCursosDocente _dsAsignaturasDocente()
        {
            WSGestorDeReportesMatriculacion.dtstCursosDocente dsCursosDocente = new WSGestorDeReportesMatriculacion.dtstCursosDocente();
            WSGestorDeReportesMatriculacion.dtstCursosDocente rstCursoDocente = new WSGestorDeReportesMatriculacion.dtstCursosDocente();
            ProxySeguro.GestorDeReportesMatriculacion rm = new ProxySeguro.GestorDeReportesMatriculacion();
            rm.CookieContainer = new System.Net.CookieContainer();
            rm.SetCodCarrera(this.UsuarioActual.CarreraActual.Codigo);

            try{
                rstCursoDocente = rm.GetCursosDocente(  this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                        this.UsuarioActual.Cedula.ToString());

                if (rstCursoDocente != null){
                    dsCursosDocente = rstCursoDocente;
                }
            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_getAsignaturasDocente");
            }

            rm.Dispose();
            return dsCursosDocente;
        }


        public string getNombreDocente()
        {
            string nombreDocente = (this.UsuarioActual != null)
                                        ? this.UsuarioActual.Nombre
                                        : string.Empty;

            return nombreDocente;
        }


        public string getFacultadCarreraDocente()
        {
            string facultadCarreraDocente = string.Empty;
            try
            {
                if (this.UsuarioActual != null)
                {
                    string facultad = (this.UsuarioActual.FacultadActual != null)
                                            ? this.UsuarioActual.FacultadActual.Nombre.ToString() + " / "
                                            : "";

                    string carrera = (this.UsuarioActual.CarreraActual != null)
                                        ? this.UsuarioActual.CarreraActual.Nombre.ToString()
                                        : "";

                    facultadCarreraDocente = facultad + carrera;
                }
            }
            catch (Exception ex)
            {
                facultadCarreraDocente = string.Empty;
                Errores err = new Errores();
                err.SetError(ex, "getFacultadCarreraDocente");
            }


            return facultadCarreraDocente;
        }


        public string getHTMLAsignaturasDocente()
        {
            string rst = string.Empty;
            string nivel = string.Empty;
            string color = "odd";
            string evActiva = string.Empty;
            string parcialActivo = string.Empty;

            rst += " <tr role='row' class='" + color + "'>";
            rst += "     <td style='align-content: center; vertical-align: middle; text-align: center;' colspan='9'>" + Language.es_ES.EST_LBL_SIN_REGISTROS + "</td>";
            rst += " </tr>";

            if (this._dtstCursosDocente.Cursos.Count > 0)
            {
                int x = 0;
                evActiva = this._evaluacion.getDataEvaluacionActiva().Replace("FN", "");

                if (!string.IsNullOrEmpty(evActiva))
                {
                    rst = string.Empty;
                    parcialActivo = (evActiva != "FNP" && evActiva != "ER" && evActiva != "NA")
                                        ? this._getNumOrdinal(evActiva)
                                        : (evActiva == "FNP")   ? Language.es_ES.DOC_TB_EV_FINAL
                                                                : Language.es_ES.DOC_TB_EV_RECUPERACION;

                    foreach (DataRow item in this._dtstCursosDocente.Cursos){
                        color = (color == "odd") ? "even" : "odd";
                        nivel = this._getNumOrdinal(item["strCodNivel"].ToString());

                        rst += "<tr role='row' class='" + color + "'>";
                        rst += "    <td style='align-content: center; vertical-align: middle; text-align: center;'>" + ++x + "</td>";
                        rst += "    <td style='align-content: center; vertical-align: middle; text-align: left;'><a href='/Docentes/EvaluacionAsignatura/" + item["strCodNivel"].ToString() + "/" + item["strCodMateria"].ToString() + "/" + item["strCodParalelo"].ToString() + "'>" + item["strNombreMateria"].ToString() + "</a></td>";
                        rst += "	<td style='align-content: center; vertical-align: middle; text-align: center;'>" + nivel + "</td>";
                        rst += "	<td style='align-content: center; vertical-align: middle; text-align: center;'>" + item["strCodParalelo"].ToString() + "</td>";
                        rst += "	<td style='align-content: center; vertical-align: middle; text-align: center;'>" + parcialActivo + "</td>";
                        rst += "	<td style='align-content: center; vertical-align: middle; text-align: center;'>";
                        rst += "	    <span id='mini-bar-chart" + ++x + "' class='mini-bar-chart'><canvas width='53' height='25' style='display: inline-block; vertical-align: top; width: 53px; height: 25px;'></canvas></span>";
                        rst += "    </td>";
                        rst += "</tr>";
                    }
                }
            }

            return rst;
        }


        public List<System.Web.Mvc.SelectListItem> getLstAsignaturasDocente()
        {
            List<System.Web.Mvc.SelectListItem> lstAsignaturasDocente = new List<System.Web.Mvc.SelectListItem>();
            System.Web.Mvc.SelectListItem asignatura = new System.Web.Mvc.SelectListItem();

            if (this._dtstCursosDocente != null && this._dtstCursosDocente.Cursos.Count > 0)
            {
                foreach (DataRow item in this._dtstCursosDocente.Cursos)
                {
                    asignatura = new System.Web.Mvc.SelectListItem();
                    asignatura.Value = item["strCodMateria"].ToString() + "|" + item["strCodNivel"].ToString() + "|" + item["strCodParalelo"].ToString();
                    asignatura.Text = item["strNombreMateria"].ToString() + " - '"+ item["strCodParalelo"].ToString() + "'";

                    if (this._strCodAsignatura == item["strCodMateria"].ToString() && this._strCodNivel == item["strCodNivel"].ToString() && this._strCodParalelo == item["strCodParalelo"].ToString()){
                        asignatura.Selected = true;
                    }

                    lstAsignaturasDocente.Add(asignatura);
                }
            }

            return lstAsignaturasDocente;
        }


        public string getNombreAsignatura()
        {
            string asignatura = string.Empty;

            try
            {
                if (this._dtstCursosDocente != null && this._dtstCursosDocente.Cursos.Rows.Count > 0)
                {
                    DataRow[] rst = this._dtstCursosDocente.Cursos.Select("strCodMateria = '" + this._strCodAsignatura + "' AND strCodNivel ='" + this._strCodNivel + "' AND strCodParalelo = '" + this._strCodParalelo + "'");
                    asignatura = (rst.Length > 0) ? rst[0]["strNombreMateria"] + " / " + rst[0]["strDescripcionNivel"] + " / " + rst[0]["strCodParalelo"]
                                                    : string.Empty;
                }
            }
            catch (Exception ex){
                asignatura = string.Empty;

                Errores err = new Errores();
                err.SetError(ex, "getNombreAsignatura");
            }

            return asignatura;
        }


        public bool estadoActa()
        {
            int numDiasTermino = this._evaluacion.getInfoNumDiasFaltantes();
            bool actaImpresa = this._evaluacion.getActaImpresa( this._strCodAsignatura,
                                                                this._strCodParalelo);

            return (numDiasTermino >= 0 && actaImpresa == false);
        }


        protected void _getDatosMateria(ref string strAsignatura,
                                       ref string strNivel,
                                       ref string strPeriodo,
                                       ref string strDocente,
                                       ref string strSistema,
                                       ref float numCreditos,
                                       ref byte fHorasTeo,
                                       ref byte fHorasPra)
        {
            try
            {
                ProxySeguro.GestorEvaluacion ge = new ProxySeguro.GestorEvaluacion();
                ge.CookieContainer = new CookieContainer();
                ge.set_fBaseDatos(this._strNombreBD);
                ge.set_fUbicacion(this._strUbicacion);

                //Buscando los datos de la materia
                ge.GetDatosMateriaActa(this._dtstPeriodoVigente.Periodos.Rows[0]["strCodigo"].ToString(),
                                        this._strCodAsignatura,
                                        this._strCodNivel,
                                        this._strCodParalelo,
                                        ref strAsignatura,
                                        ref strNivel,
                                        ref strPeriodo,
                                        ref strDocente,
                                        ref strSistema);

                //Buscando los datos del pensum de la materia
                ge.GetDatosMateriaPensum(this._dtstPeriodoVigente.Periodos.Rows[0]["strCodigo"].ToString(),
                                            this._strCodAsignatura,
                                            ref numCreditos,
                                            ref fHorasTeo,
                                            ref fHorasPra);

            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "getDatosMateria");

                strAsignatura = string.Empty;
                strNivel = string.Empty;
                strPeriodo = string.Empty;
                strDocente = string.Empty;
                strSistema = string.Empty;
            }
        }

        public string getInfoEvaluacionActiva()
        {
            return this._evaluacion.getInfoEvaluacionActiva();
        }


        public string getInfoProximaEvaluacion()
        {
            return (this._drProximoParcial != null) 
                        ? this._drProximoParcial["strCodigo"].ToString().Replace("FN", "") 
                        : string.Empty;
        }


        public string getMsgEstFMGEvAcumulativa()
        {
            string color = "danger";
            string mensajeImpresion = string.Empty;

            if ( this._evaluacionActiva == "1" || this._evaluacionActiva == "2" || this._evaluacionActiva == "3"){
                mensajeImpresion = this._getMensajeEstadoEvaluacion();
            }else{
                string strCodPA = this.getInfoProximaEvaluacion();

                if (strCodPA == "1" || strCodPA == "2" || strCodPA == "3"){
                    string dtFchInicio = this._evaluacion.getFchInicioEvaluacion(strCodPA);

                    mensajeImpresion = "<p class='text-success pull-right'>";
                    mensajeImpresion += "   Parcial <strong>'" + strCodPA + "'</strong>, activo a partir del <strong>" + dtFchInicio + "</strong>";
                    mensajeImpresion += "</p>";
                }else{
                    mensajeImpresion = "<p class='text-info pull-right'>";
                    mensajeImpresion += "   <strong>" + Language.es_ES.MSG_GESTION_NOTAS_CERRADA + "</strong>";
                    mensajeImpresion += "</p>";
                }
            }

            return mensajeImpresion;
        }


        public string getMsgEstFMGEvFinal()
        {
            string mensajeImpresion = string.Empty;

            if (this._evaluacionActiva == "P"){
                mensajeImpresion = this._getMensajeEstadoEvaluacion();
            }else if( this.getInfoProximaEvaluacion() == "S" ) {
                mensajeImpresion = "<p class='text-info pull-right'>";
                mensajeImpresion += "   <strong>" + Language.es_ES.MSG_GESTION_NOTAS_CERRADA + "</strong>";
                mensajeImpresion += "</p>";
            }

            return mensajeImpresion;
        }


        public string getMsgEstFMGEvRecuperacion()
        {
            string mensajeImpresion = string.Empty;

            if (this._evaluacionActiva == "S"){
                mensajeImpresion = this._getMensajeEstadoEvaluacion();
            }else if( string.IsNullOrEmpty( this.getInfoProximaEvaluacion() )){
                mensajeImpresion = "<p class='text-info pull-right'>";
                mensajeImpresion += "   <strong>" + Language.es_ES.MSG_GESTION_NOTAS_CERRADA + "</strong>";
                mensajeImpresion += "</p>";
            }

            return mensajeImpresion;
        }


        public string getFchInicioGestionEvFinal()
        {
            return (this._evaluacion != null) 
                        ? this._evaluacion.getFchInicioEvaluacion("FNP") 
                        : string.Empty;
        }


        public string getFchInicioGestionEvRecuperacion()
        {
            return ( this._evaluacion != null ) 
                        ? this._evaluacion.getFchInicioEvaluacion("FNS") 
                        : string.Empty;
        }


        private string _getMensajeEstadoEvaluacion()
        {
            bool actaImpresa = this._evaluacion.getActaImpresa( this._strCodAsignatura,
                                                                this._strCodParalelo);

            int numDiasTermino = this._evaluacion.getInfoNumDiasFaltantes();
            string color = (numDiasTermino <= 1) ? "danger" : "success";
            string mensajeImpresion = string.Empty;

            if (numDiasTermino >= 0 && actaImpresa == false ){
                mensajeImpresion = "<p id='msgEstadoEvaluacion' class='text-" + color + " pull-right'>";
                mensajeImpresion += (this._evaluacionActiva == "P" || this._evaluacionActiva == "S") 
                                        ? "Fecha máxima de gestión:&nbsp;<strong> " + this._evaluacion.getInfoEvaluacionActiva() + " &nbsp;&nbsp;</strong>"
                                        : "Parcial activo <strong>'" + this._evaluacionActiva + "'</strong>, fecha máxima de gestión:&nbsp;<strong> " + this._evaluacion.getInfoEvaluacionActiva() + " &nbsp;&nbsp;</strong>";
            }else if (numDiasTermino < 0 || actaImpresa == true ){
                mensajeImpresion = "<p class='text-danger pull-right'>";
                mensajeImpresion += "   <strong id='msgEstEvFinal'>" + Language.es_ES.MSG_GESTION_NOTAS_CERRADA +"</strong>";
            }

            mensajeImpresion += "</p>";
            return mensajeImpresion;
        }

    }
}