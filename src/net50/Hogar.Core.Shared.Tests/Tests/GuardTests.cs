using System;
using System.Linq;
using System.Collections.Generic;

using Xunit;

using Utilities.Core.Shared.Extensions;

namespace Utilities.Core.Shared.Tests
{
    public class GuardTests
    {
        // AgainstNull<T>
        [Fact]
        public void AgainstNull_ShouldReturnValue_WhenNotNull()
        {
            var obj = new object();
            var result = Guard.AgainstNull(obj, nameof(obj));
            Assert.Equal(obj, result);
        }

        [Fact]
        public void AgainstNull_ShouldThrow_WhenNull()
        {
            object obj = null!;
            var ex = Assert.Throws<ArgumentNullException>(() => Guard.AgainstNull(obj, "obj"));
            Assert.Equal("obj", ex.ParamName);
        }

        // AgainstNullOrEmpty (string)
        [Fact]
        public void AgainstNullOrEmpty_String_ShouldReturnValue_WhenValid()
        {
            var str = "test";
            var result = Guard.AgainstNullOrEmpty(str, nameof(str));
            Assert.Equal(str, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void AgainstNullOrEmpty_String_ShouldThrow_WhenNullOrEmpty(string? value)
        {
            var ex = Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrEmpty(value!, "str"));
            Assert.Equal("str", ex.ParamName);
            Assert.Contains("cannot be null or empty", ex.Message);
        }

        // AgainstNullOrEmpty (IEnumerable)
        [Fact]
        public void AgainstNullOrEmpty_Enumerable_ShouldReturn_WhenValid()
        {
            var list = new List<int> { 1, 2, 3 };
            var result = Guard.AgainstNullOrEmpty(list, nameof(list));
            Assert.Equal(list, result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AgainstNullOrEmpty_Enumerable_ShouldThrow_WhenInvalid(bool useNull)
        {
            IEnumerable<int>? input = useNull ? null : Enumerable.Empty<int>();

            var ex = Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrEmpty(input!, "input"));
            Assert.Equal("input", ex.ParamName);
            Assert.Contains("cannot be null or empty", ex.Message);
        }

        // AgainstDefault<T>
        [Fact]
        public void AgainstDefault_ShouldReturnValue_WhenValid()
        {
            var value = 10;
            var result = Guard.AgainstDefault(value, nameof(value));
            Assert.Equal(10, result);
        }

        [Fact]
        public void AgainstDefault_ShouldThrow_WhenDefault()
        {
            var ex = Assert.Throws<ArgumentException>(() => Guard.AgainstDefault(0, "value"));
            Assert.Equal("value", ex.ParamName);
            Assert.Contains("cannot be default value", ex.Message);
        }

        // Against<T>
        [Fact]
        public void Against_ShouldReturn_WhenPredicateIsFalse()
        {
            var value = 5;
            var result = Guard.Against(value, x => x < 0, "Must not be negative", nameof(value));
            Assert.Equal(value, result);
        }

        [Fact]
        public void Against_ShouldThrow_WhenPredicateIsTrue()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                Guard.Against(-5, x => x < 0, "Must not be negative", "value"));

            Assert.Equal("value", ex.ParamName);
            Assert.Contains("Must not be negative", ex.Message);
        }

        // NegativeOrZero
        [Fact]
        public void NegativeOrZero_ShouldReturn_WhenPositive()
        {
            var result = Guard.NegativeOrZero(1, "value");
            Assert.Equal(1, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void NegativeOrZero_ShouldThrow_WhenZeroOrNegative(int value)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Guard.NegativeOrZero(value, "value"));
            Assert.Equal("value", ex.ParamName);
            Assert.Contains("must be greater than zero", ex.Message);
        }
    }
}
