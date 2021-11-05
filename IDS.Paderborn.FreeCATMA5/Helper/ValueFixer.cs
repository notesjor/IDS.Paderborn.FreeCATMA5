using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Paderborn.FreeCATMA5.Helper
{
  public static class ValueFixer
  {
    private static Dictionary<string, string> _values = null;

    public static string Fix(string input)
    {
      if (_values == null)
        Init();

      if (_values != null && _values.ContainsKey(input))
        return _values[input];
      return input;
    }

    private static void Init()
    {
      var lines = File.ReadAllLines("Fixes.txt", Encoding.UTF8);
      _values = new Dictionary<string, string>();
      foreach (var line in lines)
      {
        var split = line.Split(new[] {"\t"}, StringSplitOptions.None);
        if (split.Length != 2)
          continue;
        _values.Add(split[0], split[1]);
      }
    }
  }
}
