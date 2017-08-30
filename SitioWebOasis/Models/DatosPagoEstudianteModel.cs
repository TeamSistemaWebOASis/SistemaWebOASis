using GestorErrores;
using SitioWebOasis.CommonClasses;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace SitioWebOasis.Models
{
    public class DatosPagoEstudianteModel
    {
        private string per_numCedula;

        protected string _strNombreBD { get; set; }

        protected string _strUbicacion { get; set; }

        protected WSInfoCarreras.dtstPeriodoVigente _dtstPeriodoVigente = new WSInfoCarreras.dtstPeriodoVigente();

        public DatosPagoEstudianteModel()
        {
            //  Quito guion del numero de cedula
            this.per_numCedula = UsuarioActual.Cedula.ToString().Replace("-", "");

            if (!string.IsNullOrEmpty(this.per_numCedula.ToString()))
            {
                //  Consumo del servicio web InfoPagos / ObtenerCreditosEstudiante
                string jsonCreditosEstudiante = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_INFO_PAGOS + "AcademicoServicio.svc" + "/ObtenerCreditosEstudiante/" + UsuarioActual.CarreraActual.Codigo.ToString() + "/" + this.per_numCedula.ToString() + "/" + this._dtstPeriodoVigente.Periodos[0]["strCodigo"].ToString());
                var dtaCreditosEstudiante = Json.Decode(jsonCreditosEstudiante);
            }
        }


        public Usuario UsuarioActual
        {
            get { return (Usuario)System.Web.HttpContext.Current.Session["UsuarioActual"]; }
        }



        protected void _cargarInformacionCarrera()
        {
            try
            {
                //  Obtengo informacion de las carreras
                ProxySeguro.InfoCarreras ic = new ProxySeguro.InfoCarreras();

                WSInfoCarreras.dtstBDCarreras dsCarrera = ic.GetCarrera(UsuarioActual.CarreraActual.Codigo.ToString());
                this._strNombreBD = dsCarrera.BDCarreras.Rows[0]["strBaseDatos"].ToString();
                this._strUbicacion = UsuarioActual.CarreraActual.Codigo.ToString();

                //  Informacion del periodo vigente en carrera
                this._dtstPeriodoVigente = ic.GetPeriodoVigenteCarrera(UsuarioActual.CarreraActual.Codigo.ToString());
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_cargarInformacionCarrera");
            }
        }


    }
}