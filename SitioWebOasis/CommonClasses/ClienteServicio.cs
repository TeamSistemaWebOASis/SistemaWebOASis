using GestorErrores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace SitioWebOasis.CommonClasses
{
    public class ClienteServicio
    {
        //	Consumo de servicios GET - 
        public static string ConsumirServicio(string strUrl)
        {
            string strResponse = string.Empty;
            try
            {
                WebRequest Webrequest;
                HttpWebResponse response;

                Webrequest = WebRequest.Create(strUrl);
                Webrequest.Method = "GET";
                Webrequest.ContentType = "application/json";

                ////Para aceptar el certificado de pruebas
                //ServicePointManager.ServerCertificateValidationCallback
                //= delegate (Object obj, X509Certificate certificate, X509Chain
                //chain, SslPolicyErrors errors)
                //{
                //    return (true);
                //};

                response = (HttpWebResponse)Webrequest.GetResponse();
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(streamResponse);
                strResponse = streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError( ex, "ConsumirServicio");
            }
            return strResponse;
        }

        public async static Task<string> ConsumirServicioAsync(string strUrl)
        {
            string strResponse = string.Empty;
            try
            {
                WebRequest Webrequest;
                WebResponse response;

                Webrequest = WebRequest.Create(strUrl);
                Webrequest.Method = "GET";
                Webrequest.ContentType = "application/json";

                ////Para aceptar el certificado de pruebas
                //ServicePointManager.ServerCertificateValidationCallback
                //= delegate (Object obj, X509Certificate certificate, X509Chain
                //chain, SslPolicyErrors errors)
                //{
                //    return (true);
                //};

                response = await Webrequest.GetResponseAsync();
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(streamResponse);
                strResponse = streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "ConsumirServicioAsync");
            }

            return strResponse;
        }


        public async static Task<string> ConsumirServicioPostAsync<T>(string strUrl, T objeto)
        {
            string strResponse = string.Empty;
            try
            {
                WebRequest Webrequest;
                WebResponse response;

                Webrequest = WebRequest.Create(strUrl);
                Webrequest.Method = "POST";
                Webrequest.ContentType = "application/json";

                ////Para aceptar el certificado de pruebas
                //ServicePointManager.ServerCertificateValidationCallback
                //= delegate (Object obj, X509Certificate certificate, X509Chain
                //chain, SslPolicyErrors errors)
                //{
                //    return (true);
                //};

                using (var streamWriter = new StreamWriter(Webrequest.GetRequestStream()))
                {

                    JsonSerializerSettings jsonConfiguracion = new JsonSerializerSettings()
                    {
                        DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                    };
                    string json = JsonConvert.SerializeObject(objeto, jsonConfiguracion);
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                response = await Webrequest.GetResponseAsync();
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(streamResponse);
                strResponse = streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "ConsumirServicioPostAsync");
            }

            return strResponse;
        }


        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <typeparam name="T"></typeparam>
        /// <param name="strUrl">   URL del servicio sin incluir parametros </param>
        /// <param name="objeto">   Object de una clase especifica (dataSet, etc) - en funcion a los parametros
        //	                        en caso de no tener la necesidad o no tener la clase creamos un "dictionary" </param>
        /// <returns></returns>
        public static string ConsumirServicioPost<T>(string strUrl, T objeto)
        {
            string strResponse = "false";
            try
            {
                WebRequest Webrequest;
                HttpWebResponse response;

                Webrequest = WebRequest.Create(strUrl);
                Webrequest.Method = "POST";
                Webrequest.ContentType = "application/json";

                ////Para aceptar el certificado de pruebas
                //ServicePointManager.ServerCertificateValidationCallback
                //= delegate (Object obj, X509Certificate certificate, X509Chain
                //chain, SslPolicyErrors errors)
                //{
                //    return (true);
                //};

                using (var streamWriter = new StreamWriter(Webrequest.GetRequestStream()))
                {

                    JsonSerializerSettings jsonConfiguracion = new JsonSerializerSettings(){
                        DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                    };

                    string json = JsonConvert.SerializeObject(objeto, jsonConfiguracion);
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                response = (HttpWebResponse)Webrequest.GetResponse();
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(streamResponse);
                strResponse = streamReader.ReadToEnd();

                strResponse = "true";
            }
            catch (Exception ex)
            {
                strResponse = "false";

                Errores err = new Errores();
                err.SetError(ex, "ConsumirServicioPost");
            }

            return strResponse;
        }


        public static string ConstruirUri(string strUrl, params string[] lstParametros)
        {
            string strUri = strUrl;
            foreach (var parametro in lstParametros){
                strUri += "/" + Uri.EscapeDataString(parametro);
            }
            return strUri;
        }

    }
}