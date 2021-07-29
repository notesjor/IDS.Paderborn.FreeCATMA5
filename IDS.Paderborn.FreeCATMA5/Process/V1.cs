using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using IDS.Paderborn.FreeCATMA5.Model.v1;
using Newtonsoft.Json;

namespace IDS.Paderborn.FreeCATMA5.Process
{
  public static class V1
  {
    public static void Process(string dir)
    {
      var files = Directory.GetFiles(dir, "*.xml");

      var output = Path.Combine(dir, "output_v1");
      if (Directory.Exists(output))
        Directory.Delete(output, true);
      Directory.CreateDirectory(output);

      string hash = null;
      string plaintext = null;
      var valid = true;
      var annotations = new List<Annotation>();
      string validFile = null;
      using var hsh = SHA512.Create();

      foreach (var file in files)
      {
        ProcessCatma5Xml(file, output, out var text, out var annos);
        if (hash == null)
        {
          plaintext = text;
          annotations = annos;
          validFile = file;
          hash = Convert.ToBase64String(hsh.ComputeHash(Encoding.UTF8.GetBytes(plaintext)));
        }
        else
        {
          var hashCompare = Convert.ToBase64String(hsh.ComputeHash(Encoding.UTF8.GetBytes(plaintext)));
          if (hashCompare == hash)
          {
            annotations.AddRange(annos);
            Console.WriteLine($"{validFile} == {file}");
          }
          else
          {
            valid = false;
            Console.WriteLine($"{validFile} <> {file}");
          }
        }
      }

      File.WriteAllText(dir + $"_v1_{(valid ? "valid" : "error")}.json", JsonConvert.SerializeObject(new Document
      {
        Annotations = annotations,
        Text = plaintext
      }), Encoding.UTF8);
    }

    private static void ProcessCatma5Xml(string file, string output, out string plaintext, out List<Annotation> annotations)
    {
      var xml = new HtmlDocument();
      using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
        xml.Load(fs);

      var body = xml.DocumentNode.SelectSingleNode("//text/body");
      var text = new StringBuilder();
      annotations = new List<Annotation>();

      foreach (var n in body.ChildNodes)
        BodyLoop(xml, n, ref text, ref annotations);

      plaintext = text.ToString();
      File.WriteAllText(Path.Combine(output, Path.GetFileName(file).Replace(".xml", ".txt")), plaintext, Encoding.UTF8);
      File.WriteAllText(Path.Combine(output, Path.GetFileName(file).Replace(".xml", ".json")), JsonConvert.SerializeObject(new Document
      {
        Annotations = annotations,
        Text = plaintext
      }), Encoding.UTF8);
    }

    private static void BodyLoop(HtmlDocument xml, HtmlNode htmlNode, ref StringBuilder text, ref List<Annotation> annoValues)
    {
      switch (htmlNode.Name)
      {
        case "ab":
          foreach (var n in htmlNode.ChildNodes)
            BodyLoop(xml, n, ref text, ref annoValues);
          return;
        case "seg":
          BodyLoopSeg(xml, htmlNode, ref text, ref annoValues);
          return;
        case "#text":
          BodyLoopText(htmlNode, ref text);
          return;
      }
    }

    private static void BodyLoopSeg(HtmlDocument xml, HtmlNode htmlNode, ref StringBuilder text, ref List<Annotation> annoValues)
    {
      var @from = text.Length;
      text.AppendLine(htmlNode.InnerText);
      var @to = text.Length;

      var ana = htmlNode.GetAttributeValue("ana", "");
      if (string.IsNullOrEmpty(ana))
        return;

      var allFs = xml.DocumentNode.SelectNodes("//fs");
      var allFsDecl = xml.DocumentNode.SelectNodes("//fsdecl");

      var anas = ana.Replace("#", "").Split(' ');
      foreach (var x in anas)
      {
        var fs = allFs.FirstOrDefault(q => q.GetAttributeValue("xml:id", "") == x);
        var annotator = RemoveWhitespace(FindAnnotator(fs.ChildNodes));

        var type = fs.GetAttributeValue("type", "");
        if (type == "")
          continue;

        var valNode = allFsDecl.FirstOrDefault(q => q.GetAttributeValue("xml:id", "") == type);
        var val = valNode.ChildNodes.FirstOrDefault(x => x.Name == "fsdescr")?.InnerText;

        var valueChain = new List<string>();
        FindParentValues(valNode, ref valueChain, ref allFsDecl);

        valueChain.Add(val);

        annoValues.Add(new Annotation
        {
          Annotator = annotator,
          From = @from,
          ValueChain = valueChain,
          To = @to,
          Value = val
        });
      }
    }

    private static void FindParentValues(HtmlNode current, ref List<string> valueChain, ref HtmlNodeCollection htmlNodeCollection)
    {
      var id = current.GetAttributeValue("xml:id", "");
      var baseType = current.GetAttributeValue("baseTypes", "");

      if (string.IsNullOrEmpty(baseType) || baseType == id)
      {
        var parent = GetLayerName(current.ParentNode);
        valueChain.Insert(0, parent);
      }
      else
      {
        var parentNode = htmlNodeCollection.FirstOrDefault(q => q.GetAttributeValue("xml:id", "") == baseType);
        var parent = parentNode.ChildNodes.FirstOrDefault(x => x.Name == "fsdescr")?.InnerText;
        valueChain.Insert(0, parent);
        FindParentValues(parentNode, ref valueChain, ref htmlNodeCollection);
      }
    }

    private static string FindAnnotator(HtmlNodeCollection fsChildNodes)
    {
      foreach (var x in fsChildNodes)
      {
        var name = x.GetAttributeValue("name", "");
        if (name == "catma_markupauthor")
          return x.InnerText;
      }

      return string.Empty;
    }

    private static void BodyLoopText(HtmlNode htmlNode, ref StringBuilder text)
    {
      text.AppendLine(htmlNode.InnerText);
    }

    private static string GetLayerName(HtmlNode node)
    {
      var value = node.GetAttributeValue("n", "");
      var split = value.Split(' ').ToList();
      split.RemoveAt(split.Count - 1);
      var layer = string.Join(" ", split);
      return layer;
    }

    private static string RemoveWhitespace(string text)
      => text.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();
  }
}
