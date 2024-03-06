using com.sun.org.apache.xerces.@internal.util;
using Controlli_ee145;
using javax.swing.text.html;
using log4net;
using MARCUS.Controll;
using MARCUS.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using sun.util.resources.cldr.dz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MARCUS.Controlli
{
    public class ControliED32
    {
        string fatformatNumber = "";
        string impformatNumber = "";
        public string matr = "A490848@enelint.global";
        public string password = "Baltik2024***";
        public string webadress = "egc-prod.awselb.enelint.global";
        public string numurs = "16459393";

        public ControliED32(Keanu keanu)
        {
            this.Keanu = keanu;
        }
        public Keanu Keanu { get; set; }
        private static readonly ILog log = LogManager.GetLogger(typeof(Controller));
        public bool Flow()
        {
            DbContext db = new DbContext();

            Keanu.PapperYourED32(matr, password, webadress);

            Keanu.Driver.Navigate().GoToUrl("https://egc-prod.awselb.enelint.global/egc/GestAccrediti/RicAccrediti.jsp");

            Keanu.Driver.SwitchTo().DefaultContent();

            try // id. doc
            {
                IWebElement iddoc = Keanu.Driver.FindElement(By.XPath("//*[@id=\"identificativoDocumento\"]/input[1]"));
                iddoc.Click();
                iddoc.SendKeys(numurs);
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
                IWebElement descrmovbanca = Keanu.Driver.FindElement(By.XPath("/html/body/center/center/table[1]/tbody/tr[13]/td[3]/table"));

                string descr = descrmovbanca.Text;
                string pattern = @"BOLLETTA (\d+)";
                Match match = Regex.Match(descr, pattern);

                if (match.Success)
                {
                    // get number
                    string fatnumber = match.Groups[1].Value;
                    fatformatNumber = "00" + fatnumber;

                    // number
                    Console.WriteLine("Number is found: " + fatformatNumber);

                    // check length
                    if (fatformatNumber.Length == 16)
                    {
                        Console.WriteLine("ОК");
                    }
                    else
                    {
                        Console.WriteLine("КО");
                    }
                }
                else
                {
                    Console.WriteLine("Number after 'BOLLETTA' not Found.");
                }
            }
            catch { }
            try
            {
                IWebElement descrmovbanca = Keanu.Driver.FindElement(By.XPath("/html/body/center/center/table[1]/tbody/tr[4]/td[3]/table/tbody/tr/td"));
                impformatNumber = descrmovbanca.Text;

            }
            catch { }
            string fatturaValue = db.SearchInDB(numurs, "fattura");
            string importoValue = db.SearchInDB(numurs, "importo");
            if (fatturaValue.Length == 0)
            {
                Keanu.Driver.Navigate().GoToUrl("https://egc-prod.awselb.enelint.global/egc/GestAccrediti/RicAccrediti.jsp");

                Keanu.Driver.SwitchTo().DefaultContent();

                try // id. doc
                {
                    IWebElement iddoc = Keanu.Driver.FindElement(By.XPath("//*[@id=\"identificativoDocumento\"]/input[1]"));
                    iddoc.Click();
                    iddoc.SendKeys(numurs);
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
                            string eneltel = db.SearchInDB(numurs, "eneltel");

                            if(eneltel == ClienteNumber)
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
                                                        RicherckaFatuura(codiceFiscale);
                                                        break;

                                                        
                                                    }
                                                }

                                            }
                                        }

                                        catch
                                        {
                                        }   
                                        
                                   }

                                    
                                }
                               void  RicherckaFatuura(string vol)
                                {

                                }


                                if (FindTablo.Text.Contains("Attivo"))
                                {
                                    
                                 IWebElement FindButtonClick = FindTablo.FindElement(By.XPath("/html/body/center/form[1]/table/tbody/tr[3]/td[11]/a"));
                                    FindButtonClick.Click();
                                }
                            }
                            else
                            {
                                Console.WriteLine("eneltel not equal.");
                            }
                            // number

                            // check length
                            
                        }
                        else
                        {
                            Console.WriteLine("Number after 'BOLLETTA' not Found.");
                        }
                    }
                    catch { }
                }
                catch { }

            }

            if (fatformatNumber == fatturaValue)
            {
                if (importoValue == impformatNumber)
                {
                    NextCheck1();
                }
                else { Console.WriteLine("KO"); }


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


            }
            else { Console.WriteLine("KO"); }

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
                }


                return false;
            }

            return true;
        }


    }



}
