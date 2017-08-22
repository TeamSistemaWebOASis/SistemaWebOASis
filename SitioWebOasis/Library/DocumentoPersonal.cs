using GestorErrores;
using SitioWebOasis.CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace SitioWebOasis.Library
{
    public class DocumentoPersonal
    {
        
        public string pid_valor { get; set; }

        public Int32 tipoDocumentoID { get; set; }

        
        public DocumentoPersonal( string idPersona )
        {
            try
            {
                //  Consumo del servicio web ServiciosDocumentoPersonal
                string jsonDtaDocumentoPersonal = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_PERSONAS + "ServiciosDocumentoPersonal.svc" + "/ObtenerDocumentoActivoPorPersona/" + idPersona);
                var dtaDocumentoPersonal = Json.Decode(jsonDtaDocumentoPersonal);

                if(dtaDocumentoPersonal != null)
                {
                    this.pid_valor = dtaDocumentoPersonal.pid_valor;
                    this.tipoDocumentoID = dtaDocumentoPersonal.tdi_id;
                }else
                {
                    this.pid_valor = string.Empty;
                    this.tipoDocumentoID = default(Int32);
                }

            }catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "updDatosEstudiantes");
            }
        }


    }
}