namespace UniT.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using UniT.Extensions;

    public class BigNumber : IComparable, IComparable<BigNumber>, IEquatable<BigNumber>
    {
        public static int          Mod     { get; set; } = (int)1e3;
        public static List<string> Letters { get; set; } = IterTools.Product(" abcdefghijklmnopqrstuvwxyz".ToCharArray(), 3).Select(chars => string.Join("", chars).Trim()).ToList();

        private readonly ReadOnlyCollection<int> _values;
        private readonly bool                    _sign; // false: positive, true: negative
        private          bool                    IsPositive => !this._sign;
        private          bool                    IsNegative => this._sign;

        public BigNumber(int value = 0) : this(new[] { Math.Abs(value) }, value < 0)
        {
        }

        private BigNumber(IEnumerable<int> values, bool sign)
        {
            var carry = 0;

            int Normalize(int value)
            {
                value += carry;
                carry =  value / Mod;
                value %= Mod;
                return value;
            }

            var normalizedValues = values.Select(Normalize).ToList();
            while (carry > 0) normalizedValues.Add(Normalize(0));
            while (normalizedValues.Count > 1 && normalizedValues[^1] == 0) normalizedValues.RemoveAt(normalizedValues.Count - 1);

            this._values = normalizedValues.AsReadOnly();
            this._sign   = sign;
        }

        public override string ToString()
        {
            var sign         = this.IsNegative ? "-" : "";
            var integerValue = this._values[^1];
            var decimalValue = this._values.Count > 1 ? $".{this._values[^2] / (Mod / 10)}" : "";
            var letter       = this._values.Count > 1 ? Letters[this._values.Count - 1] : "";
            return sign + integerValue + decimalValue + letter;
        }

        public static BigNumber Abs(BigNumber number)
        {
            return new(number._values, false);
        }

        public static implicit operator BigNumber(int value)
        {
            return new(value);
        }

        public static BigNumber operator -(BigNumber number)
        {
            return new(number._values, !number._sign);
        }

        public static BigNumber operator +(BigNumber n1, BigNumber n2)
        {
            if (n1._sign != n2._sign) return n1 - -n2;
            return new(IterTools.ZipLongest(n1._values, n2._values, (v1, v2) => v1 + v2), n1._sign);
        }

        public static BigNumber operator -(BigNumber n1, BigNumber n2)
        {
            if (n1._sign != n2._sign) return n1 + -n2;
            if (Abs(n1) < Abs(n2)) return -(n2 - n1);
            return new(IterTools.ZipLongest(n1._values, n2._values, (v1, v2) => v1 - v2), n1._sign);
        }

        public static BigNumber operator *(BigNumber n1, int n2)
        {
            return new(n1._values.Select(value => value * Math.Abs(n2)), n1.IsPositive ^ (n2 >= 0));
        }

        public static BigNumber operator *(BigNumber n1, BigNumber n2)
        {
            var padding = new List<int>();
            var sign    = n1._sign ^ n2._sign;
            return n2._values.Aggregate(new BigNumber(), (result, value) =>
            {
                result += new BigNumber(padding.Concat((n1 * value)._values), sign);
                padding.Add(0);
                return result;
            });
        }

        public static bool operator ==(BigNumber n1, BigNumber n2)
        {
            if (n1 is null || n2 is null) return n1 is null && n2 is null;
            return n1._sign == n2._sign && n1._values.SequenceEqual(n2._values);
        }

        public static bool operator !=(BigNumber n1, BigNumber n2)
        {
            return !(n1 == n2);
        }

        public static bool operator <(BigNumber n1, BigNumber n2)
        {
            if (n1._sign != n2._sign) return n1.IsNegative;
            foreach (var (v1, v2) in IterTools.ZipLongest(n1._values, n2._values).Reverse())
            {
                if (v1 < v2) return n1.IsPositive;
                if (v1 > v2) return n1.IsNegative;
            }
            return false;
        }

        public static bool operator <=(BigNumber n1, BigNumber n2)
        {
            return !(n1 > n2);
        }

        public static bool operator >(BigNumber n1, BigNumber n2)
        {
            if (n1._sign != n2._sign) return n1.IsPositive;
            foreach (var (v1, v2) in IterTools.ZipLongest(n1._values, n2._values).Reverse())
            {
                if (v1 > v2) return n1.IsPositive;
                if (v1 < v2) return n1.IsNegative;
            }
            return false;
        }

        public static bool operator >=(BigNumber n1, BigNumber n2)
        {
            return !(n1 < n2);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as BigNumber);
        }

        public bool Equals(BigNumber other)
        {
            return this == other;
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as BigNumber);
        }

        public int CompareTo(BigNumber other)
        {
            if (other is null) return this.IsNegative ? -1 : 1;
            if (this == other) return 0;
            return this < other ? -1 : 1;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            this._values.ForEach(hashCode.Add);
            hashCode.Add(this._sign);
            return hashCode.ToHashCode();
        }
    }
}