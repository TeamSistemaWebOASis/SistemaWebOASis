using GestorErrores;
using SitioWebOasis.CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace SitioWebOasis.Library
{
    public class Nacionalidad
    {
        public Int32 npe_id { get; set;}

        public Int32 nac_id { get; set;}
        
        public string nacionalidad { get; set;}

        public bool npe_default { get; set;}

        public bool npe_esNacimiento { get; set;}

        public bool npe_tieneVisaTrabajo { get; set;}


        public Nacionalidad( string idPersona )
        {
            try
            {
                //  Consumo del servicio web ObtenerActivaSegunPersona
                string jsonDtaNacionalidadPersona = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_PERSONAS + "ServiciosNacionalidadPersona.svc" + "/ObtenerActivaSegunPersona/" + idPersona);
                var dtaNacionalidad = Json.Decode(jsonDtaNacionalidadPersona);

                if( dtaNacionalidad != null)
                {
                    this.npe_id = dtaNacionalidad.npe_id;
                    this.nac_id = dtaNacionalidad.nac_id;
                    this.nacionalidad = dtaNacionalidad.nacionalidad;
                    this.npe_default = dtaNacionalidad.npe_default;
                    this.npe_esNacimiento = dtaNacionalidad.npe_esNacimiento;
                    this.npe_tieneVisaTrabajo = dtaNacionalidad.npe_tieneVisaTrabajo;
                }else
                {
                    this.npe_id = default(int);
                    this.nac_id = default(int);
                    this.nacionalidad = string.Empty;
                    this.npe_default = default(bool);
                    this.npe_esNacimiento = default(bool);
                    this.npe_tieneVisaTrabajo = default(bool);
                }

            }catch (Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "Nacionalidad");
            }
        }

    }
}