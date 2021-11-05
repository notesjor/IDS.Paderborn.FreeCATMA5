using System.Collections.Generic;
using IDS.Paderborn.FreeCATMA5.Helper;

namespace IDS.Paderborn.FreeCATMA5.Model.v2
{
  public class Annotation
  {
    private string _layer;
    private string _layerValue;

    public string Layer
    {
      get => _layer;
      set
      {
        _layer = ValueFixer.Fix(value);
      }
    }

    public string LayerValue
    {
      get => _layerValue;
      set
      {
        _layerValue = ValueFixer.Fix(value);
      }
    }

    public int From { get; set; }
    public int To { get; set; }
  }
}