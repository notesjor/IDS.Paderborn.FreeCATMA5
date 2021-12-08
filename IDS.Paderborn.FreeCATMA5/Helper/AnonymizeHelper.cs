using System.Collections.Generic;

namespace IDS.Paderborn.FreeCATMA5.Helper
{
  public class AnonymizeHelper
  {
    private Dictionary<string, string> _dict = new Dictionary<string, string>();

    public string Anonymize(string name)
    {
      if (_dict.ContainsKey(name))
        return _dict[name];

      _dict.Add(name, $"Annotator {_dict.Count + 1:D2}");
      return _dict[name];
    }
  }
}
