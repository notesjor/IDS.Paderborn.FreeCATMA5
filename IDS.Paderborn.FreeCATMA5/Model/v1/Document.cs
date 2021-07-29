using System.Collections.Generic;

namespace IDS.Paderborn.FreeCATMA5.Model.v1
{
  public class Document
  {
    public string Text { get; set; }
    public List<Annotation> Annotations { get; set; }
  }
}