#pragma warning disable CS0660, CS0661

namespace UniT.Reactive
{
    public class ReactiveInt32Property : ReactiveProperty<int>
    {
        public ReactiveInt32Property(int value) : base(value)
        {
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