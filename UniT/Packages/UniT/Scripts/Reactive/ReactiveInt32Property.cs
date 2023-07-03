#pragma warning disable CS0660, CS0661

namespace UniT.Reactive
{
    public class ReactiveInt32Property : ReactiveProperty<int>
    {
        public ReactiveInt32Property(int value) : base(value)
        {
        }

        public static ReactiveInt32Property operator ++(ReactiveInt32Property property)
        {
            ++property.Value;
            return property;
        }

        public static ReactiveInt32Property operator --(ReactiveInt32Property property)
        {
            --property.Value;
            return property;
        }

        public static ReactiveInt32Property operator +(ReactiveInt32Property property, int value)
        {
            property.Value += value;
            return property;
        }

        public static ReactiveInt32Property operator -(ReactiveInt32Property property, int value)
        {
            property.Value -= value;
            return property;
        }

        public static bool operator >(ReactiveInt32Property property, int value)
        {
            return property.Value > value;
        }

        public static bool operator <(ReactiveInt32Property property, int value)
        {
            return property.Value < value;
        }

        public static bool operator >=(ReactiveInt32Property property, int value)
        {
            return property.Value >= value;
        }

        public static bool operator <=(ReactiveInt32Property property, int value)
        {
            return property.Value <= value;
        }

        public static bool operator ==(ReactiveInt32Property property, int value)
        {
            return property!.Value == value;
        }

        public static bool operator !=(ReactiveInt32Property property, int value)
        {
            return property!.Value != value;
        }
    }
}