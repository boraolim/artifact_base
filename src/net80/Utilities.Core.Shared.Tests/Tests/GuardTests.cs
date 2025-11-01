namespace Utilities.Core.Shared.Tests;

public class GuardTests
{
    [Fact]
    public void AgainstNull_ValidInput_ReturnsValue()
    {
        var result = Guard.AgainstNull("hello", "param");
        Assert.Equal("hello", result);
    }

    [Fact]
    public void AgainstNull_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Guard.AgainstNull<string>(null, "param"));
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("x")]
    public void AgainstNullOrEmpty_ValidString_ReturnsValue(string value)
    {
        var result = Guard.AgainstNullOrEmpty(value, "param");
        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void AgainstNullOrEmpty_InvalidString_Throws(string? value)
    {
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrEmpty(value!, "param"));
    }

    [Fact]
    public void Positive_Valid_ReturnsValue()
    {
        var result = Guard.Positive(5, "param");
        Assert.Equal(5, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Positive_Invalid_Param_Throws(int value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Guard.Positive(value, "param"));
    }

    [Fact]
    public void Range_InsideRange_ReturnsValue()
    {
        var result = Guard.Range(5, 1, 10, "param");
        Assert.Equal(5, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Range_OutsideRange_Throws(int value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Guard.Range(value, 1, 10, "param"));
    }

    private enum TestEnum { A = 1, B = 2 }

    [Fact]
    public void EnumDefined_ValidEnum_ReturnsValue()
    {
        var result = Guard.EnumDefined(TestEnum.A, "param");
        Assert.Equal(TestEnum.A, result);
    }

    [Fact]
    public void EnumDefined_InvalidEnum_Throws()
    {
        Assert.Throws<ArgumentException>(() => Guard.EnumDefined((TestEnum)999, "param"));
    }

    [Fact]
    public void NotEqual_Valid_ReturnsValue()
    {
        var result = Guard.NotEqual(10, 5, "param");
        Assert.Equal(10, result);
    }

    [Fact]
    public void NotEqual_Equal_Throws()
    {
        Assert.Throws<ArgumentException>(() => Guard.NotEqual(5, 5, "param"));
    }

    [Fact]
    public void AgainstNull_ValidValue_ReturnsValue()
    {
        var input = "hello";
        var result = Guard.AgainstNull(input, nameof(input));
        Assert.Equal(input, result);
    }

    [Fact]
    public void AgainstNull_Null_Throws()
    {
        string? input = null;
        Assert.Throws<ArgumentNullException>(() => Guard.AgainstNull(input, nameof(input)));
    }

    [Fact]
    public void AgainstNullOrEmpty_Valid_ReturnsValue()
    {
        var result = Guard.AgainstNullOrEmpty("abc", "name");
        Assert.Equal("abc", result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void AgainstNullOrEmpty_Invalid_Throws(string? value)
    {
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrEmpty(value!, "param"));
    }

    [Fact]
    public void AgainstDefault_ValidInt_ReturnsValue()
    {
        var result = Guard.AgainstDefault(5, "age");
        Assert.Equal(5, result);
    }

    [Fact]
    public void AgainstDefault_Default_Throws()
    {
        Assert.Throws<ArgumentException>(() => Guard.AgainstDefault(0, "age"));
    }

    [Fact]
    public void Positive_ValidInt_ReturnsValue()
    {
        var result = Guard.Positive(10, "value");
        Assert.Equal(10, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Positive_Invalid_Value_Throws(int value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Guard.Positive(value, "value"));
    }

    [Fact]
    public void EmptyGuid_Valid_ReturnsValue()
    {
        var id = Guid.NewGuid();
        Assert.Equal(id, Guard.EmptyGuid(id, "id"));
    }

    [Fact]
    public void EmptyGuid_GuidEmpty_Throws()
    {
        Assert.Throws<ArgumentException>(() => Guard.EmptyGuid(Guid.Empty, "id"));
    }

    [Fact]
    public void Range_Valid_ReturnsValue()
    {
        var result = Guard.Range(5, 1, 10, "age");
        Assert.Equal(5, result);
    }

    [Fact]
    public void Range_Invalid_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Guard.Range(50, 1, 10, "age"));
    }

    [Fact]
    public void StartsWith_Valid_ReturnsValue()
    {
        var result = Guard.StartsWith("hello", "he", "p");
        Assert.Equal("hello", result);
    }

    [Fact]
    public void StartsWith_Invalid_Throws()
    {
        Assert.Throws<ArgumentException>(() => Guard.StartsWith("hello", "xx", "p"));
    }

    private enum SampleEnum { A = 1, B = 2 }

    [Fact]
    public void EnumDefined_Valid_ReturnsValue()
    {
        Assert.Equal(SampleEnum.A, Guard.EnumDefined(SampleEnum.A, "enum"));
    }

    [Fact]
    public void EnumDefined_Invalid_Throws()
    {
        Assert.Throws<ArgumentException>(() => Guard.EnumDefined((SampleEnum)999, "enum"));
    }

    [Fact]
    public void JsonValid_Valid_ReturnsValue()
    {
        var json = "{\"name\":\"test\"}";
        Assert.Equal(json, Guard.JsonValid(json, "json"));
    }

    [Fact]
    public void JsonValid_Invalid_Throws()
    {
        Assert.Throws<ArgumentException>(() => Guard.JsonValid("not_json", "json"));
    }

    [Fact]
    public void Url_Valid_ReturnsValue()
    {
        var url = "https://google.com";
        Assert.Equal(url, Guard.Url(url, "url"));
    }

    [Fact]
    public void Url_Invalid_Throws()
    {
        Assert.Throws<ArgumentException>(() => Guard.Url("not_url", "url"));
    }

    [Fact]
    public void NotInList_Valid_Returns()
    {
        var invalid = new List<int> { 2, 3 };
        Assert.Equal(1, Guard.NotInList(1, invalid, "value"));
    }

    [Fact]
    public void NotInList_Invalid_Throws()
    {
        var invalid = new List<int> { 2, 3 };
        Assert.Throws<ArgumentException>(() => Guard.NotInList(2, invalid, "value"));
    }

    [Theory]
    [InlineData(5, 1, 10)]
    [InlineData(1, 1, 1)]
    [InlineData(10, 1, 10)]
    public void AgainstOutOfRange_Success(int value, int min, int max)
    {
        Guard.AgainstOutOfRange(value, min, max, nameof(value));
    }

    [Theory]
    [InlineData(0, 1, 10)]
    [InlineData(11, 1, 10)]
    public void AgainstOutOfRange_Error(int value, int min, int max)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Guard.AgainstOutOfRange(value, min, max, nameof(value)));
    }

    [Fact]
    public void All_Success()
    {
        var list = new List<int> { 2, 4, 6 };
        Guard.All(list, x => x % 2 == 0, nameof(list), "Must be even");
    }

    [Fact]
    public void All_Error()
    {
        var list = new List<int> { 2, 3, 6 };
        Assert.Throws<ArgumentException>(() =>
            Guard.All(list, x => x % 2 == 0, nameof(list), "Must be even"));
    }

    [Fact]
    public void DirectoryExists_Success()
    {
        var dir = Directory.GetCurrentDirectory();
        Guard.DirectoryExists(dir, nameof(dir));
    }

    [Fact]
    public void DirectoryExists_Error()
    {
        Assert.Throws<DirectoryNotFoundException>(() =>
            Guard.DirectoryExists("C:\\Invalid_Dir_12345", "path"));
    }

    [Fact]
    public void FileExists_Success()
    {
        var path = Path.GetTempFileName();
        Guard.FileExists(path, nameof(path));
        File.Delete(path);
    }

    [Fact]
    public void FileExists_Error()
    {
        Assert.Throws<FileNotFoundException>(() =>
            Guard.FileExists("C:\\Invalid_File_12345.txt", "path"));
    }

    [Fact]
    public void Regex_Success()
    {
        var input = "ABC123";
        Guard.RegexValue(input, "^[A-Z0-9]+$", nameof(input));
    }

    [Fact]
    public void Regex_Error()
    {
        Assert.Throws<ArgumentException>(() =>
            Guard.RegexValue("abc123", "^[A-Z0-9]+$", "input"));
    }

    [Fact]
    public void EndsWith_Success()
    {
        Guard.EndsWith("filename.txt", ".txt", "file");
    }

    [Fact]
    public void EndsWith_Error()
    {
        Assert.Throws<ArgumentException>(() =>
            Guard.EndsWith("filename.txt", ".png", "file"));
    }

    [Fact]
    public void MaxLength_Success()
    {
        Guard.MaxLength("Hello", 10, "value");
    }

    [Fact]
    public void MaxLength_Error()
    {
        Assert.Throws<ArgumentException>(() =>
            Guard.MaxLength("TooLongString", 5, "value"));
    }

    [Fact]
    public void MinLength_Success()
    {
        Guard.MinLength("Hello", 3, "value");
    }

    [Fact]
    public void MinLength_Error()
    {
        Assert.Throws<ArgumentException>(() =>
            Guard.MinLength("Hi", 3, "value"));
    }

    [Fact]
    public void False_Success()
    {
        Guard.False(true, "flag");
    }

    [Fact]
    public void False_Error()
    {
        Assert.Throws<ArgumentException>(() =>
            Guard.False(false, "flag"));
    }

    [Fact]
    public void True_Success()
    {
        Guard.True(false, "flag");
    }

    [Fact]
    public void True_Error()
    {
        Assert.Throws<ArgumentException>(() =>
            Guard.True(true, "flag"));
    }

    [Fact]
    public void NegativeOrZero_Success()
    {
        Guard.NegativeOrZero(5, "value");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void NegativeOrZero_Error(int value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Guard.NegativeOrZero(value, "value"));
    }

    [Fact]
    public void AgainstPredicate_Success()
    {
        Guard.Against(10, x => x < 5, "Too small", "value");
    }

    [Fact]
    public void AgainstPredicate_Error()
    {
        Assert.Throws<ArgumentException>(() =>
            Guard.Against(3, x => x < 5, "Too small", "value"));
    }

    [Fact]
    public void AgainstNullOrWhiteSpace_Success()
    {
        Guard.AgainstNullOrWhiteSpace("hello", "value");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AgainstNullOrWhiteSpace_Error(string? input)
    {
        Assert.Throws<ArgumentException>(() =>
            Guard.AgainstNullOrWhiteSpace(input, "value"));
    }

    [Fact]
    public void AgainstNullOrEmpty_Success()
    {
        Guard.AgainstNullOrEmpty("hello", "value");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void AgainstNullOrEmpty_Error(string input)
    {
        Assert.Throws<ArgumentException>(() =>
            Guard.AgainstNullOrEmpty(input, "value"));
    }

    [Fact]
    public void AgainstNullOrEmpty_InputNull_Throws()
    {
        List<int>? input = null;

        var ex = Assert.Throws<ArgumentException>(() =>
            Guard.AgainstNullOrEmpty(input, "numbers"));

        Assert.Equal("numbers", ex.ParamName);
    }

    [Fact]
    public void AgainstNullOrEmpty_InputEmpty_Throws()
    {
        var input = new List<int>();

        var ex = Assert.Throws<ArgumentException>(() =>
            Guard.AgainstNullOrEmpty(input, "numbers"));

        Assert.Equal("numbers", ex.ParamName);
    }

    [Fact]
    public void AgainstNullOrEmpty_Valid_ReturnsSameInstance()
    {
        var input = new List<int> { 1, 2, 3 };

        var result = Guard.AgainstNullOrEmpty(input, "numbers");

        Assert.Same(input, result);
    }

    [Fact]
    public void AgainstNullOrEmpty_CustomMessage_ThrowsWithMessage()
    {
        var input = new List<int>();

        var ex = Assert.Throws<ArgumentException>(() =>
            Guard.AgainstNullOrEmpty(input, "numbers", "Lista inválida"));

        Assert.Contains("Lista inválida", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void AgainstNullOrEmpty_InvalidParameterName_Throws(string? paramName)
    {
        var input = new List<int> { 1 };

        Assert.Throws<ArgumentException>(() =>
            Guard.AgainstNullOrEmpty(input, paramName!));
    }

    [Fact]
    public void AgainstNullOrEmpty_SingleItem_Success()
    {
        var input = new List<int> { 100 };

        var result = Guard.AgainstNullOrEmpty(input, "numbers");

        Assert.Equal(input, result);
    }

    [Fact]
    public void AgainstNullOrEmpty_MultipleItems_Success()
    {
        var input = new List<string> { "A", "B", "C" };

        var result = Guard.AgainstNullOrEmpty(input, "letters");

        Assert.Equal(input, result);
    }

    [Fact]
    public void AgainstNullOrEmpty_ItemsContainNull_Success()
    {
        var input = new List<string?> { null, "X", "Y" };

        var result = Guard.AgainstNullOrEmpty(input, "values");

        Assert.Equal(input, result);
    }

    [Fact]
    public void AgainstNullOrEmpty_LazyEnumerable_Success()
    {
        IEnumerable<int> GetValues()
        {
            yield return 1;
            yield return 2;
        }

        var result = Guard.AgainstNullOrEmpty(GetValues(), "values");

        Assert.Collection(result,
            item => Assert.Equal(1, item),
            item => Assert.Equal(2, item));
    }

    [Fact]
    public void Against_PredicateFalse_ReturnsInput()
    {
        var result = Guard.Against(10, x => x < 0, "number");

        Assert.Equal(10, result);
    }

    [Fact]
    public void Against_PredicateTrue_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            Guard.Against(5, x => x > 0, "number"));

        Assert.Equal("number", ex.ParamName);
    }

    [Fact]
    public void Against_PredicateNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Guard.Against(10, null!, "number"));
    }

    [Fact]
    public void Against_InvalidParameterName_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Guard.Against(10, x => false, ""));
    }

}
