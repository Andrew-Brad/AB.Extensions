using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AB.Extensions.Tests
{
    public class IEnumerableTests
    {
        [Fact]
        public void Shuffle_Empty_List_Returns_Empty()
        {
            //Arrange
            IList<int> noNumbers = new List<int>();
            //Act
            noNumbers.Shuffle();
            //Assert
            Assert.Equal(noNumbers.Count,0);
        }

        [Theory]
        [InlineData(1,5)]
        [InlineData(2,458)]
        [InlineData(-10,10)]
        [InlineData(short.MinValue, short.MaxValue)]//int values throw OutOfMemoryExceptions
        public void Shuffle_Valid_List_Does_Shuffle(int startNumber,int totalNumbers)
        {
            //Arrange
            IList<int> orderedNumbers = Enumerable.Range(startNumber,totalNumbers).ToList();
            IList<int> shuffledNumbers = Enumerable.Range(startNumber, totalNumbers).ToList();
            //Act
            shuffledNumbers.Shuffle();
            //Assert
            Assert.False(orderedNumbers.SequenceEqual(shuffledNumbers));
        }
    }
}
