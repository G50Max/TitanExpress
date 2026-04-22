using System.Collections.Generic;

namespace TitanExpress.Models
{
    public class GameResources
    {
        public int Bandwidth { get; set; } = 10;
        public int MaxBandwidth { get; set; } = 10;
        public int Battery { get; set; } = 50;
        public int Reputation { get; set; } = 50;
        public int Money { get; set; } = 100;

        public override string ToString()
        {
            return $"带宽: {Bandwidth}/{MaxBandwidth} | 电池: {Battery} | 信誉: {Reputation} | 资金: ${Money}";
        }
    }
}
