using GestorErrores;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using SitioWebOasis.Library;
using System;
using System.Collections.Generic;
using System.Data;

namespace SitioWebOasis.Models
{
    public class DatosAsignaturasDocenteModel: DatosCarrera
    {
        private WSGestorDeReportesMatriculacion.dtstCursosDocente _dtstCursosDocente = new WSGestorDeReportesMatriculacion.dtstCursosDocente();

        public DatosAsignaturasDocenteModel( string strCodCarrera )
        {
            if (!string.IsNullOrEmpty(strCodCarrera)){
                this.UsuarioActual.SetRolCarreraActual( Roles.Docentes,
                                                        strCodCarrera);
            }

            this._dtstPeriodoVigente = this._dataPeriodoAcademicoVigente();
            this._dtstCursosDocente = this._dsAsignaturasDocente();
        }
        
        
        //  Lista de asignaturas por carrera
        private WSGestorDeReportesMatriculacion.dtstCursosDocente _dsAsignaturasDocente()
        {
            WSGestorDeReportesMatriculacion.dtstCursosDocente dsCursosDocente = new WSGestorDeReportesMatriculacion.dtstCursosDocente();
            WSGestorDeReportesMatriculacion.dtstCursosDocente rstCursoDocente = new WSGestorDeReportesMatriculacion.dtstCursosDocente();

            try
            {
                ProxySeguro.GestorDeReportesMatriculacion rm = new ProxySeguro.GestorDeReportesMatriculacion();
                rm.CookieContainer = new System.Net.CookieContainer();
                rm.SetCodCarrera(this.UsuarioActual.CarreraActual.Codigo);

                rstCursoDocente = rm.GetCursosDocente(  this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString(),
                                                        this.UsuarioActual.Cedula.ToString());

                if(rstCursoDocente != null){
                    dsCursosDocente = rstCursoDocente;
                }
            }
            catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getAsignaturasDocente");
            }

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
            catch (Exception ex) {
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

            if (this._dtstCursosDocente.Cursos.Count > 0){
                int x = 0;                
                EvaluacionActiva ea = new EvaluacionActiva();
                evActiva = ea.getDtaEvaluacionActiva();

                if( !string.IsNullOrEmpty(evActiva))
                {
                    rst = string.Empty;
                    parcialActivo = (evActiva != "EF" && evActiva != "ER" && evActiva != "NA")  
                                        ? this._getNumOrdinal(evActiva)
                                        : (evActiva == "EF")? Language.es_ES.DOC_TB_EV_FINAL
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
                        rst += "	    <span id='mini-bar-chart"+ ++x +"' class='mini-bar-chart'><canvas width='53' height='25' style='display: inline-block; vertical-align: top; width: 53px; height: 25px;'></canvas></span>";
                        rst += "    </td>";
                        rst += "</tr>";
                    }
                }
            }

            return rst;
        }

        
        public List<System.Web.Mvc.SelectListItem> getLstAsignaturasDocente(string strCodAsignatura = "")
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

                    if (strCodAsignatura == item["strCodMateria"].ToString())
                    {
                        asignatura.Selected = true;
                    }

                    lstAsignaturasDocente.Add(asignatura);
                }
            }

            return lstAsignaturasDocente;
        }


        //public string getNombreAsignatura(string strCodAsignatura, string strCodNivel, string strCodParalelo)
        //{
        //    string asignatura = string.Empty;

        //    try
        //    {
        //        if (this._dtstCursosDocente != null && this._dtstCursosDocente.Cursos.Rows.Count > 0){
        //            DataRow[] rst = this._dtstCursosDocente.Cursos.Select("strCodMateria = '" + strCodAsignatura + "' AND strCodNivel ='" + strCodNivel + "' AND strCodParalelo = '" + strCodParalelo + "'");
        //            asignatura = (rst.Length > 0)   ? rst[0]["strNombreMateria"] + " / " + rst[0]["strDescripcionNivel"] + " / " + rst[0]["strCodParalelo"]
        //                                            : string.Empty;
        //        }
        //    }catch (Exception ex){
        //        asignatura = string.Empty;

        //        Errores err = new Errores();
        //        err.SetError(ex, "getNombreAsignatura");
        //    }

        //    return asignatura;
        //}


        //public bool estadoActa( string strCodAsignatura )
        //{
        //    bool ban = false;
        //    string parcialActivo = string.Empty;

        //    try {                
        //        ProxySeguro.NotasEstudiante ne = new ProxySeguro.NotasEstudiante();
        //        string strCodCarrera    = UsuarioActual.CarreraActual.Codigo.ToString();
        //        string periodoVigente   = this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString();
        //        parcialActivo           = this.strParcialActivo;

        //        if (parcialActivo == "1" || parcialActivo == "2" || parcialActivo == "3"){
        //            ban = ne.getEstadoParcialEvAcumulativa( strCodCarrera,
        //                                                    periodoVigente,
        //                                                    strCodAsignatura,
        //                                                    parcialActivo);
        //        }else if(parcialActivo == "EF" || parcialActivo == "ER"){
        //            ban = ne.getActaImpresaEvFinalesRecuperacion(   strCodCarrera,
        //                                                            periodoVigente,
        //                                                            strCodAsignatura,
        //                                                            parcialActivo);
        //        }
        //    }
        //    catch (Exception ex){
        //        ban = false;

        //        Errores err = new Errores();
        //        err.SetError(ex, "estadoActa");
        //    }

        //    return ban;
        //}
    }
}