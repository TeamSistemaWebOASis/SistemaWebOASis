using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitioWebOasis.CommonClasses
{
    public class EndPointsServicios
    {
        #region Propiedades

        public string ServidorCentralizada { get; set; }

        public string ServicioCentralizadaFoto { get; set; }

        public string ServicioCentralizadaPersona { get; set; }

        public string ServicioCentralizadaDocumento { get; set; }

        public string ServicioCentralizadaDireccion { get; set; }


        private static EndPointsServicios instancia = null;

        #endregion

        #region METODOS WEB
        public static EndPointsServicios Instancia
        {
            get
            {
                if (instancia == null)
                    instancia = new EndPointsServicios();

                return instancia;
            }
        }
        #endregion

        #region METODOS DE LA CENTRALIZADA
        public string ObtenerCentralizadaPorDocumento
        {
            get
            {
                return string.Format("{0}/{1}/{2}", ServidorCentralizada, ServicioCentralizadaPersona, "ObtenerPorDocumento");
            }
        }

        public string ModificarCentralizadaPersona
        {
            get
            {
                return string.Format("{0}/{1}/{2}", ServidorCentralizada, ServicioCentralizadaPersona, "Modificar");
            }
        }

        public string IngresarCentralizadaPersona
        {
            get
            {
                return string.Format("{0}/{1}/{2}", ServidorCentralizada, ServicioCentralizadaPersona, "Insertar");
            }
        }

        public string ModificarCentralizadaDireccion
        {
            get
            {
                return string.Format("{0}/{1}/{2}", ServidorCentralizada, ServicioCentralizadaDireccion, "Modificar");
            }
        }

        public string IngresarCentralizadaDireccion
        {
            get
            {
                return string.Format("{0}/{1}/{2}", ServidorCentralizada, ServicioCentralizadaDireccion, "Insertar");
            }
        }

        public string ObtenerCedulaPorCodigo
        {
            get
            {
                return string.Format("{0}/{1}/{2}", ServidorCentralizada, ServicioCentralizadaDocumento, "ObtenerDocumentoActivoPorPersona");
            }
        }

        public string ObtenerDatosPersonaPorCodigo
        {
            get
            {
                return string.Format("{0}/{1}/{2}", ServidorCentralizada, ServicioCentralizadaPersona, "Obtener");
            }
        }

        public string ObtenerDireccionPersona
        {
            get
            {
                return string.Format("{0}/{1}/{2}", ServidorCentralizada, ServicioCentralizadaDireccion, "ObtenerActivaSegunPersona");
            }
        }

        public string ObtenerFotoPersona
        {
            get
            {
                return string.Format("{0}/{1}/{2}", ServidorCentralizada, ServicioCentralizadaDireccion, "ObtenerActivaSegunPersona");
            }
        }

        #endregion



        public string ServicioNivel { get; private set; }


        public string ServicioCurso { get; private set; }



        #region METODOS

        protected EndPointsServicios()
        {
            ServidorCentralizada = "http://servicioscentralizadapruebas.espoch.edu.ec";
            ServicioCentralizadaPersona = "Central/ServiciosPersona.svc";
            ServicioCentralizadaDocumento = "Central/ServiciosDocumentoPersonal.svc";
            ServicioCentralizadaDireccion = "Central/ServiciosDireccion.svc";
            ServicioCentralizadaFoto = "Central/ServiciosImagen.svc";

        }
        
        #endregion

    }
}