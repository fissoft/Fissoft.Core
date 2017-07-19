using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Fissoft.LinqIndex.Indexes;
using Fissoft.LinqIndex.Internal;

namespace Fissoft.LinqIndex
{
    public static class IndexableCollectionExtension
    {
        public static IndexableCollection<T> ToIndexableCollection<T>(this IEnumerable<T> enumerable)
        {
            return new IndexableCollection<T>(enumerable);
        }

        public static IndexableCollection<T> ToIndexableCollection<T>(this IEnumerable<T> enumerable,
            IndexSpecification<T> indexSpecification)
            where T : class
        {
            return new IndexableCollection<T>(enumerable)
                .UseIndexSpecification(indexSpecification);
        }

        public static IndexableCollection<T> ToIndexableCollection<T>(this IEnumerable<T> enumerable,
            Action<IndexSpecification<T>> newIndexSpecification)
            where T : class
        {
            var spec = new IndexSpecification<T>();

            newIndexSpecification(spec);

            return new IndexableCollection<T>(enumerable, spec);
        }

        public static IEnumerable<TResult> Join<T, TInner, TKey, TResult>(
            this IIndexableCollection<T> outer,
            IndexableCollection<TInner> inner,
            Expression<Func<T, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Func<T, TInner, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (outer == null || inner == null || outerKeySelector == null || innerKeySelector == null ||
                resultSelector == null)
                throw new ArgumentNullException();

            var haveIndex = false;
            if (innerKeySelector.NodeType == ExpressionType.Lambda
                && innerKeySelector.Body.NodeType == ExpressionType.MemberAccess
                && outerKeySelector.NodeType == ExpressionType.Lambda
                && outerKeySelector.Body.NodeType == ExpressionType.MemberAccess)
            {
                var membExpInner = (MemberExpression) innerKeySelector.Body;
                var membExpOuter = (MemberExpression) outerKeySelector.Body;

                var innerMemberName = membExpInner.Member.Name;
                var outerMemberName = membExpOuter.Member.Name;

                IIndex<TInner> innerIndex = null;
                IIndex<T> outerIndex = null;

                if (inner.ContainsIndex(innerMemberName)
                    && outer.ContainsIndex(outerMemberName))
                {
                    innerIndex = inner.GetIndexByPropertyName(innerMemberName);
                    outerIndex = outer.GetIndexByPropertyName(outerMemberName);
                    haveIndex = true;
                }

                if (haveIndex)
                    foreach (var outerKey in outerIndex.Keys)
                    {
                        var outerGroup = outerIndex.ItemsWithKey(outerKey);
                        List<TInner> innerGroup;
                        //List<TInner> innerGroup = innerIndex.ItemsWithKey(outerKey);
                        if (innerIndex.TryGetItemsForKey(outerKey, out innerGroup))
                            //if (innerGroup.Count > 0)
                        {
                            var innerEnum = innerGroup.AsEnumerable();
                            var outerEnum = outerGroup.AsEnumerable();
                            var result = outerEnum.Join(innerEnum,
                                outerKeySelector.Compile(),
                                innerKeySelector.Compile(),
                                resultSelector, comparer);
                            foreach (var resultItem in result)
                                yield return resultItem;
                        }
                        //do a join on the GROUPS based on key result
                    }
            }
            if (!haveIndex)
            {
                //this will happen if we don't have keys in the right places
                var outerEnum = outer.AsEnumerable();
                var result = outerEnum.Join(inner, outerKeySelector.Compile(), innerKeySelector.Compile(),
                    resultSelector, comparer);
                foreach (var resultItem in result)
                    yield return resultItem;
            }
        }


        public static IEnumerable<TResult> Join<T, TInner, TKey, TResult>(
            this IIndexableCollection<T> outer,
            IndexableCollection<TInner> inner,
            Expression<Func<T, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Func<T, TInner, TResult> resultSelector)
        {
            return outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector,
                EqualityComparer<TKey>.Default);
        }

        private static bool HasIndexablePropertyOnLeft<T>(Expression leftSide, IIndexableCollection<T> sourceCollection,
            out MemberExpression theMember)
        {
            theMember = null;
            var mex = leftSide as MemberExpression;
            if (leftSide.NodeType == ExpressionType.Call)
            {
                var call = (MethodCallExpression) leftSide;

                if (call.Method.Name == "CompareString")
                    mex = call.Arguments[0] as MemberExpression;
            }

            if (mex == null) return false;

            theMember = mex;
            return sourceCollection.ContainsIndex(mex.Member.Name);
        }


        private static int? GetHashRight(Expression leftSide, Expression rightSide)
        {
            if (leftSide.NodeType == ExpressionType.Call)
            {
                var call = (MethodCallExpression) leftSide;
                if (call.Method.Name == "CompareString")
                {
                    var evalRight = Expression.Lambda(call.Arguments[1], null);
                    //Compile it, invoke it, and get the resulting hash
                    return evalRight.Compile().DynamicInvoke(null).GetHashCode();
                }
            }
            //rightside is where we get our hash...
            switch (rightSide.NodeType)
            {
                //shortcut constants, dont eval, will be faster
                case ExpressionType.Constant:
                    var constExp = (ConstantExpression) rightSide;
                    var value = constExp.Value ?? 0;
                    return value.GetHashCode();

                //if not constant (which is provably terminal in a tree), convert back to Lambda and eval to get the hash.
                default:
                    //Lambdas can be created from expressions... yay
                    var evalRight = Expression.Lambda(rightSide, null);
                    //Compile that mutherf-ker, invoke it, and get the resulting hash
                    return evalRight.Compile().DynamicInvoke(null).GetHashCode();
            }
        }

        //extend the where when we are working with indexable collections! 
        public static IEnumerable<T> Where<T>
        (
            this IIndexableCollection<T> sourceCollection,
            Expression<Func<T, bool>> predicate
        )
        {
            //our indexes work from the hash values of that which is indexed, regardless of type
            int? hashRight;
            var noIndex = true;

            //indexes only work on equality expressions here
            if (predicate.Body.NodeType == ExpressionType.Equal)
            {
                //Equality is a binary expression
                var binExp = (BinaryExpression) predicate.Body;
                //Get some aliases for either side
                var leftSide = binExp.Left;
                var rightSide = binExp.Right;

                hashRight = GetHashRight(leftSide, rightSide);

                //if we were able to create a hash from the right side (likely)
                MemberExpression returnedEx;
                if (hashRight.HasValue && HasIndexablePropertyOnLeft(leftSide, sourceCollection, out returnedEx))
                {
                    //cast to MemberExpression - it allows us to get the property
                    var property = returnedEx.Member.Name;
                    var myIndex =
                        sourceCollection.GetIndexByPropertyName(property);
                    if (myIndex.ContainsKey(hashRight.Value))
                    {
                        IEnumerable<T> sourceEnum = myIndex.ItemsWithKey(hashRight.Value);
                        var result = sourceEnum.Where(predicate.Compile());
                        foreach (var item in result)
                            yield return item;
                    }
                    noIndex = false; //we found an index, whether it had values or not is another matter
                }
            }
            if (noIndex) //no index?  just do it the normal slow way then...
            {
                var result = sourceCollection.Where(predicate.Compile());
                foreach (var resultItem in result)
                    yield return resultItem;
            }
        }

        //Observable items extension
        public static IEnumerable<T> Where<TCollection, T>(this TCollection sourceCollection,
            Expression<Func<T, bool>> predicate)
            where TCollection : class, INotifyPropertyChanged, IEnumerable<T>
        {
            IIndexableCollection<T> observableIndexFound;
            if (InternalObservablesHook.TryGetIndexForObservable(sourceCollection, out observableIndexFound))
                return observableIndexFound.Where(predicate);

            return sourceCollection.Where(predicate.Compile());
        }

        public static IIndexableCollection<T> AsObservable<T>(this IEnumerable<T> sourceCollection)
        {
            IIndexableCollection<T> observableIndexFound;
            if (InternalObservablesHook.TryGetIndexForObservable(sourceCollection, out observableIndexFound))
                return observableIndexFound;

            throw new ObservableIndexNotFoundException(
                "Cannot find the requested collection in the ObservablesMonitor. Before trying to use an indexed observable collection, first add the collection to the ObservablesMonitor with a specific IndexSpecification.");
        }
    }
}