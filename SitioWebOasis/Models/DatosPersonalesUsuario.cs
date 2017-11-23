using GestorErrores;
using SitioWebOasis.CommonClasses;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using SitioWebOasis.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SitioWebOasis.Models
{
    public class DatosPersonalesUsuario
    {
        public string per_numCedula { get; set; }
        public string per_id = string.Empty;
        public bool existePersona = false;
        public bool esUsuarioOASis = false;

        public Catalogos catalogo;
        public Persona dtaEstudiante;
        public DocumentoPersonal dtaDocPersonal;
        public Direccion dtaDireccionEstudiante;
        public Nacionalidad dtaNacionalidadEstudiante;

        private WSSeguridad.dtstUsuario _dsUsuarioOASis = new WSSeguridad.dtstUsuario();

        public DatosPersonalesUsuario()
        {
            this.per_numCedula = (UsuarioActual != null)? UsuarioActual.Cedula.ToString().Replace("-", "")
                                                        : per_numCedula;

            this._cargarDatosPersonalesUsuario();
        }

        public DatosPersonalesUsuario( string strNumCedula )
        {
            this.per_numCedula = strNumCedula;
            this._cargarDatosPersonalesUsuario();
            this.esUsuarioOASis = this._esUsuarioOASis();
        }


        private string _addGuionCedula(string numCedula)
        {
            string rst = this.per_numCedula;

            if (rst.Length == 10){
                rst = this.per_numCedula.Insert(9, "-");
            }

            return rst;
        }


        private void _cargarDatosPersonalesUsuario()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.per_numCedula.ToString()))
                {
                    this.catalogo = new Catalogos();

                    //  Consumo del servicio web ObtenerPorDocumento (cedula)
                    string jsonDtaPersona = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_PERSONAS + "ServiciosPersona.svc" + "/ObtenerPorDocumento/" + this.per_numCedula.ToString());
                    var dtaPersona = Json.Decode(jsonDtaPersona);

                    if (!string.IsNullOrEmpty(Convert.ToString(dtaPersona.per_id)) && Convert.ToString(dtaPersona.per_id) != "0")
                    {
                        this.existePersona = true;
                        dtaEstudiante = new Persona(jsonDtaPersona);
                        dtaDocPersonal = new DocumentoPersonal(dtaEstudiante.per_id.ToString());
                        dtaDireccionEstudiante = new Direccion(dtaEstudiante.per_id.ToString());
                        dtaNacionalidadEstudiante = new Nacionalidad(dtaEstudiante.per_id.ToString());
                    }
                }
            }catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_cargarDatosPersonalesUsuario");
            }
        }


        public Usuario UsuarioActual
        {
            get { return (Usuario)System.Web.HttpContext.Current.Session["UsuarioActual"]; }
        }


        public bool updDatosEstudiantes(System.Web.HttpRequestBase dtaFrmEstudiante)
        {
            bool rst = true;

            try
            {
                if( this._updDatosPersonalesEstudiante(dtaFrmEstudiante) && this._updDireccionEstudiante(dtaFrmEstudiante)){
                    this._registrarDatosPersonalesEstudiante();
                    this._registrarDireccionEstudiante();
                }
            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "updDatosEstudiantes");
                rst = false;
            }

            return rst;
        }


        private bool _updDatosPersonalesEstudiante( System.Web.HttpRequestBase dtaFrmEstudiante )
        {
            bool rst = true;

            try
            {
                //  Informacion de persona
                this.dtaEstudiante.eci_id = Convert.ToInt32(dtaFrmEstudiante["ddlEstadoCivil"].ToString().Trim());
                this.dtaEstudiante.etn_id = Convert.ToInt32(dtaFrmEstudiante["ddlEtnia"].ToString().Trim());
                this.dtaEstudiante.tsa_id = Convert.ToInt32(dtaFrmEstudiante["ddlTipoSangre"].ToString().Trim());
                this.dtaEstudiante.gen_id = Convert.ToInt32(dtaFrmEstudiante["ddlGenero"].ToString().Trim());
                this.dtaEstudiante.per_telefonoCelular = dtaFrmEstudiante["txtTelefonoCelular"].ToString().Trim();
                this.dtaEstudiante.per_telefonoCasa = dtaFrmEstudiante["txtTelefonoFijo"].ToString().Trim();
                this.dtaEstudiante.per_emailAlternativo = dtaFrmEstudiante["txtCorreoAlternativo"].ToString().Trim();
                
                string DPA_FN = dtaFrmEstudiante["ddl_FNPais"].ToString().Trim() + "|" +
                                dtaFrmEstudiante["ddl_FNProvincias"].ToString().Trim() + "|" +
                                dtaFrmEstudiante["ddl_FNCiudades"].ToString().Trim() + "|" +
                                dtaFrmEstudiante["ddl_FNParroquias"].ToString().Trim();

            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_updDatosPersonalesEstudiante");
                rst = false;
            }

            return rst;
        }


        private bool _updDireccionEstudiante(System.Web.HttpRequestBase dtaFrmEstudiante)
        {
            bool rst = true;

            try
            {
                string DPA_DE = dtaFrmEstudiante["ddl_DUPais"].ToString().Trim() + "|" +
                                dtaFrmEstudiante["ddl_DUProvincias"].ToString().Trim() + "|" +
                                dtaFrmEstudiante["ddl_DUCiudades"].ToString().Trim() + "|" +
                                dtaFrmEstudiante["ddl_DUParroquias"].ToString().Trim();

                this.dtaDireccionEstudiante.dir_callePrincipal = dtaFrmEstudiante["txtDirCallePrincipal"].ToString().Trim();
                this.dtaDireccionEstudiante.dir_numero = dtaFrmEstudiante["txtDirNumeroCasa"].ToString().Trim();
                this.dtaDireccionEstudiante.dir_calleTransversal = dtaFrmEstudiante["txtDirCalleSecundaria"].ToString().Trim();
                this.dtaDireccionEstudiante.dir_referencia = dtaFrmEstudiante["txtDirReferencia"].ToString().Trim();
                this.dtaDireccionEstudiante.dir_referencia = dtaFrmEstudiante["txtDirReferencia"].ToString().Trim();
            }catch( Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_updDireccionEstudiante");
                rst = false;
            }

            return rst;
        }


        private bool _registrarDatosPersonalesEstudiante()
        {
            bool rst = true;

            try
            {
                string urlWS = CENTRALIZADA.WS_URL.WS_PERSONAS + "ServiciosPersona.svc" + "/Modificar";
                object estado = ClienteServicio.ConsumirServicioPost(urlWS, dtaEstudiante);
            }
            catch( Exception ex)
            {
                rst = false;
                Errores err = new Errores();
                err.SetError(ex, "registrarDatosEstudiante");
            }

            return rst;
        }
        

        private bool _registrarDireccionEstudiante()
        {
            bool rst = true;

            try
            {
                string urlWS = CENTRALIZADA.WS_URL.WS_PERSONAS + "ServiciosDireccion.svc" + "/Modificar";
                object estado = ClienteServicio.ConsumirServicioPost(urlWS, dtaDireccionEstudiante);
            }
            catch (Exception ex)
            {
                rst = false;
                Errores err = new Errores();
                err.SetError(ex, "_registrarDireccionEstudiante");
            }

            return rst;
        }


        public bool updCtaCorreoSistemaAcademico(string strCtaCorreo)
        {
            bool ban = false;

            //  Verifico si el usuario tiene perfil en el sistema academico OASis
            if(this.esUsuarioOASis){
                //  Actualizo información información de cta de correo en la BD del sistema académico  
                this._updCorreoOASis(strCtaCorreo);

                //  Actualizo información de correo en la centralizada
                this._updCorreoCentralizada(strCtaCorreo);

                ban = true;                
            }
            
            return ban;
        }



        private bool _esUsuarioOASis()
        {
            bool ban = false;

            try{
                SitioWebOasis.ProxySeguro.Seguridad seg = new ProxySeguro.Seguridad();
                ProxySeguro.DatosUsuario du = new ProxySeguro.DatosUsuario();

                //  GESTIONA EL ROL DE UN USUARIO REGISTRADO
                bool blnUsuarioValido = seg.AutenticarUsuario(  this._addGuionCedula( this.per_numCedula ),
                                                                "e",
                                                                out this._dsUsuarioOASis);

                ban = (this._dsUsuarioOASis.Carreras.Rows.Count > 0)? true 
                                                                    : false;
            }
            catch(Exception ex){
                ban = false;
                Errores err = new Errores();
                err.SetError(ex, "_registrarDireccionEstudiante");
            }

            return ban;
        }
        

        private bool _updCorreoOASis( string strCtaCorreo )
        {
            bool ban = false;

            try
            {
                if( this._dsUsuarioOASis.Roles.Count > 0){
                    //  Actualizo informacion de correo por rol de usuario
                    foreach(DataRow item in this._dsUsuarioOASis.Carreras){
                        switch (item["strIDRol"].ToString()){
                            case "DOC":
                                this._updDtaCtaCorreoDocente(   item["strCodigo"].ToString(), 
                                                                strCtaCorreo);
                            break;

                            case "EST":
                                this._udpCorreoEstudiante(strCtaCorreo);
                            break;
                        }
                    }
                }
            }catch(Exception ex){
                ban = false;
                Errores err = new Errores();
                err.SetError(ex, "_updCorreoSistemaAcademico");
            }
            
            return ban;
        }


        private bool _updDtaCtaCorreoDocente(string strCodCarrera, string strCtaCorreo)
        {
            bool ban = false;

            try{
                ProxySeguro.DatosUsuario du = new ProxySeguro.DatosUsuario();
                WSDatosUsuario.dtstDatosDocente dsDocente = du.GetDatosDocente( this._addGuionCedula( this.per_numCedula ),
                                                                                strCodCarrera);

                DataRow dtaDocentes = dsDocente.Tables["Docentes"].Rows[0];

                if( dsDocente.Docentes.Rows.Count > 0){
                    dtaDocentes["strEmail"] = strCtaCorreo;

                    //  Actualizo informacion de correo del docente en carrera
                    du.updDatosCorreoDocente(   dsDocente, 
                                                strCodCarrera);

                    ban = true;
                }
            }catch(Exception ex){
                ban = false;
                Errores err = new Errores();
                err.SetError(ex, "_udpCorreoDocente");
            }

            return ban;
        }


        private bool _udpCorreoEstudiante(string strCtaCorreo)
        {
            bool ban = false;

            try{
                ProxySeguro.DatosUsuario du = new ProxySeguro.DatosUsuario();
                WSDatosUsuario.dtstDatosEstudiante dsEstudiante = du.GetDatosEstudiante(this._addGuionCedula(this.per_numCedula));

                if (dsEstudiante.Estudiantes.Rows.Count > 0){
                    DataRow dtaEstudiante = dsEstudiante.Tables["Estudiantes"].Rows[0];
                    dtaEstudiante["strEmail"] = strCtaCorreo;
                    du.SetDatosEstudiante(dsEstudiante);

                    ban = true;
                }
            }catch(Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "_udpCorreoEstudiante");
            }

            return ban;
        }



        private bool _updCorreoCentralizada(string strCtaCorreo)
        {
            bool ban = false;

            try{
                //  dtaEstudiante.per_email = strCtaCorreo;
                dtaEstudiante.per_email = "miguelduquev@gmail.com";
                ban = this._registrarDatosPersonalesEstudiante();
            }
            catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_udpCorreoEstudiante");
            }

            return ban;
        }
    }
}