namespace UniT.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;

    public class BigNumber : IComparable, IComparable<BigNumber>, IEquatable<BigNumber>
    {
        public static int      Mod     = (int)1e3;
        public static string[] Letters = IterTools.Product(" abcdefghijklmnopqrstuvwxyzx".ToCharArray(), 3).Select(chars => string.Join("", chars).Trim()).ToArray();

        private readonly int[] values;
        private readonly bool  sign; // false: positive, true: negative

        public BigNumber(int value = 0) : this(new() { Math.Abs(value) }, value < 0)
        {
        }

        private BigNumber(List<int> values, bool sign)
        {
            var carry = 0;

            int Normalize(int value)
            {
                value += carry;
                carry =  value / Mod;
                value %= Mod;
                return value;
            }

            values = values.Select(Normalize).ToList();

            while (carry > 0)
            {
                values.Add(Normalize(0));
            }

            while (values.Count > 1 && values[^1] == 0)
            {
                values.RemoveAt(values.Count - 1);
            }

            this.values = values.ToArray();
            this.sign   = sign;
        }

        public override string ToString()
        {
            var sign         = this.sign ? "-" : "";
            var integerValue = this.values[^1];
            var decimalValue = this.values.Length > 1 ? $".{this.values[^2] / (Mod / 10)}" : "";
            var letter       = this.values.Length > 1 ? Letters[this.values.Length - 1] : "";
            return sign + integerValue + decimalValue + letter;
        }

        public static BigNumber Abs(BigNumber number)
        {
            return new(number.values.ToList(), false);
        }

        public static implicit operator BigNumber(int value)
        {
            return new(value);
        }

        public static BigNumber operator -(BigNumber number)
        {
            return new(number.values.ToList(), !number.sign);
        }

        public static BigNumber operator +(BigNumber n1, BigNumber n2)
        {
            if (n1.sign != n2.sign)
            {
                return n1 - -n2;
            }

            return new(IterTools.ZipLongest(n1.values, n2.values, (v1, v2) => v1 + v2).ToList(), n1.sign);
        }

        public static BigNumber operator -(BigNumber n1, BigNumber n2)
        {
            if (n1.sign != n2.sign)
            {
                return n1 + -n2;
            }

            if (Abs(n1) < Abs(n2))
            {
                return -(n2 - n1);
            }

            return new(IterTools.ZipLongest(n1.values, n2.values, (v1, v2) => v1 - v2).ToList(), n1.sign);
        }

        public static BigNumber operator *(BigNumber n1, int n2)
        {
            return new(n1.values.Select(value => value * Math.Abs(n2)).ToList(), n1.sign ^ (n2 < 0));
        }

        public static BigNumber operator *(BigNumber n1, BigNumber n2)
        {
            var padding = new List<int>();
            return n2.values.Aggregate(new BigNumber(new(), n1.sign ^ n2.sign), (result, value) =>
            {
                result += new BigNumber(padding.Concat((n1 * value).values).ToList(), result.sign);
                padding.Add(0);
                return result;
            });
        }

        public static bool operator ==(BigNumber n1, BigNumber n2)
        {
            if (n1 is null || n2 is null)
            {
                return n1 is null && n2 is null;
            }

            return n1.sign == n2.sign && IterTools.SequenceEqual(n1.values, n2.values);
        }

        public static bool operator !=(BigNumber n1, BigNumber n2)
        {
            return !(n1 == n2);
        }

        public static bool operator <(BigNumber n1, BigNumber n2)
        {
            if (n1.sign != n2.sign)
            {
                return n1.sign;
            }

            if (n1.sign)
            {
                return -n1 >= -n2;
            }

            return IterTools.SequenceSmaller(n1.values, n2.values);
        }

        public static bool operator <=(BigNumber n1, BigNumber n2)
        {
            return !(n1 > n2);
        }

        public static bool operator >(BigNumber n1, BigNumber n2)
        {
            if (n1.sign != n2.sign)
            {
                return n2.sign;
            }

            if (n1.sign)
            {
                return -n1 <= -n2;
            }

            return IterTools.SequenceGreater(n1.values, n2.values);
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
            if (other is null) return this.sign ? -1 : 1;
            if (this == other) return 0;
            return this < other ? -1 : 1;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.values, this.sign);
        }
    }
}