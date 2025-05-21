using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagerApi.Attributes;

namespace TaskManagerApi.Tests.Validators
{
    public class FutureDateAttributeTests
    {
        private readonly FutureDateAttribute _attribute = new();

        [Fact]
        public void IsValid_WithFutureDate_ReturnsTrue()
        {
            var futureDate = DateTime.Now.AddDays(1);
            var result = _attribute.IsValid(futureDate);
            Assert.True(result);
        }

        [Fact]
        public void IsValid_WithPastDate_ReturnsFalse()
        {
            var pastDate = DateTime.Now.AddDays(-1);
            var result = _attribute.IsValid(pastDate);
            Assert.False(result);
        }

        [Fact]
        public void IsValid_WithNull_ReturnsFalse()
        {
            var result = _attribute.IsValid(null);
            Assert.False(result);
        }

        [Fact]
        public void IsValid_WithInvalidType_ReturnsFalse()
        {
            var result = _attribute.IsValid("not a date");
            Assert.False(result);
        }
    }
}