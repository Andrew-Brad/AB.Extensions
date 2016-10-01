using Xunit;
using AB.Extensions;
using System;
using static AB.Extensions.Common;

namespace ABExtensions.Tests
{
    public class StringTests
    {
        [Fact]
        public void ReverseString_1()
        {
            string input = "hi!";
            string expected = "!ih";
            string operation = input.ToReverseString();
            Assert.Equal(expected, operation);
        }

        [Fact]
        public void ReverseString_2()
        {
            string input = "weRdNab";
            string expected = "baNdRew";
            string operation = input.ToReverseString();
            Assert.Equal(expected, operation);
        }

        [Fact]
        public void ReverseString_3()
        {
            string input = "";
            string expected = string.Empty;
            string operation = input.ToReverseString();
            Assert.Equal(expected, operation);
        }

        [Fact]
        public void ReverseString_4()
        {
            string input = null;
            string expected = null;
            string operation = input.ToReverseString();
            Assert.Equal(expected, operation);
        }

        [Fact]
        public void String_ToGuid_Valid()
        {
            //Arrange
            string input = "d2650790-bc80-41a9-ae63-dd55e2240296";
            Guid desiredOutput = Guid.Parse("d2650790-bc80-41a9-ae63-dd55e2240296");
            //Act
            var result = input.ToGuid();
            //Assert
            Assert.Equal(desiredOutput, result);
        }

        [Fact]
        public void String_ToGuid_Invalid()
        {
            //Arrange
            string input = "d2sdddddddddddddddddddddddddddddddddddddd650790-bc80-41a9-ae63-dd55e2240296";
            Guid desiredOutput = Guid.Empty;
            //Act
            var result = input.ToGuid();
            //Assert
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public void String_ToGuid_Null()
        {
            //Arrange
            string input = null;
            Guid desiredOutput = Guid.Empty;
            //Act
            var result = input.ToGuid();
            //Assert
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public void String_To_Enum_Type_Of()
        {
            //Arrange
            string input = "Ascending";
            OrderByDirection desiredOutput = OrderByDirection.Ascending;
            //Act
            var result = input.ToEnumTypeOf<OrderByDirection>();
            //Assert
            Assert.Equal(desiredOutput, result);
        }

        [Fact]
        public void String_To_Enum_Type_Of_Invalid()
        {
            //Arrange
            string input = "lol";
            //Act

            //Assert
            Assert.Throws<ArgumentException>( () => input.ToEnumTypeOf<OrderByDirection>());
        }
    }
}
