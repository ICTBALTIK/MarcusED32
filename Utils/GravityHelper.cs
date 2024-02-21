using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SmartAutoMV.Helpers
{
    public class GravityHelper
    {
        public enum Method { GET, POST }

        /// <summary>
        /// si tratta dell'url dell'endpoint sul quale risponde il servizio api rest
        /// </summary>
        public static string ApiUrl { get; set; } = "http://172.23.1.72/GravityApi/api/";
        //public static string ApiUrl { get; set; } = "http://localhost:58173/api/";

        /// <summary>
        /// per il tipo di autenticazione
        /// </summary>
        public static string GrantType { get; set; } = "password";

        /// <summary>
        /// Il token di accesso, viene restituito dal servizio dopo il login 
        /// e popolato in automatico dalla funzione login
        /// </summary>
        public static string AccessToken { get; private set; } = null;

        /// <summary>
        /// il tipo di token, viene restituito dal servizio dipo il login
        /// e popolato in automatico dalla funzione login
        /// </summary>
        public static string TokenType { get; private set; } = null;

        /// <summary>
        /// secondi per cui il token restituito dal servizio risulta valido
        /// </summary>
        public static int ExpiresIn { get; private set; } = 0;

        /// <summary>
        /// Si tratta della data e l'ora alla quale è stata effettuato il login
        /// viene popolata in automatico dalla funzione login
        /// </summary>
        public static DateTime LoginTime { get; private set; }

        /// <summary>
        /// indica la data e l'ora in cui scade la sezione e la validità del token di autenticazione
        /// viene valorizzato dalla funzione login in base alla risposta del servizio web
        /// </summary>
        public static DateTime ExpiredTime { get; private set; }

        /// <summary>
        /// l'ultima stringa json restituita dal servizio web nell'ultima chiamata effettuata
        /// </summary>
        public static string JsonLastResult { get; private set; }

        /// <summary>
        /// Indica se la sezione di autenticazione stabilita dopo il login è terminata oppure ancora valido
        /// se è terminata il token è scaduto e ogni richiesta successiva viene ignorata dal servizio
        /// </summary>
        /// <returns></returns>
        public static bool IsExpiredSession()
        {
            if (ExpiredTime <= DateTime.Now) { return true; }
            return false;
        }

        /// <summary>
        /// Effettua il login sul servizo web, questo consente di recuperare un token valido per un lasso di tempo,
        /// dopo aver effettuato il login una volta verrà utilizzato il token recuperato per le successive chiamate
        /// se ci sono problemi durante l'autenticazione viene sollevata un eccezione
        /// </summary>
        /// <param name="username">username dell'account</param>
        /// <param name="password">password associata all'username</param>
        public static void Login(string username, string password)
        {
            try
            {
                string serverUrlGetSecureToken = ApiUrl + "getsecuretoken";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serverUrlGetSecureToken);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                var bytes = Encoding.UTF8.GetBytes("grant_type=" + GrantType + "&username=" + username + "&password=" + password);
                request.ContentLength = bytes.Length;

                using (var stream = request.GetRequestStream()) { stream.Write(bytes, 0, bytes.Length); }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.ContentType.Equals("text/html; charset=utf-8")) { throw new Exception("Non Autorizzato effettuare il login"); }

                StreamReader reader = new StreamReader(response.GetResponseStream());
                JsonLastResult = reader.ReadToEnd();

                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonLastResult);

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    AccessToken = result["access_token"].ToString();
                    TokenType = result["token_type"].ToString();
                    ExpiresIn = int.Parse(result["expires_in"].ToString());
                    LoginTime = DateTime.Now;
                    ExpiredTime = LoginTime.AddSeconds(ExpiresIn);
                }
                else
                {
                    string error = result["error"].ToString();
                    string error_description = result["error_description"].ToString();
                    throw new Exception("Error Login : " + error + " - " + error_description);
                }
            }
            catch (Exception e) { throw e; }
        }

        /// <summary>
        /// effettua una chiamata di tipo get al servizio api, con i parametri specificati
        /// e restituisce la risposta del servizio in un dictionary
        /// </summary>
        /// <param name="controller">controller del servizio da richiamare</param>
        /// <param name="action">action del servizio da richiamare</param>
        /// <param name="parameters">parametri da passare al servizio</param>
        /// <returns>dizionario decodificato dalla risposta json del servizio</returns>
        public static Dictionary<string, object> Get(string controller, string action, Dictionary<string, string> parameters) { return request<Dictionary<string, object>>(Method.GET, controller, action, parameters); }

        /// <summary>
        /// effettua una chiamata di tipo post al servizio api, con i parametri specificati
        /// e restituisce la risposta del servizio in un dictionary
        /// </summary>
        /// <param name="controller">controller del servizio da richiamare</param>
        /// <param name="action">action del servizio da richiamare</param>
        /// <param name="parameters">parametri da passare al servizio</param>
        /// <returns>dizionario decodificato dalla risposta json del servizio</returns>
        public static Dictionary<string, object> Post(string controller, string action, Dictionary<string, string> parameters) { return request<Dictionary<string, object>>(Method.POST, controller, action, parameters); }

        /// <summary>
        /// effettua una chiamata di tipo get al servizio api, con i parametri specificati
        /// e restituisce la risposta del servizio in un oggetto mappato col tipo specificato
        /// </summary>
        /// <typeparam name="T">tipo dell'oggetto sul quale mappare la risposta json del servizo api</typeparam>
        /// <param name="controller">controller del servizio da richiamare</param>
        /// <param name="action">action del servizio da richiamare</param>
        /// <param name="parameters">parametri da passare al servizio</param>
        /// <returns>oggetto di tipo T mappato con i risultati</returns>
        public static T Get<T>(string controller, string action, Dictionary<string, string> parameters) { return request<T>(Method.GET, controller, action, parameters); }

        /// <summary>
        /// effettua una chiamata di tipo post al servizio api, con i parametri specificati
        /// e restituisce la risposta del servizio in un oggetto mappato col tipo specificato
        /// </summary>
        /// <typeparam name="T">tipo dell'oggetto sul quale mappare la risposta json del servizo api</typeparam>
        /// <param name="controller">controller del servizio da richiamare</param>
        /// <param name="action">action del servizio da richiamare</param>
        /// <param name="parameters">parametri da passare al servizio</param>
        /// <returns>oggetto di tipo T mappato con i risultati</returns>
        public static T Post<T>(string controller, string action, Dictionary<string, string> parameters) { return request<T>(Method.POST, controller, action, parameters); }

        /// <summary>
        /// metodo privato per la richiesta al servizio web
        /// </summary>
        /// <typeparam name="T">il tipo da restituire come valore decodificato della risposta</typeparam>
        /// <param name="method">se un get o un post</param>
        /// <param name="controller">il controller del servizio api da richiamare</param>
        /// <param name="action">l'action del servizio api da richiamare</param>
        /// <param name="par">i parametri da passare al servizio</param>
        /// <returns>risposta del servizio</returns>
        private static T request<T>(Method method, string controller, string action, Dictionary<string, string> par)
        {
            //verifico chè ho in memoria il token(quindi che è stato già fatto l'autenticazione)
            if (AccessToken == null) { throw new Exception("Errore autenticazione non eseguita"); }
            if (IsExpiredSession()) { throw new Exception("token scaduto"); }

            //costruisco l'url
            string serverUrl = ApiUrl + controller + "/" + action;
            string postData = null;
            if (par != null)
            {
                postData = "";
                foreach (KeyValuePair<string, string> kv in par) { postData += kv.Key + "=" + htmlEncode(kv.Value) + "&"; }
                serverUrl += "?" + postData;
            }

            //creo la richiesta
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serverUrl);
            request.ServicePoint.Expect100Continue = false;
            request.Headers.Add("Authorization", TokenType + " " + AccessToken);
            request.Method = "GET";

            //se post codifico i dati nel corpo della richiesta
            if (method.Equals(Method.POST))
            {
                request.Method = "POST";
                request.ContentType = "text/plain;charset=utf-8";

                //codifico i dati da trasmettere
                if (postData != null)
                {
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] bytes = encoding.GetBytes(postData);
                    request.ContentLength = bytes.Length;
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                }
            }

            //effettuo la chiamata e recupero la risposta
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            //verifico se la risposta è la pagina di login
            if (response.ContentType.Equals("text/html; charset=utf-8")) { throw new Exception("Non Autorizzato effettuare il login"); }

            //recupero il corpo del messaggio di rispota
            StreamReader reader = new StreamReader(response.GetResponseStream());
            JsonLastResult = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(JsonLastResult);
        }

        /// <summary>
        /// Codifica la stringa
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string htmlEncode(string value)
        {
            return value
              .Replace("<", "&lt;")
              .Replace(">", "&gt;")
              .Replace("\"", "&quot;")
              .Replace("'", "&apos;")
              .Replace("&", "&amp;");
        }

    }
}