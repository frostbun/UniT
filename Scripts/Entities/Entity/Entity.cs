namespace UniT.Entities
{
    #if UNIT_UNITASK
    using System.Threading;
    #else
    using System.Collections;
    #endif
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using UniT.Extensions;
    using UnityEngine;

    public abstract class BaseEntity : MonoBehaviour, IEntity
    {
        IEntityManager IEntity.Manager { get => this.Manager; set => this.Manager = value; }

        bool IEntity.IsDestroyed => !this;

        private ReadOnlyCollection<FieldInfo>          resetOnRecycleFields;
        private ReadOnlyCollection<PropertyInfo>       resetOnRecycleProperties;
        private ReadOnlyDictionary<MemberInfo, object> defaultValues;

        void IEntity.OnInstantiate()
        {
            this.transform = base.transform;
            this.OnInstantiate();

            #region ResetOnRecycle

            this.resetOnRecycleFields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field => field.GetCustomAttribute<ResetOnRecycleAttribute>() is { })
                .ToReadOnlyCollection();

            this.resetOnRecycleProperties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(property => property.GetCustomAttribute<ResetOnRecycleAttribute>() is { })
                .ToReadOnlyCollection();

            var defaultValues = new Dictionary<MemberInfo, object>();
            this.resetOnRecycleFields.ForEach(field => defaultValues.Add(field, field.GetValue(this)));
            this.resetOnRecycleProperties.ForEach(property => defaultValues.Add(property, property.GetValue(this)));
            this.defaultValues = defaultValues.AsReadOnly();

            #endregion
        }

        void IEntity.OnSpawn() => this.OnSpawn();

        void IEntity.OnRecycle()
        {
            #region CancellationToken

            #if UNIT_UNITASK
            this.hideCts?.Cancel();
            this.hideCts?.Dispose();
            this.hideCts = null;
            #endif

            #endregion

            this.OnRecycle();

            #region ResetOnRecycle

            this.resetOnRecycleFields.ForEach(field => field.SetValue(this, this.defaultValues[field]));
            this.resetOnRecycleProperties.ForEach(property => property.SetValue(this, this.defaultValues[property]));

            #endregion
        }

        protected IEntityManager Manager { get; private set; }

        public new Transform transform { get; private set; }

        #if UNIT_UNITASK
        private CancellationTokenSource hideCts;

        protected CancellationToken GetCancellationTokenOnHide()
        {
            return (this.hideCts ??= new CancellationTokenSource()).Token;
        }
        #else
        private readonly HashSet<IEnumerator> runningCoroutines = new();

        public new void StartCoroutine(IEnumerator coroutine)
        {
            base.StartCoroutine(coroutine);
            this.runningCoroutines.Add(coroutine);
        }

        public new void StopCoroutine(IEnumerator coroutine)
        {
            base.StopCoroutine(coroutine);
            this.runningCoroutines.Remove(coroutine);
            (coroutine as IDisposable)?.Dispose();
        }

        protected virtual void OnDisable()
        {
            this.runningCoroutines.SafeForEach(this.StopCoroutine);
        }

        public IEnumerator GatherCoroutines(params IEnumerator[] coroutines)
        {
            try
            {
                var count = coroutines.Length;
                coroutines.ForEach(coroutine => this.StartCoroutine(coroutine.Then(() => --count)));
                yield return new WaitUntil(() => count is 0);
            }
            finally
            {
                coroutines.ForEach(this.StopCoroutine);
            }
        }

        public IEnumerator GatherCoroutines(IEnumerable<IEnumerator> coroutines)
        {
            return this.GatherCoroutines(coroutines.ToArray());
        }
        #endif

        protected virtual void OnInstantiate()
        {
        }

        protected virtual void OnSpawn()
        {
        }

        protected virtual void OnRecycle()
        {
        }
    }

    public abstract class Entity : BaseEntity, IEntityWithoutModel
    {
    }

    public abstract class Entity<TModel> : BaseEntity, IEntityWithModel<TModel>
    {
        TModel IEntityWithModel<TModel>.Model { set => this.Model = value; }

        protected TModel Model { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ResetOnRecycleAttribute : Attribute
    {
    }
}