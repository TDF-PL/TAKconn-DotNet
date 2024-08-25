using System.Linq.Expressions;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;

namespace WOT.TAK.Connection.TestCommon;

public static class FluentAssertionExtensions
{
    public static async Task<AndWhichConstraint<TAssertions, T>> EventuallyContain<TCollection, T, TAssertions>(
        this GenericCollectionAssertions<TCollection, T, TAssertions> assertion,
        Expression<Func<T, bool>> predicate,
        TimeSpan timeout,
        string because = "",
        params object[] becauseArgs)
        where TCollection : IEnumerable<T>
        where TAssertions : GenericCollectionAssertions<TCollection, T, TAssertions>
    {
        var func = predicate.Compile();

        var totalWaitTime = TimeSpan.Zero;
        AssertionScope scope;
        do
        {
            var condition = assertion.Subject.Any(func);
            scope = Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(condition);

            if (condition)
            {
                break;
            }

            const int poolingFrequency = 100;
            totalWaitTime += TimeSpan.FromMilliseconds(poolingFrequency);
            await Task.Delay(poolingFrequency).ConfigureAwait(false);
        } while (totalWaitTime < timeout);

        scope.FailWith(
            "Expected {context:collection} {0} to have an item matching {1}{reason}.",
            assertion.Subject,
            predicate.Body);

        var matches = assertion.Subject.Where(func);

        return new AndWhichConstraint<TAssertions, T>((TAssertions)assertion, matches);
    }
}