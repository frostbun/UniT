#pragma warning disable CS0660, CS0661

namespace UniT.Reactive
{
    using System;

    public sealed class ReactiveSingleProperty : ReactiveProperty<float>
    {
        public ReactiveSingleProperty(float value = default) : base(value)
        {
        }

        public static bool operator >(ReactiveSingleProperty property, float value)
        {
            return property.Value > value;
        }

        public static bool operator <(ReactiveSingleProperty property, float value)
        {
            return property.Value < value;
        }

        public static bool operator >=(ReactiveSingleProperty property, float value)
        {
            return property.Value >= value;
        }

        public static bool operator <=(ReactiveSingleProperty property, float value)
        {
            return property.Value <= value;
        }

        public static bool operator ==(ReactiveSingleProperty property, float value)
        {
            return Math.Abs(property!.Value - value) < float.Epsilon;
        }

        public static bool operator !=(ReactiveSingleProperty property, float value)
        {
            return Math.Abs(property!.Value - value) > float.Epsilon;
        }
    }
}