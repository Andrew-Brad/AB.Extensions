using Xunit;

namespace AB.Extensions.Tests;

public class IEnumerableTests
{
    [Fact]
    public void Shuffle_Empty_List_Returns_Empty()
    {
        IList<int> noNumbers = new List<int>();
        noNumbers.Shuffle();
        Assert.Empty(noNumbers);
    }

    // A shuffle is a permutation: same elements, reordered. Asserting that (not "order changed") keeps it deterministic.
    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 458)]
    [InlineData(-10, 10)]
    [InlineData(short.MinValue, short.MaxValue)]
    [InlineData(byte.MinValue, byte.MaxValue)]
    //[InlineData(0, 2147483647)] // huge ranges OOM — unrelated to the logic
    public void Shuffle_PreservesAllElements(int startNumber, int totalNumbers)
    {
        IList<int> original = Enumerable.Range(startNumber, totalNumbers).ToList();
        IList<int> shuffled = Enumerable.Range(startNumber, totalNumbers).ToList();

        shuffled.Shuffle();

        Assert.Equal(original.OrderBy(x => x), shuffled.OrderBy(x => x));
    }

    // Seeded Random makes the shuffle reproducible; across 500 elements "unchanged" is effectively impossible.
    [Fact]
    public void Shuffle_WithSeededRandom_IsReproducibleAndReorders()
    {
        IList<int> original = Enumerable.Range(1, 500).ToList();
        IList<int> first = Enumerable.Range(1, 500).ToList();
        IList<int> second = Enumerable.Range(1, 500).ToList();

        first.Shuffle(new Random(12345));
        second.Shuffle(new Random(12345));

        Assert.Equal(first, second);                                   // same seed → same permutation
        Assert.NotEqual(original, first);                              // it reordered
        Assert.Equal(original.OrderBy(x => x), first.OrderBy(x => x)); // still a permutation
    }

    [Fact]
    public void Shuffle_SingleElement_IsUnchanged()
    {
        IList<int> numbers = new List<int> { 42 };

        numbers.Shuffle();

        Assert.Equal(new[] { 42 }, numbers);
    }

    [Fact]
    public void TakeUntil_Subset_Valid_List()
    {
        IList<int> numbers = Enumerable.Range(1, 10).ToList();

        var result = numbers.TakeUntil(x => x == 3).ToList();

        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(1000)]
    [InlineData(short.MaxValue)]
    public void TakeUntil_All_Elements_Valid_List(int count)
    {
        IList<int> numbers = Enumerable.Range(1, count).ToList();

        var result = numbers.TakeUntil(x => x == numbers.Count).ToList();

        Assert.Equal(Enumerable.Range(1, count), result);
    }

    [Theory]
    [InlineData(new int[] { 0, 0 }, true)]
    [InlineData(new int[] { 0, 1 }, true)]
    [InlineData(new int[] { 1, 1 }, true)]
    [InlineData(new int[] { 2, 1 }, false)]
    [InlineData(new int[] { -1, 1 }, true)]
    [InlineData(new int[] { -9, -6, -1, 0, 1 }, true)]
    [InlineData(new int[] { -9, -10, -1, 0, 1 }, false)]
    public void Is_Monotonically_Increasing_Get_Enumerator(int[] inputList, bool isIncreasingAssert) =>
        Assert.Equal(isIncreasingAssert, inputList.IsMonotonicallyIncreasing());

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
        Assert.Equal(new[] { "Cy", "Bob", "Amy" }, result); // age asc, name desc within age 30
    }

    [Fact]
    public void ThenBy_Ascending_AppliesSecondarySort()
    {
        var result = People.OrderBy(p => p.Age, OrderByDirection.Ascending)
                           .ThenBy(p => p.Name, Comparer<string>.Default, OrderByDirection.Ascending)
                           .Select(p => p.Name);
        Assert.Equal(new[] { "Cy", "Amy", "Bob" }, result); // age asc, name asc within age 30
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

    // --- TakeUntil null guards ---

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
