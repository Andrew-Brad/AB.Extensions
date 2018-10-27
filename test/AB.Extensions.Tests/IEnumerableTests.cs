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
            Assert.Equal(0, noNumbers.Count);
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

        [Fact]
        public void TakeUntil_Subset_Valid_List()
        {
            //Arrange
            IList<int> numbers = Enumerable.Range(1, 10).ToList();
            IList<int> expected = Enumerable.Range(1, 3).ToList();
            //Act
            var assertionList = numbers.TakeUntil(x => x == 3).ToList();
            //Assert
            Assert.True(Enumerable.SequenceEqual(assertionList, expected));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(1000)]
        [InlineData(short.MaxValue)]
        public void TakeUntil_All_Elements_Valid_List(int count)
        {
            //Arrange
            IList<int> numbers = Enumerable.Range(1, count).ToList();
            IList<int> expected = Enumerable.Range(1, count).ToList();
            //Act
            var assertionList = numbers.TakeUntil(x => x == numbers.Count).ToList();
            //Assert
            Assert.True(Enumerable.SequenceEqual(assertionList, expected));
        }

        [Theory]
        [InlineData(new int[] { 0, 0 }, true)]
        [InlineData(new int[] { 0, 1 }, true)]
        [InlineData(new int[] { 1, 1 }, true)]
        [InlineData(new int[] { 2, 1 }, false)]
        [InlineData(new int[] { -1, 1 }, true)]
        [InlineData(new int[] { -9, -6, -1, 0, 1 }, true)]
        [InlineData(new int[] { -9, -10, -1, 0, 1 }, false)]
        public void Is_Monotonically_Increasing_Get_Enumerator(int[] inputList, bool isIncreasingAssert)
        {
            //Arrange

            //Act
            bool isIncreasing = inputList.IsMonotonicallyIncreasing();
            //Assert
            Assert.Equal(isIncreasingAssert, isIncreasing);
        }
    }
}
