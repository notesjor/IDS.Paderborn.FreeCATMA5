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
      foreach (var arg in args)
      {
        // Falls keine Unterverzeichnisse vorhanden sind, erstelle diese
        var dirs = Directory.GetDirectories(arg);
        if (dirs.Length == 0)
          dirs = PackFilesInDirectories(arg);

        // Konvertierung von Unterverzeichnissen
        foreach (var dir in dirs)
        {
          V1.Process(dir);
          V2.Process(dir);
        }
      }

      Console.WriteLine("FERTIG!");
      Console.ReadLine();
    }

    private static string[] PackFilesInDirectories(string path)
    {
      var files = Directory.GetFiles(path);
      foreach (var file in files)
      {
        var name = Path.GetFileNameWithoutExtension(file);
        var split = name.Split(new[] {"-"}, StringSplitOptions.RemoveEmptyEntries);
        var dir = "";

        foreach (var x in split)
        {
          if (!x.StartsWith("0"))
            continue;
          dir = Path.Combine(path, x);
          if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
          break;
        }

        File.Move(file, Path.Combine(dir, Path.GetFileName(file)));
      }

      return Directory.GetDirectories(path);
    }
  }
}
