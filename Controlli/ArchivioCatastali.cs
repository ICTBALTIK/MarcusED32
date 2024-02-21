using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controlli_ee145
{
    public class ArchivioCatastali
    {

        public string RIFERIMENTO { get; set; }
        public string NUMERO_CLIENTE { get; set; }
        public DateTime DATA { get; set; }

        public int STATO { get; set; }


    }
}
