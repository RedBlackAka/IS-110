using System;
using System.Collections.Generic;
using System.Text;

namespace Universitetssystem
{
    internal class Bok
    {
        public string Tittel { get; set; }
        public string Forfatter { get; set; }
        public int BokID { get; set; }
        public int UtgivelsesÅr { get; set; }
        public int AntallEksemplarer { get; set; }

        public Bok(string tittel, string forfatter, int bokId, int utgivelsesÅr, int antallEksemplarer)
        {
            Tittel = tittel;
            Forfatter = forfatter;
            BokID = bokId;
            UtgivelsesÅr = utgivelsesÅr;
            AntallEksemplarer = antallEksemplarer;
        }
    }
}