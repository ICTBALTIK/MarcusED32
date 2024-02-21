using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace MARCUS.Helpers
{
    class SqlQuery
    {
        private const string SQL_GET_MODULI_LOCAL = "SELECT * FROM [BALTIK_DB].[dbo].[MODULI_LOCAL] WHERE [BALTIK_DB].[dbo].[MODULI_LOCAL].[RIFERIMENTO] = @RIFERIMENTO";
        public List<Records.RecordsModuliLocal> GetModuliLocalList(string riferimento)
        {
            List<Records.RecordsModuliLocal> list = new List<Records.RecordsModuliLocal>();
            SqlParameter[] Parms = SQLHelper.GetCachedParameters(SQL_GET_MODULI_LOCAL);
            if (Parms == null)
            {
                Parms = new SqlParameter[]{
                   new SqlParameter("@RIFERIMENTO", SqlDbType.VarChar),
                };
            }
            Parms[0].Value = riferimento;

            try
            {
                using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.ConnStringEE145(), CommandType.Text, SQL_GET_MODULI_LOCAL, Parms))
                {
                    while (rdr.Read())
                    {
                        var p = new Records.RecordsModuliLocal
                        {
                            DOC_TYPE = rdr.GetString(0),
                            SUCC_NUMBER = rdr.GetString(1),
                            RIFERIMENTO = rdr.GetString(2),
                            NUMERO_CLIENTE = rdr.GetString(3),
                            NUMERO_ORDINE = rdr.GetString(4),
                            FIRMA = rdr.GetString(5),
                            CODICE_FISCALE = rdr.GetString(6),
                            COMUNE_DOMICILIO = rdr.GetString(7),
                            PROVINCIA_DOMICILIO = rdr.GetString(8),
                            COMUNE_SEDE = rdr.GetString(9),
                            PROVINCIA_SEDE = rdr.GetString(10),
                            PARTITA_IVA = rdr.GetString(11),
                            TOPONOMASTICA_FORNITURA = rdr.GetString(12),
                            NOME_VIA_FORNITURA = rdr.GetString(13),
                            CIVICO_FORNITURA = rdr.GetString(14),
                            LOCALITA_FORNITURA = rdr.GetString(15),
                            COMUNE_FORNITURA = rdr.GetString(16),
                            CAP_COMUNE_FORNITURA = rdr.GetString(17),
                            QUALIFICA = rdr.GetString(18),
                            COMUNE_AMMINISTRATIVO = rdr.GetString(19),
                            COMUNE_CATASTALE = rdr.GetString(20),
                            CODICE_COMUNE_CATASTALE = rdr.GetString(21),
                            TIPO_UNITA = rdr.GetString(22),
                            SEZIONE = rdr.GetString(23),
                            FOGLIO = rdr.GetString(24),
                            PARTICELLA = rdr.GetString(25),
                            SUBALTERNO = rdr.GetString(26),
                            ESTENSIONE_PARTICELLA = rdr.GetString(27),
                            TIPO_PARTICELLA = rdr.GetString(28),
                            IMMOBILI_ESCLUSI = rdr.GetString(29),
                            DATA = rdr.GetDateTime(30),
                            DATA_ATTIVAZIONE = rdr.GetDateTime(31),
                            DATA_INSERIMENTO = rdr.GetDateTime(32),
                            ID_USER = rdr.GetInt32(33)
                        };
                        list.Add(p);
                    }
                }
                return list;
            }
            catch
            {
                return null;
            }
        }

        private string SQL_GET_MULTI_COUNT = "SELECT COUNT(RIFERIMENTO) " +    
                                                   "FROM [MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML] " +
                                                   "WHERE RIFERIMENTO LIKE @RIFERIMENTO%";

        private const string SQL_GET_VALUES_INSERIMENTO = "SELECT " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].QUALIFICA, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].COMUNE_CATASTALE, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].TIPO_UNITA, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].FOGLIO, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].ESTENSIONE_PARTICELLA," +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].SUBALTERNO, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].FIRMA, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].IMMOBILI_ESCLUSI, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].COMUNE_AMMINISTRATIVO, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].CODICE_COMUNE_CATASTALE, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].SEZIONE, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].PARTICELLA, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].TIPO_PARTICELLA, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].DATA, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].CODICE_FISCALE, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].NUMERO_CLIENTE, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].DATA_INSERIMENTO, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].COMUNE_DOMICILIO, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].PROVINCIA_DOMICILIO, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].COMUNE_SEDE, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].PROVINCIA_SEDE, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].TOPONOMASTICA_FORNITURA, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].NOME_VIA_FORNITURA, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].CIVICO_FORNITURA, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].LOCALITA_FORNITURA, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].COMUNE_FORNITURA, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].CAP_COMUNE_FORNITURA, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].PARTITA_IVA, " +
                                                          "[MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].ID_USER " +
                                                          "FROM [MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML] " +
                                                          "WHERE [MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML].[RIFERIMENTO] = @RIFERIMENTO";

        public Records.RecordsEE145 GetValuesInserimentoBySF(string riferimento)
        {
            Records.RecordsEE145 record = new Records.RecordsEE145();
            SqlParameter[] Parms = SQLHelper.GetCachedParameters(SQL_GET_VALUES_INSERIMENTO);
            if (Parms == null)
            {
                Parms = new SqlParameter[]{
                   new SqlParameter("@RIFERIMENTO", SqlDbType.VarChar)
                };
            }
            if (riferimento.StartsWith("{"))//26062019
            {
                Parms[0].Value = riferimento.Remove(15);
            }
            else
            {
                Parms[0].Value = riferimento;
            }

            try
            {
                using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.ConnStringEE145(), CommandType.Text, SQL_GET_VALUES_INSERIMENTO, Parms))
                {

                    if (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0)) record.Qualifica = rdr.GetString(0);
                        if (!rdr.IsDBNull(1)) record.ComuneCatastale = rdr.GetString(1);
                        if (!rdr.IsDBNull(2)) record.TipoUnita = rdr.GetString(2);
                        if (!rdr.IsDBNull(3)) record.Foglio = rdr.GetString(3);
                        if (!rdr.IsDBNull(4)) record.EstensioneParticella = rdr.GetString(4);
                        if (!rdr.IsDBNull(5)) record.Subalterno = rdr.GetString(5);
                        if (!rdr.IsDBNull(6)) record.Firma = rdr.GetString(6);
                        if (!rdr.IsDBNull(7)) record.ImmobiliEsclusi = rdr.GetString(7);
                        if (!rdr.IsDBNull(8)) record.ComuneAmministrativo = rdr.GetString(8);
                        if (!rdr.IsDBNull(9)) record.CodiceComuneCatastale = rdr.GetString(9);
                        if (!rdr.IsDBNull(10)) record.Sezione = rdr.GetString(10);
                        if (!rdr.IsDBNull(11)) record.Particella = rdr.GetString(11);
                        if (!rdr.IsDBNull(12)) record.TipoParticella = rdr.GetString(12);
                        if (!rdr.IsDBNull(13)) record.Data = rdr.GetDateTime(13);
                        if (!rdr.IsDBNull(14)) record.CodiceFiscale = rdr.GetString(14).Trim();
                        if (!rdr.IsDBNull(15)) record.NumeroCliente = rdr.GetString(15);
                        if (!rdr.IsDBNull(16)) record.DataInserimento = rdr.GetDateTime(16);
                        if (!rdr.IsDBNull(17)) record.ComuneDomicilio = rdr.GetString(17);
                        if (!rdr.IsDBNull(18)) record.ProvinciaDomicilio = rdr.GetString(18);
                        if (!rdr.IsDBNull(19)) record.ComuneSede = rdr.GetString(19);
                        if (!rdr.IsDBNull(20)) record.ProvinciaSede = rdr.GetString(20);
                        if (!rdr.IsDBNull(21)) record.TopomasticaFornitura = rdr.GetString(21);
                        if (!rdr.IsDBNull(22)) record.NomeViaFornitura = rdr.GetString(22);
                        if (!rdr.IsDBNull(23)) record.CivicoFornitura = rdr.GetString(23);
                        if (!rdr.IsDBNull(24)) record.LocalitaFornitura = rdr.GetString(24);
                        if (!rdr.IsDBNull(25)) record.ComuneFornitura = rdr.GetString(25);
                        if (!rdr.IsDBNull(26)) record.CapComuneFornitura = rdr.GetString(26);
                        if (!rdr.IsDBNull(27)) record.Piva = rdr.GetString(27);
                        if (!rdr.IsDBNull(28)) record.IdUser = rdr.GetInt32(28).ToString();
                    }
                }
                return record;
            }
            catch
            {
                return null;
            }
        }

        public int GetMultiRelatedCountDocs(string riferimento) {

            SqlParameter[] Parms = new SqlParameter[]{
                new SqlParameter("@RIFERIMENTO", SqlDbType.VarChar)
            };

            int finalCount = 0;

            if (riferimento.StartsWith("{"))//26062019
            {
                Parms[0].Value = riferimento.Remove(15);
            } else {
                Parms[0].Value = riferimento;
            }

            var commandString = $"SELECT COUNT(RIFERIMENTO) " +
                                $"FROM [MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML] " +
                                $"WHERE RIFERIMENTO LIKE '{riferimento}%'";

            try {
                using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.ConnStringEE145(), CommandType.Text, commandString, null)) {
                    if (rdr.Read()) {
                        if (!rdr.IsDBNull(0)) {
                            finalCount = rdr.GetInt32(0);
                        }
                    }
                }
                } catch {
                finalCount = 0;
                }

            return finalCount;
        }

        private const string SQL_INSERT_SMIS = "INSERT INTO PROD_DATI_FLUSSI_SMIS " +
                                               "([ID_USER], [DATA], [PIVA_UTENTE], [PIVA_DISTRIBUTORE], [COD_DISP], [POD], [MOTIVAZIONE], [DATA_SMONTAGGIO], [DATA_MONTAGGIO], [TIPO_LETTURA], [TIPO_MISURATORE_SMONTAGGIO], [COD_TIPO_MISURATORE_SMONTAGGIO], " +
                                               "[S_EA_F0], [S_EA_F1], [S_EA_F2], [S_EA_F3], [S_ER_F0], [S_ER_F1], [S_ER_F2], [S_ER_F3], [DATA_MESSA_REGIME], [TIPO_MISURATORE_MONTAGGIO], [COD_TIPO_MISURATORE_MONTAGGIO], [MATRICOLA], [CIFRE], [TENSIONE], [KA], [KR], [KP], [M_EA_F0], [M_EA_F1], [M_EA_F2], [M_EA_F3], [M_ER_F0], [M_ER_F1], [M_ER_F2], [M_ER_F3], [ID_AGENTE], [STATO], " +
                                               " [S_EA_F4], [S_EA_F5], [S_EA_F6], [S_ER_F4], [S_ER_F5], [S_ER_F6], [M_EA_F4], [M_EA_F5], [M_EA_F6], [M_ER_F4], [M_ER_F5], [M_ER_F6], " +
                                               " [S_P_F0], [S_P_F1], [S_P_F2], [S_P_F3], [S_P_F4], [S_P_F5], [S_P_F6], [M_P_F0], [M_P_F1], [M_P_F2], [M_P_F3], [M_P_F4], [M_P_F5], [M_P_F6]) " +
                                               " VALUES " +
                                               " (@ID_USER, @DATA, @PIVA_UTENTE, @PIVA_DISTRIBUTORE, @COD_DISP, @POD, @MOTIVAZIONE, @DATA_SMONTAGGIO, @DATA_MONTAGGIO, @TIPO_LETTURA, @TIPO_MISURATORE_SMONTAGGIO, @COD_TIPO_MISURATORE_SMONTAGGIO, " +
                                               "@S_EA_F0, @S_EA_F1, @S_EA_F2, @S_EA_F3, @S_ER_F0, @S_ER_F1, @S_ER_F2, @S_ER_F3, @DATA_MESSA_REGIME, @TIPO_MISURATORE_MONTAGGIO, @COD_TIPO_MISURATORE_MONTAGGIO, @MATRICOLA, @CIFRE, @TENSIONE, @KA, @KR, @KP, @M_EA_F0, @M_EA_F1, @M_EA_F2, @M_EA_F3, @M_ER_F0, @M_ER_F1, @M_ER_F2, @M_ER_F3, @ID_AGENTE, @STATO, " +
                                               " @S_EA_F4, @S_EA_F5, @S_EA_F6, @S_ER_F4, @S_ER_F5, @S_ER_F6, @M_EA_F4, @M_EA_F5, @M_EA_F6, @M_ER_F4, @M_ER_F5, @M_ER_F6, " +
                                               " @S_P_F0, @S_P_F1, @S_P_F2, @S_P_F3, @S_P_F4, @S_P_F5, @S_P_F6, @M_P_F0, @M_P_F1, @M_P_F2, @M_P_F3, @M_P_F4, @M_P_F5, @M_P_F6);SELECT @@IDENTITY;";

        public int InsertSmis(Records.Smis _Smis)
        {
            SqlParameter[] Parms = SQLHelper.GetCachedParameters(SQL_INSERT_SMIS);
            if (Parms == null)
            {
                Parms = new SqlParameter[]{
                   new SqlParameter("@ID_USER", SqlDbType.NVarChar),
                   new SqlParameter("@DATA", SqlDbType.DateTime),
                   new SqlParameter("@PIVA_UTENTE", SqlDbType.NVarChar),
                   new SqlParameter("@PIVA_DISTRIBUTORE", SqlDbType.NVarChar),
                   new SqlParameter("@COD_DISP", SqlDbType.NVarChar),
                   new SqlParameter("@POD", SqlDbType.NVarChar),
                   new SqlParameter("@MOTIVAZIONE", SqlDbType.NVarChar),
                   new SqlParameter("@DATA_SMONTAGGIO", SqlDbType.DateTime),
                   new SqlParameter("@DATA_MONTAGGIO", SqlDbType.DateTime),
                   new SqlParameter("@TIPO_LETTURA", SqlDbType.NVarChar),
                   new SqlParameter("@TIPO_MISURATORE_SMONTAGGIO", SqlDbType.NVarChar),
                   new SqlParameter("@COD_TIPO_MISURATORE_SMONTAGGIO", SqlDbType.NVarChar),
                   new SqlParameter("@S_EA_F0", SqlDbType.NVarChar),
                   new SqlParameter("@S_EA_F1", SqlDbType.NVarChar),
                   new SqlParameter("@S_EA_F2", SqlDbType.NVarChar),
                   new SqlParameter("@S_EA_F3", SqlDbType.NVarChar),
                   new SqlParameter("@S_ER_F0", SqlDbType.NVarChar),
                   new SqlParameter("@S_ER_F1", SqlDbType.NVarChar),
                   new SqlParameter("@S_ER_F2", SqlDbType.NVarChar),
                   new SqlParameter("@S_ER_F3", SqlDbType.NVarChar),
                   new SqlParameter("@DATA_MESSA_REGIME", SqlDbType.DateTime),
                   new SqlParameter("@TIPO_MISURATORE_MONTAGGIO", SqlDbType.NVarChar),
                   new SqlParameter("@COD_TIPO_MISURATORE_MONTAGGIO", SqlDbType.NVarChar),
                   new SqlParameter("@MATRICOLA", SqlDbType.NVarChar),
                   new SqlParameter("@CIFRE", SqlDbType.NVarChar),
                   new SqlParameter("@TENSIONE", SqlDbType.NVarChar),
                   new SqlParameter("@KA", SqlDbType.NVarChar),
                   new SqlParameter("@KR", SqlDbType.NVarChar),
                   new SqlParameter("@KP", SqlDbType.NVarChar),
                   new SqlParameter("@M_EA_F0", SqlDbType.NVarChar),
                   new SqlParameter("@M_EA_F1", SqlDbType.NVarChar),
                   new SqlParameter("@M_EA_F2", SqlDbType.NVarChar),
                   new SqlParameter("@M_EA_F3", SqlDbType.NVarChar),
                   new SqlParameter("@M_ER_F0", SqlDbType.NVarChar),
                   new SqlParameter("@M_ER_F1", SqlDbType.NVarChar),
                   new SqlParameter("@M_ER_F2", SqlDbType.NVarChar),
                   new SqlParameter("@M_ER_F3", SqlDbType.NVarChar),
                   new SqlParameter("@ID_AGENTE", SqlDbType.NVarChar),
                   new SqlParameter("@STATO", SqlDbType.Int),
                   new SqlParameter("@S_EA_F4", SqlDbType.NVarChar),
                   new SqlParameter("@S_EA_F5", SqlDbType.NVarChar),
                   new SqlParameter("@S_EA_F6", SqlDbType.NVarChar),
                   new SqlParameter("@S_ER_F4", SqlDbType.NVarChar),
                   new SqlParameter("@S_ER_F5", SqlDbType.NVarChar),
                   new SqlParameter("@S_ER_F6", SqlDbType.NVarChar),
                   new SqlParameter("@M_EA_F4", SqlDbType.NVarChar),
                   new SqlParameter("@M_EA_F5", SqlDbType.NVarChar),
                   new SqlParameter("@M_EA_F6", SqlDbType.NVarChar),
                   new SqlParameter("@M_ER_F4", SqlDbType.NVarChar),
                   new SqlParameter("@M_ER_F5", SqlDbType.NVarChar),
                   new SqlParameter("@M_ER_F6", SqlDbType.NVarChar),
                   new SqlParameter("@S_P_F0", SqlDbType.NVarChar),
                   new SqlParameter("@S_P_F1", SqlDbType.NVarChar),
                   new SqlParameter("@S_P_F2", SqlDbType.NVarChar),
                   new SqlParameter("@S_P_F3", SqlDbType.NVarChar),
                   new SqlParameter("@S_P_F4", SqlDbType.NVarChar),
                   new SqlParameter("@S_P_F5", SqlDbType.NVarChar),
                   new SqlParameter("@S_P_F6", SqlDbType.NVarChar),
                   new SqlParameter("@M_P_F0", SqlDbType.NVarChar),
                   new SqlParameter("@M_P_F1", SqlDbType.NVarChar),
                   new SqlParameter("@M_P_F2", SqlDbType.NVarChar),
                   new SqlParameter("@M_P_F3", SqlDbType.NVarChar),
                   new SqlParameter("@M_P_F4", SqlDbType.NVarChar),
                   new SqlParameter("@M_P_F5", SqlDbType.NVarChar),
                   new SqlParameter("@M_P_F6", SqlDbType.NVarChar)
                };
            }
            Parms[0].Value = _Smis.IdUser;
            Parms[1].Value = _Smis.Data;
            Parms[2].Value = "06655971007";
            Parms[3].Value = "12883450152";
            Parms[4].Value = "DP1676";
            Parms[5].Value = _Smis.Pod;
            Parms[6].Value = _Smis.Motivazione;
            Parms[7].Value = _Smis.DataSmontaggio;
            Parms[8].Value = _Smis.DataMontaggio;
            Parms[9].Value = _Smis.TipoLettura.Substring(0, 1);
            Parms[10].Value = _Smis.TipoMisuratoreSmontaggio;
            Parms[11].Value = _Smis.CodiceTipoMisuratoreSmontaggio;
            Parms[12].Value = _Smis.Ea0Smontaggio;
            Parms[13].Value = _Smis.Ea1Smontaggio;
            Parms[14].Value = _Smis.Ea2Smontaggio;
            Parms[15].Value = _Smis.Ea3Smontaggio;
            Parms[16].Value = _Smis.Er0Smontaggio;
            Parms[17].Value = _Smis.Er1Smontaggio;
            Parms[18].Value = _Smis.Er2Smontaggio;
            Parms[19].Value = _Smis.Er3Smontaggio;
            Parms[20].Value = _Smis.DataMessaRegime;
            Parms[21].Value = _Smis.TipoMisuratoreMontaggio;
            Parms[22].Value = _Smis.CodiceTipoMisuratoreMontaggio;
            Parms[23].Value = _Smis.Matricola;
            Parms[24].Value = _Smis.Cifre;
            Parms[25].Value = _Smis.Tensione;
            Parms[26].Value = _Smis.Ka;
            Parms[27].Value = _Smis.Kr;
            Parms[28].Value = _Smis.Kp;
            Parms[29].Value = _Smis.Ea0Montaggio;
            Parms[30].Value = _Smis.Ea1Montaggio;
            Parms[31].Value = _Smis.Ea2Montaggio;
            Parms[32].Value = _Smis.Ea3Montaggio;
            Parms[33].Value = _Smis.Er0Montaggio;
            Parms[34].Value = _Smis.Er1Montaggio;
            Parms[35].Value = _Smis.Er2Montaggio;
            Parms[36].Value = _Smis.Er3Montaggio;
            Parms[37].Value = _Smis.IdAgente;
            Parms[38].Value = 1;
            Parms[39].Value = _Smis.Ea4Smontaggio;
            Parms[40].Value = _Smis.Ea5Smontaggio;
            Parms[41].Value = _Smis.Ea6Smontaggio;
            Parms[42].Value = _Smis.Er4Smontaggio;
            Parms[43].Value = _Smis.Er5Smontaggio;
            Parms[44].Value = _Smis.Er6Smontaggio;
            Parms[45].Value = _Smis.Er4Montaggio;
            Parms[46].Value = _Smis.Er5Montaggio;
            Parms[47].Value = _Smis.Ea6Montaggio;
            Parms[48].Value = _Smis.Er4Montaggio;
            Parms[49].Value = _Smis.Er5Montaggio;
            Parms[50].Value = _Smis.Er6Montaggio;
            Parms[51].Value = _Smis.P0Smontaggio;
            Parms[52].Value = _Smis.P1Smontaggio;
            Parms[53].Value = _Smis.P2Smontaggio;
            Parms[54].Value = _Smis.P3Smontaggio;
            Parms[55].Value = _Smis.P4Smontaggio;
            Parms[56].Value = _Smis.P5Smontaggio;
            Parms[57].Value = _Smis.P6Smontaggio;
            Parms[58].Value = _Smis.P0Montaggio;
            Parms[59].Value = _Smis.P1Montaggio;
            Parms[60].Value = _Smis.P2Montaggio;
            Parms[61].Value = _Smis.P3Montaggio;
            Parms[62].Value = _Smis.P4Montaggio;
            Parms[63].Value = _Smis.P5Montaggio;
            Parms[64].Value = _Smis.P6Montaggio;

            try
            {
                using (SqlConnection conn = new SqlConnection(SQLHelper.getConnStringDbsca()))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            int rdr = Convert.ToInt32(SQLHelper.ExecuteScalar(trans, CommandType.Text, SQL_INSERT_SMIS, Parms));
                            trans.Commit();
                            return rdr;
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                return -1;
            }
        }

        private const string SQL_GET_PROD_NOTE_BY_ID_DETT_RIF = "SELECT TOP(1) [CAMPI] FROM [DBSCA].[dbo].[PROD_NOTE] where [ID_DETTAGLIO_TIPO_LAVORAZIONE] = @ID_DETT_TIPO_LAVORAZIONE and RIFERIMENTO = @RIFERIMENTO";

        public string GetProdNoteByIdDettAndRif(int idDettTipoLavorazione, string riferimento)
        {
            string sql = SQL_GET_PROD_NOTE_BY_ID_DETT_RIF;
            SqlParameter[] Parms = SQLHelper.GetCachedParameters(sql);
            if (Parms == null)
            {
                Parms = new SqlParameter[]{
                   new SqlParameter("@ID_DETT_TIPO_LAVORAZIONE", SqlDbType.Int),
                   new SqlParameter("@RIFERIMENTO", SqlDbType.NVarChar)
                };
            }
            Parms[0].Value = idDettTipoLavorazione;
            Parms[1].Value = riferimento;

            try
            {
                string campi = "";
                using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.getConnStringDbsca(), CommandType.Text, sql, Parms))
                {
                    while (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0)) campi = rdr.GetString(0);
                    }
                }
                return campi;
            }
            catch
            {
                return null;
            }
        }

        private const string SQL_UPDATE_PROD_NOTE_BY_DETTAGLIO = "UPDATE [DBSCA].[dbo].[PROD_NOTE] SET [CAMPI] = @CAMPI WHERE [RIFERIMENTO]=@RIFERIMENTO AND ID_DETTAGLIO_TIPO_LAVORAZIONE = @ID_DETTAGLIO_TIPO_LAVORAZIONE";

        public int UpdateProdNote(string campi, string riferimento, int idDettaglioTipoLavorazione)
        {
            try
            {
                SqlParameter[] Parms = SQLHelper.GetCachedParameters(SQL_UPDATE_PROD_NOTE_BY_DETTAGLIO);
                if (Parms == null)
                {
                    Parms = new SqlParameter[]{
                                new SqlParameter("@CAMPI", SqlDbType.NVarChar),
                                new SqlParameter("@RIFERIMENTO", SqlDbType.NVarChar),
                                new SqlParameter("@ID_DETTAGLIO_TIPO_LAVORAZIONE", SqlDbType.Int)
                };
                }
                Parms[0].Value = campi;
                Parms[1].Value = riferimento;
                Parms[2].Value = idDettaglioTipoLavorazione;
                using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.getConnStringDbsca(), CommandType.Text, SQL_UPDATE_PROD_NOTE_BY_DETTAGLIO, Parms))
                {
                    return 1;
                }
            }
            catch
            {
                return -1;
            }
        }

        private const string SQL_GET_VALUES_INSERIMENTO_DIMPAG_IN_SAP = "SELECT * FROM [DBSCA].[dbo].[DATI_EE62] WHERE [DBSCA].[dbo].[DATI_EE62].[RIFERIMENTO] = @RIFERIMENTO";

        public Records.RecordsEE62 GetValuesInserimentoDimPagInSapByRiferimento(string riferimento)
        {
            Records.RecordsEE62 record = new Records.RecordsEE62();
            SqlParameter[] Parms = SQLHelper.GetCachedParameters(SQL_GET_VALUES_INSERIMENTO_DIMPAG_IN_SAP);
            if (Parms == null)
            {
                Parms = new SqlParameter[]{
                   new SqlParameter("@RIFERIMENTO", SqlDbType.VarChar)
                };
            }
            Parms[0].Value = riferimento;

            try
            {
                using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.getConnStringDbsca(), CommandType.Text, SQL_GET_VALUES_INSERIMENTO_DIMPAG_IN_SAP, Parms))
                {

                    if (rdr.Read())
                    {
                        record.Fattura = rdr["NUMERO_FATTURA"].ToString();
                        record.Importo = rdr["IMPORTO"].ToString();
                        record.CanalePagamento = rdr["CANALE_PAGAMENTO"].ToString();
                        record.NumeroCC = rdr["NUMERO_CC"].ToString();
                        record.Vcy = rdr["VCY"].ToString();
                        record.Agenzia = rdr["AGENZIA"].ToString();
                        record.CodiceSportello = rdr["CODICE_SPORTELLO"].ToString();
                        record.Banca = rdr["BANCA"].ToString().ToUpper();
                        record.NumeroTransazione = rdr["TRANSAZIONE"].ToString();
                        record.DescTipoPagamento = rdr["DESCRIZIONE_TIPO_PAGAMENTO"].ToString();
                        record.DataPagamento = rdr["DATA_PAGAMENTO"].ToString().Substring(0, 10).Replace("/", ".");
                        record.NumeroRicevuta = rdr["NUMERO_RICEVUTA"].ToString();
                    }
                }
                return record;
            }
            catch
            {
                return null;
            }
        }

        //  ZAPROS SIFITES

        private const string SQL_GET_VALUES_SIFITES = "SELECT * FROM [DBSCA].[dbo].[DATI_ED32] WHERE [DBSCA].[dbo].[DATI_ED32].[ID_DOCUMENTO] = @ID_DOCUMENTO";

        public Records.RecordsSifites GetValuesSifites(string riferimento)
        {
            Records.RecordsSifites record = new Records.RecordsSifites();
            SqlParameter[] Parms = SQLHelper.GetCachedParameters(SQL_GET_VALUES_SIFITES);
            if (Parms == null)
            {
                Parms = new SqlParameter[]{
                   new SqlParameter("@ID_DOCUMENTO", SqlDbType.VarChar)
                };
            }
            Parms[0].Value = riferimento;

            try
            {
                using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.getConnStringDbsca(), CommandType.Text, SQL_GET_VALUES_SIFITES, Parms))
                {

                    if (rdr.Read())
                    {
                        record.Id = rdr["ID"].ToString();
                        record.IdDocumento = rdr["ID_DOCUMENTO"].ToString();
                        record.DataEmissione = rdr["DATA_EMISSIONE"].ToString();
                        record.Importo = rdr["IMPORTO"].ToString();
                        record.Nota = rdr["NOTA"].ToString();
                        record.Fattura = rdr["FATTURA"].ToString();
                        record.IdOperatore = rdr["ID_OPERATORE"].ToString();
                        record.DataInserimento = rdr["DATA_INSERIMENTO"].ToString().ToUpper();
                        record.Eneltel = rdr["ENELTEL"].ToString();
                    }
                }
                return record;
            }
            catch
            {
                return null;
            }
        }

        // TUT ON ZAKANCHIVAETSA

        private const string SQL_GET_ATTIVITA_IN_NOTA = "SELECT TOP 1 " +
                                                        "[DBSCA].[dbo].[PROD_VOLUMI_LAVORATI].[NOTE] " +
                                                        "FROM [DBSCA].[dbo].[PROD_VOLUMI_LAVORATI]" +
                                                        "WHERE [DBSCA].[dbo].[PROD_VOLUMI_LAVORATI].[DETTAGLIO] = @RIFERIMENTO " +
                                                        "AND " +
                                                        "([DBSCA].[dbo].[PROD_VOLUMI_LAVORATI].[ID_DETTAGLIO_TIPO_LAVORAZIONE] = 3722 OR [DBSCA].[dbo].[PROD_VOLUMI_LAVORATI].[ID_DETTAGLIO_TIPO_LAVORAZIONE] = 3731) " +
                                                        "ORDER BY [DBSCA].[dbo].[PROD_VOLUMI_LAVORATI].[NOTE] DESC";

        public string GetAttivitaInNotaByRiferimento(string riferimento)
        {
            string result = string.Empty;
            SqlParameter[] Parms = SQLHelper.GetCachedParameters(SQL_GET_ATTIVITA_IN_NOTA);
            if (Parms == null)
            {
                Parms = new SqlParameter[]{
                   new SqlParameter("@RIFERIMENTO", SqlDbType.VarChar)
                };
            }
            Parms[0].Value = riferimento;

            try
            {
                using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.getConnStringDbsca(), CommandType.Text, SQL_GET_ATTIVITA_IN_NOTA, Parms))
                {
                    if (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0)) result = rdr.GetString(0);
                    }
                }
                return result;
            }
            catch
            {
                return result;//22062019
                //return null;
            }
        }

        private const string SQL_GET_ATTIVITA_IN_NUMERO_CLIENTE = "SELECT TOP 1 " +
                                                                  "[DBSCA].[dbo].[PROD_VOLUMI_LAVORATI].[NUMERO_CLIENTE] " +
                                                                  "FROM [DBSCA].[dbo].[PROD_VOLUMI_LAVORATI]" +
                                                                  "WHERE [DBSCA].[dbo].[PROD_VOLUMI_LAVORATI].[DETTAGLIO] = @RIFERIMENTO " +
                                                                  "AND " +
                                                                  "([DBSCA].[dbo].[PROD_VOLUMI_LAVORATI].[ID_DETTAGLIO_TIPO_LAVORAZIONE] = 3722 OR [DBSCA].[dbo].[PROD_VOLUMI_LAVORATI].[ID_DETTAGLIO_TIPO_LAVORAZIONE] = 3731) " +
                                                                  "ORDER BY [DBSCA].[dbo].[PROD_VOLUMI_LAVORATI].[NOTE] DESC";

        public string GetAttivitaInNumeroClienteByRiferimento(string riferimento)
        {
            string result = string.Empty;
            SqlParameter[] Parms = SQLHelper.GetCachedParameters(SQL_GET_ATTIVITA_IN_NUMERO_CLIENTE);
            if (Parms == null)
            {
                Parms = new SqlParameter[]{
                   new SqlParameter("@RIFERIMENTO", SqlDbType.VarChar)
                };
            }
            Parms[0].Value = riferimento;

            try
            {
                using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.getConnStringDbsca(), CommandType.Text, SQL_GET_ATTIVITA_IN_NUMERO_CLIENTE, Parms))
                {
                    if (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0)) result = rdr.GetString(0);
                    }
                }
                return result;
            }
            catch
            {
                return result;//22062019
                //return null;
            }
        }

        private const string SQL_GET_ELEMENT_COUNT_IN_LAVORAZIONI_AGENTE_BY_RIFERIMENTO = "SELECT COUNT(*) FROM [DBSCA].[dbo].[LAVORAZIONI_AGENTE] WHERE [DBSCA].[dbo].[LAVORAZIONI_AGENTE].[ID_RIFERIMENTO]=@RIFERIMENTO AND [DBSCA].[dbo].[LAVORAZIONI_AGENTE].[ID_DETTAGLIO_TIPO_LAVORAZIONE]=@IDLAV";

        public int GetElementCountInLavorazioniAgenteByRiferimento(string riferimento, int docCheckerId)
        {
            int result = -1;
            SqlParameter[] Parms = SQLHelper.GetCachedParameters(SQL_GET_ELEMENT_COUNT_IN_LAVORAZIONI_AGENTE_BY_RIFERIMENTO);
            if (Parms == null)
            {
                Parms = new SqlParameter[]{
                   new SqlParameter("@RIFERIMENTO", SqlDbType.VarChar),
                   new SqlParameter("@IDLAV", SqlDbType.VarChar)
                };
            }
            Parms[0].Value = riferimento;
            Parms[1].Value = docCheckerId;

            try
            {
                using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.getConnStringDbsca(), CommandType.Text, SQL_GET_ELEMENT_COUNT_IN_LAVORAZIONI_AGENTE_BY_RIFERIMENTO, Parms))
                {
                    if (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0)) result = rdr.GetInt32(0);
                    }
                }
                return result;
            }
            catch
            {
                return result;
            }
        }

        private const string SQL_GET_ELEMENT_COUNT_IN_PROD_VOLUMI_LAVORAVI_BY_DETTAGLIO = "SELECT COUNT(*) FROM [DBSCA].[dbo].[PROD_VOLUMI_LAVORATI] WHERE [DBSCA].[dbo].[PROD_VOLUMI_LAVORATI].[DETTAGLIO]=@RIFERIMENTO AND [DBSCA].[dbo].[PROD_VOLUMI_LAVORATI].[ID_DETTAGLIO_TIPO_LAVORAZIONE]=@IDLAV";

        public int GetElementCountInProdVolumiLavoratiByDettaglio(string riferimento, int docCheckerId)
        {
            int result = -1;
            SqlParameter[] Parms = SQLHelper.GetCachedParameters(SQL_GET_ELEMENT_COUNT_IN_PROD_VOLUMI_LAVORAVI_BY_DETTAGLIO);
            if (Parms == null)
            {
                Parms = new SqlParameter[]{
                   new SqlParameter("@RIFERIMENTO", SqlDbType.VarChar),
                   new SqlParameter("@IDLAV", SqlDbType.VarChar)
                };
            }
            Parms[0].Value = riferimento;
            Parms[1].Value = docCheckerId;

            try
            {
                using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.getConnStringDbsca(), CommandType.Text, SQL_GET_ELEMENT_COUNT_IN_PROD_VOLUMI_LAVORAVI_BY_DETTAGLIO, Parms))
                {
                    if (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0)) result = rdr.GetInt32(0);
                    }
                }
                return result;
            }
            catch
            {
                return result;
            }
        }

        private const string SQL_GET_ELEMENT_COUNT_IN_PROD_VOLUMI_DA_LAVORARE_BY_DETTAGLIO = "SELECT COUNT(*) FROM [DBSCA].[dbo].[PROD_VOLUMI_DA_LAVORARE] WHERE [DBSCA].[dbo].[PROD_VOLUMI_DA_LAVORARE].[DETTAGLIO]=@RIFERIMENTO AND [DBSCA].[dbo].[PROD_VOLUMI_DA_LAVORARE].[ID_DETTAGLIO_TIPO_LAVORAZIONE]=@IDLAV";

        public int GetElementCountInProdVolumiDaLavorareByDettaglio(string riferimento, int docCheckerId)
        {
            int result = -1;
            SqlParameter[] Parms = SQLHelper.GetCachedParameters(SQL_GET_ELEMENT_COUNT_IN_PROD_VOLUMI_DA_LAVORARE_BY_DETTAGLIO);
            if (Parms == null)
            {
                Parms = new SqlParameter[]{
                   new SqlParameter("@RIFERIMENTO", SqlDbType.VarChar),
                   new SqlParameter("@IDLAV", SqlDbType.VarChar)
                };
            }
            Parms[0].Value = riferimento;
            Parms[1].Value = docCheckerId;

            try
            {
                using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.getConnStringDbsca(), CommandType.Text, SQL_GET_ELEMENT_COUNT_IN_PROD_VOLUMI_DA_LAVORARE_BY_DETTAGLIO, Parms))
                {
                    if (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0)) result = rdr.GetInt32(0);
                    }
                }
                return result;
            }
            catch
            {
                return result;
            }
        }

        private const string SQL_GET_COUNT_BY_ID_DETTAGLIO_TIPO_LAVORAZIONE = "SELECT COUNT(*) FROM [PROD_VOLUMI_DA_LAVORARE] WHERE [ID_DETTAGLIO_TIPO_LAVORAZIONE] = @ID_DETTAGLIO_TIPO_LAVORAZIONE AND ID_STATO_SOSPENSIONE = 0";

        public int GetDaLavorareByIdDettaglioTipoLavorazione(int idDettaglioTipoLavorazione)
        {
            SqlParameter[] Parms = SQLHelper.GetCachedParameters(SQL_GET_COUNT_BY_ID_DETTAGLIO_TIPO_LAVORAZIONE);
            if (Parms == null)
            {
                Parms = new SqlParameter[]{
                   new SqlParameter("@ID_DETTAGLIO_TIPO_LAVORAZIONE", SqlDbType.Int)
                };
            }
            Parms[0].Value = idDettaglioTipoLavorazione;
            try
            {
                using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.getConnStringDbsca(), CommandType.Text, SQL_GET_COUNT_BY_ID_DETTAGLIO_TIPO_LAVORAZIONE, Parms))
                {
                    if (rdr.Read())
                        return rdr.GetInt32(0);
                }
                return -1;
            }
            catch
            {
                return -1;
            }
        }

        private const string SQL_GET_EE253_SOSPESI = "SELECT TOP 5000 [DETTAGLIO] FROM [dbsca].[dbo].[PROD_VOLUMI_DA_LAVORARE] WITH(NOLOCK) LEFT OUTER JOIN [dbsca].[dbo].[USER] ON [dbsca].[dbo].[PROD_VOLUMI_DA_LAVORARE].ID_OPERATORE=[DBSCA].[dbo].[USER].ID_USER WHERE ID_DETTAGLIO_TIPO_LAVORAZIONE = @ID_DETTAGLIO_TIPO_LAVORAZIONE AND ID_STATO_SOSPENSIONE = @ID_STATO_SOSPENSIONE AND DETTAGLIO NOT LIKE '%%2-%' AND DETTAGLIO NOT LIKE '%$%' AND ID_SEDE = '6'";
        public List<string> GetSospesiFromDaLavorareByIdDettaglio(string idDettaglioTipoLavorazione, string idStatoSospensione)
        {
            List<string> listSospesiEE253 = new List<string>();
            SqlParameter[] Parms = SQLHelper.GetCachedParameters(SQL_GET_EE253_SOSPESI);
            if (Parms == null)
            {
                Parms = new SqlParameter[]{
                   new SqlParameter("@ID_DETTAGLIO_TIPO_LAVORAZIONE", SqlDbType.VarChar),
                   new SqlParameter("@ID_STATO_SOSPENSIONE", SqlDbType.VarChar)
                };
            }
            Parms[0].Value = idDettaglioTipoLavorazione;
            Parms[1].Value = idStatoSospensione;

            try
            {
                using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.getConnStringDbsca(), CommandType.Text, SQL_GET_EE253_SOSPESI, Parms))
                {
                    while (rdr.Read())
                    {
                        var sht = rdr["DETTAGLIO"].ToString();
                        listSospesiEE253.Add(sht);
                    }
                }
                return listSospesiEE253;
            }
            catch
            {
                return null;
            }
        }

        #region EE176
        public string GetDataMatrixRicDocumento(string riferimento)
        {
            SqlParameter[] Parms = SQLHelper.GetCachedParameters("SELECT TOP(1) [DATA_MATRIX_SMART] FROM [CDA].[dbo].[RIC_DOCUMENTO] WHERE NOME_FILE LIKE @RIFERIMENTO");
            bool flag = Parms == null;
            if (flag)
            {
                Parms = new SqlParameter[]
                {
                new SqlParameter("@RIFERIMENTO", SqlDbType.NVarChar)
                };
                SQLHelper.CacheParameters("SELECT TOP(1) [DATA_MATRIX_SMART] FROM [CDA].[dbo].[RIC_DOCUMENTO] WHERE NOME_FILE LIKE @RIFERIMENTO", Parms);
            }
            Parms[0].Value = riferimento + "%";
            string result2;
            try
            {
                using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.ConnStringEE176(), CommandType.Text, "SELECT TOP(1) [DATA_MATRIX_SMART] FROM [CDA].[dbo].[RIC_DOCUMENTO] WHERE NOME_FILE LIKE @RIFERIMENTO", Parms))
                {
                    string result = "";
                    while (rdr.Read())
                    {
                        bool flag2 = !rdr.IsDBNull(0);
                        if (flag2)
                        {
                            result = rdr.GetString(0);
                        }
                    }
                    result2 = result;
                }
            }
            catch
            {
                result2 = "";
            }
            return result2;
        }

        public string GetDataMatrixRicDocumento2(string riferimento)
        {
            SqlParameter[] Parms = SQLHelper.GetCachedParameters("SELECT TOP(1) [DATA_MATRIX] FROM [CDA].[dbo].[RIC_DOCUMENTO] WHERE NOME_FILE LIKE @RIFERIMENTO");
            bool flag = Parms == null;
            if (flag)
            {
                Parms = new SqlParameter[]
                {
                new SqlParameter("@RIFERIMENTO", SqlDbType.NVarChar)
                };
                SQLHelper.CacheParameters("SELECT TOP(1) [DATA_MATRIX] FROM [CDA].[dbo].[RIC_DOCUMENTO] WHERE NOME_FILE LIKE @RIFERIMENTO", Parms);
            }
            Parms[0].Value = riferimento + "%";
            string result2;
            try
            {
                using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.ConnStringEE176(), CommandType.Text, "SELECT TOP(1) [DATA_MATRIX] FROM [CDA].[dbo].[RIC_DOCUMENTO] WHERE NOME_FILE LIKE @RIFERIMENTO", Parms))
                {
                    string result = "";
                    while (rdr.Read())
                    {
                        bool flag2 = !rdr.IsDBNull(0);
                        if (flag2)
                        {
                            result = rdr.GetString(0);
                        }
                    }
                    result2 = result;
                }
            }
            catch
            {
                result2 = "";
            }
            return result2;
        }

        public string GetBarcode(string riferimento)
        {
            SqlParameter[] Parms = SQLHelper.GetCachedParameters("SELECT TOP(1) [BARCODE] FROM [CDA].[dbo].[PROD_BARCODE_SCANNER] WHERE RIFERIMENTO = @RIFERIMENTO");
            bool flag = Parms == null;
            if (flag)
            {
                Parms = new SqlParameter[]
                {
                new SqlParameter("@RIFERIMENTO", SqlDbType.NVarChar)
                };
                SQLHelper.CacheParameters("SELECT TOP(1) [BARCODE] FROM [CDA].[dbo].[PROD_BARCODE_SCANNER] WHERE RIFERIMENTO = @RIFERIMENTO", Parms);
            }
            Parms[0].Value = riferimento;
            string result2;
            try
            {
                using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.ConnStringEE176(), CommandType.Text, "SELECT TOP(1) [BARCODE] FROM [CDA].[dbo].[PROD_BARCODE_SCANNER] WHERE RIFERIMENTO = @RIFERIMENTO", Parms))
                {
                    string result = "";
                    while (rdr.Read())
                    {
                        bool flag2 = !rdr.IsDBNull(0);
                        if (flag2)
                        {
                            result = rdr.GetString(0);
                        }
                    }
                    result2 = result;
                }
            }
            catch
            {
                result2 = "";
            }
            return result2;
        }
        #endregion

        #region MARCUSMSG
        private const string SQL_MARCUSCHAT = "INSERT INTO [baltikit_db].[dbo].[MessageTable] ([MessageFrom], [MessageTo], [MessageDate], [Message], [UserID], [UserIDFrom]) VALUES (@MESSAGEFROM, @MESSAGETO, @MESSAGEDATE, @MESSAGE, @USERID, @USERIDFROM); SELECT @@IDENTITY;";

        public int Mark(string msg)
        {
            SqlParameter[] Parms = SQLHelper.GetCachedParameters(SQL_MARCUSCHAT);
            if (Parms == null)
            {
                Parms = new SqlParameter[]{
                   new SqlParameter("@MESSAGEFROM", SqlDbType.NVarChar),
                   new SqlParameter("@MESSAGETO", SqlDbType.NVarChar),
                   new SqlParameter("@MESSAGEDATE", SqlDbType.DateTime),
                   new SqlParameter("@MESSAGE", SqlDbType.NVarChar),
                   new SqlParameter("@USERID", SqlDbType.Int),
                   new SqlParameter("@USERIDFROM", SqlDbType.Int)
                };
            }
            Parms[0].Value = "MARCUS";
            Parms[1].Value = "ICT";
            Parms[2].Value = DateTime.Now;
            Parms[3].Value = msg;
            Parms[4].Value = 666;
            Parms[5].Value = 666;
            try
            {
                using (SqlConnection conn = new SqlConnection("Data Source=BALTIKPDC;Initial Catalog=baltikit_db;Integrated Security=True"))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            int rdr = Convert.ToInt32(SQLHelper.ExecuteScalar(trans, CommandType.Text, SQL_MARCUSCHAT, Parms));
                            trans.Commit();
                            return rdr;
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                return -1;
            }
        }
        #endregion
    }
}