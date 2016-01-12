using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;

namespace Andamio
{
    /// <summary>
    /// Provides method extension for LINQ related functionality.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Filters a sequence of values based on a predicate based on a condition.
        /// </summary>
        /// <remarks>
        /// This method will perform the same operation as Queryable.Where if the specified condition is met;
        /// otherwise the entire source is returned and the operation is skipped.
        /// </remarks>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An IQueryable(T) to filter.</param>
        /// <param name="condition">The condition that determines whether the source will be filtered or not.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        /// If the condition is met then, an IQueryable(T) that contains elements from the input sequence that satisfy the condition specified by predicate; 
        /// otherwise the entire source.
        /// </returns>
        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate based on a condition.
        /// </summary>
        /// <remarks>
        /// This method will perform the same operation as Enumerable.Where if the specified condition is met;
        /// otherwise the entire source is returned and the operation is skipped.
        /// </remarks>        
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An IEnumerable(T) to filter.</param>
        /// <param name="condition">The condition that determines whether the source will be filtered or not.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        /// If the condition is met then, an IQueryable(T) that contains elements from the input sequence that satisfy the condition specified by predicate; 
        /// otherwise the entire source.
        /// </returns>
        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }

        /// <summary>
        /// Returns a specified number of elements from the sequence.
        /// </summary>
        /// <remarks>
        /// This method will perform the same operation as Queryable.Take if the specified condition is met;
        /// otherwise the entire source is returned and the operation is skipped.
        /// </remarks>         
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An IQueryable(T) containing the sequence.</param>
        /// <param name="condition">The condition that determines whether only the specified number of elements from the source will returned.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>An IQueryable(T) that contains the specified number of elements from the source.</returns>
        public static IQueryable<TSource> TakeIf<TSource>(this IQueryable<TSource> source, bool condition, int count)
        {
            return condition ? source.Take(count) : source;
        }

        /// <summary>
        /// Returns a specified number of elements from the sequence.
        /// </summary>
        /// <remarks>
        /// This method will perform the same operation as Enumerable.Take if the specified condition is met;
        /// otherwise the entire source is returned and the operation is skipped.
        /// </remarks>         
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An IEnumerable(T) containing the sequence.</param>
        /// <param name="condition">The condition that determines whether only the specified number of elements from the source will returned.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>An IEnumerable(T) that contains the specified number of elements from the source.</returns>
        public static IEnumerable<TSource> TakeIf<TSource>(this IEnumerable<TSource> source, bool condition, int count)
        {
            return condition ? source.Take(count) : source;
        }

        /// <summary>
        /// Performs a left-join operation on the two specified sequences.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to left-join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
        /// <returns>An IQueryable(T) that has elements of type TResult obtained by performing an left-join on two sequences.</returns>
        public static IQueryable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>
        (
            this IQueryable<TOuter> outer,
            IEnumerable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, TInner, TResult>> resultSelector
        )
        {
            var leftJoinQuery = outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector).DefaultIfEmpty();
            return leftJoinQuery;
        }

        /// <summary>
        /// Performs a left-join operation on the two specified sequences.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to left-join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
        /// <returns>An IEnumerable(T) that has elements of type TResult obtained by performing an left-join on two sequences.</returns>
        public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>
        (
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector
        )
        {
            var leftJoinQuery = outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector).DefaultIfEmpty();
            return leftJoinQuery;
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate based on a condition.
        /// </summary>
        /// <remarks>
        /// This method will perform the same operation as Queryable.Where if the specified condition is met;
        /// otherwise the entire source is returned and the operation is skipped.
        /// </remarks>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An IQueryable(T) to filter.</param>
        /// <param name="condition">The condition that determines whether the source will be filtered or not.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        /// If the condition is met then, an IQueryable(T) that contains elements from the input sequence that satisfy the condition specified by predicate; 
        /// otherwise the entire source.
        /// </returns>
        public static IQueryable<TSource> OrderByIf<TSource, TKey>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, TKey>> sorting)
        {
            return condition ? source.OrderBy(sorting) : source;
        }

        public static bool ContainsAny<T>(this IEnumerable<T> leftList, IEnumerable<T> rightList)
        {
            foreach (T rightItem in rightList)
            {
                if (leftList.Contains(rightItem))
                { return true; }
            }
            return false;
        }

        public static bool ContainsAny<T>(this IEnumerable<T> leftList, IEnumerable<T> rightList, IEqualityComparer<T> comparer)
        {
            foreach (T rightItem in rightList)
            {
                if (leftList.Contains(rightItem, comparer))
                { return true; }
            }
            return false;
        }

        public static bool ContainsAll<T>(this IEnumerable<T> leftList, IEnumerable<T> rightList)
        {
            foreach (T rightItem in rightList)
            {
                if (!leftList.Contains(rightItem))
                { return false; }
            }
            return true;
        }

        public static bool ContainsAll<T>(this IEnumerable<T> leftList, IEnumerable<T> rightList, IEqualityComparer<T> comparer)
        {
            foreach (T rightItem in rightList)
            {
                if (!leftList.Contains(rightItem, comparer))
                { return false; }
            }
            return true;
        }

        public static void RemoveAll<T>(this IList<T> source, IEnumerable<T> items)
        {
            if (source == null || !source.Any())
            { return; }

            if (items != null && items.Any())
            {
                foreach (T item in items)
                {
                    source.Remove(item);
                }
            }
        }

        public static void RemoveAll<T>(this IList<T> source, IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            if (source == null || !source.Any())
            { return; }

            if (items != null && items.Any())
            {
                foreach (T item in items)
                {
                    source.RemoveAll(source.Where(sourceItem => comparer.Equals(sourceItem, item)).ToArray());
                }
            }
        }

        public static void RemoveAll<T>(this IList<T> source, Func<T, bool> predicate)
        {
            if (source == null || !source.Any())
            { return; }

            var deletedItems = source.Where(predicate).ToArray();
            if (deletedItems != null && deletedItems.Any())
            {
                source.RemoveAll(deletedItems);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items != null && items.Any())
            {
                foreach (T item in items)
                {
                    action(item);
                }
            }
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> items, T exclude, IEqualityComparer<T> comparer)
        {
            if (items == null) throw new ArgumentNullException("item");
            if (exclude == null) throw new ArgumentNullException("exclude");
            return items.Except<T>(new[] { exclude }, comparer);
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> items, params T[] exclude)
        {
            if (items == null) throw new ArgumentNullException("item");
            if (exclude == null) throw new ArgumentNullException("exclude");
            return items.Except<T>(exclude);
        }

        public static IEnumerable<object> Values(this IEnumerable enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException("enumerable");
            foreach (var value in enumerable)
            {
                yield return value;
            }
        }

        public static IEnumerable<object> Values<TResult>(this IEnumerable enumerable, Func<object, TResult> selector)
        {
            if (enumerable == null) throw new ArgumentNullException("enumerable");
            foreach (object value in enumerable.Values())
            {
                yield return selector(value);
            }
        }

        public static Expression<Func<TInput, bool>> AndAlso<TInput>(this Expression<Func<TInput, bool>> left, Expression<Func<TInput, bool>> right)
        {
            var paramReplacer = new ExpressionParameterReplacer(right.Parameters, left.Parameters);
            return Expression.Lambda<Func<TInput, bool>>(Expression.AndAlso(left.Body, paramReplacer.Visit(right.Body)), left.Parameters);
        }

        public static Expression<Func<TInput, bool>> OrElse<TInput>(this Expression<Func<TInput, bool>> func1, Expression<Func<TInput, bool>> func2)
        {
            return Expression.Lambda<Func<TInput, bool>>(
                Expression.AndAlso(
                    func1.Body, new ExpressionParameterReplacer(func2.Parameters, func1.Parameters).Visit(func2.Body)),
                func1.Parameters);
        }

        private class ExpressionParameterReplacer : ExpressionVisitor
        {
            readonly IDictionary<ParameterExpression, ParameterExpression> ParameterReplacements = new Dictionary<ParameterExpression, ParameterExpression>();
            public ExpressionParameterReplacer(IList<ParameterExpression> from, IList<ParameterExpression> to)
            {
                for (int i = 0; i != from.Count && i != to.Count; i++)
                {
                    ParameterReplacements.Add(from[i], to[i]);
                }
            }
            protected override Expression VisitParameter(ParameterExpression node)
            {
                ParameterExpression replacement;
                if (ParameterReplacements.TryGetValue(node, out replacement))
                {
                    node = replacement;
                }
                return base.VisitParameter(node);
            }
        }
    }
}
