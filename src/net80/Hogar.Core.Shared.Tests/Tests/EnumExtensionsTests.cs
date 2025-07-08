namespace Hogar.Core.Shared.Tests;

public class EnumExtensionsTests
{
    [Fact]
    public void GetAttribute_ShouldReturnAttribute_WhenExists()
    {
        var attr = SampleValues.First.GetAttribute<DescriptionAttribute>();
        Assert.NotNull(attr);
        Assert.Equal("First value", attr.Description);
    }

    [Fact]
    public void GetAttribute_ShouldReturnNull_WhenNoAttribute()
    {
        var attr = SampleValues.Third.GetAttribute<DescriptionAttribute>();
        Assert.Null(attr);
    }

    [Fact]
    public void GetDescription_ShouldReturnDescription_WhenExists()
    {
        var desc = SampleValues.Second.GetDescription();
        Assert.Equal("Second value", desc);
    }

    [Fact]
    public void GetDescription_ShouldReturnEnumName_WhenNoDescription()
    {
        var desc = SampleValues.Third.GetDescription();
        Assert.Equal("Third", desc);
    }

    [Fact]
    public void GetUnderlyingType_ShouldReturnInt()
    {
        var type = SampleValues.First.GetUnderlyingType();
        Assert.Equal(typeof(int), type);
    }

    [Fact]
    public void Parse_ShouldReturnCorrectEnum()
    {
        var result = "First".Parse<SampleValues>();
        Assert.Equal(SampleValues.First, result);
    }

    [Fact]
    public void GetName_ShouldReturnEnumName()
    {
        var name = SampleValues.Second.GetName();
        Assert.Equal("Second", name);
    }

    [Fact]
    public void CompareTo_ShouldReturnComparisonResult()
    {
        var result = SampleValues.First.CompareTo(SampleValues.Second);
        Assert.True(result < 0);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenSameValue()
    {
        var result = SampleValues.First.Equals(SampleValues.First);
        Assert.True(result);
    }

    [Fact]
    public void GetHashCode_ShouldBeConsistent()
    {
        var code = SampleValues.First.GetHashCode();
        Assert.Equal(SampleValues.First.GetHashCode(), code);
    }

    [Fact]
    public void GetTypeCode_ShouldReturnInt32()
    {
        var code = SampleValues.First.GetTypeCode();
        Assert.Equal(TypeCode.Int32, code);
    }

    [Fact]
    public void HasFlag_ShouldReturnTrue_WhenFlagPresent()
    {
        var combined = FlagValues.Read | FlagValues.Write;
        Assert.True(combined.HasFlag(FlagValues.Write));
    }

    [Fact]
    public void GetValues_ShouldReturnAllEnumValues()
    {
        var values = EnumExtensions.GetValues<SampleValues>();
        Assert.Equal(3, values.Count);
        Assert.Contains(SampleValues.First, values);
    }

    [Fact]
    public void IsDefined_ShouldReturnTrue_WhenDefined()
    {
        Assert.True(SampleValues.First.IsDefined());
    }

    [Fact]
    public void ToStringValue_ShouldReturnEnumName()
    {
        var str = SampleValues.First.ToStringValue();
        Assert.Equal("First", str);
    }

    [Fact]
    public void ToStringLowerValue_ShouldReturnLowerCase()
    {
        var str = SampleValues.First.ToStringLowerValue();
        Assert.Equal("first", str);
    }

    [Fact]
    public void ToStringUpperValue_ShouldReturnUpperCase()
    {
        var str = SampleValues.First.ToStringUpperValue();
        Assert.Equal("FIRST", str);
    }

    [Fact]
    public void GetRandomValue_ShouldReturnDefinedEnum()
    {
        var value = EnumExtensions.GetRandomValue<SampleValues>();
        var defined = EnumExtensions.GetValues<SampleValues>();
        Assert.Contains(value, defined);
    }

    [Fact]
    public void ToNullable_ShouldReturnNullAlways()
    {
        var nullable = SampleValues.First.ToNullable();
        Assert.Null(nullable);
    }

    [Fact]
    public void CompareTo_ShouldReturnZero_WhenEqual()
    {
        var result = SampleStatus.Started.CompareTo(SampleStatus.Started);
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareTo_ShouldReturnNegative_WhenLessThan()
    {
        var result = SampleStatus.Started.CompareTo(SampleStatus.Completed);
        Assert.True(result < 0);
    }

    [Fact]
    public void CompareTo_ShouldReturnPositive_WhenGreaterThan()
    {
        var result = SampleStatus.Completed.CompareTo(SampleStatus.Started);
        Assert.True(result > 0);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenEqual()
    {
        var result = SampleStatus.Started.Equals(SampleStatus.Started);
        Assert.True(result);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenDifferent()
    {
        var result = SampleStatus.Started.Equals(SampleStatus.Completed);
        Assert.False(result);
    }

    [Fact]
    public void GetHashCode_ShouldBeStable()
    {
        var hash1 = SampleStatus.Completed.GetHashCode();
        var hash2 = SampleStatus.Completed.GetHashCode();
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetTypeCode_ShouldSampleStatusReturnInt32()
    {
        var typeCode = SampleStatus.Started.GetTypeCode();
        Assert.Equal(TypeCode.Int32, typeCode);
    }

    [Theory]
    [InlineData(SampleValues.First, "First value")]
    [InlineData(SampleValues.Second, "Second value")]
    [InlineData(SampleValues.Third, "Third")]
    public void GetDescription_ShouldReturnCorrectDescription(SampleValues value, string expected)
    {
        var result = value.GetDescription();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("First", SampleValues.First)]
    [InlineData("Second", SampleValues.Second)]
    [InlineData("Third", SampleValues.Third)]
    public void Parse_ShouldParseStringToEnum(string input, SampleValues expected)
    {
        var result = input.Parse<SampleValues>();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(SampleValues.First, "FIRST", "first")]
    [InlineData(SampleValues.Second, "SECOND", "second")]
    public void ToStringVariants_ShouldReturnCorrectCases(SampleValues value, string expectedUpper, string expectedLower)
    {
        Assert.Equal(expectedUpper, value.ToStringUpperValue());
        Assert.Equal(expectedLower, value.ToStringLowerValue());
    }

    [Theory]
    [InlineData(SampleValues.First, SampleValues.Second)]
    [InlineData(SampleValues.Second, SampleValues.First)]
    public void CompareTo_ShouldWorkCorrectly(SampleValues left, SampleValues right)
    {
        var result = left.CompareTo(right);
        Assert.Equal(left.CompareTo(right), result);
    }
}
