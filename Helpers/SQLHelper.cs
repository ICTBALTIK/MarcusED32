using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace MARCUS.Helpers
{
    public abstract class SQLHelper
    {
        // Hashtable to store cached parameters
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        //indica il tempo di attesa massimo di ogni query
        private static readonly int CMD_TIMEOUT = Convert.ToInt32(ConfigurationSettings.AppSettings["QUERY_TIMEOUT"]);

        //stringa di connessione a cadoc MV - remoto 172.23.0.53 SIMPLEDOC
        private static string CONN_STRING_SIMPLEDOC = Decrypt("Lc4Ho2dHxK5ElcphLDunWz8wrN9f7P6bAqHl1gAGLkW4nbDbO0wXjF+iayyHr/sEqxItvuj4wl0eoQPQdCc9sXlSA4YSUkQizrVv2I0QxsQ=");
        private static string CONN_STRING_DBSCA = Decrypt("Lc4Ho2dHxK5ElcphLDunWz8wrN9f7P6bAqHl1gAGLkW4nbDbO0wXjF+iayyHr/sEqxItvuj4wl0eoQPQdCc9sd7yHFP48wRT5qo1trSd1lQ=");
        public static string getConnStringSimpleDoc() { return CONN_STRING_SIMPLEDOC; }

        //stringa di connessione a just in team - remoto 172.23.0.43 dbsca
        private static string CONN_STRING_JUSTEAM_43 = Decrypt("Lc4Ho2dHxK5ElcphLDunW6Axji70LjgwoN/XISyEQXO+Kg/WTaNaYyTJzogay7mSY9YdMF9o0IH8/wm1SCctgoi6Mwt0K90Y+Tzia4fsfu0=");

        public static string getConnStringDbsca()
        {
            DateTime dataCambioDB = new DateTime(2020, 04, 20);
            if (DateTime.Now >= dataCambioDB)
            {
                return CONN_STRING_JUSTEAM_43;
            }
            else
            {
                return CONN_STRING_DBSCA;
            }
        }

        public static string ConnStringEE176()
        {
            string sht = Decrypt("Lc4Ho2dHxK5ElcphLDunWz8wrN9f7P6bAqHl1gAGLkW4nbDbO0wXjF+iayyHr/sEqxItvuj4wl0eoQPQdCc9sVloS4owpY3PL5dx1QzH/KY=");
            return sht;
        }
        private static string CONN_STRING_53 = @"server=172.23.0.53;user id = macro1; password = Macro2012";
        public static string getConnstring53() { return CONN_STRING_53; }

        public static string ConnStringEE145()
        {
            return @"server=172.23.0.53; user id=macro1; password=Macro2012;";
        }

        //per gestire viste indicizzate
        const bool SET_ARITHABORT_ON = true;

        //chiavi di decriptaggio della stringa di conessione
        private const string chiave = "A!sSpktyh32#kKpA"; //16 byte
        private const string iv = "k!pkR43!kTTjC4fF"; //16 byte

        /// <summary>
        /// Retrieve cached parameters
        /// </summary>
        /// <param name="cacheKey">key used to lookup parameters</param>
        /// <returns>Cached SqlParamters array</returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];
            if (cachedParms == null)
            {
                return null;
            }
            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];
            for (int i = 0, j = cachedParms.Length; i < j; i++)
            {
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();
            }
            return clonedParms;
        }

        /// <summary>
        /// Execute a SqlCommand that returns a resultset against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  SqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>A SqlDataReader containing the results</returns>
        public static SqlDataReader ExecuteReader(string connString, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = CMD_TIMEOUT;
            SqlConnection conn = new SqlConnection(connString);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {

                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch (Exception ex)
            {
                string err = ex.Message + "  " + ex.StackTrace;
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// Prepare a command for execution
        /// </summary>
        /// <param name="cmd">SqlCommand object</param>
        /// <param name="conn">SqlConnection object</param>
        /// <param name="trans">SqlTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            try
            {
                cmd.CommandTimeout = CMD_TIMEOUT;

                if (conn.State != ConnectionState.Open)
                    conn.Open();

                cmd.Connection = conn;
                cmd.CommandText = cmdText;

                if (trans != null)
                    cmd.Transaction = trans;

                cmd.CommandType = cmdType;

                if (cmdParms != null)
                {
                    foreach (SqlParameter parm in cmdParms)
                        cmd.Parameters.Add(parm);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add parameter array to the cache
        /// </summary>
        /// <param name="cacheKey">Key to the parameter cache</param>
        /// <param name="cmdParms">an array of SqlParamters to be cached</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] cmdParms)
        {
            parmCache[cacheKey] = cmdParms;
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) using an existing SQL Transaction 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">an existing sql transaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = CMD_TIMEOUT;

                // 20-09-2006 Modifica necessaria per gestione viste indicizzate ---------------------------------
                if (SET_ARITHABORT_ON)
                {
                    PrepareCommand(cmd, trans.Connection, trans, CommandType.Text, "SET ARITHABORT ON", null);
                    int val1 = cmd.ExecuteNonQuery();
                }
                // -----------------------------------------------------------------------------------------------
                PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Execute a SqlCommand that returns the first column of the first record against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">an existing database connection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = CMD_TIMEOUT;

                // 20-09-2006 Modifica necessaria per gestione viste indicizzate ---------------------------------
                if (SET_ARITHABORT_ON)
                {
                    PrepareCommand(cmd, trans.Connection, trans, CommandType.Text, "SET ARITHABORT ON", null);
                    int val1 = cmd.ExecuteNonQuery();
                }
                // -----------------------------------------------------------------------------------------------

                PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
            catch
            {
                throw;
            }
        }

        public static string Encrypt(string Str)
        {
            RijndaelManaged rjm = new RijndaelManaged();
            rjm.KeySize = 128;
            rjm.BlockSize = 128;
            rjm.Key = Encoding.ASCII.GetBytes(chiave);
            rjm.IV = Encoding.ASCII.GetBytes(iv);
            try
            {
                Byte[] input = Convert.FromBase64String(Str);
                Byte[] output = rjm.CreateEncryptor().TransformFinalBlock(input, 0, input.Length);
                return Encoding.UTF8.GetString(output);
            }
            catch { return Str; }
        }

        private static string Decrypt(string Str)
        {
            RijndaelManaged rjm = new RijndaelManaged();
            rjm.KeySize = 128;
            rjm.BlockSize = 128;
            rjm.Key = ASCIIEncoding.ASCII.GetBytes(chiave);
            rjm.IV = ASCIIEncoding.ASCII.GetBytes(iv);
            try
            {
                byte[] input = Convert.FromBase64String(Str);
                byte[] output = rjm.CreateDecryptor().TransformFinalBlock(input, 0, input.Length);
                return Encoding.UTF8.GetString(output);
            }
            catch { return Str; }
        }
    }
}