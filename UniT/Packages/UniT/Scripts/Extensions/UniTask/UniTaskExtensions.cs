namespace UniT.Extensions.UniTask
{
    using Cysharp.Threading.Tasks;

    public static class UniTaskExtensions
    {
        public static UniTask<TResult> Cast<TSource, TResult>(this UniTask<TSource> task) where TSource : TResult
        {
            return task.ContinueWith(result => (TResult)result);
        }
    }
}