﻿using System;
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
    }
}
