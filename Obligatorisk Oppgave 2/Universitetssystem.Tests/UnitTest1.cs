using Universitetssystem;

namespace Universitetssystem.Tests;

public class UniversitetssystemTests
{
    [Fact]
    public void Professor_Cannot_Create_Course_With_Duplicate_Code_Or_Name()
    {
        List<Kurs> kursListe = new();
        UniversitetService service = new(kursListe);
        Ansatt professor = new(1, "Prof", "p@u.no", "pw", BrukerRolle.Professor, "IT");

        bool opprettetForste = service.OpprettKurs("IS-110", "Programmering", 10, 50, professor, out _);
        bool opprettetDuplikatKode = service.OpprettKurs("IS-110", "Avansert Program", 10, 50, professor, out string melding1);
        bool opprettetDuplikatNavn = service.OpprettKurs("IS-220", "Programmering", 10, 50, professor, out string melding2);

        Assert.True(opprettetForste);
        Assert.False(opprettetDuplikatKode);
        Assert.False(opprettetDuplikatNavn);
        Assert.Contains("finnes allerede", melding1, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("finnes allerede", melding2, StringComparison.OrdinalIgnoreCase);
        Assert.Single(kursListe);
    }

    [Fact]
    public void Student_Cannot_Enroll_In_Same_Course_Twice()
    {
        List<Kurs> kursListe = new();
        UniversitetService service = new(kursListe);
        Ansatt professor = new(1, "Prof", "p@u.no", "pw", BrukerRolle.Professor, "IT");
        Student student = new(1001, "Student", "s@u.no", "pw", new List<string>());

        service.OpprettKurs("IS-110", "Programmering", 10, 2, professor, out _);
        bool forstePamelding = service.MeldStudentTilKurs(student, "IS-110", out _);
        bool andrePamelding = service.MeldStudentTilKurs(student, "IS-110", out string melding);

        Assert.True(forstePamelding);
        Assert.False(andrePamelding);
        Assert.Contains("allerede", melding, StringComparison.OrdinalIgnoreCase);
        Assert.Single(student.PameldteKursKoder);
        Assert.Single(kursListe[0].Deltakere);
    }

    [Fact]
    public void Student_And_Professor_Can_Return_Borrowed_Books()
    {
        BibliotekService bibliotek = new();
        bibliotek.RegistrerBok(new Bok(1, "Clean Code", "Robert C. Martin", 2008, 2));

        Student student = new(1001, "Student", "s@u.no", "pw", new List<string>());
        Ansatt professor = new(1, "Prof", "p@u.no", "pw", BrukerRolle.Professor, "IT");

        bool studentLaan = bibliotek.LanUtBok(1, student, out _);
        bool professorLaan = bibliotek.LanUtBok(1, professor, out _);
        bool studentRetur = bibliotek.ReturnerBok(1, student, out _);
        bool professorRetur = bibliotek.ReturnerBok(1, professor, out _);

        Assert.True(studentLaan);
        Assert.True(professorLaan);
        Assert.True(studentRetur);
        Assert.True(professorRetur);
        Assert.Empty(bibliotek.HentAktiveLan());
    }
}
