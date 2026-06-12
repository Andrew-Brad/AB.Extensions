using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

namespace AB.Extensions.Tests;

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
        Assert.Empty(noNumbers);
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 458)]
    [InlineData(-10, 10)]
    [InlineData(short.MinValue, short.MaxValue)]
    [InlineData(byte.MinValue, byte.MaxValue)]
    //[InlineData(0, 147483647)]
    //[InlineData(0, 2147483647)] // certain high int values throw OutOfMemoryExceptions (unrelated to logic)
    public void Shuffle_Valid_List_Does_Shuffle(int startNumber, int totalNumbers)
    {
        //Arrange
        IList<int> orderedNumbers = Enumerable.Range(startNumber, totalNumbers).ToList();
        IList<int> shuffledNumbers = Enumerable.Range(startNumber, totalNumbers).ToList();

        //Act
        shuffledNumbers.Shuffle();

        //Assert
        Assert.False(orderedNumbers.SequenceEqual(shuffledNumbers));
    }

    [Fact]
    public void Shuffle_Valid_List_Handles_1_Element()
    {
        //Arrange
        IList<int> orderedNumbers = Enumerable.Range(1, 1).ToList();
        IList<int> shuffledNumbers = Enumerable.Range(1, 1).ToList();

        //Act
        shuffledNumbers.Shuffle();

        //Assert
        Assert.True(orderedNumbers.SequenceEqual(shuffledNumbers));
    }

    [Fact]
    public void Shuffle_Valid_List_Handles_0_Elements()
    {
        //Arrange
        IList<int> orderedNumbers = new List<int>();
        IList<int> shuffledNumbers = new List<int>();

        //Act
        shuffledNumbers.Shuffle();

        //Assert
        Assert.True(orderedNumbers.SequenceEqual(shuffledNumbers));
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

    // --- OrderBy(direction) ---

    [Fact]
    public void OrderBy_Ascending_SortsAscending() =>
        Assert.Equal(new[] { 1, 2, 3 }, new[] { 3, 1, 2 }.OrderBy(x => x, OrderByDirection.Ascending));

    [Fact]
    public void OrderBy_Descending_SortsDescending() =>
        Assert.Equal(new[] { 3, 2, 1 }, new[] { 3, 1, 2 }.OrderBy(x => x, OrderByDirection.Descending));

    [Fact]
    public void OrderBy_WithExplicitComparer_Descending()
    {
        string[] words = { "bb", "a", "ccc" };
        // order by length, descending, via the comparer overload
        var result = words.OrderBy(w => w.Length, Comparer<int>.Default, OrderByDirection.Descending);
        Assert.Equal(new[] { "ccc", "bb", "a" }, result);
    }

    // --- ThenBy(direction) ---

    private static readonly (string Name, int Age)[] People =
    {
        ("Bob", 30), ("Amy", 30), ("Cy", 20),
    };

    [Fact]
    public void ThenBy_Descending_AppliesSecondarySort()
    {
        var result = People.OrderBy(p => p.Age, OrderByDirection.Ascending)
                           .ThenBy(p => p.Name, OrderByDirection.Descending)
                           .Select(p => p.Name);
        Assert.Equal(new[] { "Cy", "Bob", "Amy" }, result); // age asc, then name desc within age 30
    }

    [Fact]
    public void ThenBy_Ascending_AppliesSecondarySort()
    {
        var result = People.OrderBy(p => p.Age, OrderByDirection.Ascending)
                           .ThenBy(p => p.Name, Comparer<string>.Default, OrderByDirection.Ascending)
                           .Select(p => p.Name);
        Assert.Equal(new[] { "Cy", "Amy", "Bob" }, result); // age asc, then name asc within age 30
    }

    // --- SkipUntil ---

    [Fact]
    public void SkipUntil_SkipsThroughAndIncludingMatch()
    {
        var result = Enumerable.Range(1, 5).SkipUntil(x => x == 3).ToList();
        Assert.Equal(new[] { 4, 5 }, result);
    }

    [Fact]
    public void SkipUntil_NoMatch_SkipsEverything()
    {
        var result = Enumerable.Range(1, 5).SkipUntil(x => x == 99).ToList();
        Assert.Empty(result);
    }

    [Fact]
    public void SkipUntil_NullSource_Throws() =>
        Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null!).SkipUntil(x => true));

    [Fact]
    public void SkipUntil_NullPredicate_Throws() =>
        Assert.Throws<ArgumentNullException>(() => Enumerable.Range(1, 3).SkipUntil(null!));

    // --- TakeUntil null guards (happy path covered above) ---

    [Fact]
    public void TakeUntil_NullSource_Throws() =>
        Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null!).TakeUntil(x => true));

    [Fact]
    public void TakeUntil_NullPredicate_Throws() =>
        Assert.Throws<ArgumentNullException>(() => Enumerable.Range(1, 3).TakeUntil(null!));

    // --- ForEach ---

    [Fact]
    public void ForEach_InvokesActionForEachElement()
    {
        var collected = new List<int>();
        new[] { 1, 2, 3 }.ForEach(collected.Add);
        Assert.Equal(new[] { 1, 2, 3 }, collected);
    }

    [Fact]
    public void ForEach_EmptySequence_DoesNothing()
    {
        int count = 0;
        Array.Empty<int>().ForEach(_ => count++);
        Assert.Equal(0, count);
    }
}
