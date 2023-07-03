#pragma warning disable CS0660, CS0661
namespace UniT.Reactive
{
    using System;

    public class ReactiveSingleProperty : ReactiveProperty<float>
    {
        public ReactiveSingleProperty(float value) : base(value)
        {
        }

        public static ReactiveSingleProperty operator ++(ReactiveSingleProperty property)
        {
            ++property.Value;
            return property;
        }

        public static ReactiveSingleProperty operator --(ReactiveSingleProperty property)
        {
            --property.Value;
            return property;
        }

        public static ReactiveSingleProperty operator +(ReactiveSingleProperty property, float value)
        {
            property.Value += value;
            return property;
        }

        public static ReactiveSingleProperty operator -(ReactiveSingleProperty property, float value)
        {
            property.Value -= value;
            return property;
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