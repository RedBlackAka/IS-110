using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

Console.WriteLine("Velkommen til UiA's universitetssystem (UUS). Du kan velge et av følgende alternativ:");

Console.WriteLine("Brukerprofil: 1");
Console.WriteLine("Oppmelding til kurs: 2");
Console.WriteLine("Bibliotek: 3");

string navn = Console.ReadLine();
int id = int.Parse(Console.ReadLine());
