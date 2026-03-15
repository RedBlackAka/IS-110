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

        public Student()
        {
            Navn = Navn;
            ID = ID;
            Email = Email;
            PåmeldteKurs = PåmeldteKurs;
        }
    }
}