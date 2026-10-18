using System.ComponentModel;

namespace KrzaqRPG.Backend.Common.Enums
{
    public static class Parameter
    {
        public enum ParamKey
        {
            [Description(ParamName.AreaBaseTravelTime)]
            AreaBaseTravelTime,
            [Description(ParamName.DistanceToTravelTimeConversion)]
            DistanceToTravelTimeConversion,
        }

        public struct ParamName
        {
            public const string AreaBaseTravelTime = "AreaBaseTravelTime";
            public const string DistanceToTravelTimeConversion = "DistanceToTravelTimeConversionFactor";
        }
    }
}
