using com.sun.org.apache.xerces.@internal.util;
using Controlli_ee145;
using javax.swing.text.html;
using log4net;
using MARCUS.Controll;
using MARCUS.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using SFALibrary.Model;
using sun.util.resources.cldr.dz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MARCUS.Controlli
{
    public class ControliED32
    {
        string name = "CONTROL";
        int reso = 37196;
        string clienteName = "";
        string descbankanote = "";
        string fatformatNumber = "";
        string impformatNumber = "";
        public string matr = "A490848@enelint.global";
        public string password = "Baltik2024***";
        public string webadress = "egc-prod.awselb.enelint.global";
        public string numurs = "";
        string stato = "";
        string koment = "";
        public string date1 = "";
        public string date2 = "";
        public DateTime dataBase;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        //Option - get values from lavarizone agente table where ID_CODICE_RESO + 37196 and DATA_RICEZIONE_DOCUMENTO is DateTimeNow


        public ControliED32(Keanu keanu)
        {
            this.Keanu = keanu;
        }

        public List<ArchivioEgc> v = new List<ArchivioEgc>();
        public List<ArchivioEgc> FindAtt { get; set; }
        public Keanu Keanu { get; set; }
        private static readonly ILog log = LogManager.GetLogger(typeof(Controller));
        public bool Flow()
        {
            if (Keanu.Excel145 == false)
            {
                StartDate = Keanu.fromDateK;
                date1 = StartDate?.ToString("yyyy-MM-dd");

                EndDate = Keanu.toDateK;
                date2 = EndDate?.ToString("yyyy-MM-dd");

                DbContext db = new DbContext();
                FindAtt = db.GetebanutijAttivita4(date1, date2);
            }

            List<string> aktjivitiis = FindAtt.Select(item => item.Attivita).ToList();
            ExcelManipulations ex = new ExcelManipulations();
            /*ex.ImportExcelEGC(out aktjivitiis);
            if (aktjivitiis == null)
            {
                log.Error($"null");
                return false;
            }
            if (aktjivitiis.Count > 0)
                log.Debug($"{aktjivitiis.Count} attività");
            else
            {
                log.Error($"{aktjivitiis.Count} attività");
                return false;
            }*/
            //prepare the portal before each statement
            Keanu.PapperYourED32(Keanu.LoginEGC, Keanu.PassEGC, webadress);

            foreach (var act in aktjivitiis)
            {
                numurs = act;
                DbContext db = new DbContext();
                Keanu.Driver.Navigate().GoToUrl("https://egc-prod.awselb.enelint.global/egc/GestAccrediti/RicAccrediti.jsp");

                Keanu.Driver.SwitchTo().DefaultContent();
                //goes to lupa icon and clicks
                if (!FirstSearch(act))
                    continue;
               /* using ( var client  = new HttpClient() ) 
                {
                    var endpoint = new Uri("https://egc-prod.awselb.enelint.global/egc/GestAccrediti/DettAccrediti.jsp?identificativoDocumento=16588922&impegnato=");
                    var result = client.GetAsync(endpoint).Result;
                    var json = result.Content.ReadAsStringAsync().Result;
                }*/
                //searches Lupa              
                string fatturaValue = db.SearchInDB(act, "fattura");
                string importoValue = db.SearchInDB(act, "importo");
                if (!LupaSearch(db, act))
                {
                    koment = "fattura in not present in descr.Banka";
                    Keanu.Bad.Scarto++;
                    continue;
                }
                if (fatturaValue.Length == 0)
                {//if fattura not provided this, need modifying
                    if (!IfFatturaNotPressent(db, act))
                    {
                        Keanu.Bad.Scarto++;
                        stato = "KO";
                        koment = "fattura in not present";
                        UpdateArchivio();
                        continue;
                    }
                    else
                    {
                        Keanu.Bad.Fatto++;
                        stato = "OK";
                        UpdateArchivio();
                        continue;
                    }
          
                    
                }

                if (fatformatNumber == fatturaValue)
                {
                    if (importoValue == impformatNumber)
                    {
                        if (!NextCheck1()){
                            Keanu.Bad.Scarto++;
                            stato = "KO";
                            UpdateArchivio();
                            continue;
                        }
                        else
                        {
                            Keanu.Bad.Fatto++;
                            stato = "OK";
                            UpdateArchivio();
                            continue;
                        }
                    }
                    else { log.Error("KO"); Keanu.Bad.Scarto++; UpdateArchivio(); continue;  }
                    


                    /*try //riecerca
                    {
                        IWebElement ricercabutton = Keanu.Driver.FindElement(By.XPath("//*[@id=\"ricercagood\"]/a"));
                        ricercabutton.Click();
                    }
                    catch { }
                    try //clickzalupa
                    {
                        IWebElement lupa = Keanu.Driver.FindElement(By.XPath("/html/body/center/form[1]/table/tbody/tr[2]/td[10]/a"));
                        lupa.Click();
                    }
                    catch { }
                    */

                }
                else { log.Error("KO"); }

                bool NextCheck1()
                {
                    Keanu.Driver.Navigate().GoToUrl("https://egc-prod.awselb.enelint.global/egc/GestCrediti/RicCrediti.jsp");
                    IWebElement asda = Keanu.Driver.FindElement(By.XPath("/html/body/center/form/table[1]/tbody/tr[1]/td/table/tbody/tr/td[3]/input"));
                    asda.SendKeys(fatformatNumber);
                    IWebElement ricercabutton = Keanu.Driver.FindElement(By.Id("ButtonConferma"));
                    ricercabutton.Click();


                    IWebElement descrmovbanca = Keanu.Driver.FindElement(By.XPath("/html/body/center/table[3]/tbody/tr/td[2]/form[1]/table"));
                    {
                        string descr = descrmovbanca.Text;
                        if (descr.Contains(importoValue))
                        {
                                return true;
                        }
                        else
                        {
                            IList<IWebElement> datiElements = Keanu.Driver.FindElements(By.CssSelector("td.dati"));
                            bool foundMatch = false;
                            foreach (IWebElement element in datiElements)
                            {
                                string text = element.Text.Trim();
                                if (text == importoValue)
                                {
                                    log.Debug("Value matched: " + importoValue);
                                    foundMatch = true;
                                    break;
                                }
                                if(!foundMatch)
                                {
                                    
                                    IWebElement td10Element = Keanu.Driver.FindElement(By.XPath("/html/body/center/table[3]/tbody/tr/td[2]/form[1]/table/tbody/tr[3]/td[10]"));
                                    string td10Text = td10Element.Text.Trim();

                                    IWebElement td13Element = Keanu.Driver.FindElement(By.XPath("/html/body/center/table[3]/tbody/tr/td[2]/form[1]/table/tbody/tr[3]/td[13]"));
                                    string td13Text = td13Element.Text.Trim();

                                   
                                    if (double.TryParse(td10Text, out double td10Value) && double.TryParse(td13Text, out double td13Value))
                                    {
                                        
                                        double difference = td10Value - td13Value;
                                        if (difference == double.Parse(importoValue))
                                        {
                                            log.Debug("Difference matched: " + importoValue);
                                            koment = "Importo Sakriit ar totale fatture - cannone rai";
                                                return true;
                                        }
                                        else
                                        {
                                            log.Debug("Difference did not match: " + importoValue);
                                            koment = "Importo Nesakriit ar totale fatture - cannone rai";
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        log.Debug("Values not numeric or second value not present.");
                                        return false;
                                    }
                                }
                            }
                            

                        }
                        koment = "Nav Importo summa totale fattura un cannone rai atnemot ar nesakrit ";
                        return false;

                    }

                }


            }
            ex.ExportExcelEGC(v);
            return true;
        }

       public void UpdateArchivio()
        {
            v.Add(new ArchivioEgc
            {
                Attivita = numurs,
                STATOS = stato,
                KOMENT = koment

            });
        }
        public bool FirstSearch(string attivita)
        {

            try // id. doc
            {
                IWebElement iddoc = Keanu.Driver.FindElement(By.XPath("//*[@id=\"identificativoDocumento\"]/input[1]"));
                iddoc.Click();
                iddoc.SendKeys(attivita);
            }
            catch { return false; }

            try //riecerca
            {
                IWebElement ricercabutton = Keanu.Driver.FindElement(By.XPath("//*[@id=\"ricercagood\"]/a"));
                ricercabutton.Click();
            }
            catch { return false; }
            try //clickzalupa
            {
                IWebElement lupa = Keanu.Driver.FindElement(By.XPath("/html/body/center/form[1]/table/tbody/tr[2]/td[10]/a"));
                lupa.Click();
            }
            catch { return false; }
            return true;
        }

        public bool LupaSearch(DbContext db, string act)
        {
            try
            {
                IWebElement descrmovbanca = Keanu.Driver.FindElement(By.XPath("/html/body/center/center/table[1]/tbody/tr[13]/td[3]/table"));

                string descr = descrmovbanca.Text;
                descbankanote = descrmovbanca.Text;
                string pattern = @"BOLLETTA (\d+)";
                Match match = Regex.Match(descr, pattern);

                if (match.Success)
                {
                    string fatnumber = match.Groups[1].Value;
                    fatformatNumber = "00" + fatnumber;

                    // number
                    log.Debug("Number is found: " + fatformatNumber);

                    // check length
                    if (fatformatNumber.Length == 16)
                    {
                        log.Debug("ОК");
                    }
                    else
                    {
                        log.Error("Fattura Number is not 16");
                        return false;
                    }
                }
                else
                {
                    //if fatturaValue has 0 but descr note dont have 0 
                    string fatturaValue = db.SearchInDB(act, "fattura");
                    string fatturaWithChanges = fatturaValue.Substring(1);
                    fatformatNumber = fatturaValue; // this for check later
                    if (descbankanote.Contains(fatturaWithChanges))
                    {
                        log.Debug("ОК");
                    }
                }
            }

            catch { }
            try
            {
                IWebElement descrmovbanca = Keanu.Driver.FindElement(By.XPath("/html/body/center/center/table[1]/tbody/tr[4]/td[3]/table/tbody/tr/td"));
                impformatNumber = descrmovbanca.Text;

            }
            catch { }
            return true;
        }

        public bool IfFatturaNotPressent(DbContext db, string act)
        {
                Keanu.Driver.Navigate().GoToUrl("https://egc-prod.awselb.enelint.global/egc/GestAccrediti/RicAccrediti.jsp");

                Keanu.Driver.SwitchTo().DefaultContent();
                try // id. doc
                {
                    IWebElement iddoc = Keanu.Driver.FindElement(By.XPath("//*[@id=\"identificativoDocumento\"]/input[1]"));
                    iddoc.Click();
                    iddoc.SendKeys(act);
                }
                catch { }
                try //riecerca
                {
                    IWebElement ricercabutton = Keanu.Driver.FindElement(By.XPath("//*[@id=\"ricercagood\"]/a"));
                    ricercabutton.Click();
                }
                catch { }
                try //clickzalupa
                {
                    IWebElement lupa = Keanu.Driver.FindElement(By.XPath("/html/body/center/form[1]/table/tbody/tr[2]/td[10]/a"));
                    lupa.Click();
                }
                catch { }
                try
                {
                    try
                    {
                        IWebElement descrmovbanca = Keanu.Driver.FindElement(By.XPath("/html/body/center/center/table[1]/tbody/tr[13]/td[3]/table"));

                        string descr = descrmovbanca.Text;
                        string pattern = @"CLIENTE (\d+)";
                        Match match = Regex.Match(descr, pattern);

                        if (match.Success)
                        {
                            // get number
                            string ClienteNumber = match.Groups[1].Value;
                            string eneltel = db.SearchInDB(act, "eneltel");

                            if (eneltel == ClienteNumber)
                            {
                                Keanu.Driver.Navigate().GoToUrl("https://egc-prod.awselb.enelint.global/egc/GestPrese/RicPrese.jsp");
                                IWebElement Ricercaeneltel = Keanu.Driver.FindElement(By.XPath("/html/body/center/center/p/table[1]/tbody/tr[13]/td[3]/input"));
                                Ricercaeneltel.SendKeys(eneltel);
                                IWebElement pressricerca = Keanu.Driver.FindElement(By.Id("ricercagood"));
                                pressricerca.Click();
                                IWebElement FindTablo = Keanu.Driver.FindElement(By.XPath("/html/body/center/form[1]/table"));
                                IWebElement tbody = FindTablo.FindElement(By.TagName("tbody"));
                                foreach (IWebElement row in tbody.FindElements(By.TagName("td")))
                                {
                                    if (tbody.Text.Contains("Attivo"))
                                    {
                                        string rowText = tbody.Text;
                                        var lines = rowText.Split('\n', (char)StringSplitOptions.RemoveEmptyEntries);
                                        try
                                        {
                                            for (int i = 1; i < lines.Length; i++)
                                            {
                                                var cells = lines[i].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                                                if (cells.Contains("Attivo"))
                                                {
                                                    int cessatoIndex = Array.IndexOf(cells, "Attivo");

                                                    if (cessatoIndex > 0)
                                                    {
                                                        string codiceFiscale = cells[cessatoIndex - 5];
                                                        if (RicherckaFatuura(codiceFiscale))
                                                            return true;
                                                        else return false;
                                                    }
                                                }
                                                else
                                                {
                                                log.Debug("Not Attivo");
                                                return false;
                                                }
                                            }
                                        }
                                        catch
                                        {
                                        log.Debug("Cant get rows");
                                        }
                                    }
                                }

                                if (FindTablo.Text.Contains("Attivo"))
                                {
                                    IWebElement FindButtonClick = FindTablo.FindElement(By.XPath("/html/body/center/form[1]/table/tbody/tr[3]/td[11]/a"));
                                    FindButtonClick.Click();
                                }
                            }
                            else
                            {
                                log.Debug("eneltel not equal.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Number after 'BOLLETTA' not Found.");
                        }
                    }
                    catch { }
                }
                catch { }
            return true;
        }
        public bool RicherckaFatuura(string vol)
        {
            /*   IWebElement RicercaClienti = Keanu.Driver.FindElement(By.Id("oCMenu_top8"));
               RicercaClienti.Click();
               IWebElement RicercaClientitwo = Keanu.Driver.FindElement(By.Id("oCMenu_3"));
               RicercaClientitwo.Click();*/
            Keanu.Driver.Navigate().GoToUrl("https://egc-prod.awselb.enelint.global/egc/GestCrediti/RicCrediti.jsp");
            bool shouldBreak = false;
            bool optionFound = false;
            if (fatformatNumber.Length == 0)
            {
                IWebElement RicercaClienti = Keanu.Driver.FindElement(By.XPath("/html/body/center/form/table[1]/tbody/tr[3]/td/table/tbody/tr[1]/td[3]/input"));
                RicercaClienti.SendKeys(vol);

                IWebElement checkbox = Keanu.Driver.FindElement(By.XPath("/html/body/center/form/table[1]/tbody/tr[3]/td/table/tbody/tr[4]/td[5]/table/tbody/tr/td[4]/input"));
                checkbox.Click();

                IWebElement subdivison = Keanu.Driver.FindElement(By.XPath("/html/body/center/form/table[1]/tbody/tr[3]/td/table/tbody/tr[1]/td[4]/select"));
                subdivison.Click();

                IWebElement selectElement = Keanu.Driver.FindElement(By.Name("idUnitaOrg"));
                var options = selectElement.FindElements(By.TagName("option"));

                for (int i = 1; i <= options.Count; i++)
                {
                    IWebElement currentOption = Keanu.Driver.FindElement(By.XPath($"/html/body/center/form/table[1]/tbody/tr[3]/td/table/tbody/tr[1]/td[4]/select/option[{i}]"));
                    currentOption.Click();

                    IWebElement conferma = Keanu.Driver.FindElement(By.Id("ButtonConferma"));
                    conferma.Click();

                    IWebElement tablinskis = Keanu.Driver.FindElement(By.XPath("/html/body/center/table[3]/tbody/tr/td[2]/form[1]/table"));
                    if (tablinskis.Text.Contains(impformatNumber))
                    {
                        log.Debug("ir");
                        shouldBreak = true;
                        break;
                    }
                    else
                    {
                        log.Debug("NAvv");
                        if (CheckPages())
                        {
                            shouldBreak = true;
                            break;
                        }
                        else
                        {

                            Keanu.Driver.Navigate().GoToUrl("https://egc-prod.awselb.enelint.global/egc/GestCrediti/RicCrediti.jsp");
                            IWebElement RicercaClienti2 = Keanu.Driver.FindElement(By.XPath("/html/body/center/form/table[1]/tbody/tr[3]/td/table/tbody/tr[1]/td[3]/input"));
                            RicercaClienti2.SendKeys(vol);

                            IWebElement checkbox1 = Keanu.Driver.FindElement(By.XPath("/html/body/center/form/table[1]/tbody/tr[3]/td/table/tbody/tr[4]/td[5]/table/tbody/tr/td[4]/input"));
                            checkbox1.Click();

                            IWebElement subdivison1 = Keanu.Driver.FindElement(By.XPath("/html/body/center/form/table[1]/tbody/tr[3]/td/table/tbody/tr[1]/td[4]/select"));
                            subdivison1.Click();
                        }
                    }
                    optionFound = true;
                }
            }


            if (shouldBreak)
            {
                koment = "Importo contains totale fattura";
                return true;

            }
            if (optionFound)
            {
                koment = "Importo doesnt contains totale fattura";
                log.Debug("Doesn't contain through all options");
                return false;
            }
            IWebElement ricercaRiff = Keanu.Driver.FindElement(By.XPath("/html/body/center/form/table[1]/tbody/tr[1]/td/table/tbody/tr/td[3]/input"));
            ricercaRiff.SendKeys(fatformatNumber);
            IWebElement conferma2 = Keanu.Driver.FindElement(By.Id("ButtonConferma"));
            conferma2.Click();
            if (RicercaFatturaTable(impformatNumber))
            {
                return true;
            }
            return false;
        }

        public bool CheckPages()
        {
            string url = "https://egc-prod.awselb.enelint.global/egc/GestCrediti/VisCrediti.jsp";
            if (Keanu.Driver.Url == url)
            {
                IWebElement Pic = Keanu.Driver.FindElement(By.XPath("/html/body/center/table[4]/tbody/tr/td[1]/a/img"));
                for (int i = 0; i < 6; i++)
                    GoDOWN();
                IWebElement page = Keanu.Driver.FindElement(By.XPath("/html/body/center/table[5]/tbody/tr/td"));
                if (page.Text.Contains("Pag. 1 di"))
                {
                    Pic.Click();
                    IWebElement tablinskis = Keanu.Driver.FindElement(By.XPath("/html/body/center/table[3]/tbody/tr/td[2]/form[1]/table"));
                    if (tablinskis.Text.Contains(impformatNumber))
                    {
                        log.Debug("ir");
                            return true;
                    }
                    else
                    {
                        IWebElement navigationElement = Keanu.Driver.FindElement(By.XPath("//td[@colspan='5' and @align='center']"));

                        if (navigationElement.Displayed)
                        {
                            IWebElement tablinskis2 = Keanu.Driver.FindElement(By.XPath("/html/body/center/table[3]/tbody/tr/td[2]/form[1]/table"));
                            if (tablinskis2.Text.Contains(impformatNumber))
                            {
                                log.Debug("ir");
                                    return true;
                            }
                            else
                            {
                                IWebElement navigationElement2 = Keanu.Driver.FindElement(By.XPath("//td[@colspan='5' and @align='center']"));

                                if (navigationElement2.Displayed)
                                {
                                    IWebElement tablinskis3 = Keanu.Driver.FindElement(By.XPath("/html/body/center/table[3]/tbody/tr/td[2]/form[1]/table"));
                                    if (tablinskis3.Text.Contains(impformatNumber))
                                    {
                                        log.Debug("ir");
                                            return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                else return false;
                            }
                        }
                        else
                        {
                            // The element is not visible
                            log.Debug("Navigation element is not visible");
                            return false;
                        }
                    }
                }
                else return false;
            }
            else return false;


            return true;
        }

            void GoDOWN()
            {
                    IJavaScriptExecutor js = (IJavaScriptExecutor)Keanu.Driver;
                    js.ExecuteScript($"window.scrollBy(0,300)", "");
                    Thread.Sleep(500);
            }

            public bool findClienteName()
            {
                IWebElement suddivisonCliente = Keanu.Driver.FindElement(By.XPath("/html/body/center/table[1]/tbody/tr[1]/td[5]"));
                string clientecheckone = suddivisonCliente.Text.Replace("000 - ", null);
                clientecheckone = clientecheckone.Trim();
            if (descbankanote.Contains(clientecheckone))
                {
                log.Debug( "Sakriit clients ar desct.nota");
                return true;
                    // Additional actions if needed
                }
                return false;
            }
            public bool RicercaFatturaTable(string imp)
            {
                IWebElement tablinskis = Keanu.Driver.FindElement(By.XPath("/html/body/center/table[3]/tbody/tr/td[2]/form[1]/table"));
                if (tablinskis.Text.Contains(imp))
                {

                    log.Debug("Sakriit importo ar totale fattura ");
                return true;
                }
                log.Debug("NAvv");
                return false;


            }
        }
    } 





