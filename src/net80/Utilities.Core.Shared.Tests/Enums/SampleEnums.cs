using System;
using System.ComponentModel;

namespace Utilities.Core.Shared.Tests.Enums
{
    public enum SampleValues
    {
        [Description("First value")]
        First = 1,

        [Description("Second value")]
        Second = 2,

        Third = 4
    }

    [Flags]
    public enum FlagValues
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4
    }

    public enum SampleStatus
    {
        None = 0,
        Started = 1,
        Completed = 2
    }
}
