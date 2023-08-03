namespace UniT.Extensions.UniTask
{
    using System;
    using Cysharp.Threading.Tasks;

    public static class UniTaskExtensions
    {
        public static async UniTask Catch(this UniTask task, Action<Exception> handler)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                handler(e);
            }
        }

        public static async UniTask<T> Catch<T>(this UniTask<T> task, Func<Exception, T> handler)
        {
            try
            {
                return await task;
            }
            catch (Exception e)
            {
                return handler(e);
            }
        }

        public static async UniTask Catch<TException>(this UniTask task, Action<TException> handler) where TException : Exception
        {
            try
            {
                await task;
            }
            catch (TException e)
            {
                handler(e);
            }
        }

        public static async UniTask<T> Catch<T, TException>(this UniTask<T> task, Func<TException, T> handler) where TException : Exception
        {
            try
            {
                return await task;
            }
            catch (TException e)
            {
                return handler(e);
            }
        }

        public static async UniTask Catch(this UniTask task, Func<Exception, UniTask> handler)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                await handler(e);
            }
        }

        public static async UniTask<T> Catch<T>(this UniTask<T> task, Func<Exception, UniTask<T>> handler)
        {
            try
            {
                return await task;
            }
            catch (Exception e)
            {
                return await handler(e);
            }
        }

        public static async UniTask Catch<TException>(this UniTask task, Func<TException, UniTask> handler) where TException : Exception
        {
            try
            {
                await task;
            }
            catch (TException e)
            {
                await handler(e);
            }
        }

        public static async UniTask<T> Catch<T, TException>(this UniTask<T> task, Func<TException, UniTask<T>> handler) where TException : Exception
        {
            try
            {
                return await task;
            }
            catch (TException e)
            {
                return await handler(e);
            }
        }

        public static async UniTask Finally(this UniTask task, Action handler)
        {
            try
            {
                await task;
            }
            finally
            {
                handler();
            }
        }

        public static async UniTask<T> Finally<T>(this UniTask<T> task, Action handler)
        {
            try
            {
                return await task;
            }
            finally
            {
                handler();
            }
        }

        public static async UniTask Finally(this UniTask task, Func<UniTask> handler)
        {
            try
            {
                await task;
            }
            finally
            {
                await handler();
            }
        }

        public static async UniTask<T> Finally<T>(this UniTask<T> task, Func<UniTask> handler)
        {
            try
            {
                return await task;
            }
            finally
            {
                await handler();
            }
        }
    }
}