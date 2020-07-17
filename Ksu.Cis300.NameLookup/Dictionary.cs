/* Dictionary.cs
 * Author: Rod Howell
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KansasStateUniversity.TreeViewer2;

namespace Ksu.Cis300.NameLookup
{
    /// <summary>
    /// A generic dictionary in which keys must implement IComparable.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class Dictionary<TKey, TValue> //where TKey : IComparable<TKey>
    {
        /// <summary>
        /// The keys and values in the dictionary.
        /// </summary>
        private LinkedListCell<KeyValuePair<TKey,TValue>>[] _elements = new 
            LinkedListCell<KeyValuePair<TKey, TValue>>[23];
        public int x = 0;

        /// <summary>
        /// compute and return table location from given key
        /// use Hashcode, then remove it's sign bit
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        private int GetLocation(TKey k)
        {
            int x = k.GetHashCode() & 0x7fffffff;
            int y = x % x;
            return y;

        }

        /// <summary>
        /// find and return cell with given Key from LL
        /// use Equals method to search
        /// return null if LL does not contain key
        /// </summary>
        /// <param name="k"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private LinkedListCell<KeyValuePair<TKey, TValue>> GetCell(TKey k, LinkedListCell<KeyValuePair<TKey, TValue>> list)
        {
            if (list.Data.Key.Equals(null)) return null;
            else 
            {
                for (int j = 0; j < _elements.Length; j++)
                {
                    if (list.Next.Equals(_elements.Rank)) return list;
                    else break;
                }
            }
            return null;
        
        }

        /// <summary>
        /// insets given cell into LL
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="loc"></param>
        private void Insert(LinkedListCell<KeyValuePair<TKey, TValue>> cell, int loc)
        {
            LinkedListCell<KeyValuePair<TKey, TValue>> temp = GetCell(cell.Data.Key, cell);
            loc = GetLocation(temp.Data.Key);
            Insert(temp, loc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <param name="v"></param>
        /// <param name="loc"></param>
        private void Insert(TKey k, TValue v, int loc)
        {
            //Insert(k, v, loc); 
            LinkedListCell<KeyValuePair<TKey, TValue>> temp = new 
                LinkedListCell<KeyValuePair<TKey, TValue>>();
            k = temp.Data.Key;
            v = temp.Data.Value;
            Insert(k, v, loc);
        }

        /// <summary>
        /// Checks to see if the given key is null, and if so, throws an
        /// ArgumentNullException.
        /// </summary>
        /// <param name="key">The key to check.</param>
        private static void CheckKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }
        }

       


        /// <summary>
        /// Tries to get the value associated with the given key.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value associated with k, or the default value if
        /// k is not in the dictionary.</param>
        /// <returns>Whether k was found as a key in the dictionary.</returns>
        public bool TryGetValue(TKey k, out TValue v)
        {
            CheckKey(k);

            //BinaryTreeNode<KeyValuePair<TKey, TValue>> p = Find(k, _elements);
            if (_elements == null)
            {
                v = default(TValue);
                return false;
            }
            else
            {
                v = _elements[0].Data.Value;
                return true;
            }
        }

        /// <summary>
        /// Adds the given key with the given associated value.
        /// If the given key is already in the dictionary, throws an
        /// InvalidOperationException.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value.</param>
        public void Add(TKey k, TValue v)
        {
            int x = _elements.Length;
            CheckKey(k);
            Insert(k,v,Convert.ToInt32(_elements[x].Data.Key));
        }

        /// <summary>
        /// Builds the tree obtained by removing the minimum key from the given nonempty
        /// binary search tree.
        /// </summary>
        /// <param name="t">The binary search tree.</param>
        /// <param name="min">The key and value removed.</param>
        /// <returns>The result of removing the minimum key from t.</returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>>
            RemoveMininumKey(BinaryTreeNode<KeyValuePair<TKey, TValue>> t, out KeyValuePair<TKey, TValue> min)
        {
            if (t.LeftChild == null)
            {
                min = t.Data;
                return t.RightChild;
            }
            else
            {
                return BinaryTreeNode<KeyValuePair<TKey, TValue>>.GetAvlTree(t.Data, RemoveMininumKey(t.LeftChild, out min),
                    t.RightChild);
            }
        }
    }
}
