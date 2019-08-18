using System;
using System.Linq;

namespace PhotoFolder.Core.Domain
{
    /// <summary>
    ///     Represents a hash value
    /// </summary>
    [Serializable]
    public struct Hash : IEquatable<Hash>
    {
        /// <summary>
        ///     Initialize a new instance of <see cref="Hash" />
        /// </summary>
        /// <param name="hashData">The data of the hash</param>
        public Hash(byte[] hashData)
        {
            HashData = hashData;
        }

        /// <summary>
        ///     The data of the hash
        /// </summary>
        public byte[] HashData { get; }

        /// <summary>
        ///     True if the hash length matches the length of a SHA256 hash (32 bytes/256 bits)
        /// </summary>
        public bool IsSha256Size => HashData.Length == 32;

        /// <summary>
        ///     Returns a value indicating whether this instance and a specified <see cref="Hash" /> object represent the same
        ///     value.
        /// </summary>
        /// <param name="other">A <see cref="Hash" /> to compare to this instance.</param>
        /// <returns>true if obj is equal to this instance; otherwise, false.</returns>
        public bool Equals(Hash other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return HashData.SequenceEqual(other.HashData);
        }

        /// <summary>
        ///     Parse a hex string
        /// </summary>
        /// <param name="value">The hex string which represents a hash value</param>
        /// <returns>Return</returns>
        public static Hash Parse(string value)
        {
            return new Hash(ToByteArray(value));
        }

        /// <summary>
        ///     Try parse a hex string
        /// </summary>
        /// <param name="value">The hex string which represents a hash value</param>
        /// <param name="hash">The result hash</param>
        /// <returns>True if the <see cref="value" /> was successfully parsed, else false.</returns>
        public static bool TryParse(string? value, out Hash? hash)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                hash = null;
                return false;
            }

            if (value.Length % 2 == 1)
            {
                hash = null;
                return false;
            }

            try
            {
                hash = new Hash(ToByteArray(value));
                return true;
            }
            catch (Exception)
            {
                hash = null;
                return false;
            }
        }

        /// <summary>
        ///     Convert the <see cref="HashData" /> to a hexadecimal string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return BitConverter.ToString(HashData).Replace("-", null).ToLowerInvariant();
        }

        public static bool operator ==(Hash obj1, Hash obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(Hash obj1, Hash obj2)
        {
            return !(obj1 == obj2);
        }

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare to this instance.</param>
        /// <returns>true if obj is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Hash)obj);
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current <see cref="Hash" />.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                foreach (var b in HashData)
                    result = result * 31 ^ b;
                return result;
            }
        }

        /// <summary>
        ///     Convert a hex string to a byte array
        /// </summary>
        /// <param name="source">The hex string</param>
        /// <returns>Return the byte array from the hex string</returns>
        public static byte[] ToByteArray(string source)
        {
            if (source.Length % 2 == 1)
                throw new ArgumentException("The binary key cannot have an odd number of digits", nameof(source));

            int GetHexVal(char hex)
            {
                var isHex = hex >= '0' && hex <= '9' ||
                            hex >= 'a' && hex <= 'f' ||
                            hex >= 'A' && hex <= 'F';
                if (!isHex)
                    throw new ArgumentException($"The char '{hex}' is not a valid hexadecimal character.",
                        nameof(source));

                return hex - (hex < 58 ? 48 : hex < 97 ? 55 : 87);
            }

            var arr = new byte[source.Length >> 1];
            for (var i = 0; i < source.Length >> 1; ++i)
                arr[i] = (byte)((GetHexVal(source[i << 1]) << 4) + GetHexVal(source[(i << 1) + 1]));

            return arr;
        }
    }
}