using log4net;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace MARCUS.Helpers
{
    public class ExcelManipulations
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ExcelManipulations));

        public bool ExportExcel(List<string> list, string filename)
        {
            try
            {
                string datt = $"'{DateTime.Now.AddDays(-10):dd/MM/yyyy}";
                string user = Environment.UserName.ToUpper();
                string path = $"C:\\Users\\{user}\\Desktop\\MARCUSSOSPESI\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                Microsoft.Office.Interop.Excel.Application Excel = new Microsoft.Office.Interop.Excel.Application();
                Workbook workbook = Excel.Workbooks.Add(System.Reflection.Missing.Value);
                Worksheet Sheet1 = (Worksheet)workbook.Sheets[1];
                Sheet1.Columns.AutoFit();
                if (list != null)
                {
                    Sheet1.Cells[1, 1] = "RIFERIMENTO";
                    Sheet1.Cells[1, 2] = "DATA PROTOCOLLO";
                    int row = 2;
                    foreach (var item in list)
                    {
                        Sheet1.Cells[row, 1] = item;
                        Sheet1.Cells[row, 2] = datt;
                        row++;
                    }
                }
                workbook.SaveAs($"{path}{filename}.xlsx");
                workbook.Close(0);
                Excel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(Excel);
                //Excel.Visible = true;
                log.Debug($"{path}{filename}.xlsx");
                return true;
            }
            catch
            {
                log.Error($"{filename} fail");
                return false;
            }
        }

        public bool ExportExcelSFAgente(List<Records.SfaChekah> list, string filename)
        {
            try
            {
                //string datt = $"'{DateTime.Now.AddDays(-3):dd/MM/yyyy}";
                string user = Environment.UserName.ToUpper();
                string path = $"C:\\Users\\{user}\\Desktop\\MARCUSSF\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                Microsoft.Office.Interop.Excel.Application Excel = new Microsoft.Office.Interop.Excel.Application();
                Workbook workbook = Excel.Workbooks.Add(System.Reflection.Missing.Value);
                Worksheet Sheet1 = (Worksheet)workbook.Sheets[1];
                Sheet1.Columns.AutoFit();
                if (list != null)
                {
                    Sheet1.Cells[1, 1] = "ATTIVITÀ";
                    Sheet1.Cells[1, 2] = "TIPO ATTIVITÀ";
                    Sheet1.Cells[1, 3] = "STATO SFA";
                    Sheet1.Cells[1, 4] = "NUMERO DOCUMENTO";
                    Sheet1.Cells[1, 5] = "OP";
                    Sheet1.Cells[1, 6] = "STATO AGENTE";
                    Sheet1.Cells[1, 7] = "DESC_ALTRO";
                    Sheet1.Cells[1, 8] = "DATA APPARTURE";
                    int row = 2;
                    foreach (var item in list)
                    {
                        Sheet1.Cells[row, 1] = item.Attivita;
                        Sheet1.Cells[row, 2] = item.TipoSfa;
                        Sheet1.Cells[row, 3] = item.StatoSfa;
                        Sheet1.Cells[row, 4] = item.SF;
                        Sheet1.Cells[row, 5] = item.UserAgente;
                        Sheet1.Cells[row, 6] = item.StatoAgente;
                        Sheet1.Cells[row, 7] = item.Description;
                        Sheet1.Cells[row, 8] = item.DataA;
                        row++;
                    }
                }
                workbook.SaveAs($"{path}{filename}.xlsx");
                workbook.Close(0);
                Excel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(Excel);
                //Excel.Visible = true;
                log.Debug($"{path}{filename}.xlsx");
                return true;
            }
            catch
            {
                log.Error($"{filename} fail");
                return false;
            }
        }

        public bool ExportExcelSF(List<Records.SfaChekah> list, string filename)
        {
            try
            {
                //string datt = $"'{DateTime.Now.AddDays(-3):dd/MM/yyyy}";
                string user = Environment.UserName.ToUpper();
                string path = $"C:\\Users\\{user}\\Desktop\\MARCUSSF\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                Microsoft.Office.Interop.Excel.Application Excel = new Microsoft.Office.Interop.Excel.Application();
                Workbook workbook = Excel.Workbooks.Add(System.Reflection.Missing.Value);
                Worksheet Sheet1 = (Worksheet)workbook.Sheets[1];
                Sheet1.Columns.AutoFit();
                if (list != null)
                {
                    Sheet1.Cells[1, 1] = "RIFERIMENTO";
                    Sheet1.Cells[1, 2] = "SF";
                    int row = 2;
                    foreach (var item in list)
                    {
                        Sheet1.Cells[row, 1] = item.Attivita;
                        Sheet1.Cells[row, 2] = item.SF;
                        row++;
                    }
                }
                workbook.SaveAs($"{path}{filename}.xlsx");
                workbook.Close(0);
                Excel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(Excel);
                //Excel.Visible = true;
                log.Debug($"{path}{filename}.xlsx");
                return true;
            }
            catch
            {
                log.Error($"{filename} fail");
                return false;
            }
        }

        public bool ImportExcel(out List<string> list)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.DefaultExt = ".xlsx";
                ofd.Filter = "Excel Workbook|*.xlsx|Excel 97-2003 Workbook|*.xls";
                var sel = ofd.ShowDialog();
                if (sel == true)
                {
                    string fileName = ofd.FileName;
                    Microsoft.Office.Interop.Excel.Application Excel = new Microsoft.Office.Interop.Excel.Application();
                    Workbook workbook = Excel.Workbooks.Open(fileName, Type.Missing, true);
                    Worksheet Sheet1 = (Worksheet)workbook.Sheets[1];
                    Range range = Sheet1.UsedRange;
                    list = new List<string>();
                    foreach (Range item in range.Rows.Cells)
                    {
                        string vallie = "";
                        try
                        {
                            vallie = item.Value;
                            if (string.IsNullOrEmpty(vallie))
                                continue;
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        if (vallie.Contains("A-"))
                            list.Add(vallie.Trim());
                    }
                    workbook.Close(0);
                    Excel.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(Excel);
                }
                else
                {
                    log.Error($"Fail");
                    list = null;
                }
                return true;
            }
            catch
            {
                log.Error($"Fail");
                list = null;
                return false;
            }
        }
    }
}