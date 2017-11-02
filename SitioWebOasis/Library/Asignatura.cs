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


        private WSGestorDeReportesMatriculacion.dtstCursosDocente _dtstCursosDocente = new WSGestorDeReportesMatriculacion.dtstCursosDocente();

        protected EvaluacionActiva _evaluacion = null;

        public Asignatura()
        {
            if (!string.IsNullOrEmpty(this._strCodCarrera)){
                this.UsuarioActual.SetRolCarreraActual( Roles.Docentes,
                                                        this._strCodCarrera);
            }
            
            this._evaluacion = new EvaluacionActiva();
            this._dtstPeriodoVigente = this._dataPeriodoAcademicoVigente();
            this._dtstCursosDocente = this._dsAsignaturasDocente();
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
                    asignatura.Text = item["strNombreMateria"].ToString();

                    if (this._strCodAsignatura == item["strCodMateria"].ToString())
                    {
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
            bool ban = false;
            string evaluacionActiva = string.Empty;
            ProxySeguro.NotasEstudiante ne = new ProxySeguro.NotasEstudiante();

            try
            {                
                string strCodCarrera = UsuarioActual.CarreraActual.Codigo.ToString();
                string periodoVigente = this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString();
                evaluacionActiva = _evaluacion.getDataEvaluacionActiva();

                if (evaluacionActiva == "1" || evaluacionActiva == "2" || evaluacionActiva == "3"){
                    ban = ne.getEstadoParcialEvAcumulativa( strCodCarrera,
                                                            periodoVigente,
                                                            this._strCodAsignatura,
                                                            evaluacionActiva);
                }else if (evaluacionActiva == "FNP" || evaluacionActiva == "ER"){
                    ban = ne.getActaImpresaEvFinalesRecuperacion(   strCodCarrera,
                                                                    periodoVigente,
                                                                    this._strCodAsignatura,
                                                                    evaluacionActiva);
                }
            }catch (Exception ex){
                ban = false;

                Errores err = new Errores();
                err.SetError(ex, "estadoActa");
            }

            ne.Dispose();
            return ban;
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


        public string getMensajeEstadoFMGEvAcumulativa()
        {
            bool actaImpresa = this._evaluacion.getActaImpresa(this._strCodAsignatura);
            string color = (this._evaluacion.getInfoNumDiasFaltantes() <= 1) ? "danger" : "success";
            string  mensajeImpresion  = string.Empty;

            if ( actaImpresa == false){
                mensajeImpresion = "<p class='text-" + color + " pull-right'>";
                mensajeImpresion += "   Fecha máxima de gestión:&nbsp;<strong> " + this._evaluacion.getInfoEvaluacionActiva() + " &nbsp;&nbsp;</strong>";
            }
            else if( actaImpresa == true ){
                mensajeImpresion = "<p class='text-danger pull-right'>";
                mensajeImpresion += "   <strong>Gestion de Notas 'Cerrada'</strong>";
            }

            mensajeImpresion += "</p>";

            return mensajeImpresion;
        }

    }
}