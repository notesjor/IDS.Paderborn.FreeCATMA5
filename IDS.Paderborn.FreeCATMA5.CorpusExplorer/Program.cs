using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using CorpusExplorer.Sdk.Ecosystem;

namespace IDS.Paderborn.FreeCATMA5
{
  class Program
  {
    static void Main(string[] args)
    {
      CorpusExplorerEcosystem.InitializeMinimal();

      foreach (var arg in args)
      {
        var files = Directory.GetFiles(arg, "*_v2_valid.json", SearchOption.TopDirectoryOnly);
        var subCorpus = arg.Replace(Path.GetDirectoryName(arg), "").Replace("\\", "").Replace("/", "");
        var meta = File.ReadAllLines(Path.Combine(arg, "Metadata.tsv"), Encoding.UTF8).Select(x=>x.Split("\t", StringSplitOptions.None)).ToArray();

        foreach (var file in files)
        {
          try
          {
            var id = Path.GetFileName(file).Replace("_v2_valid.json", "");
            var entry = (from x in meta where x[0] == subCorpus && x[1] == id select x).Single();
            
            var output = Path.Combine(arg, $"{subCorpus}-{id}.cec6");
            if (File.Exists(output))
              continue;

            var import = new CorpusExplorer.Sdk.Extern.Json.SimpleStandoff.SimpleJsonStandoffImporter();
            var corpus = import.Execute(new[] {file}).First();

            var dsel = corpus.DocumentGuids.Single();
            corpus.SetDocumentMetadata(dsel, new Dictionary<string, object>
            {
              {"SubCorpus", entry[0]},
              {"SubCorpusID", entry[1]},
              {"Filename", entry[2]},
              {"Textsorte", entry[3]},
              {"Autor", entry[4]},
              {"Herausgeber", entry[5]},
              {"Titel", entry[6]},
              {"Untertitel", entry[7]},
              {"Ort", entry[8]},
              {"Verlag", entry[9]},
              {"Jahr", entry[10]},
              {"Seiten", entry[11]},
              {"Akteur", entry[12]},
              {"Entstehung", entry[13]},
              {"Entstehung (genau)", entry[14]},
              {"Seitenzahlen", entry[15]}
            });

            corpus.Save(output, false);
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
            Console.WriteLine("");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine("-----");
          }
        }
      }
    }
  }
}
