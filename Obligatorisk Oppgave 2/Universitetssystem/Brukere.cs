using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universitetssystem
{
    internal enum BrukerRolle
    {
        Student,
        Bibliotekar,
        Professor
    }

    internal abstract class Bruker
    {
        public string Navn { get; set; }
        public string Email { get; set; }
        public string Passord { get; set; }
        public BrukerRolle Rolle { get; set; }

        protected Bruker(string navn, string email, string passord, BrukerRolle rolle)
        {
            Navn = navn;
            Email = email;
            Passord = passord;
            Rolle = rolle;
        }

        public abstract string HentID();
    }

    internal class Student : Bruker
    {
        public int StudentId { get; set; }
        public List<string> PameldteKursKoder { get; } = new();

        public Student(
            int studentId,
            string navn,
            string email,
            string passord,
            List<string> pameldteKursKoder)
            : base(navn, email, passord, BrukerRolle.Student)
        {
            StudentId = studentId;
            PameldteKursKoder = pameldteKursKoder;
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
            string passord,
            List<string> pameldteKursKoder,
            string hjemuniversitet,
            string land,
            string periode)
            : base(studentId, navn, email, passord, pameldteKursKoder)
        {
            Hjemuniversitet = hjemuniversitet;
            Land = land;
            Periode = periode;
        }
    }

    internal class Ansatt : Bruker
    {
        public int AnsattId { get; set; }
        public string Avdeling { get; set; }

        public Ansatt(
            int ansattId,
            string navn,
            string email,
            string passord,
            BrukerRolle rolle,
            string avdeling)
            : base(navn, email, passord, rolle)
        {
            AnsattId = ansattId;
            Avdeling = avdeling;
        }

        public override string HentID()
        {
            return $"AnsattID: {AnsattId}";
        }
    }
}
