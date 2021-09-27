﻿using CsvHelper.Configuration;

namespace OutlookOkan.Types
{
    public sealed class DeferredDeliveryMinutes
    {
        public string TargetAddress { get; set; }
        public int DeferredMinutes { get; set; }
    }

    public sealed class DeferredDeliveryMinutesMap : ClassMap<DeferredDeliveryMinutes>
    {
        public DeferredDeliveryMinutesMap()
        {
            _ = Map(m => m.TargetAddress).Index(0);
            _ = Map(m => m.DeferredMinutes).Index(1).Default(0);
        }
    }
}