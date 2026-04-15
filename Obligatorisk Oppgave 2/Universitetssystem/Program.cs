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
    private static readonly UniversitetService Universitet = new(KursListe);

    private static int _nesteStudentId = 2000;
    private static int _nesteAnsattId = 700;

    private static void Main()
    {
        InitialiserData();

        bool avslutt = false;
        while (!avslutt)
        {
            try
            {
                Console.WriteLine("Velg brukerstatus:");
                Console.WriteLine("[1] Eksisterende bruker");
                Console.WriteLine("[2] Ny bruker (registrer)");
                Console.WriteLine("[0] Avslutt");
                string? valg = Console.ReadLine();

                switch (valg)
                {
                    case "1":
                    {
                        Bruker? innlogget = LoggInn();
                        if (innlogget is not null)
                        {
                            KjorRolleMeny(innlogget);
                        }

                        break;
                    }
                    case "2":
                    {
                        Bruker? registrert = RegistrerNyBruker();
                        if (registrert is not null)
                        {
                            Console.WriteLine("Registrering fullfort. Du er na logget inn.");
                            KjorRolleMeny(registrert);
                        }

                        break;
                    }
                    case "0":
                        avslutt = true;
                        Console.WriteLine("Avslutter universitetssystemet.");
                        break;
                    default:
                        Console.WriteLine("Ugyldig valg, prøv igjen.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Det oppstod en feil: {ex.Message}");
            }

            Console.WriteLine();
        }
    }

    private static void InitialiserData()
    {
        Studenter.Add(new Student(1001, "Ola N", "ola@uia.no", "ola123", new List<string>{ "IS-110" }));
        Studenter.Add(new Student(1002, "Kari H", "kari@uia.no", "kari123", new List<string>{ "IS-110" }));
        Studenter.Add(new Utvekslingsstudent(
            1100,
            "Rianne L",
            "rianne@uia.no",
            "rianne123",
            new List<string>{ "EN-168" },
            "Universite d'Orleans",
            "Frankrike",
            "2024 - 2025"));

        Ansatte.Add(new Ansatt(501, "Alf T", "alf@uia.no", "alf123", BrukerRolle.Professor, "Engelsk"));
        Ansatte.Add(new Ansatt(502, "Espen L", "espen@uia.no", "espen123", BrukerRolle.Professor, "Informasjonssystemer"));
        Ansatte.Add(new Ansatt(601, "Bente B", "bente@uia.no", "bente123", BrukerRolle.Bibliotekar, "Bibliotek"));

        Universitet.OpprettKurs("EN-168", "Engelsk", 10, 100, Ansatte.First(a => a.AnsattId == 501), out _);
        Universitet.OpprettKurs("IS-110", "Programmering", 10, 100, Ansatte.First(a => a.AnsattId == 502), out _);

        Universitet.MeldStudentTilKurs(Studenter[0], "IS-110", out _);
        Universitet.MeldStudentTilKurs(Studenter[1], "IS-110", out _);
        Universitet.MeldStudentTilKurs(Studenter[2], "EN-168", out _);

        Bibliotek.RegistrerBok(new Bok(1001, "1984", "George Orwell", 1949, 3));
        Bibliotek.RegistrerBok(new Bok(1002, "Ulysses", "James Joyce", 1920, 2));
        Bibliotek.RegistrerBok(new Bok(1003, "The C Programming Language", "Brian Kernighan & Dennis Ritchie", 1979, 1));
    }

    private static Bruker? LoggInn()
    {
        string navn = LesPaakrevdTekst("Brukernavn: ");
        string passord = LesPaakrevdTekst("Passord: ");

        Bruker? bruker = HentAlleBrukere().FirstOrDefault(b =>
            b.Navn.Equals(navn, StringComparison.OrdinalIgnoreCase) &&
            b.Passord == passord);

        if (bruker is null)
        {
            Console.WriteLine("Feil brukernavn eller passord.");
            return null;
        }

        Console.WriteLine($"Innlogging vellykket. Velkommen {bruker.Navn} ({bruker.Rolle}).");
        return bruker;
    }

    private static Bruker? RegistrerNyBruker()
    {
        Console.WriteLine("Velg rolle: [1] Student [2] Bibliotekar [3] Professor");
        string? rollevalg = Console.ReadLine();

        BrukerRolle rolle;
        switch (rollevalg)
        {
            case "1":
                rolle = BrukerRolle.Student;
                break;
            case "2":
                rolle = BrukerRolle.Bibliotekar;
                break;
            case "3":
                rolle = BrukerRolle.Professor;
                break;
            default:
                Console.WriteLine("Ugyldig rollevalg.");
                return null;
        }

        string navn = LesPaakrevdTekst("Navn: ");
        string email = LesPaakrevdTekst("E-post: ");
        string passord = LesPaakrevdTekst("Passord: ");

        if (HentAlleBrukere().Any(b => b.Navn.Equals(navn, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("Brukernavnet er allerede i bruk.");
            return null;
        }

        if (rolle == BrukerRolle.Student)
        {
            Student student = new(_nesteStudentId++, navn, email, passord, new List<string>());
            Studenter.Add(student);
            return student;
        }

        Ansatt ansatt = new(_nesteAnsattId++, navn, email, passord, rolle, "Ikke satt");
        Ansatte.Add(ansatt);
        return ansatt;
    }

    private static void KjorRolleMeny(Bruker bruker)
    {
        bool loggUt = false;
        while (!loggUt)
        {
            try
            {
                switch (bruker.Rolle)
                {
                    case BrukerRolle.Student:
                        loggUt = KjorStudentMeny((Student)bruker);
                        break;
                    case BrukerRolle.Bibliotekar:
                        loggUt = KjorBibliotekarMeny((Ansatt)bruker);
                        break;
                    case BrukerRolle.Professor:
                        loggUt = KjorProfessorMeny((Ansatt)bruker);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Operasjonen feilet: {ex.Message}");
            }

            Console.WriteLine();
        }
    }

    private static bool KjorStudentMeny(Student student)
    {
        Console.WriteLine("Studentmeny:");
        Console.WriteLine("[1] Meld deg pa kurs");
        Console.WriteLine("[2] Meld deg av kurs");
        Console.WriteLine("[3] Se mine kurs");
        Console.WriteLine("[4] Se mine karakterer");
        Console.WriteLine("[5] Sok pa bok");
        Console.WriteLine("[6] Lan bok");
        Console.WriteLine("[7] Returner bok");
        Console.WriteLine("[0] Logg ut");

        string? valg = Console.ReadLine();
        switch (valg)
        {
            case "1":
                MeldInnStudent(student);
                break;
            case "2":
                MeldAvStudent(student);
                break;
            case "3":
                VisStudentKurs(student);
                break;
            case "4":
                VisStudentKarakterer(student);
                break;
            case "5":
                SokPaBok();
                break;
            case "6":
                LanBok(student);
                break;
            case "7":
                ReturnerBok(student);
                break;
            case "0":
                return true;
            default:
                Console.WriteLine("Ugyldig valg.");
                break;
        }

        return false;
    }

    private static bool KjorBibliotekarMeny(Ansatt bibliotekar)
    {
        Console.WriteLine($"Bibliotekarmeny ({bibliotekar.Navn}):");
        Console.WriteLine("[1] Registrer bok");
        Console.WriteLine("[2] Se aktive lan");
        Console.WriteLine("[3] Se lanehistorikk");
        Console.WriteLine("[4] Sok pa bok");
        Console.WriteLine("[0] Logg ut");

        string? valg = Console.ReadLine();
        switch (valg)
        {
            case "1":
                RegistrerBok();
                break;
            case "2":
                VisAktiveLan();
                break;
            case "3":
                VisLaneHistorikk();
                break;
            case "4":
                SokPaBok();
                break;
            case "0":
                return true;
            default:
                Console.WriteLine("Ugyldig valg.");
                break;
        }

        return false;
    }

    private static bool KjorProfessorMeny(Ansatt professor)
    {
        Console.WriteLine($"Professormeny ({professor.Navn}):");
        Console.WriteLine("[1] Opprett kurs");
        Console.WriteLine("[2] Sok pa kurs");
        Console.WriteLine("[3] Sok pa bok");
        Console.WriteLine("[4] Lan bok");
        Console.WriteLine("[5] Returner bok");
        Console.WriteLine("[6] Sett karakter");
        Console.WriteLine("[7] Registrer pensum");
        Console.WriteLine("[8] Se egne kurs");
        Console.WriteLine("[0] Logg ut");

        string? valg = Console.ReadLine();
        switch (valg)
        {
            case "1":
                OpprettKurs(professor);
                break;
            case "2":
                SokPaKurs();
                break;
            case "3":
                SokPaBok();
                break;
            case "4":
                LanBok(professor);
                break;
            case "5":
                ReturnerBok(professor);
                break;
            case "6":
                SettKarakter(professor);
                break;
            case "7":
                RegistrerPensum(professor);
                break;
            case "8":
                VisProfessorKurs(professor);
                break;
            case "0":
                return true;
            default:
                Console.WriteLine("Ugyldig valg.");
                break;
        }

        return false;
    }

    private static void OpprettKurs(Ansatt professor)
    {
        string kode = LesPaakrevdTekst("Kurskode: ");
        string navn = LesPaakrevdTekst("Kursnavn: ");
        int studiepoeng = LesPositivInt("Studiepoeng: ");
        int maksPlasser = LesPositivInt("Maks antall plasser: ");

        Universitet.OpprettKurs(kode, navn, studiepoeng, maksPlasser, professor, out string melding);
        Console.WriteLine(melding);
    }

    private static void SokPaKurs()
    {
        string sok = LesPaakrevdTekst("Skriv kode eller navn: ");
        List<Kurs> treff = Universitet.SokKurs(sok);

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

    private static void MeldInnStudent(Student student)
    {
        string kurskode = LesPaakrevdTekst("Kurskode: ");
        Universitet.MeldStudentTilKurs(student, kurskode, out string melding);
        Console.WriteLine(melding);
    }

    private static void MeldAvStudent(Student student)
    {
        string kurskode = LesPaakrevdTekst("Kurskode: ");
        Universitet.MeldStudentAvKurs(student, kurskode, out string melding);
        Console.WriteLine(melding);
    }

    private static void VisStudentKurs(Student student)
    {
        List<Kurs> mineKurs = KursListe.Where(k =>
                k.Deltakere.Any(s => s.StudentId == student.StudentId))
            .ToList();

        if (!mineKurs.Any())
        {
            Console.WriteLine("Du er ikke meldt pa noen kurs.");
            return;
        }

        foreach (Kurs kurs in mineKurs)
        {
            Console.WriteLine($"- {kurs.KursKode} {kurs.KursNavn}");
        }
    }

    private static void VisStudentKarakterer(Student student)
    {
        List<Kurs> kursMedKarakter = KursListe.Where(k =>
                k.Karakterer.ContainsKey(student.StudentId))
            .ToList();

        if (!kursMedKarakter.Any())
        {
            Console.WriteLine("Ingen karakterer registrert enda.");
            return;
        }

        foreach (Kurs kurs in kursMedKarakter)
        {
            Console.WriteLine($"- {kurs.KursKode}: {kurs.HentKarakter(student.StudentId)}");
        }
    }

    private static void SettKarakter(Ansatt professor)
    {
        string kurskode = LesPaakrevdTekst("Kurskode: ");
        Kurs? kurs = KursListe.FirstOrDefault(k =>
            k.KursKode.Equals(kurskode, StringComparison.OrdinalIgnoreCase));

        if (kurs is null)
        {
            Console.WriteLine("Fant ikke kurs.");
            return;
        }

        if (kurs.UnderviserAnsattId != professor.AnsattId)
        {
            Console.WriteLine("Du kan bare sette karakter i kurs du underviser.");
            return;
        }

        int studentId = LesPositivInt("StudentID: ");
        string karakter = LesPaakrevdTekst("Karakter (A-F): ");

        if (!kurs.SettKarakter(studentId, karakter))
        {
            Console.WriteLine("Kunne ikke sette karakter. Kontroller at studenten er meldt pa kurset.");
            return;
        }

        Console.WriteLine("Karakter registrert.");
    }

    private static void RegistrerPensum(Ansatt professor)
    {
        string kurskode = LesPaakrevdTekst("Kurskode: ");
        Kurs? kurs = KursListe.FirstOrDefault(k =>
            k.KursKode.Equals(kurskode, StringComparison.OrdinalIgnoreCase));

        if (kurs is null)
        {
            Console.WriteLine("Fant ikke kurs.");
            return;
        }

        if (kurs.UnderviserAnsattId != professor.AnsattId)
        {
            Console.WriteLine("Du kan bare registrere pensum i kurs du underviser.");
            return;
        }

        string pensum = LesPaakrevdTekst("Pensum: ");
        if (!kurs.RegistrerPensum(pensum))
        {
            Console.WriteLine("Pensum finnes allerede eller var ugyldig.");
            return;
        }

        Console.WriteLine("Pensum registrert.");
    }

    private static void VisProfessorKurs(Ansatt professor)
    {
        List<Kurs> egneKurs = KursListe.Where(k => k.UnderviserAnsattId == professor.AnsattId).ToList();
        if (!egneKurs.Any())
        {
            Console.WriteLine("Du underviser ingen kurs enda.");
            return;
        }

        foreach (Kurs kurs in egneKurs)
        {
            Console.WriteLine($"{kurs.KursKode} - {kurs.KursNavn}");

            if (kurs.Pensum.Any())
            {
                Console.WriteLine("Pensum:");
                foreach (string pensum in kurs.Pensum)
                {
                    Console.WriteLine($"- {pensum}");
                }
            }
            else
            {
                Console.WriteLine("Ingen pensum registrert.");
            }
        }
    }

    private static void RegistrerBok()
    {
        int id = LesPositivInt("Bok-ID: ");
        string tittel = LesPaakrevdTekst("Tittel: ");
        string forfatter = LesPaakrevdTekst("Forfatter: ");
        int ar = LesPositivInt("Utgivelsesar: ");
        int antall = LesPositivInt("Antall eksemplarer: ");

        Bibliotek.RegistrerBok(new Bok(id, tittel, forfatter, ar, antall));
        Console.WriteLine("Bok registrert.");
    }

    private static void SokPaBok()
    {
        string sok = LesPaakrevdTekst("Sok etter tittel, forfatter eller id: ");

        Bok? bok = Bibliotek.SokBok(sok);
        if (bok is null)
        {
            Console.WriteLine("Fant ingen bok.");
            return;
        }

        Console.WriteLine($"Bok {bok.ID}: {bok.Tittel} av {bok.Forfatter} ({bok.UtgivelsesAr})");
        Console.WriteLine($"Tilgjengelig: {bok.TilgjengeligeEksemplarer}/{bok.AntallEksemplarer}");
    }

    private static void LanBok(Bruker bruker)
    {
        int bokId = LesPositivInt("Bok-ID som skal lanes: ");
        Bibliotek.LanUtBok(bokId, bruker, out string melding);
        Console.WriteLine(melding);
    }

    private static void ReturnerBok(Bruker bruker)
    {
        int bokId = LesPositivInt("Bok-ID som skal returneres: ");
        Bibliotek.ReturnerBok(bokId, bruker, out string melding);
        Console.WriteLine(melding);
    }

    private static void VisAktiveLan()
    {
        List<Lan> aktiveLan = Bibliotek.HentAktiveLan().ToList();
        if (!aktiveLan.Any())
        {
            Console.WriteLine("Ingen aktive lan.");
            return;
        }

        foreach (Lan lan in aktiveLan)
        {
            Console.WriteLine($"- {lan.BokTittel} -> {lan.Laner.Navn} ({lan.Laner.HentID()}) [{lan.Lanedato:g}]");
        }
    }

    private static void VisLaneHistorikk()
    {
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

    private static IEnumerable<Bruker> HentAlleBrukere()
    {
        return Studenter.Cast<Bruker>().Concat(Ansatte);
    }

    private static int LesPositivInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int tall) && tall > 0)
            {
                return tall;
            }

            Console.WriteLine("Ugyldig tall. Skriv et positivt heltall.");
        }
    }

    private static string LesPaakrevdTekst(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }

            Console.WriteLine("Feltet kan ikke vaere tomt.");
        }
    }
}

internal class UniversitetService
{
    private readonly List<Kurs> _kursListe;

    public UniversitetService(List<Kurs> kursListe)
    {
        _kursListe = kursListe;
    }

    public bool OpprettKurs(
        string kurskode,
        string kursnavn,
        int studiepoeng,
        int maksPlasser,
        Ansatt professor,
        out string melding)
    {
        if (professor.Rolle != BrukerRolle.Professor)
        {
            melding = "Bare professorer kan opprette kurs.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(kurskode) || string.IsNullOrWhiteSpace(kursnavn))
        {
            melding = "Kurskode og kursnavn må fylles ut.";
            return false;
        }

        if (studiepoeng <= 0 || maksPlasser <= 0)
        {
            melding = "Studiepoeng og maks plasser må være større enn 0.";
            return false;
        }

        bool finnesAllerede = _kursListe.Any(k =>
            k.KursKode.Equals(kurskode, StringComparison.OrdinalIgnoreCase) ||
            k.KursNavn.Equals(kursnavn, StringComparison.OrdinalIgnoreCase));

        if (finnesAllerede)
        {
            melding = "Kurs med samme kode eller navn finnes allerede.";
            return false;
        }

        _kursListe.Add(new Kurs(kurskode.Trim(), kursnavn.Trim(), studiepoeng, maksPlasser, professor.AnsattId));
        melding = "Kurs opprettet.";
        return true;
    }

    public bool MeldStudentTilKurs(Student student, string kurskode, out string melding)
    {
        if (string.IsNullOrWhiteSpace(kurskode))
        {
            melding = "Kurskode må fylles ut.";
            return false;
        }

        Kurs? kurs = _kursListe.FirstOrDefault(k =>
            k.KursKode.Equals(kurskode.Trim(), StringComparison.OrdinalIgnoreCase));

        if (kurs is null)
        {
            melding = "Fant ikke kurs.";
            return false;
        }

        if (!kurs.HarLedigPlass())
        {
            melding = "Kurset er fullt.";
            return false;
        }

        if (!kurs.MeldPaStudent(student))
        {
            melding = "Studenten er allerede påmeldt dette kurset.";
            return false;
        }

        melding = "Student meldt på kurs.";
        return true;
    }

    public bool MeldStudentAvKurs(Student student, string kurskode, out string melding)
    {
        Kurs? kurs = _kursListe.FirstOrDefault(k =>
            k.KursKode.Equals(kurskode.Trim(), StringComparison.OrdinalIgnoreCase));

        if (kurs is null)
        {
            melding = "Fant ikke kurs.";
            return false;
        }

        if (!kurs.MeldAvStudent(student))
        {
            melding = "Studenten var ikke påmeldt kurset.";
            return false;
        }

        melding = "Student meldt av kurs.";
        return true;
    }

    public List<Kurs> SokKurs(string sok)
    {
        if (string.IsNullOrWhiteSpace(sok))
        {
            return new List<Kurs>();
        }

        return _kursListe.Where(k =>
                k.KursKode.Contains(sok, StringComparison.OrdinalIgnoreCase) ||
                k.KursNavn.Contains(sok, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
