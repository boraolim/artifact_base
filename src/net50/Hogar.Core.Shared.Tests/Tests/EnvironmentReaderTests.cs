using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Moq;
using Xunit;

using Utilities.Core.Shared.Services;

namespace Utilities.Core.Shared.Tests
{
    public class EnvironmentReaderTests
    {
        private const string TestKey = "TEST_ENV_VAR";
        private const string DefaultValue = "DefaultValue";
        private const string TestValue = "TestValue";

        [Fact]
        public void GetVariable_ShouldReturnSuccess_WhenVariableExists()
        {
            // Arrange
            const string key = "TEST_ENV_VAR";
            const string value = "TestValue";
            Environment.SetEnvironmentVariable(key, value);

            var reader = new EnvironmentReader();

            // Act
            var result = reader.GetVariable(key);

            // Assert
            Assert.True(result.Succeded);
            Assert.Equal(value, result.MessageDescription);
        }

        [Fact]
        public void GetVariable_WithDefault_ShouldReturnSuccess_WhenVariableExists()
        {
            // Arrange
            const string key = "TEST_ENV_VAR_2";
            const string value = "AnotherTestValue";
            Environment.SetEnvironmentVariable(key, value);

            var reader = new EnvironmentReader();

            // Act
            var result = reader.GetVariable(key, "DefaultValue");

            // Assert
            Assert.True(result.Succeded);
            Assert.Equal(value, result.Data);
        }

        [Fact]
        public void GetVariable_ShouldReturnFailure_WhenVariableDoesNotExist()
        {
            // Arrange
            const string key = "NON_EXISTENT_VAR";
            Environment.SetEnvironmentVariable(key, null); // Asegura que no exista
            var reader = new EnvironmentReader();

            // Act
            var result = reader.GetVariable(key);

            // Assert
            Assert.False(result.Succeded);
            Assert.Contains(key, result.Data);
        }

        [Fact]
        public void GetVariable_WithDefault_ShouldReturnFailure_WhenVariableDoesNotExist()
        {
            // Arrange
            const string key = "NON_EXISTENT_VAR_2";
            Environment.SetEnvironmentVariable(key, null);
            var reader = new EnvironmentReader();

            // Act
            var result = reader.GetVariable(key, "DefaultValue");

            // Assert
            Assert.True(result.Succeded);
            Assert.DoesNotContain(key, result.Data);
        }

        [Fact]
        public void GetVariable_Should_Return_Value_When_Exists()
        {
            // Arrange
            Environment.SetEnvironmentVariable(TestKey, TestValue);
            var reader = new EnvironmentReader();

            // Act
            var result = reader.GetVariable(TestKey);

            // Assert
            Assert.True(result.Succeded);
            Assert.Equal(TestValue, result.MessageDescription);
        }

        [Fact]
        public void GetVariable_Should_Return_Failure_When_Not_Found()
        {
            // Arrange
            Environment.SetEnvironmentVariable(TestKey, null);
            var reader = new EnvironmentReader();

            // Act
            var result = reader.GetVariable(TestKey);

            // Assert
            Assert.False(result.Succeded);
            Assert.Contains(TestKey, result.Data);
        }

        [Fact]
        public void GetVariable_With_DefaultValue_Should_Return_Value_When_Exists()
        {
            // Arrange
            Environment.SetEnvironmentVariable(TestKey, TestValue);
            var reader = new EnvironmentReader();

            // Act
            var result = reader.GetVariable(TestKey, DefaultValue);

            // Assert
            Assert.Equal(TestValue, result.Data);
        }

        [Fact]
        public void GetVariable_With_DefaultValue_Should_Return_Default_When_Not_Found()
        {
            // Arrange
            Environment.SetEnvironmentVariable(TestKey, null);
            var reader = new EnvironmentReader();

            // Act
            var result = reader.GetVariable(TestKey, DefaultValue);

            // Assert
            Assert.Equal(default, result.Data);
        }

        [Fact]
        public void GetVariable_Should_Use_Machine_Target_When_DefaultOS()
        {
            // Arrange
            Environment.SetEnvironmentVariable("TEST_KEY", "value");
            var reader = new EnvironmentReader();

            // Act
            var result = reader.GetVariable("TEST_KEY");

            // Assert
            Assert.True(result.Succeded);
            Assert.Equal("value", result.MessageDescription);
        }
    }
}
