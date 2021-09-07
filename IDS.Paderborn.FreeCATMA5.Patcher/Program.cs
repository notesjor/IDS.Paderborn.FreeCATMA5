using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IDS.Paderborn.FreeCATMA5.Model.v2;
using Newtonsoft.Json;

namespace IDS.Paderborn.FreeCATMA5.Patcher
{
  class Program
  {
    static void Main(string[] args)
    {
      var res = new HashSet<string>();

      foreach (var arg in args)
      {
        var files = Directory.GetFiles(arg, "*_v2_valid.json");
        foreach (var file in files)
        {
          var doc = JsonConvert.DeserializeObject<Document>(File.ReadAllText(file, Encoding.UTF8));
          foreach (var anno in doc.Annotations)
          {
            res.Add(anno.LayerValue);
          }
        }
      }

      File.WriteAllLines("fix.txt", res, Encoding.UTF8);
    }
  }
}
