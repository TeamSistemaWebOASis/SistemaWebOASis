using GestorErrores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitioWebOasis.Library
{
    public class CorreoElectronico
    {
        public CorreoElectronico() { }

        public bool EnviarCorreo(string correoDestino)
        {
            bool ban = false;
            try {

            }catch(Exception ex){
                Errores err = new Errores();
                err.SetError(ex, "EnviarCorreo");
            }

            return ban;
        }
    }
}