using System;
using System.IO;
using System.Linq;
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

        foreach (var file in files)
        {
          try
          {
            var output = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".cec6");
            if (File.Exists(output))
              continue;

            var import = new CorpusExplorer.Sdk.Extern.Json.SimpleStandoff.SimpleJsonStandoffImporter();
            var corpus = import.Execute(new[] {file}).First();
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
