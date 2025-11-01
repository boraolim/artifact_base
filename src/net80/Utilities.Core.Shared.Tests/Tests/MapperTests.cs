namespace Utilities.Core.Shared.Tests;

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
    public void MapTo_ShouldConvertDictionary_WithRealData()
    {
        // Arrange: Diccionario fuente
        var source = new SourceWithDict
        {
            Data = new Dictionary<string, int>
            {
                { "A", 10 },
                { "B", 20 },
                { "C", 30 }
            }
        };

        // Act: Mapear al tipo destino
        var result = source.MapTo<TargetWithDict>();

        // Assert: Validar que se mapeó correctamente
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Count);

        Assert.Equal(10L, result.Data["A"]);
        Assert.Equal(20L, result.Data["B"]);
        Assert.Equal(30L, result.Data["C"]);
    }

    [Fact]
    public void MapTo_ShouldHandleEmptyDictionary()
    {
        // Arrange
        var source = new SourceWithDict
        {
            Data = new Dictionary<string, int>()
        };

        // Act
        var result = source.MapTo<TargetWithDict>();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact]
    public void MapTo_ShouldHandleNullDictionary()
    {
        // Arrange
        var source = new SourceWithDict { Data = null };

        // Act
        var result = source.MapTo<TargetWithDict>();

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Data);
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
    public void GetSourceExpression_Should_Handle_Properties_Fields_And_Others_Reflection()
    {
        // Obtener el método por Reflection
        var method = typeof(MemberBindingHelper)
            .GetMethod("GetSourceExpression", BindingFlags.Public | BindingFlags.Static);
        Assert.NotNull(method);

        // Parámetros Expression
        var sourceTypedProp = Expression.Parameter(typeof(SourceWithProperty), "source");
        var sourceTypedField = Expression.Parameter(typeof(SourceWithField), "source");
        var sourceTypedMethod = Expression.Parameter(typeof(SourceWithMethod), "source");

        // 1) Propiedad: "Name"
        var exprProp = method.Invoke(null, new object[]
        {
        typeof(SourceWithProperty),
        sourceTypedProp,
        "Name",
        typeof(string)
        }) as Expression;

        Assert.NotNull(exprProp);
        Assert.Equal(typeof(string), exprProp!.Type);
        Assert.IsAssignableFrom<MemberExpression>(exprProp);

        // 2) Campo: "Age"
        var exprField = method.Invoke(null, new object[]
        {
        typeof(SourceWithField),
        sourceTypedField,
        "Age",
        typeof(int)
        }) as Expression;

        Assert.NotNull(exprField);
        Assert.Equal(typeof(int), exprField!.Type);
        Assert.IsAssignableFrom<MemberExpression>(exprField);

        // 3) Miembro que no es propiedad ni campo: "DoSomething" (método)
        var ex = Assert.Throws<TargetInvocationException>(() =>
            method.Invoke(null, new object[]
            {
            typeof(SourceWithMethod),
            sourceTypedMethod,
            "DoSomething",
            typeof(void)
            })
        );

        // Verificamos que la excepción interna sea la que lanzamos en GetSourceExpression
        Assert.IsType<InvalidOperationException>(ex.InnerException);
        Assert.Contains("No se encontró un miembro público legible", ex.InnerException!.Message);
    }


    [Fact]
    public void GetGenericMethod_Should_Return_Correct_MethodInfo()
    {
        // Obtener el método privado mediante Reflection
        var methodInfo = typeof(TypeConversionHelper)
            .GetMethod("GetGenericMethod", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(methodInfo);

        // 1️⃣ Obtener solo la definición genérica (sin construir)
        var genericDefinition = methodInfo.Invoke(null, new object[]
        {
            typeof(Enumerable),   // type
            "Select",             // methodName
            2,                    // genericArgCount
            null                  // typeArgs = null para mantener la definición
        }) as MethodInfo;

        Assert.NotNull(genericDefinition);
        Assert.Equal("Select", genericDefinition!.Name);
        Assert.True(genericDefinition.IsGenericMethodDefinition);
        Assert.False(genericDefinition.IsConstructedGenericMethod);
        Assert.Equal(2, genericDefinition.GetGenericArguments().Length);

        // 2️⃣ Construir el método genérico con tipos concretos
        var constructedMethod = genericDefinition.MakeGenericMethod(typeof(int), typeof(string));

        Assert.True(constructedMethod.IsConstructedGenericMethod);
        Assert.False(constructedMethod.IsGenericMethodDefinition);
        Assert.Equal(2, constructedMethod.GetGenericArguments().Length);
        Assert.Equal(typeof(int), constructedMethod.GetGenericArguments()[0]);
        Assert.Equal(typeof(string), constructedMethod.GetGenericArguments()[1]);
    }

    [Fact]
    public void GetMemberType_Should_Return_Type_For_PropertyInfo()
    {
        // Arrange
        var method = typeof(MemberBindingHelper)
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
        var method = typeof(MemberBindingHelper)
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
        var method = typeof(MemberBindingHelper)
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
        var method = typeof(MemberBindingHelper)
            .GetMethod("GetMemberType", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var methodInfo = typeof(TestClass).GetMethod(nameof(TestClassForProp.SomeMethod));

        var ex = Assert.Throws<TargetInvocationException>(() =>
            method.Invoke(null, new object[] { methodInfo }));

        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }

    [Theory]
    [InlineData("John Doe", 30, "Main St.", "New York", "NY", "Developer", "123-456-7890")]
    [InlineData("Jane Smith", 25, "Oak Ave.", "Los Angeles", "CA", "Designer", "098-765-4321")]
    public void Map_ShouldMapPropertiesCorrectly_WithTheory(string fullName,
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
            City = city,
            State = state,
            Ocuppation = occupation,
            PhoneNumber = phoneNumber
        };

        var target = sourcePerson.MapTo<TargetPerson>();

        Assert.NotNull(target);
        Assert.Equal(sourcePerson.FullName, target.FullName);
        Assert.Equal(sourcePerson.Age, target.Age);
        Assert.Equal(sourcePerson.Street, target.Street);
        Assert.Equal(sourcePerson.City, target.City);
        Assert.Equal(sourcePerson.State, target.State);
        Assert.Equal(sourcePerson.Ocuppation, target.JobTitle);
        Assert.Equal(sourcePerson.PhoneNumber, target.PhoneNumber);
    }

    [Fact]
    public void ConvertCollection_Should_Convert_Int_To_Long_List_Using_Reflection()
    {
        // Arrange
        var method = typeof(TypeConversionHelper)
            .GetMethod("ConvertCollection", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var list = new List<int> { 1, 2, 3 };
        var sourceExpr = Expression.Constant(list);

        // Act
        var result = method.Invoke(null, new object?[] { sourceExpr, typeof(int), typeof(long) }) as Expression;
        Assert.NotNull(result);

        // Ajuste: puede haber un Convert envolviendo ToList
        MethodCallExpression toListCall;
        if(result is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
        {
            toListCall = Assert.IsAssignableFrom<MethodCallExpression>(unary.Operand);
        }
        else
        {
            toListCall = Assert.IsAssignableFrom<MethodCallExpression>(result);
        }

        Assert.Equal("ToList", toListCall.Method.Name);

        var selectCall = Assert.IsAssignableFrom<MethodCallExpression>(toListCall.Arguments[0]);
        Assert.Equal("Select", selectCall.Method.Name);

        var lambda = Assert.IsAssignableFrom<LambdaExpression>(selectCall.Arguments[1]);
        Assert.Equal(typeof(long), lambda.Body.Type);

        var param = Assert.IsAssignableFrom<ParameterExpression>(lambda.Parameters[0]);
        Assert.Equal(typeof(int), param.Type);
        Assert.Equal("x", param.Name);
    }

    public class SourceItem { public int Value { get; set; } }
    public class TargetItem { public long Value { get; set; } }


    [Fact]
    public void ConvertCollection_Should_Return_Source_When_Types_Are_Assignable_Reflection()
    {
        // Obtener el método privado por reflection
        var method = typeof(TypeConversionHelper)
            .GetMethod("ConvertCollection", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var source = Expression.Parameter(typeof(List<string>), "source");

        // Act
        var result = method.Invoke(null, new object?[] { source, typeof(string), typeof(IEnumerable<string>) }) as Expression;

        // Assert
        Assert.Equal(source, result); // Retorna la misma expresión
    }

    [Fact]
    public void ConvertCollection_Should_Convert_Int_To_Long_List_Reflection()
    {
        // Obtener el método privado
        var method = typeof(TypeConversionHelper)
            .GetMethod("ConvertCollection", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var source = Expression.Parameter(typeof(List<int>), "source");

        // Act
        var result = method.Invoke(null, new object?[] { source, typeof(int), typeof(long) }) as Expression;
        Assert.NotNull(result);

        // Manejar posible Convert envolviendo ToList
        MethodCallExpression toListCall = result is UnaryExpression unary && unary.NodeType == ExpressionType.Convert
            ? Assert.IsAssignableFrom<MethodCallExpression>(unary.Operand)
            : Assert.IsAssignableFrom<MethodCallExpression>(result);

        Assert.Equal("ToList", toListCall.Method.Name);

        var selectCall = Assert.IsAssignableFrom<MethodCallExpression>(toListCall.Arguments[0]);
        Assert.Equal("Select", selectCall.Method.Name);

        var lambda = Assert.IsAssignableFrom<LambdaExpression>(selectCall.Arguments[1]);
        var param = Assert.IsAssignableFrom<ParameterExpression>(lambda.Parameters[0]);
        Assert.Equal(typeof(int), param.Type);
        Assert.Equal("x", param.Name);

        Assert.Equal(typeof(long), lambda.Body.Type);
    }

    [Fact]
    public void ConvertCollection_Should_Handle_Object_Conversion_Reflection()
    {
        // Obtener el método privado
        var method = typeof(TypeConversionHelper)
            .GetMethod("ConvertCollection", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var sourceExpr = Expression.Parameter(typeof(List<SourceItem>), "source");

        // Act
        object?[] parameters = new object?[] { sourceExpr, typeof(SourceItem), typeof(TargetItem) };

        var result = method.Invoke(null, parameters) as Expression;
        Assert.NotNull(result);

        // Manejar posible conversión de tipos (UnaryExpression)
        MethodCallExpression toListCall;
        if(result is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            toListCall = Assert.IsAssignableFrom<MethodCallExpression>(unary.Operand);
        else
            toListCall = Assert.IsAssignableFrom<MethodCallExpression>(result);

        // Verificar ToList externo
        Assert.Equal("ToList", toListCall.Method.Name);

        // Verificar Select interno
        var selectCall = Assert.IsAssignableFrom<MethodCallExpression>(toListCall.Arguments[0]);
        Assert.Equal("Select", selectCall.Method.Name);

        // Verificar lambda
        var lambda = Assert.IsAssignableFrom<LambdaExpression>(selectCall.Arguments[1]);
        var param = Assert.IsAssignableFrom<ParameterExpression>(lambda.Parameters[0]);
        Assert.Equal(typeof(SourceItem), param.Type);
        Assert.Equal(typeof(TargetItem), lambda.Body.Type);
    }


    [Fact]
    public void ConvertCollection_Should_Handle_Empty_List_Expression_Reflection()
    {
        // Obtener el método privado
        var method = typeof(TypeConversionHelper)
            .GetMethod("ConvertCollection", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var source = Expression.Parameter(typeof(List<int>), "source");

        // Act
        var result = method.Invoke(null, new object?[] { source, typeof(int), typeof(int) }) as Expression;

        // Assert
        Assert.Equal(source, result);
    }


    public class SourceClass { public int Value { get; set; } }
    public class TargetClass { public long Value { get; set; } }

    [Fact]
    public void Should_Return_Source_When_Types_Are_Assignable_Reflection()
    {
        var method = typeof(TypeConversionHelper)
            .GetMethod("ConvertExpressionNonNull", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var param = Expression.Parameter(typeof(string), "source");

        var result = method.Invoke(null, new object?[] { param, typeof(object) }) as Expression;

        Assert.NotNull(result);
        Assert.Equal(param, result);
    }

    [Fact]
    public void Should_Convert_Collection_Types_Reflection()
    {
        var method = typeof(TypeConversionHelper)
            .GetMethod("ConvertExpressionNonNull", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var source = Expression.Parameter(typeof(List<int>), "source");

        var result = method.Invoke(null, new object?[] { source, typeof(List<long>) }) as Expression;
        Assert.NotNull(result);

        var outerCall = Assert.IsAssignableFrom<MethodCallExpression>(result);
        Assert.Equal("ToList", outerCall.Method.Name);

        var selectCall = Assert.IsAssignableFrom<MethodCallExpression>(outerCall.Arguments[0]);
        Assert.Equal("Select", selectCall.Method.Name);
    }

    [Fact]
    public void Should_Convert_Complex_Class_Types_Reflection()
    {
        var method = typeof(TypeConversionHelper)
            .GetMethod("ConvertExpressionNonNull", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var source = Expression.Parameter(typeof(SourceClass), "source");

        var result = method.Invoke(null, new object?[] { source, typeof(TargetClass) }) as Expression;
        Assert.NotNull(result);

        var call = Assert.IsAssignableFrom<MethodCallExpression>(result);
        Assert.Equal("MapTo", call.Method.Name);
    }

    [Fact]
    public void Should_Convert_Simple_Types_Reflection()
    {
        var method = typeof(TypeConversionHelper)
            .GetMethod("ConvertExpressionNonNull", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var source = Expression.Parameter(typeof(int), "source");

        var result = method.Invoke(null, new object?[] { source, typeof(long) }) as Expression;
        Assert.NotNull(result);

        var unary = Assert.IsAssignableFrom<UnaryExpression>(result);
        Assert.Equal(ExpressionType.Convert, unary.NodeType);
    }
}
