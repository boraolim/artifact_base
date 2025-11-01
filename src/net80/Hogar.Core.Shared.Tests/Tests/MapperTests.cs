namespace Hogar.Core.Shared.Tests;

public class MapperTests
{
    [Fact]
    public void MapTo_ShouldMapBasicProperties()
    {
        var source = new SourceModel { Name = "Olimpo", Age = 30, Tags = new() { "A", "B" } };

        var result = source.MapTo<TargetModel>();

        Assert.NotNull(result);
        Assert.Equal("Olimpo", result.Name);
        Assert.Equal(30, result.Age);
        Assert.Equal(2, result.Tags.Count);
        Assert.Contains("A", result.Tags);
    }

    [Fact]
    public void MapTo_ShouldConvertCompatibleTypes()
    {
        var source = new SourceModel { Name = "Olimpo", Age = 42, Tags = new() { "X", "Y" } };

        var result = source.MapTo<ComplexTargetModel>();

        Assert.Equal("Olimpo", result.Name);
        Assert.Equal(42L, result.Age); // int -> long
    }

    [Fact]
    public void MapTo_ShouldCacheDelegate()
    {
        var source1 = new SourceModel { Name = "First", Age = 1 };
        var source2 = new SourceModel { Name = "Second", Age = 2 };

        var result1 = source1.MapTo<TargetModel>();
        var result2 = source2.MapTo<TargetModel>();

        Assert.Equal("First", result1.Name);
        Assert.Equal("Second", result2.Name);
    }

    [Fact]
    public void MapTo_ShouldThrow_WhenSourceIsNull()
    {
        SourceModel source = null!;
        var ex = Assert.Throws<ArgumentNullException>(() => source.MapTo<TargetModel>());
        Assert.Contains("source", ex.ParamName);
    }

    [Fact]
    public void MapTo_ShouldHandleMissingMatchingProperties()
    {
        var source = new { Title = "Solo propiedad diferente" }; // No hay propiedades coincidentes

        var result = source.MapTo<TargetModel>();

        Assert.NotNull(result);
        Assert.Null(result.Name);
        Assert.Equal(0, result.Age);
    }

    [Fact]
    public void MapTo_ShouldMapListOfObjects()
    {
        var sourceList = new List<SourceModel>
        {
            new() { Name = "A", Age = 1 },
            new() { Name = "B", Age = 2 }
        };

        var mapped = sourceList.Select(x => x.MapTo<TargetModel>()).ToList();

        Assert.Equal(2, mapped.Count);
        Assert.Equal("A", mapped[0].Name);
        Assert.Equal("B", mapped[1].Name);
    }

    [Fact]
    public void MapTo_ShouldConvertDictionaryTypes()
    {
        var source = new SourceWithDict
        {
            Data = new Dictionary<string, int> { { "X", 1 }, { "Y", 2 } }
        };

        var result = source.MapTo<TargetWithDict>();

        Assert.NotNull(result.Data);
        Assert.Equal(1L, result.Data["X"]);
        Assert.Equal(2L, result.Data["Y"]);
    }

    [Fact]
    public void MapTo_ShouldMapToConstructorWithParameters()
    {
        var source = new SourceCtor { FullName = "Olimpo", Years = 35 };
        var result = source.MapTo<TargetWithConstructor>();

        Assert.Equal("Olimpo", result.FullName);
        Assert.Equal(35, result.Years);
    }

    [Fact]
    public void MapTo_ShouldMapNestedObject()
    {
        var source = new SourceNested
        {
            Name = "Carlos",
            Address = new Address { Street = "Av. Reforma", City = "CDMX" }
        };

        var result = source.MapTo<TargetNested>();

        Assert.Equal("Carlos", result.Name);
        Assert.NotNull(result.Address);
        Assert.Equal("Av. Reforma", result.Address.Street);
        Assert.Equal("CDMX", result.Address.City);
    }

    [Fact]
    public void GetGenericMethod_Should_Return_Correct_MethodInfo()
    {
        var methodInfo = typeof(GenericMapperExtensions)
            .GetMethod("GetGenericMethod", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(methodInfo);

        var result = methodInfo.Invoke(null, new object[]
        {
        typeof(Enumerable),
        "Select",
        2,
        new Type[] { typeof(int), typeof(string) }
        }) as MethodInfo;

        Assert.NotNull(result);
        Assert.Equal("Select", result.Name);
        Assert.True(result.IsGenericMethod);
        Assert.False(result.IsGenericMethodDefinition);
        Assert.Equal(2, result.GetGenericArguments().Length);
    }

    [Fact]
    public void ConvertCollection_Should_Create_Valid_Expression_And_Convert()
    {
        var methodInfo = typeof(GenericMapperExtensions)
            .GetMethod("ConvertCollection", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(methodInfo);

        // source: List<int> with some values
        var sourceParam = Expression.Parameter(typeof(IEnumerable<int>), "source");

        // Invoke ConvertCollection to get an expression that converts IEnumerable<int> to List<long>
        var expr = methodInfo.Invoke(null, new object[] { sourceParam, typeof(int), typeof(long) }) as Expression;
        Assert.NotNull(expr);

        // Build lambda Func<IEnumerable<int>, List<long>>
        var lambda = Expression.Lambda<Func<IEnumerable<int>, List<long>>>(expr, sourceParam);
        var func = lambda.Compile();

        // Execute and check
        var input = new List<int> { 1, 2, 3 };
        var result = func(input);

        Assert.NotNull(result);
        Assert.IsType<List<long>>(result);
        Assert.Equal(new List<long> { 1L, 2L, 3L }, result);
    }

    [Fact]
    public void MapTo_Should_Convert_List_Of_Int_To_List_Of_Long()
    {
        var source = new Source { Numbers = new List<int> { 1, 2, 3 } };
        var target = source.MapTo<Target>();

        Assert.NotNull(target);
        Assert.NotNull(target.Numbers);
        Assert.All(target.Numbers, item => Assert.IsType<long>(item));
        Assert.Equal(new List<long> { 1L, 2L, 3L }, target.Numbers);
    }

    [Fact]
    public void GetSourceMemberExpression_Should_Handle_Properties_Fields_And_Others()
    {
        var method = typeof(GenericMapperExtensions)
            .GetMethod("GetSourceMemberExpression", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);

        // Parámetro Expression para cada tipo
        var sourceTypedProp = Expression.Parameter(typeof(SourceWithProperty), "source");
        var sourceTypedField = Expression.Parameter(typeof(SourceWithField), "source");
        var sourceTypedMethod = Expression.Parameter(typeof(SourceWithMethod), "source");

        // 1) Propiedad: "Name"
        var exprProp = method.Invoke(null, new object[] {
        typeof(SourceWithProperty),
        sourceTypedProp,
        "Name",
        typeof(string)
    }) as Expression;
        Assert.NotNull(exprProp);
        Assert.Equal(typeof(string), exprProp.Type);
        Assert.IsAssignableFrom<MemberExpression>(exprProp);

        // 2) Campo: "Age"
        var exprField = method.Invoke(null, new object[] {
        typeof(SourceWithField),
        sourceTypedField,
        "Age",
        typeof(int)
    }) as Expression;
        Assert.NotNull(exprField);
        Assert.Equal(typeof(int), exprField.Type);
        Assert.IsAssignableFrom<MemberExpression>(exprField);

        // 3) Miembro que no es propiedad ni campo: "DoSomething" (método)
        var exprNull = method.Invoke(null, new object[] {
        typeof(SourceWithMethod),
        sourceTypedMethod,
        "DoSomething",
        typeof(void)
    }) as Expression;
        Assert.Null(exprNull);
    }

    [Fact]
    public void GetMemberType_Should_Return_Type_For_PropertyInfo()
    {
        // Arrange
        var method = typeof(GenericMapperExtensions)
            .GetMethod("GetMemberType", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method); // validar que lo encontró

        var propInfo = typeof(TestClassForProp).GetProperty(nameof(TestClassForProp.SomeProperty));

        // Act
        var result = method.Invoke(null, new object[] { propInfo }) as Type;

        // Assert
        Assert.Equal(typeof(string), result);
    }

    [Fact]
    public void GetMemberType_Should_Return_Type_For_FieldInfo()
    {
        var method = typeof(GenericMapperExtensions)
            .GetMethod("GetMemberType", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var fieldInfo = typeof(TestClassForProp).GetField(nameof(TestClassForProp.SomeField));
        var result = method.Invoke(null, new object[] { fieldInfo }) as Type;

        Assert.Equal(typeof(int), result);
    }

    [Fact]
    public void GetMemberType_Should_Throw_For_Invalid_MemberInfo()
    {
        // Arrange
        var method = typeof(GenericMapperExtensions)
            .GetMethod("GetMemberType", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var ctorInfo = typeof(TestClassForProp).GetConstructors().First();

        // Act & Assert
        var exception = Assert.Throws<TargetInvocationException>(() =>
            method.Invoke(null, new object[] { ctorInfo })
        );

        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    [Fact]
    public void GetMemberType_Should_Throw_For_Other_MemberInfo()
    {
        var method = typeof(GenericMapperExtensions)
            .GetMethod("GetMemberType", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var methodInfo = typeof(TestClass).GetMethod(nameof(TestClassForProp.SomeMethod));

        var ex = Assert.Throws<TargetInvocationException>(() =>
            method.Invoke(null, new object[] { methodInfo }));

        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }

    [Fact]
    public void GetSourceMemberExpression_Should_Return_Null_When_Member_Not_Found()
    {
        // Arrange
        var method = typeof(GenericMapperExtensions)
            .GetMethod("GetSourceMemberExpression", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        // Parámetros:
        // - sourceType: TestClass
        // - sourceTyped: null (para la prueba no importa, porque no llegará a usarse)
        // - memberName: Un nombre que no existe
        // - targetType: Cualquier tipo, ej. string

        var result = method.Invoke(null, new object[]
        {
            typeof(TestClassForProp),
            null, // no importa, no se usa si no encuentra miembro
            "NombreQueNoExiste",
            typeof(string)
        });

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetSourceMemberExpression_Should_Return_Valid_Expression_When_Member_Found()
    {
        // Arrange
        var method = typeof(GenericMapperExtensions)
            .GetMethod("GetSourceMemberExpression", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        // Preparar un parámetro para la expresión
        var sourceParam = Expression.Parameter(typeof(TestClassForProp), "source");

        // Act
        var result = method.Invoke(null, new object[]
        {
            typeof(TestClassForProp),
            sourceParam,                  // Este sí importa, porque va a generar la expresión
            nameof(TestClassForProp.SomeProperty),
            typeof(string)
        });

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<MemberExpression>(result); // Debe devolver una expresión de acceso a propiedad

        var memberExpr = result as MemberExpression;
        Assert.Equal(nameof(TestClassForProp.SomeProperty), memberExpr.Member.Name);
    }

    [Fact]
    public void GetSourceMemberExpression_Should_Return_Valid_Expression_For_Field()
    {
        // Arrange
        var method = typeof(GenericMapperExtensions)
            .GetMethod("GetSourceMemberExpression", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        // Crear parámetro de expresión (sourceTyped)
        var sourceParam = Expression.Parameter(typeof(TestClassForProp), "source");

        // Act
        var result = method.Invoke(null, new object[]
        {
            typeof(TestClassForProp),
            sourceParam,                   // Expression sourceTyped
            nameof(TestClassForProp.SomeField),   // Campo público
            typeof(int)
        });

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<MemberExpression>(result); // Debe ser MemberExpression (igual que con propiedad)

        var memberExpr = result as MemberExpression;
        Assert.Equal(nameof(TestClassForProp.SomeField), memberExpr.Member.Name);
    }

    [Theory]
    [InlineData("John Doe", 30, "Main St.", "New York", "NY", "Developer", "123-456-7890")]
    [InlineData("Jane Smith", 25, "Oak Ave.", "Los Angeles", "CA", "Designer", "098-765-4321")]
    public void Map_ShouldMapPropertiesCorrectly_WithTheory(
            string fullName,
            int age,
            string street,
            string city,
            string state,
            string occupation,
            string phoneNumber)
    {
        // Arrange
        var sourcePerson = new SourcePerson
        {
            FullName = fullName,
            Age = age,
            Street = street,
            Ocuppation = occupation
        };

        var target = sourcePerson.MapTo<TargetPerson>();

        Assert.NotNull(target);
        Assert.Equal(sourcePerson.FullName, target.FullName);
        Assert.Equal(sourcePerson.Ocuppation, target.JobTitle);
    }
}
