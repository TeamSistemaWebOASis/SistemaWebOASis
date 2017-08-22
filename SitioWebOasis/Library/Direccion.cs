using GestorErrores;
using SitioWebOasis.CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace SitioWebOasis.Library
{
    public class Direccion
    {
        public Int32 dir_id { get; set; }

        public string dir_callePrincipal { get; set; }

        public string dir_calleTransversal { get; set; }

        public string dir_numero { get; set; }

        public string dir_procedencia { get; set; }

        public string dir_referencia { get; set; }

        public Int32 prq_id { get; set; }

        public String[] dir_dpa;
        
        
        public Direccion(string idPersona)
        {
            this._cargarDatosDireccionEstudiante(idPersona);
        }


        private void _cargarDatosDireccionEstudiante(string idPersona)
        {
            try
            {
                //  Consumo del servicio web ObtenerActivaSegunPersona
                string jsonDtaDireccionPersona = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_PERSONAS + "ServiciosDireccion.svc" + "/ObtenerActivaSegunPersona/" + idPersona);
                var dtaDireccion = Json.Decode(jsonDtaDireccionPersona);

                if( dtaDireccion != null)
                {
                    this.dir_id = dtaDireccion.dir_id;
                    this.dir_callePrincipal = dtaDireccion.dir_callePrincipal;
                    this.dir_calleTransversal = dtaDireccion.dir_calleTransversal;
                    this.dir_numero = dtaDireccion.dir_numero;
                    this.dir_procedencia = dtaDireccion.dir_procedencia;
                    this.dir_referencia = dtaDireccion.dir_referencia;
                    this.prq_id = dtaDireccion.prq_id;

                    this.dir_dpa = this._getDirDPA();
                }else{
                    this.dir_id = default(Int32);
                    this.dir_callePrincipal = string.Empty;
                    this.dir_calleTransversal = string.Empty;
                    this.dir_numero = string.Empty;
                    this.dir_procedencia = string.Empty;
                    this.dir_referencia = string.Empty;
                    this.prq_id = default(Int32);

                    this.dir_dpa = this._getDirDPA();
                }

            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_registrarDireccionEstudiante");
            }
        }



        private String[] _getDirDPA( string dtaDirDPA = "6|2|3|22" )
        {
            return dtaDirDPA.Split('|');
        }

    }
}