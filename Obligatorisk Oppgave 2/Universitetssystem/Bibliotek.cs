using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Universitetssystem
{
    internal class Bok
    {
        public int ID { get; set; }
        public string Tittel { get; set; }
        public string Forfatter { get; set; }
        public int UtgivelsesAr { get; set; }
        public int AntallEksemplarer { get; set; }
        public int UtlanteEksemplarer { get; private set; }

        public Bok(int id, string tittel, string forfatter, int utgivelsesAr, int antallEksemplarer)
        {
            ID = id;
            Tittel = tittel;
            Forfatter = forfatter;
            UtgivelsesAr = utgivelsesAr;
            AntallEksemplarer = antallEksemplarer;
        }

        public int TilgjengeligeEksemplarer => AntallEksemplarer - UtlanteEksemplarer;

        public bool LanUtEksemplar()
        {
            if (TilgjengeligeEksemplarer <= 0)
            {
                return false;
            }

            UtlanteEksemplarer++;
            return true;
        }

        public bool ReturnerEksemplar()
        {
            if (UtlanteEksemplarer <= 0)
            {
                return false;
            }

            UtlanteEksemplarer--;
            return true;
        }
    }

    internal class Lan
    {
        public int BokId { get; set; }
        public string BokTittel { get; set; }
        public Bruker Laner { get; set; }
        public DateTime Lanedato { get; set; }
        public DateTime? Innleveringsdato { get; set; }

        public bool ErAktivt => Innleveringsdato is null;

        public Lan(int bokId, string bokTittel, Bruker laner)
        {
            BokId = bokId;
            BokTittel = bokTittel;
            Laner = laner;
            Lanedato = DateTime.Now;
        }
    }

    internal class BibliotekService
    {
        public List<Bok> Boker { get; } = new();
        public List<Lan> LaneHistorikk { get; } = new();

        public void RegistrerBok(Bok bok)
        {
            Bok? eksisterende = Boker.FirstOrDefault(b => b.ID == bok.ID);
            if (eksisterende is not null)
            {
                eksisterende.AntallEksemplarer += bok.AntallEksemplarer;
                return;
            }

            Boker.Add(bok);
        }

        public Bok? SokBok(string sok)
        {
            return Boker.FirstOrDefault(b =>
                b.Tittel.Contains(sok, StringComparison.OrdinalIgnoreCase) ||
                b.Forfatter.Contains(sok, StringComparison.OrdinalIgnoreCase) ||
                b.ID.ToString() == sok);
        }

        public bool LanUtBok(int bokId, Bruker bruker, out string melding)
        {
            Bok? bok = Boker.FirstOrDefault(b => b.ID == bokId);
            if (bok is null)
            {
                melding = "Fant ingen bok med den ID-en.";
                return false;
            }

            if (!bok.LanUtEksemplar())
            {
                melding = "Ingen eksemplarer tilgjengelig.";
                return false;
            }

            LaneHistorikk.Add(new Lan(bok.ID, bok.Tittel, bruker));
            melding = "Boken ble lånt ut.";
            return true;
        }

        public bool ReturnerBok(int bokId, Bruker bruker, out string melding)
        {
            Bok? bok = Boker.FirstOrDefault(b => b.ID == bokId);
            if (bok is null)
            {
                melding = "Fant ingen bok.";
                return false;
            }

            Lan? aktivtLan = LaneHistorikk.LastOrDefault(l =>
                l.BokId == bokId &&
                l.ErAktivt &&
                SammeBruker(l.Laner, bruker));

            if (aktivtLan is null)
            {
                melding = "Fant ikke lån for denne brukeren og boken.";
                return false;
            }

            if (!bok.ReturnerEksemplar())
            {
                melding = "Kunne ikke registrere innlevering.";
                return false;
            }

            aktivtLan.Innleveringsdato = DateTime.Now;
            melding = "Boken er returnert.";
            return true;
        }

        public IEnumerable<Lan> HentAktiveLan()
        {
            return LaneHistorikk.Where(l => l.ErAktivt);
        }

        private static bool SammeBruker(Bruker a, Bruker b)
        {
            if (a is Student sa && b is Student sb)
            {
                return sa.StudentId == sb.StudentId;
            }

            if (a is Ansatt aa && b is Ansatt ab)
            {
                return aa.AnsattId == ab.AnsattId;
            }

            return false;
        }
    }
}