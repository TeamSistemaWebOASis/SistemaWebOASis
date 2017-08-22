using GestorErrores;
using OAS_Seguridad.Cliente;
using SitioWebOasis.CommonClasses.GestionUsuarios;
using System;
using System.Data;

namespace SitioWebOasis.Library
{
    public class DatosCarrera
    {
        protected string _strNombreBD { get; set; }

        protected string _strUbicacion { get; set; }

        protected string _strCodAsignatura { get; set; }

        protected string _strCodNivel { get; set; }

        protected string _strCodParalelo { get; set; }

        protected WSInfoCarreras.dtstPeriodoVigente _dtstPeriodoVigente = new WSInfoCarreras.dtstPeriodoVigente();

        //  public OASisLogin fLogin = SitioWebOasis.CommonClasses.CacheConfig.Get("OASisLogin") as OASisLogin;

        public DatosCarrera() { }

        
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


        public WSInfoCarreras.dtstPeriodoVigente _dataPeriodoAcademicoVigente()
        {
            WSInfoCarreras.dtstPeriodoVigente dsPeriodoVigente = new WSInfoCarreras.dtstPeriodoVigente();

            try
            {
                ProxySeguro.InfoCarreras ic = new ProxySeguro.InfoCarreras();
                dsPeriodoVigente = ic.GetPeriodoVigenteCarrera(UsuarioActual.CarreraActual.Codigo.ToString());
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_dataPeriodoAcademicoVigente");
            }

            return dsPeriodoVigente;
        }



        protected string _getNumOrdinal(string numero, string tpo)
        {
            string[] ciclosAcademicos = new string[10] { "1er", "2do", "3er", "4to", "5to", "6to", "7mo", "8vo", "9no", "10mo" };
            string[] matricula = new string[3] { "1ra", "2da", "3ra" };

            return (tpo == "nivel") ? ciclosAcademicos[Convert.ToInt32(numero.ToString()) - 1]
                                    : matricula[Convert.ToInt32(numero.ToString()) - 1];
        }


        protected string _getPromedio(DataRow item)
        {
            string prm = string.Empty;
            decimal n1 = default(decimal);
            decimal n2 = default(decimal);
            decimal n3 = default(decimal);
            decimal rst = default(decimal);

            try
            {
                n1 = Convert.ToDecimal(item["bytNota1"]);
                n2 = Convert.ToDecimal(item["bytNota2"]);
                n3 = Convert.ToDecimal(item["bytNota3"]);
                rst = (n1 + n2 + n3);

                prm = Decimal.Round(Decimal.Divide(rst, Convert.ToDecimal("3")), 2).ToString();
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getAsignaturasDocente");
            }

            return prm;
        }

    }
}