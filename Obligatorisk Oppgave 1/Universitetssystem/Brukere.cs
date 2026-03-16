using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universitetssystem
{
    internal abstract class Bruker
    {
        public string Navn { get; set; }
        public string Email { get; set; }

        protected Bruker(string navn, string email)
        {
            Navn = navn;
            Email = email;
        }

        public abstract string HentID();
    }

    internal class Student : Bruker
    {
        public int StudentId { get; set; }
        public List<string> PameldteKursKoder { get; } = new();

        public Student(int studentId, string navn, string email)
            : base(navn, email)
        {
            StudentId = studentId;
        }

        public override string HentID()
        {
            return $"StudentID: {StudentId}";
        }
    }

    internal class Utvekslingsstudent : Student
    {
        public string Hjemuniversitet { get; set; }
        public string Land { get; set; }
        public string Periode { get; set; }

        public Utvekslingsstudent(
            int studentId,
            string navn,
            string email,
            string hjemuniversitet,
            string land,
            string periode)
            : base(studentId, navn, email)
        {
            Hjemuniversitet = hjemuniversitet;
            Land = land;
            Periode = periode;
        }
    }

    internal class Ansatt : Bruker
    {
        public int AnsattId { get; set; }
        public string Stilling { get; set; }
        public string Avdeling { get; set; }

        public Ansatt(int ansattId, string navn, string email, string stilling, string avdeling)
            : base(navn, email)
        {
            AnsattId = ansattId;
            Stilling = stilling;
            Avdeling = avdeling;
        }

        public override string HentID()
        {
            return $"AnsattID: {AnsattId}";
        }
    }
}