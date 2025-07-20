using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HomeScreen.Service.Media.Common.Extensions;

public static class AsyncEnumerableExceptByExtensions
{
    private static ActivitySource ActivitySource => new(nameof(AsyncEnumerableExceptByExtensions));

    public static IAsyncEnumerable<TSource> ExceptBy<TSource, TKey>(
        this IAsyncEnumerable<TSource> first,
        IAsyncEnumerable<TKey> second,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = ActivitySource.StartActivity();
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(keySelector);

        return ExceptByIterator(first, second, keySelector, comparer, cancellationToken);
    }

    private static async IAsyncEnumerable<TSource> ExceptByIterator<TSource, TKey>(
        IAsyncEnumerable<TSource> first,
        IAsyncEnumerable<TKey> second,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        using var activity = ActivitySource.StartActivity();
        var set = new HashSet<TKey>(await second.ToListAsync(cancellationToken).ConfigureAwait(false), comparer);

        await foreach (var element in first.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            activity?.AddEvent(new ActivityEvent());
            if (!set.Add(keySelector(element))) continue;
            yield return element;
        }
    }
}
