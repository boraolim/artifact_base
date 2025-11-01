using System;
using System.Linq;
using System.Collections.Generic;

using Xunit;

using Utilities.Core.Shared.Extensions;
using Utilities.Core.Shared.Tests.Models;

namespace Utilities.Core.Shared.Tests
{
    public class ObjectExtensionTests
    {
        [Fact]
        public void ConvertTo_Should_Convert_List_Correctly()
        {
            var input = new List<int> { 1, 2, 3 };
            var result = input.ConvertTo(x => x.ToString());
            Assert.Equal(new List<string> { "1", "2", "3" }, result);
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("True", true)]
        [InlineData("1", true)]
        [InlineData("false", false)]
        [InlineData("0", false)]
        [InlineData("invalid", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void ToBooleanSafe_Should_Parse_Correctly(string input, bool expected)
        {
            bool result = input.ToBooleanSafe();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void OrdenarLista_Should_Sort_Correctly()
        {
            var data = new List<Dummy>
            {
                new Dummy { Id = 2, Name = "B" },
                new Dummy { Id = 1, Name = "A" },
                new Dummy { Id = 3, Name = "C" },
            };

            var resultAsc = data.OrdenarLista("Id", true);
            Assert.Equal(new[] { 1, 2, 3 }, resultAsc.Select(x => x.Id));

            var resultDesc = data.OrdenarLista("Id", false);
            Assert.Equal(new[] { 3, 2, 1 }, resultDesc.Select(x => x.Id));
        }

        [Fact]
        public void OrdenarLista_Should_Throw_On_Invalid_Property()
        {
            var data = new List<Dummy>();
            Assert.Throws<ArgumentException>(() => data.OrdenarLista("NonExistent", true));
        }
    }
}
