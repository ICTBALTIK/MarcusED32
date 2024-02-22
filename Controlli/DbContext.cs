using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFALibrary.Helpers;

namespace Controlli_ee145
{
    public class DbContext
    {


        private const string SQL_GET_MODULI_LOCAL = "SELECT TOP (1) [RIFERIMENTO] ,[NUMERO_CLIENTE] ,[DATA], [DATA_INSERIMENTO] FROM [MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML] order by DATA_INSERIMENTO DESC";

        string riffermineto = "";

        public List<ArchivioCatastali> GetModuliRifferimetno()
        {
            List<ArchivioCatastali> list = new List<ArchivioCatastali>();
            using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.getConnStringSimpleDoc(), CommandType.Text, SQL_GET_MODULI_LOCAL))
            {
                while (rdr.Read())
                {
                    ArchivioCatastali arch = new ArchivioCatastali();
                    if (!rdr.IsDBNull(0)) arch.RIFERIMENTO = rdr.GetString(0);
                    if (!rdr.IsDBNull(1)) arch.NUMERO_CLIENTE = rdr.GetString(1);
                    if (!rdr.IsDBNull(2)) arch.DATA = rdr.GetDateTime(2);
                    if (arch.RIFERIMENTO.Contains("-"))
                    {
                        arch.RIFERIMENTO = arch.RIFERIMENTO.Substring(0, arch.RIFERIMENTO.Length - 2);
                    }
                    list.Add(arch);
                }
            }
            return list;

        }

        private const string SQL_GET_ATTT_LOCAL = "SELECT TOP (1)[ID_RIFERIMENTO] FROM [dbsca].[dbo].[LAVORAZIONI_AGENTE]  where [NUMERO_CLIENTE]  = @NUMERO_CLIENTE order by ID desc ";


        public List<ArchivioCatastali> GetebanutijAttivita(string riff)
        {
            List<ArchivioCatastali> list = new List<ArchivioCatastali>();

            SqlParameter parameter = new SqlParameter("@NUMERO_CLIENTE", riff);
            using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.getConnStringJusteam(), CommandType.Text, SQL_GET_ATTT_LOCAL, parameter))
            {
                while (rdr.Read())
                {
                    ArchivioCatastali arch = new ArchivioCatastali();
                    if (!rdr.IsDBNull(0)) arch.RIFERIMENTO = rdr.GetString(0).Trim();

                    list.Add(arch);
                }
            }
            return list;

        }

        private const string SQL_GET_RESO_LOCAL = "SELECT TOP (1)[ID_CODICE_RESO] FROM [dbsca].[dbo].[LAVORAZIONI_AGENTE]  where [NUMERO_CLIENTE]  = @NUMERO_CLIENTE order by ID desc ";


        public List<ArchivioCatastali> GetResoibem(string riff)
        {
            List<ArchivioCatastali> list = new List<ArchivioCatastali>();

            SqlParameter parameter = new SqlParameter("@NUMERO_CLIENTE", riff);
            using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.getConnStringJusteam(), CommandType.Text, SQL_GET_RESO_LOCAL, parameter))
            {
                while (rdr.Read())
                {
                    ArchivioCatastali arch = new ArchivioCatastali();
                    if (!rdr.IsDBNull(0)) arch.STATO = rdr.GetInt32(0);

                    list.Add(arch);
                }
            }
            return list;

        }
    }

}
