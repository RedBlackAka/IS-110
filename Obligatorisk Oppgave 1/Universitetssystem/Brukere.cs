using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universitetssystem
{
    internal class Bruker
    {
        public string Navn { get; set;}
        public int ID { get; set; }
        public string Email { get; set;}
        public string PåmeldteKurs { get; set; }

        public Bruker(string navn, int id, string email, string påmeldteKurs)
        {
            Navn = navn;
            ID = ID;
            Email = email;
            PåmeldteKurs = påmeldteKurs;
        }
    }
}