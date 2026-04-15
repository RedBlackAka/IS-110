using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Universitetssystem.UnitTests;

public class UniversitetssystemUnitTests
{
    [Fact]
    public void Professor_Cannot_Create_Course_With_Duplicate_Code_Or_Name()
    {
        // Arrange
        var mockKursListe = new Mock<List<Kurs>>();
        var kursListe = new List<Kurs>();
        var service = new UniversitetService(kursListe);
        var professor = new Ansatt(1, "Prof", "p@u.no", "pw", BrukerRolle.Professor, "IT");

        // Act
        bool opprettetForste = service.OpprettKurs("IS-110", "Programmering", 10, 50, professor, out _);
        bool opprettetDuplikatKode = service.OpprettKurs("IS-110", "Avansert Program", 10, 50, professor, out string melding1);
        bool opprettetDuplikatNavn = service.OpprettKurs("IS-220", "Programmering", 10, 50, professor, out string melding2);

        // Assert
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
        // Arrange
        var kursListe = new List<Kurs>();
        var service = new UniversitetService(kursListe);
        var professor = new Ansatt(1, "Prof", "p@u.no", "pw", BrukerRolle.Professor, "IT");
        var student = new Student(1001, "Student", "s@u.no", "pw", new List<string>());

        service.OpprettKurs("IS-110", "Programmering", 10, 2, professor, out _);

        // Act
        bool forstePamelding = service.MeldStudentTilKurs(student, "IS-110", out _);
        bool andrePamelding = service.MeldStudentTilKurs(student, "IS-110", out string melding);

        // Assert
        Assert.True(forstePamelding);
        Assert.False(andrePamelding);
        Assert.Contains("allerede", melding, StringComparison.OrdinalIgnoreCase);
        Assert.Single(student.PameldteKursKoder);
        Assert.Single(kursListe[0].Deltakere);
    }

    [Fact]
    public void Student_And_Professor_Can_Return_Borrowed_Books()
    {
        // Arrange
        var bibliotek = new BibliotekService();
        var bok = new Bok(1, "Clean Code", "Robert C. Martin", 2008, 2);
        bibliotek.RegistrerBok(bok);

        var student = new Student(1001, "Student", "s@u.no", "pw", new List<string>());
        var professor = new Ansatt(1, "Prof", "p@u.no", "pw", BrukerRolle.Professor, "IT");

        // Act
        bool studentLaan = bibliotek.LanUtBok(1, student, out _);
        bool professorLaan = bibliotek.LanUtBok(1, professor, out _);
        bool studentRetur = bibliotek.ReturnerBok(1, student, out _);
        bool professorRetur = bibliotek.ReturnerBok(1, professor, out _);

        // Assert
        Assert.True(studentLaan);
        Assert.True(professorLaan);
        Assert.True(studentRetur);
        Assert.True(professorRetur);
        Assert.Empty(bibliotek.HentAktiveLan());
    }
}
