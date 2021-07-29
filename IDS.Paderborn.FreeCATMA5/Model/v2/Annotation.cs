using System.Collections.Generic;

namespace IDS.Paderborn.FreeCATMA5.Model.v2
{
  public class Annotation
  {
    public string Layer { get; set; }
    public string LayerValue { get; set; }
    public int From { get; set; }
    public int To { get; set; }
  }
}