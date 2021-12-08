using System.Collections.Generic;

namespace IDS.Paderborn.FreeCATMA5.Model.v1
{
  public class Annotation
  {
    public string Annotator { get; set; }
    public List<string> ValueChain { get; set; }
    public int From { get; set; }
    public int To { get; set; }
  }
}