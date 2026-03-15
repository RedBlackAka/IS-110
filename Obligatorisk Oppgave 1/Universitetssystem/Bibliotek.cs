using System;
using System.Collections.Generic;
using System.Text;

namespace Universitetssystem
{
    internal class Bok
    {
        public string Tittel { get; set; }
        public string Forfatter { get; set; }
        public int ID { get; set; }
        public int Utgivelsesår { get; set; }
        public int AntallEksemplarer { get; set; }

        public Bok()
        {
            
        }
    }
}