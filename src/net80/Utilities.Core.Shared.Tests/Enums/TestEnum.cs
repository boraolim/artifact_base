namespace Utilities.Core.Shared.Tests;

public enum TestEnum
{
    [Description("Activo")]
    Active = 1,

    [Description("Inactivo")]
    Inactive = 2,

    NoDescription = 3
}

// ======== MODELOS DE PRUEBA =========

public enum StatusEnum
{
    [Description("Activo")]
    Active,

    [Description("Inactivo")]
    Inactive
}

public enum SimpleEnum
{
    A,
    B
}




