using System.Collections.Generic;

namespace HotelBookingSystem.Services
{
    public static class RoomFeatureLookup
    {
        public static readonly Dictionary<string, List<string>> FeaturesByType =
            new Dictionary<string, List<string>>(System.StringComparer.OrdinalIgnoreCase)
            {
                {
                    "Suite", new List<string>
                    {
                        "wifi",
                        "shower",
                        "heat",
                        "snowflake",
                        "couch",
                        "plate-utensils",
                        "terrace",
                        "holding-hand-dinner",
                        "screen",
                        "people-roof",
                        "bed-alt"
                    }
                },
                {
                    "Standard", new List<string>
                    {
                        "wifi",
                        "shower",
                        "heat",
                        "screen",
                        "bed-alt"
                    }
                },
                {
                    "Economy", new List<string>
                    {
                        "wifi",
                        "shower",
                        "bed-alt"
                    }
                }
            };
    }
}
