using Xunit;

namespace AB.Extensions.Tests
{
    public class IntExtensionsTests
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(987654321, 9)]
        [InlineData(-10, 1)]
        [InlineData(int.MaxValue, 2)]
        [InlineData(-1, 1)]
        [InlineData(0,0)]
        [InlineData(int.MinValue, 2)]
        [InlineData(1000000000, 1)]
        public void Int_Get_First_Digit(int givenNumber, int expectedFirstDigit)
        {
            //Arrange

            //Act
            int firstDigit = givenNumber.LeadingDigit();

            //Assert
            Assert.Equal(expectedFirstDigit, firstDigit);
        }

    }
}
