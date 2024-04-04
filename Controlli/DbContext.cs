﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARCUS.Controlli;
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

        public string SearchInDB(string riferimento, string fieldName)
        {

            string fieldValue = "";
            string columnName = "";

            // Устанавливаем имя столбца в зависимости от переданного параметра
            switch (fieldName)
            {
                case "fattura":
                    columnName = "FATTURA";
                    break;
                case "importo":
                    columnName = "IMPORTO";
                    break;
                case "eneltel":
                    columnName = "ENELTEL";
                    break;
                default:
                    throw new ArgumentException("Неподдерживаемое имя поля", nameof(fieldName));
            }
            using (SqlConnection con = new SqlConnection(@"Data Source=172.23.0.43; Persist Security Info=True; Initial Catalog=DBSCA;User ID=macro1;Password=Macro2012"))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = $"SELECT {columnName} FROM [DBSCA].[dbo].[DATI_ED32] WHERE ID_DOCUMENTO = @Riferimento";
                    cmd.Parameters.AddWithValue("@Riferimento", riferimento);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            fieldValue = reader[columnName].ToString();
                        }
                    }
                }
            }
            return fieldValue;
        }

        private const string SQL_GET_ATTT_LOCAL5 = "SELECT [ID_RIFERIMENTO] FROM [dbsca].[dbo].[LAVORAZIONI_AGENTE] WHERE [ID_CODICE_RESO] = '37196' AND CONVERT(date, END_DATE) BETWEEN @ID_RIFERIMENTO AND @ID_RIFERIMENTO2 ORDER BY ID DESC";
        public List<ArchivioEgc> GetebanutijAttivita4(string riffer4, string riffer5)
        {
            List<ArchivioEgc> list = new List<ArchivioEgc>();
            using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.getConnStringJusteam(), CommandType.Text, SQL_GET_ATTT_LOCAL5,
                new SqlParameter("@ID_RIFERIMENTO", riffer4),
                new SqlParameter("@ID_RIFERIMENTO2", riffer5)))
            {
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        ArchivioEgc arch = new ArchivioEgc();
                        if (!rdr.IsDBNull(0))
                            arch.Attivita = rdr.GetString(0).Trim();
                        list.Add(arch);
                    }
                }
            }
            return list;
        }
    }

}
