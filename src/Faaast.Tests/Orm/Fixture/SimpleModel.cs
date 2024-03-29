﻿using System;

namespace Faaast.Tests.Orm.Fixture
{
    public enum TestState
    {
        Undefined = 0,
        ItWorks = 1
    }

    public class SimpleModel
    {
        public int V1 { get; set; }
        public string V2 { get; set; }
        public DateTime V3 { get; set; }
        public Guid V4 { get; set; }
        public float? V5 { get; set; }
        public long? V6 { get; set; }
        public double? V7 { get; set; }
        public bool V8 { get; set; }
        public TestState EnumValue { get; set; }
    }
}
