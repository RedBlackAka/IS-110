using System;
using System.Collections.Generic;
using System.Linq;

namespace Universitetssystem;

internal class Program
{
    private static readonly List<Student> Studenter = new();
    private static readonly List<Ansatt> Ansatte = new();
    private static readonly List<Kurs> KursListe = new();
    private static readonly BibliotekService Bibliotek = new();

    private static void Main()
    {
        InitialiserData();

        bool avslutt = false;
        while (!avslutt)
        {
            SkrivMeny();
            string? valg = Console.ReadLine();

            Console.WriteLine();
            switch (valg)
            {
                case "1":
                    OpprettKurs();
                    break;
                case "2":
                    HandterKursPamelding();
                    break;
                case "3":
                    PrintKursOgDeltagere();
                    break;
                case "4":
                    SokPaKurs();
                    break;
                case "5":
                    SokPaBok();
                    break;
                case "6":
                    LanBok();
                    break;
                case "7":
                    ReturnerBok();
                    break;
                case "8":
                    RegistrerBok();
                    break;
                case "0":
                    avslutt = true;
                    Console.WriteLine("Avslutter universitetssystemet.");
                    break;
                default:
                    Console.WriteLine("Ugyldig valg, prøv igjen.");
                    break;
            }

            Console.WriteLine();
        }
    }

    private static void SkrivMeny()
    {
        Console.WriteLine("Velkommen til UiA's universitetssystem (UUS). Velg et alternativ:");
        Console.WriteLine("[1] Opprett kurs");
        Console.WriteLine("[2] Meld student til kurs");
        Console.WriteLine("[3] Print kurs og deltagere");
        Console.WriteLine("[4] Søk pa kurs");
        Console.WriteLine("[5] Søk pa bok");
        Console.WriteLine("[6] Lån bok");
        Console.WriteLine("[7] Returner bok");
        Console.WriteLine("[8] Registrer bok");
        Console.WriteLine("[0] Avslutt");
    }

    private static void InitialiserData()
    {
        Studenter.Add(new Student(1001, "Ola N", "ola@uia.no", ["IS-110"]));
        Studenter.Add(new Student(1002, "Kari H", "kari@uia.no", ["IS-110"]));
        Studenter.Add(new Utvekslingsstudent(
            1100,
            "Rianne L",
            "rianne@uia.no",
            ["EN-168"],
            "Universite d'Orleans",
            "Frankrike",
            "2024 - 2025"));

        Ansatte.Add(new Ansatt(501, "Alf T", "alf@uia.no", "Professor", "Engelsk"));
        Ansatte.Add(new Ansatt(502, "Espen L", "espen@uia.no", "Professor", "Informasjonssytemer"));

        KursListe.Add(new Kurs("EN-168", "Engelsk", 10, 100));
        KursListe.Add(new Kurs("IS-110", "Programmering", 10, 100));

        Bibliotek.RegistrerBok(new Bok(1001, "1984", "George Orwell", 1949, 3));
        Bibliotek.RegistrerBok(new Bok(1002, "Ulysses", "James Joyce", 1920, 2));
        Bibliotek.RegistrerBok(new Bok(1003, "The C Programming Language", "Brian Kernighan & Dennis Ritchie", 1979, 1));
    }

    private static void OpprettKurs()
    {
        Console.WriteLine("Kurskode: ");
        string kode = Console.ReadLine() ?? string.Empty;
        Console.WriteLine("Kursnavn: ");
        string navn = Console.ReadLine() ?? string.Empty;

        int studiepoeng = LesInt("Studiepoeng: ");
        int maksPlasser = LesInt("Maks antall plasser: ");

        if (KursListe.Any(k => k.KursKode.Equals(kode, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("Kurs med denne koden finnes allerede.");
            return;
        }

        KursListe.Add(new Kurs(kode, navn, studiepoeng, maksPlasser));
        Console.WriteLine("Kurs opprettet.");
    }

    private static void HandterKursPamelding()
    {
        Console.WriteLine("[1] Meld ny student til kurs");
        Console.WriteLine("[2] Meld student av kurs");
        string? valg = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(valg) || valg == "1")
        {
            MeldStudentTilKurs();
            return;
        }

        if (valg == "2")
        {
            MeldStudentAvKurs();
            return;
        }

        Console.WriteLine("Ugyldig valg.");
    }

    private static void MeldStudentTilKurs(int? forhondsutfyltStudentId = null)
    {
        Console.WriteLine("Registrer ny student for kursopptak:");
        int studentId = forhondsutfyltStudentId ?? LesInt("StudentID: ");

        Student? eksisterendeStudent = Studenter.FirstOrDefault(s => s.StudentId == studentId);

        Console.WriteLine("Navn: ");
        string navnInput = Console.ReadLine() ?? string.Empty;
        Console.WriteLine("Email: ");
        string emailInput = Console.ReadLine() ?? string.Empty;
        List<string> pameldteKursKoder = null;

        Student student;
        if (eksisterendeStudent is null)
        {
            student = new Student(studentId, navnInput, emailInput, pameldteKursKoder);
        }
        else
        {
            student = eksisterendeStudent;
            if (!string.IsNullOrWhiteSpace(navnInput))
            {
                student.Navn = navnInput;
            }

            if (!string.IsNullOrWhiteSpace(emailInput))
            {
                student.Email = emailInput;
            }

            Console.WriteLine($"Bruker finnes fra før: {student.Navn} ({student.StudentId})");
        }

        Console.WriteLine("Kurskode: ");
        string kurskode = (Console.ReadLine() ?? string.Empty).Trim();

        Kurs? kurs = KursListe.FirstOrDefault(k =>
            k.KursKode.Equals(kurskode, StringComparison.OrdinalIgnoreCase));

        if (kurs is null)
        {
            Console.WriteLine("Fant ikke kurs.");
            return;
        }

        if (!kurs.HarLedigPlass())
        {
            Console.WriteLine("Kurset er fullt.");
            return;
        }

        if (!kurs.MeldPaStudent(student))
        {
            Console.WriteLine("Studenten er allerede pameldt eller kurset er fullt.");
            return;
        }

        if (eksisterendeStudent is null)
        {
            Studenter.Add(student);
        }

        Console.WriteLine($"Student {student.Navn} er pameldt {kurs.KursKode}.");
        Console.WriteLine($"Kurset har na {kurs.Deltakere.Count}/{kurs.AntallPlasser} deltagere.");
        Console.WriteLine("Registrerte deltagere i kurset:");
        foreach (Student deltaker in kurs.Deltakere)
        {
            Console.WriteLine($"- {deltaker.Navn} ({deltaker.StudentId})");
        }
    }

    private static void MeldStudentAvKurs()
    {
        int studentId = LesInt("StudentID: ");
        Console.WriteLine("Kurskode: ");
        string kurskode = Console.ReadLine() ?? string.Empty;

        Student? student = Studenter.FirstOrDefault(s => s.StudentId == studentId);
        Kurs? kurs = KursListe.FirstOrDefault(k =>
            k.KursKode.Equals(kurskode, StringComparison.OrdinalIgnoreCase));

        if (student is null || kurs is null)
        {
            Console.WriteLine("Fant ikke student eller kurs.");
            return;
        }

        if (!kurs.MeldAvStudent(student))
        {
            Console.WriteLine("Studenten var ikke pameldt kurset.");
            return;
        }

        Console.WriteLine("Studenten er meldt av kurset.");
    }

    private static void PrintKursOgDeltagere()
    {
        if (!KursListe.Any())
        {
            Console.WriteLine("Ingen kurs registrert.");
            return;
        }

        foreach (Kurs kurs in KursListe)
        {
            List<Student> deltagere = HentDeltagereForKurs(kurs);

            Console.WriteLine($"{kurs.KursKode} - {kurs.KursNavn} ({kurs.StudiePoeng} sp)");
            Console.WriteLine($"Plasser: {deltagere.Count}/{kurs.AntallPlasser}");

            if (!deltagere.Any())
            {
                Console.WriteLine("Ingen deltagere.");
            }
            else
            {
                foreach (Student deltaker in deltagere)
                {
                    Console.WriteLine($"- {deltaker.Navn} ({deltaker.StudentId})");
                }
            }

            Console.WriteLine();
        }
    }

    private static List<Student> HentDeltagereForKurs(Kurs kurs)
    {
        IEnumerable<Student> fraStudentPamelding = Studenter.Where(s =>
            s.PameldteKursKoder.Any(kode => kode.Equals(kurs.KursKode, StringComparison.OrdinalIgnoreCase)));

        IEnumerable<Student> fraKursliste = kurs.Deltakere;

        return fraStudentPamelding
            .Concat(fraKursliste)
            .GroupBy(s => s.StudentId)
            .Select(g => g.First())
            .ToList();
    }

    private static void SokPaKurs()
    {
        Console.WriteLine("Skriv kode eller navn: ");
        string sok = Console.ReadLine() ?? string.Empty;

        List<Kurs> treff = KursListe.Where(k =>
                k.KursKode.Contains(sok, StringComparison.OrdinalIgnoreCase) ||
                k.KursNavn.Contains(sok, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!treff.Any())
        {
            Console.WriteLine("Ingen kurs funnet.");
            return;
        }

        foreach (Kurs kurs in treff)
        {
            Console.WriteLine($"{kurs.KursKode} - {kurs.KursNavn} ({kurs.StudiePoeng} sp)");
        }
    }

    private static void RegistrerBok()
    {
        int id = LesInt("Bok-ID: ");
        Console.WriteLine("Tittel: ");
        string tittel = Console.ReadLine() ?? string.Empty;
        Console.WriteLine("Forfatter: ");
        string forfatter = Console.ReadLine() ?? string.Empty;
        int ar = LesInt("Utgivelsesar: ");
        int antall = LesInt("Antall eksemplarer: ");

        Bibliotek.RegistrerBok(new Bok(id, tittel, forfatter, ar, antall));
        Console.WriteLine("Bok registrert.");
    }

    private static void SokPaBok()
    {
        Console.WriteLine("Sok etter tittel, forfatter eller id: ");
        string sok = Console.ReadLine() ?? string.Empty;

        Bok? bok = Bibliotek.SokBok(sok);
        if (bok is null)
        {
            Console.WriteLine("Fant ingen bok.");
            return;
        }

        Console.WriteLine($"Bok {bok.ID}: {bok.Tittel} av {bok.Forfatter} ({bok.UtgivelsesAr})");
        Console.WriteLine($"Tilgjengelig: {bok.TilgjengeligeEksemplarer}/{bok.AntallEksemplarer}");
    }

    private static void LanBok()
    {
        Bruker? bruker = FinnBrukerFraInput();
        if (bruker is null)
        {
            Console.WriteLine("Fant ikke bruker.");
            return;
        }

        int bokId = LesInt("Bok-ID som skal lanes: ");
        Bibliotek.LanUtBok(bokId, bruker, out string melding);
        Console.WriteLine(melding);
        VisLanOversikt();
    }

    private static void ReturnerBok()
    {
        Bruker? bruker = FinnBrukerFraInput();
        if (bruker is null)
        {
            Console.WriteLine("Fant ikke bruker.");
            return;
        }

        int bokId = LesInt("Bok-ID som skal returneres: ");
        Bibliotek.ReturnerBok(bokId, bruker, out string melding);
        Console.WriteLine(melding);
        VisLanOversikt();
    }

    private static void VisLanOversikt()
    {
        List<Lan> aktiveLan = Bibliotek.HentAktiveLan().ToList();

        Console.WriteLine("Aktive lan:");
        if (!aktiveLan.Any())
        {
            Console.WriteLine("Ingen aktive lan.");
        }
        else
        {
            foreach (Lan lan in aktiveLan)
            {
                Console.WriteLine($"- {lan.BokTittel} -> {lan.Laner.Navn} ({lan.Laner.HentID()}) [{lan.Lanedato:g}]");
            }
        }

        Console.WriteLine();
        Console.WriteLine("Historikk:");
        if (!Bibliotek.LaneHistorikk.Any())
        {
            Console.WriteLine("Ingen historikk enda.");
            return;
        }

        foreach (Lan lan in Bibliotek.LaneHistorikk)
        {
            string status = lan.ErAktivt ? "Aktivt" : $"Levert {lan.Innleveringsdato:g}";
            Console.WriteLine($"- {lan.BokTittel} / {lan.Laner.Navn} / {status}");
        }
    }

    private static Bruker? FinnBrukerFraInput()
    {
        Console.WriteLine("Brukertype: [1] Student [2] Ansatt");
        string? type = Console.ReadLine();

        if (type == "1")
        {
            int studentId = LesInt("StudentID: ");
            return Studenter.FirstOrDefault(s => s.StudentId == studentId);
        }

        if (type == "2")
        {
            int ansattId = LesInt("AnsattID: ");
            return Ansatte.FirstOrDefault(a => a.AnsattId == ansattId);
        }

        return null;
    }

    private static int LesInt(string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int tall))
            {
                return tall;
            }

            Console.WriteLine("Ugyldig tall, prøv igjen.");
        }
    }
}
