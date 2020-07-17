/* DictionaryTests.cs
 * Author: Rod Howell
 */
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Ksu.Cis300.NameLookup.Tests
{
    /// <summary>
    /// Unit tests for the Dictionary class.
    /// </summary>
    [TestFixture]
    public class DictionaryTests
    {
        /// <summary>
        /// Tests that looking up a nonexistent key gives a value of false and sets the out
        /// parameter to it default value.
        /// </summary>
        [Test]
        public void TestALookUpEmpty()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            int v;
            bool b = d.TryGetValue("key", out v);
            Assert.Multiple(() =>
            {
                Assert.That(b, Is.False);
                Assert.That(v, Is.EqualTo(0));
            });
        }

        /// <summary>
        /// Tests that wehn a duplicate key is added, the proper exception is thrown.
        /// </summary>
        [Test]
        public void TestAAddDuplicateKey()
        {
            Dictionary<int, string> d = new Dictionary<int, string>();
            d.Add(4, "four");
            Exception e = null;
            try
            {
                d.Add(4, "again");
            }
            catch (Exception ex)
            {
                e = ex;
            }
            Assert.That(e, Is.Not.Null.And.TypeOf(typeof(ArgumentException)));
        }

        /// <summary>
        /// Adds a key and a value, then looks up that key.
        /// </summary>
        [Test]
        public void TestBAddOneLookItUp()
        {
            Dictionary<HashTableTester, string> d = new Dictionary<HashTableTester, string>();
            HashTableTester k = new HashTableTester(100000);
            d.Add(k, "value");
            string v;
            bool b = d.TryGetValue(k, out v);
            Assert.Multiple(() =>
            {
                Assert.That(b, Is.True);
                Assert.That(v, Is.EqualTo("value"));
            });
        }

        /// <summary>
        /// Adds two keys that should be stored in the same list, then looks up the first (which
        /// should be second in the list).
        /// </summary>
        [Test]
        public void TestCAddTwoLookUpFirst()
        {
            Dictionary<HashTableTester, string> d = new Dictionary<HashTableTester, string>();
            HashTableTester k1 = new HashTableTester(1000);
            HashTableTester k2 = new HashTableTester(1023);
            d.Add(k1, "first");
            d.Add(k2, "second");
            string v;
            bool b = d.TryGetValue(k1, out v);
            Assert.Multiple(() =>
            {
                Assert.That(b, Is.True);
                Assert.That(v, Is.EqualTo("first"));
            });
        }

        /// <summary>
        /// Adds two keys that should be stored in the same list, then looks up the second.
        /// </summary>
        [Test]
        public void TestCAddTwoLookUpSecond()
        {
            Dictionary<HashTableTester, string> d = new Dictionary<HashTableTester, string>();
            HashTableTester k1 = new HashTableTester(10000);
            HashTableTester k2 = new HashTableTester(10023);
            d.Add(k1, "first");
            d.Add(k2, "second");
            string v;
            bool b = d.TryGetValue(k2, out v);
            Assert.Multiple(() =>
            {
                Assert.That(b, Is.True);
                Assert.That(v, Is.EqualTo("second"));
            });
        }

        /// <summary>
        /// Adds three keys which should be placed in the same linked list, then looks up all three.
        /// </summary>
        [Test]
        public void TestDAddThreeLookUpAll()
        {
            Dictionary<HashTableTester, int> d = new Dictionary<HashTableTester, int>();
            HashTableTester k1 = new HashTableTester(700);
            HashTableTester k2 = new HashTableTester(723);
            HashTableTester k3 = new HashTableTester(746);
            d.Add(k1, 1);
            d.Add(k2, 2);
            d.Add(k3, 3);
            List<int> list = new List<int>();
            int v;
            d.TryGetValue(k1, out v);
            list.Add(v);
            d.TryGetValue(k2, out v);
            list.Add(v);
            d.TryGetValue(k3, out v);
            list.Add(v);
            Assert.That(list, Is.Ordered.And.EquivalentTo(new int[] { 1, 2, 3 }));
        }

        /// <summary>
        /// Test that two keys with hash codes that differ by 23 map to the same location.
        /// </summary>
        [Test]
        public void TestETwoInstancesSameLocation()
        {
            Dictionary<AllEqual, int> d = new Dictionary<AllEqual, int>();
            AllEqual k1 = new AllEqual(100);
            AllEqual k2 = new AllEqual(123);
            d.Add(k1, 7);
            int v;
            // Because all instances of AllEqual are equal to each other, the dictionary
            // should find k2 if it maps to the same array location as k1.
            Assert.That(d.TryGetValue(k2, out v), Is.True);
        }

        /// <summary>
        /// Adds 22 keys that should end up in different locations, then checks whether each
        /// of the 23 locations has a key.
        /// </summary>
        [Test]
        public void TestEDifferentLocations()
        {
            Dictionary<AllEqual, int> d = new Dictionary<AllEqual, int>();
            List<int> elements = new List<int>(); // Will contain the values stored in the dictionary
            elements.Add(0); // Represents one hash table location that will remain empty.
            for (int i = 500; i < 522; i++)
            {
                d.Add(new AllEqual(i), i);
                elements.Add(i);
            }
            List<int> retrieved = new List<int>();

            // The following loop should check each of the table locations.
            // If the location contains an empty list, v will be set to 0.
            // Otherwise, v will be set to the value from the first key-value pair
            // in the list at that location. The first list checked should be
            // empty, and the others should contain the elements added in the above
            // loop, in the same order.
            for (int i = 522; i < 545; i++)
            {
                int v;
                d.TryGetValue(new AllEqual(i), out v);
                retrieved.Add(v);
            }
            Assert.That(retrieved, Is.Ordered.And.EquivalentTo(elements));
        }

        /// <summary>
        /// A class whose instances can be used as keys to test a hash table. Instances store only their
        /// hash codes, and are equal only if their hash codes are equal. Furthermore, their hash
        /// codes are set via a parameter to the constructor.
        /// </summary>
        private class HashTableTester
        {
            /// <summary>
            /// The hash code.
            /// </summary>
            private int _hashCode;

            /// <summary>
            /// Constructs a new instance having the given hash code.
            /// </summary>
            /// <param name="hashCode">The hash code.</param>
            public HashTableTester(int hashCode)
            {
                _hashCode = hashCode;
            }

            /// <summary>
            /// Gets the hash code.
            /// </summary>
            /// <returns>The hash code.</returns>
            public override int GetHashCode()
            {
                return _hashCode;
            }

            /// <summary>
            /// Determines whether the given object is equal to this instance.
            /// </summary>
            /// <param name="obj">The object to compare to.</param>
            /// <returns>Whether obj is equal to this instance.</returns>
            public override bool Equals(object obj)
            {
                if (obj is HashTableTester)
                {
                    HashTableTester x = (HashTableTester)obj;
                    return x == this;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Determines whether the given instances are equal.
            /// </summary>
            /// <param name="x">One instance to compare.</param>
            /// <param name="y">The other instance.</param>
            /// <returns>Whether x and y are equal.</returns>
            public static bool operator ==(HashTableTester x, HashTableTester y)
            {
                if (Equals(x, null))
                {
                    return Equals(y, null);
                }
                else if (Equals(y, null))
                {
                    return false;
                }
                else
                {
                    return x._hashCode == y._hashCode;
                }
            }

            /// <summary>
            /// Determines whether the given instances are different.
            /// </summary>
            /// <param name="x">One instance to compare.</param>
            /// <param name="y">The other instance.</param>
            /// <returns>Whether x and y are different.</returns>
            public static bool operator !=(HashTableTester x, HashTableTester y)
            {
                return !(x == y);
            }
        }

        /// <summary>
        /// A class whose instances are all equal, but whose hash codes can be set via a parameter
        /// to its constructor. This is an incorrect implementation of the GetHashCode method and equality,
        /// but it is useful in testing a hash table.
        /// </summary>
        private class AllEqual
        {
            /// <summary>
            /// The hash code.
            /// </summary>
            private int _hashTable;

            /// <summary>
            /// Constructs a new instance with the given hash code.
            /// </summary>
            /// <param name="hashTable"></param>
            public AllEqual(int hashTable)
            {
                _hashTable = hashTable;
            }

            /// <summary>
            /// Gets the hash code.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return _hashTable;
            }

            /// <summary>
            /// Determines whether the given object is equal to this instance.
            /// </summary>
            /// <param name="obj">The object to compare this instance to.</param>
            /// <returns>Whether obj is equal to this instance.</returns>
            public override bool Equals(object obj)
            {
                if (obj is AllEqual)
                {
                    AllEqual x = (AllEqual)obj;
                    return this == x;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Determines whether the two given instances are equal.
            /// </summary>
            /// <param name="x">One instance to compare.</param>
            /// <param name="y">The other instance.</param>
            /// <returns>Whether x and y are equal.</returns>
            public static bool operator ==(AllEqual x, AllEqual y)
            {
                if (Equals(x, null))
                {
                    return Equals(y, null);
                }
                else if (Equals(y, null))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            /// <summary>
            /// Determines whether the two given instances are different.
            /// </summary>
            /// <param name="x">One instance to compare.</param>
            /// <param name="y">The other instance.</param>
            /// <returns>Whether x and y are different.</returns>
            public static bool operator !=(AllEqual x, AllEqual y)
            {
                return !(x == y);
            }
        }
    }
}