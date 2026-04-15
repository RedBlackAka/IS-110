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
        public int UnderviserAnsattId { get; set; }
        public List<Student> Deltakere { get; } = new();
        public List<string> Pensum { get; } = new();
        public Dictionary<int, string> Karakterer { get; } = new();

        public Kurs(string kursKode, string kursNavn, int studiePoeng, int antallPlasser, int underviserAnsattId)
        {
            KursKode = kursKode;
            KursNavn = kursNavn;
            StudiePoeng = studiePoeng;
            AntallPlasser = antallPlasser;
            UnderviserAnsattId = underviserAnsattId;
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

        public bool RegistrerPensum(string pensumlinje)
        {
            if (string.IsNullOrWhiteSpace(pensumlinje))
            {
                return false;
            }

            if (Pensum.Any(p => p.Equals(pensumlinje, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            Pensum.Add(pensumlinje.Trim());
            return true;
        }

        public bool SettKarakter(int studentId, string karakter)
        {
            if (!Deltakere.Any(s => s.StudentId == studentId))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(karakter))
            {
                return false;
            }

            Karakterer[studentId] = karakter.Trim().ToUpperInvariant();
            return true;
        }

        public string? HentKarakter(int studentId)
        {
            return Karakterer.TryGetValue(studentId, out string? karakter)
                ? karakter
                : null;
        }
    }
}