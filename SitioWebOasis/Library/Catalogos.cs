using GestorErrores;
using Newtonsoft.Json;
using SitioWebOasis.CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SitioWebOasis.Library
{
    /// <summary>
    ///     Clase que gestiona informacion general de tipos de caracteristicas de una persona como 
    ///     sexo, genero, estado civil
    /// </summary>
    public class Catalogos
    {

        #region INFORMACION GENERAL ( Estado Civil, Genero, Sexo, ...... )
        public List<System.Web.Mvc.SelectListItem> getLstEtnias(string idEtnia = "")
        {
            List<System.Web.Mvc.SelectListItem> lstEtnias = new List<System.Web.Mvc.SelectListItem>();
            SelectListItem etnia = new SelectListItem();

            string dtaJsonEtnias = _getLstEtnias();
            if (!string.IsNullOrEmpty(dtaJsonEtnias))
            {
                dynamic _dtaEtnias = JsonConvert.DeserializeObject(dtaJsonEtnias);

                if (_dtaEtnias.Count > 0)
                {
                    etnia = new System.Web.Mvc.SelectListItem();
                    etnia.Value = "-1";
                    etnia.Text = Language.es_ES.PER_CB_ETNIA_SELECCIONE;
                    etnia.Selected = true;
                    lstEtnias.Add(etnia);

                    foreach (var item in _dtaEtnias){
                        etnia = new System.Web.Mvc.SelectListItem();
                        etnia.Value = item.etn_id;
                        etnia.Text = item.etn_nombre;

                        if( item.etn_id.ToString() == idEtnia){
                            etnia.Selected = true;
                        }

                        lstEtnias.Add(etnia);
                    }
                }else{
                    etnia = new System.Web.Mvc.SelectListItem();
                    etnia.Value = "-2";
                    etnia.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                    etnia.Selected = true;

                    lstEtnias.Add(etnia);
                }
            }else{
                etnia = new System.Web.Mvc.SelectListItem();
                etnia.Value = "-2";
                etnia.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                etnia.Selected = true;

                lstEtnias.Add(etnia);
            }

            return lstEtnias;
        }


        private string _getLstEtnias()
        {
            string lstEtnias = string.Empty;
            try
            {
                //  Consumo del servicio web ObtenerPorDocumento (cedula)
                lstEtnias = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_GENERAL + "ServiciosEtnia.svc" + "/ObtenerListado");
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getLstEtnias");
            }

            return lstEtnias;
        }

        //  NACIONALIDADES
        public List<System.Web.Mvc.SelectListItem> getLstNacionalidades(string idPais = "")
        {
            List<System.Web.Mvc.SelectListItem> lstNacionalidades = new List<System.Web.Mvc.SelectListItem>();
            SelectListItem nacionalidad = new SelectListItem();

            string dtaJsonNacionalidades = _getLstNacionalidades();
            if (!string.IsNullOrEmpty(dtaJsonNacionalidades)){
                dynamic _dtaNacionalidades = JsonConvert.DeserializeObject(dtaJsonNacionalidades);

                if (_dtaNacionalidades.Count > 0)
                {
                    nacionalidad = new System.Web.Mvc.SelectListItem();
                    nacionalidad.Value = "-1";
                    nacionalidad.Text = Language.es_ES.PER_CB_NACIONALIDAD_SELECCIONE;
                    nacionalidad.Selected = true;
                    lstNacionalidades.Add(nacionalidad);

                    foreach (var item in _dtaNacionalidades)
                    {
                        nacionalidad = new System.Web.Mvc.SelectListItem();
                        nacionalidad.Value = item.nac_id;
                        nacionalidad.Text = item.nac_nombre;

                        if (item.nac_id.ToString() == idPais){
                            nacionalidad.Selected = true;
                        }

                        lstNacionalidades.Add(nacionalidad);
                    }
                }
                else
                {
                    nacionalidad = new System.Web.Mvc.SelectListItem();
                    nacionalidad.Value = "-2";
                    nacionalidad.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                    nacionalidad.Selected = true;

                    lstNacionalidades.Add(nacionalidad);
                }
                
            }
            else
            {
                nacionalidad = new System.Web.Mvc.SelectListItem();
                nacionalidad.Value = "-2";
                nacionalidad.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                nacionalidad.Selected = true;

                lstNacionalidades.Add(nacionalidad);
            }

            return lstNacionalidades;
        }

        
        private string _getLstNacionalidades()
        {
            string lstEtnias = string.Empty;
            try
            {
                //  Consumo del servicio web ObtenerPorDocumento (cedula)
                lstEtnias = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_GENERAL + "ServiciosNacionalidad.svc" + "/ObtenerListado");
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getLstNacionalidades");
            }

            return lstEtnias;
        }

        
        //  ESTADO CIVIL
        public List<System.Web.Mvc.SelectListItem> getLstEstadoCivil(string idEstCivil = "")
        {
            List<System.Web.Mvc.SelectListItem> lstEstCivil = new List<System.Web.Mvc.SelectListItem>();
            SelectListItem estadoCivil = new SelectListItem();

            string dtaJsonEstCivil = _getLstEstadoCivil();
            if (!string.IsNullOrEmpty(dtaJsonEstCivil))
            {
                dynamic _dtaEstCivil = JsonConvert.DeserializeObject(dtaJsonEstCivil);

                if (_dtaEstCivil.Count > 0)
                {
                    estadoCivil = new System.Web.Mvc.SelectListItem();
                    estadoCivil.Value = "-1";
                    estadoCivil.Text = Language.es_ES.PER_CB_EST_CIVIL_SELECCIONE;
                    estadoCivil.Selected = true;
                    lstEstCivil.Add(estadoCivil);

                    foreach (var item in _dtaEstCivil)
                    {
                        estadoCivil = new System.Web.Mvc.SelectListItem();
                        estadoCivil.Value = item.eci_id;
                        estadoCivil.Text = item.eci_nombre;

                        if (item.eci_id.ToString() == idEstCivil){
                            estadoCivil.Selected = true;
                        }

                        lstEstCivil.Add(estadoCivil);
                    }
                }
                else
                {
                    estadoCivil = new System.Web.Mvc.SelectListItem();
                    estadoCivil.Value = "-2";
                    estadoCivil.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                    estadoCivil.Selected = true;

                    lstEstCivil.Add(estadoCivil);
                }
            }
            else
            {
                estadoCivil = new System.Web.Mvc.SelectListItem();
                estadoCivil.Value = "-2";
                estadoCivil.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                estadoCivil.Selected = true;

                lstEstCivil.Add(estadoCivil);
            }

            return lstEstCivil;
        }


        private string _getLstEstadoCivil()
        {
            string lstEstadoCivil = string.Empty;
            try
            {
                //  Consumo del servicio web ObtenerPorDocumento (cedula)
                lstEstadoCivil = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_GENERAL + "ServiciosEstadoCivil.svc" + "/ObtenerListado");
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getLstEstadoCivil");
            }

            return lstEstadoCivil;
        }


        //  GENERO
        public List<System.Web.Mvc.SelectListItem> getLstGenero(string idGenero = "")
        {
            List<System.Web.Mvc.SelectListItem> lstGenero = new List<System.Web.Mvc.SelectListItem>();
            SelectListItem genero = new SelectListItem();

            string dtaJsonGenero = _getLstGenero();
            if (!string.IsNullOrEmpty(dtaJsonGenero))
            {
                dynamic _dtaGenero = JsonConvert.DeserializeObject(dtaJsonGenero);

                if (_dtaGenero.Count > 0)
                {
                    genero = new System.Web.Mvc.SelectListItem();
                    genero.Value = "-1";
                    genero.Text = Language.es_ES.PER_CB_EST_CIVIL_SELECCIONE;
                    genero.Selected = true;
                    lstGenero.Add(genero);

                    foreach (var item in _dtaGenero)
                    {
                        genero = new System.Web.Mvc.SelectListItem();
                        genero.Value = item.gen_id;
                        genero.Text = item.gen_nombre;

                        if (item.gen_id.ToString() == idGenero){
                            genero.Selected = true;
                        }

                        lstGenero.Add(genero);
                    }
                }else{
                    genero = new System.Web.Mvc.SelectListItem();
                    genero.Value = "-2";
                    genero.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                    genero.Selected = true;

                    lstGenero.Add(genero);
                }
            }
            else
            {
                genero = new System.Web.Mvc.SelectListItem();
                genero.Value = "-2";
                genero.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                genero.Selected = true;

                lstGenero.Add(genero);
            }

            return lstGenero;
        }

        private string _getLstGenero()
        {
            string lstGenero = string.Empty;
            try
            {
                //  Consumo del servicio web ObtenerPorDocumento (cedula)
                lstGenero = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_GENERAL + "ServiciosGenero.svc" + "/ObtenerListado");
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getLstGenero");
            }

            return lstGenero;
        }


        //  ETNIA
        public List<System.Web.Mvc.SelectListItem> getLstEtnia(string idEtnia = "")
        {
            List<System.Web.Mvc.SelectListItem> lstEtnia = new List<System.Web.Mvc.SelectListItem>();
            SelectListItem etnia = new SelectListItem();

            string dtaJsonEtnia = _getLstEtnia();
            if (!string.IsNullOrEmpty(dtaJsonEtnia))
            {
                dynamic _dtaEtnia = JsonConvert.DeserializeObject(dtaJsonEtnia);

                if (_dtaEtnia.Count > 0)
                {
                    etnia = new System.Web.Mvc.SelectListItem();
                    etnia.Value = "-1";
                    etnia.Text = Language.es_ES.PER_CB_ETNIA_SELECCIONE;
                    etnia.Selected = true;
                    lstEtnia.Add(etnia);

                    foreach (var item in _dtaEtnia)
                    {
                        etnia = new System.Web.Mvc.SelectListItem();
                        etnia.Value = item.etn_id;
                        etnia.Text = item.etn_nombre;

                        if (item.etn_id.ToString() == idEtnia){
                            etnia.Selected = true;
                        }

                        lstEtnia.Add(etnia);
                    }
                }
                else
                {
                    etnia = new System.Web.Mvc.SelectListItem();
                    etnia.Value = "-2";
                    etnia.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                    etnia.Selected = true;

                    lstEtnia.Add(etnia);
                }
            }
            else
            {
                etnia = new System.Web.Mvc.SelectListItem();
                etnia.Value = "-2";
                etnia.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                etnia.Selected = true;

                lstEtnia.Add(etnia);
            }

            return lstEtnia;
        }

        private string _getLstEtnia()
        {
            string lstGenero = string.Empty;
            try
            {
                //  Consumo del servicio web ObtenerPorDocumento (cedula)
                lstGenero = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_GENERAL + "ServiciosEtnia.svc" + "/ObtenerListado");
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getLstGenero");
            }

            return lstGenero;
        }


        //  TIPO SANGRE
        public List<System.Web.Mvc.SelectListItem> getLstTipoSangre(string idTipoSangre = "")
        {
            List<System.Web.Mvc.SelectListItem> lstTpoSangre = new List<System.Web.Mvc.SelectListItem>();
            SelectListItem tpoSangre = new SelectListItem();

            string dtaJsonTpoSangre = _getLstTipoSangre();
            if (!string.IsNullOrEmpty(dtaJsonTpoSangre))
            {
                dynamic _dtaTpoSangre = JsonConvert.DeserializeObject(dtaJsonTpoSangre);

                if (_dtaTpoSangre.Count > 0)
                {
                    tpoSangre = new System.Web.Mvc.SelectListItem();
                    tpoSangre.Value = "-1";
                    tpoSangre.Text = Language.es_ES.PER_CB_TIPO_SANGRE_SELECCIONE;
                    tpoSangre.Selected = true;
                    lstTpoSangre.Add(tpoSangre);

                    foreach (var item in _dtaTpoSangre)
                    {
                        tpoSangre = new System.Web.Mvc.SelectListItem();
                        tpoSangre.Value = item.tsa_id;
                        tpoSangre.Text = item.tsa_nombre;

                        if (item.tsa_id.ToString() == idTipoSangre){
                            tpoSangre.Selected = true;
                        }

                        lstTpoSangre.Add(tpoSangre);
                    }
                }
                else
                {
                    tpoSangre = new System.Web.Mvc.SelectListItem();
                    tpoSangre.Value = "-2";
                    tpoSangre.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                    tpoSangre.Selected = true;

                    lstTpoSangre.Add(tpoSangre);
                }
            }
            else
            {
                tpoSangre = new System.Web.Mvc.SelectListItem();
                tpoSangre.Value = "-2";
                tpoSangre.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                tpoSangre.Selected = true;

                lstTpoSangre.Add(tpoSangre);
            }

            return lstTpoSangre;
        }

        private string _getLstTipoSangre()
        {
            string lstGenero = string.Empty;
            try
            {
                //  Consumo del servicio web ObtenerPorDocumento (cedula)
                lstGenero = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_GENERAL + "ServiciosTipoSangre.svc" + "/ObtenerListado");
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getLstTipoSangre");
            }

            return lstGenero;
        }


        //  TIPO DOCUMENTO
        public List<System.Web.Mvc.SelectListItem> getLstTipoDocumento(string idTipoDocumento = "")
        {
            List<System.Web.Mvc.SelectListItem> lstTpoDocumento = new List<System.Web.Mvc.SelectListItem>();
            SelectListItem tpoDocumento = new SelectListItem();

            string dtaJsonTpoDocumento = _getLstTipoDocumento();
            if (!string.IsNullOrEmpty(dtaJsonTpoDocumento))
            {
                dynamic _dtaTpoDocumento = JsonConvert.DeserializeObject(dtaJsonTpoDocumento);

                if (_dtaTpoDocumento.Count > 0){
                    tpoDocumento = new System.Web.Mvc.SelectListItem();
                    tpoDocumento.Value = "-1";
                    tpoDocumento.Text = Language.es_ES.PER_CB_TIPO_DOCUMENTO_SELECCIONE;
                    tpoDocumento.Selected = true;
                    lstTpoDocumento.Add(tpoDocumento);

                    foreach (var item in _dtaTpoDocumento){
                        tpoDocumento = new System.Web.Mvc.SelectListItem();
                        tpoDocumento.Value = item.tdi_id;
                        tpoDocumento.Text = item.tdi_nombre;

                        if (item.tdi_id.ToString() == idTipoDocumento){
                            tpoDocumento.Selected = true;
                        }

                        lstTpoDocumento.Add(tpoDocumento);
                    }
                }
                else
                {
                    tpoDocumento = new System.Web.Mvc.SelectListItem();
                    tpoDocumento.Value = "-2";
                    tpoDocumento.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                    tpoDocumento.Selected = true;

                    lstTpoDocumento.Add(tpoDocumento);
                }
            }
            else
            {
                tpoDocumento = new System.Web.Mvc.SelectListItem();
                tpoDocumento.Value = "-2";
                tpoDocumento.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                tpoDocumento.Selected = true;

                lstTpoDocumento.Add(tpoDocumento);
            }

            return lstTpoDocumento;
        }

        private string _getLstTipoDocumento()
        {
            string lstGenero = string.Empty;
            try
            {
                //  Consumo del servicio web ObtenerPorDocumento (cedula)
                lstGenero = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_GENERAL + "ServiciosTipoDocumentoID.svc" + "/ObtenerListado");
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getLstTipoSangre");
            }

            return lstGenero;
        }


        //  SEXO
        public List<System.Web.Mvc.SelectListItem> getLstSexo(string idSexo = "")
        {
            List<System.Web.Mvc.SelectListItem> lstSexo = new List<System.Web.Mvc.SelectListItem>();
            SelectListItem sexo = new SelectListItem();

            string dtaJsonLstSexo = _getLstSexo();
            if (!string.IsNullOrEmpty(dtaJsonLstSexo))
            {
                dynamic _dtaSexo = JsonConvert.DeserializeObject(dtaJsonLstSexo);

                if (_dtaSexo.Count > 0)
                {
                    sexo = new System.Web.Mvc.SelectListItem();
                    sexo.Value = "-1";
                    sexo.Text = Language.es_ES.PER_CB_SEXO_SELECCIONE;
                    sexo.Selected = true;
                    lstSexo.Add(sexo);

                    foreach (var item in _dtaSexo)
                    {
                        sexo = new System.Web.Mvc.SelectListItem();
                        sexo.Value = item.sex_id;
                        sexo.Text = item.sex_nombre;

                        if (item.sex_id.ToString() == idSexo){
                            sexo.Selected = true;
                        }

                        lstSexo.Add(sexo);
                    }
                }
                else
                {
                    sexo = new System.Web.Mvc.SelectListItem();
                    sexo.Value = "-2";
                    sexo.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                    sexo.Selected = true;

                    lstSexo.Add(sexo);
                }
            }
            else
            {
                sexo = new System.Web.Mvc.SelectListItem();
                sexo.Value = "-2";
                sexo.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                sexo.Selected = true;

                lstSexo.Add(sexo);
            }

            return lstSexo;
        }

        private string _getLstSexo()
        {
            string lstSexo = string.Empty;
            try
            {
                //  Consumo del servicio web ObtenerPorDocumento (cedula)
                lstSexo = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_GENERAL + "ServiciosSexo.svc" + "/ObtenerListado");
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getLstSexo");
            }

            return lstSexo;
        }

        #endregion

        #region División Politica Administrativa

        //  PAISES
        public List<System.Web.Mvc.SelectListItem> getLstPaises(string idPais = "6")
        {
            List<System.Web.Mvc.SelectListItem> lstPaises = new List<System.Web.Mvc.SelectListItem>();
            SelectListItem pais = new SelectListItem();

            string dtaJsonPaises = _getLstPaises();
            if (!string.IsNullOrEmpty(dtaJsonPaises))
            {
                dynamic _dtaNacionalidades = JsonConvert.DeserializeObject(dtaJsonPaises);

                if (_dtaNacionalidades.Count > 0)
                {
                    pais = new System.Web.Mvc.SelectListItem();
                    pais.Value = "-1";
                    pais.Text = Language.es_ES.PER_CB_PAIS_SELECCIONE;
                    pais.Selected = true;
                    lstPaises.Add(pais);

                    foreach (var item in _dtaNacionalidades)
                    {
                        pais = new System.Web.Mvc.SelectListItem();
                        pais.Value = item.pai_id;
                        pais.Text = item.pai_nombre;

                        if (item.pai_id == idPais){
                            pais.Selected = true;
                        }

                        lstPaises.Add(pais);
                    }
                }
                else
                {
                    pais = new System.Web.Mvc.SelectListItem();
                    pais.Value = "-2";
                    pais.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                    pais.Selected = true;

                    lstPaises.Add(pais);
                }
            }
            else
            {
                pais = new System.Web.Mvc.SelectListItem();
                pais.Value = "-2";
                pais.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                pais.Selected = true;
            }

            return lstPaises;
        }


        private string _getLstPaises()
        {
            string lstEtnias = string.Empty;
            try
            {
                //  Consumo del servicio web ObtenerPorDocumento (cedula)
                lstEtnias = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_GENERAL + "ServiciosPais.svc" + "/ObtenerListado");
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getLstPaises");
            }

            return lstEtnias;
        }


        //  PROVINCIAS
        public List<System.Web.Mvc.SelectListItem> getLstProvincias(string idPais = "6", string idProvincia = "0")
        {
            List<System.Web.Mvc.SelectListItem> lstProvincias = new List<System.Web.Mvc.SelectListItem>();
            SelectListItem provincia = new SelectListItem();

            string dtaJsonProvincias = _getLstProvincias(idPais);
            if (!string.IsNullOrEmpty(dtaJsonProvincias))
            {
                dynamic _dtaNacionalidades = JsonConvert.DeserializeObject(dtaJsonProvincias);

                if (_dtaNacionalidades.Count > 0)
                {
                    provincia = new System.Web.Mvc.SelectListItem();
                    provincia.Value = "-1";
                    provincia.Text = Language.es_ES.PER_CB_PROVINCIA_SELECCIONE;
                    provincia.Selected = true;
                    lstProvincias.Add(provincia);

                    foreach (var item in _dtaNacionalidades)
                    {
                        provincia = new System.Web.Mvc.SelectListItem();
                        provincia.Value = item.pro_id;
                        provincia.Text = item.pro_nombre;

                        if (item.pro_id.ToString() == idProvincia){
                            provincia.Selected = true;
                        }

                        lstProvincias.Add(provincia);
                    }
                }
                else
                {
                    provincia = new System.Web.Mvc.SelectListItem();
                    provincia.Value = "-2";
                    provincia.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                    provincia.Selected = true;

                    lstProvincias.Add(provincia);
                }
            }
            else
            {
                provincia = new System.Web.Mvc.SelectListItem();
                provincia.Value = "-2";
                provincia.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                provincia.Selected = true;

                lstProvincias.Add(provincia);
            }

            return lstProvincias;
        }

        private string _getLstProvincias(string idPais)
        {
            string lstProvincias = string.Empty;
            try
            {
                //  Consumo del servicio web ObtenerListadoSegunPais (cedula)
                lstProvincias = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_GENERAL + "ServiciosProvincia.svc" + "/ObtenerListadoSegunPais/" + idPais);
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getLstProvincias");
            }

            return lstProvincias;
        }


        //  CIUDADES
        public List<System.Web.Mvc.SelectListItem> getLstCiudades(string idProvincia, string idCiudad = "0")
        {
            List<System.Web.Mvc.SelectListItem> lstCiudades = new List<System.Web.Mvc.SelectListItem>();
            SelectListItem ciudad = new SelectListItem();

            string dtaJsonCiudad = _getLstCiudades(idProvincia);
            if (!string.IsNullOrEmpty(dtaJsonCiudad))
            {
                dynamic _dtaNacionalidades = JsonConvert.DeserializeObject(dtaJsonCiudad);

                if (_dtaNacionalidades.Count > 0)
                {
                    ciudad = new System.Web.Mvc.SelectListItem();
                    ciudad.Value = "-1";
                    ciudad.Text = Language.es_ES.PER_CB_CIUDAD_SELECCIONE;
                    ciudad.Selected = true;
                    lstCiudades.Add(ciudad);

                    foreach (var item in _dtaNacionalidades)
                    {
                        ciudad = new System.Web.Mvc.SelectListItem();
                        ciudad.Value = item.ciu_id;
                        ciudad.Text = item.ciu_nombre;

                        if (item.ciu_id.ToString() == idCiudad){
                            ciudad.Selected = true;
                        }

                        lstCiudades.Add(ciudad);
                    }
                }
                else
                {
                    ciudad = new System.Web.Mvc.SelectListItem();
                    ciudad.Value = "-2";
                    ciudad.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                    ciudad.Selected = true;

                    lstCiudades.Add(ciudad);
                }
            }
            else
            {
                ciudad = new System.Web.Mvc.SelectListItem();
                ciudad.Value = "-2";
                ciudad.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                ciudad.Selected = true;

                lstCiudades.Add(ciudad);
            }

            return lstCiudades;
        }

        private string _getLstCiudades(string idProvincia)
        {
            string lstProvincias = string.Empty;
            try
            {
                //  Consumo del servicio web ObtenerListadoSegunPais (cedula)
                lstProvincias = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_GENERAL + "ServiciosCiudad.svc" + "/ObtenerListadoSegunProvincia/" + idProvincia);
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getLstCiudades");
            }

            return lstProvincias;
        }


        //  PARROQUIAS
        public List<System.Web.Mvc.SelectListItem> getLstParroquias(string idCiudad, string idParroquia = "0")
        {
            List<System.Web.Mvc.SelectListItem> lstParroquias = new List<System.Web.Mvc.SelectListItem>();
            SelectListItem parroquia = new SelectListItem();

            string dtaJsonParroquias = _getLstParroquias(idCiudad);
            if (!string.IsNullOrEmpty(dtaJsonParroquias))
            {
                dynamic _dtaNacionalidades = JsonConvert.DeserializeObject(dtaJsonParroquias);

                if (_dtaNacionalidades.Count > 0){
                    parroquia = new System.Web.Mvc.SelectListItem();
                    parroquia.Value = "-1";
                    parroquia.Text = Language.es_ES.PER_CB_PARROQUIA_SELECCIONE;
                    parroquia.Selected = true;
                    lstParroquias.Add(parroquia);

                    foreach (var item in _dtaNacionalidades)
                    {
                        parroquia = new System.Web.Mvc.SelectListItem();
                        parroquia.Value = item.prq_id;
                        parroquia.Text = item.prq_nombre;

                        if (item.prq_id.ToString() == idParroquia){
                            parroquia.Selected = true;
                        }

                        lstParroquias.Add(parroquia);
                    }
                }
                else
                {
                    parroquia = new System.Web.Mvc.SelectListItem();
                    parroquia.Value = "-2";
                    parroquia.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                    parroquia.Selected = true;

                    lstParroquias.Add(parroquia);
                }
            }
            else
            {
                parroquia = new System.Web.Mvc.SelectListItem();
                parroquia.Value = "-2";
                parroquia.Text = Language.es_ES.PER_CB_SIN_VALOR_REGISTRADO;
                parroquia.Selected = true;

                lstParroquias.Add(parroquia);
            }

            return lstParroquias;
        }

        private string _getLstParroquias(string idCiudad)
        {
            string lstParroquias = string.Empty;
            try
            {
                //  Consumo del servicio web ObtenerListadoSegunPais (cedula)
                lstParroquias = ClienteServicio.ConsumirServicio(CENTRALIZADA.WS_URL.WS_GENERAL + "ServiciosParroquia.svc" + "/ObtenerListadoSegunCiudad/" + idCiudad);
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "_getLstParroquias");
            }

            return lstParroquias;
        }


        #endregion

    }
}