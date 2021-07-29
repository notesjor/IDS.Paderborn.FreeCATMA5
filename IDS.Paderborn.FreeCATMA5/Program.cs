using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using HtmlAgilityPack;
using IDS.Paderborn.FreeCATMA5.Process;
using Newtonsoft.Json;

namespace IDS.Paderborn.FreeCATMA5
{
  class Program
  {
    static void Main(string[] args)
    {
      var dirs = Directory.GetDirectories(args[0]);
      foreach (var dir in dirs)
      {
        V1.Process(dir);
        V2.Process(dir);
      }

      Console.WriteLine("FERTIG!");
      Console.ReadLine();
    }

    
  }
}
