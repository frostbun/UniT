namespace UniT.Extensions.UniTask
{
    using System;
    using Cysharp.Threading.Tasks;

    public static class TupleUniTaskExtensions
    {
        public static UniTask ContinueWith<TFirst, TSecond>(this UniTask<(TFirst, TSecond)> task, Func<TFirst, TSecond, UniTask> continuationFunction)
        {
            return task.ContinueWith(tuple => continuationFunction(tuple.Item1, tuple.Item2));
        }

        public static UniTask ContinueWith<TFirst, TSecond>(this UniTask<(TFirst, TSecond)> task, Action<TFirst, TSecond> continuationAction)
        {
            return task.ContinueWith(tuple => continuationAction(tuple.Item1, tuple.Item2));
        }

        public static UniTask<TResult> ContinueWith<TFirst, TSecond, TResult>(this UniTask<(TFirst, TSecond)> task, Func<TFirst, TSecond, UniTask<TResult>> continuationFunction)
        {
            return task.ContinueWith(tuple => continuationFunction(tuple.Item1, tuple.Item2));
        }

        public static UniTask<TResult> ContinueWith<TFirst, TSecond, TResult>(this UniTask<(TFirst, TSecond)> task, Func<TFirst, TSecond, TResult> continuationFunction)
        {
            return task.ContinueWith(tuple => continuationFunction(tuple.Item1, tuple.Item2));
        }
    }
}