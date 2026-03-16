using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Universitetssystem
{
    internal class Kurs
    {
        public string KursKode { get; set; }
        public string KursNavn { get; set; }
        public int StudiePoeng { get; set; }
        public int AntallPlasser { get; set; }
        public List<Student> Deltakere { get; } = new();

        public Kurs(string kursKode, string kursNavn, int studiePoeng, int antallPlasser)
        {
            KursKode = kursKode;
            KursNavn = kursNavn;
            StudiePoeng = studiePoeng;
            AntallPlasser = antallPlasser;
        }

        public bool HarLedigPlass()
        {
            return Deltakere.Count < AntallPlasser;
        }

        public bool MeldPaStudent(Student student)
        {
            if (!HarLedigPlass())
            {
                return false;
            }

            if (Deltakere.Any(s => s.StudentId == student.StudentId))
            {
                return false;
            }

            Deltakere.Add(student);

            if (!student.PameldteKursKoder.Contains(KursKode))
            {
                student.PameldteKursKoder.Add(KursKode);
            }

            return true;
        }

        public bool MeldAvStudent(Student student)
        {
            Student? registrertStudent = Deltakere.FirstOrDefault(s => s.StudentId == student.StudentId);
            if (registrertStudent is null)
            {
                return false;
            }

            Deltakere.Remove(registrertStudent);
            student.PameldteKursKoder.Remove(KursKode);
            return true;
        }
    }
}