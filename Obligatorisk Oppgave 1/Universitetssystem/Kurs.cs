using System;
using System.Text;

namespace Universitetssystem
{
    internal class Kurs
    {
        public string KursKode { get; set; }
        public string KursNavn { get; set; }
        public int StudiePoeng { get; set; }
        public int AntallPlasser { get; set; }

        public Kurs(string kursKode, string kursNavn, int studiePoeng, int antallPlasser)
        {
            KursKode = kursKode;
            KursNavn = kursNavn;
            StudiePoeng = studiePoeng;
            AntallPlasser = antallPlasser;
        }
    }
}