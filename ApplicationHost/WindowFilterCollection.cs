using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ApplicationHost
{
    /// <summary>
    /// Collection of window filters.
    /// </summary>
    public class WindowFilterCollection : Collection<WindowFilter>
    {
        internal bool IsFrozen { get; set; }

        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </summary>
        protected override void ClearItems()
        {
            VerifyNotFrozen();

            base.ClearItems();
        }

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert. The value can be null for reference types.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
        protected override void InsertItem(int index, WindowFilter item)
        {
            VerifyNotFrozen();

            base.InsertItem(index, item);
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.-or-<paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
        protected override void RemoveItem(int index)
        {
            VerifyNotFrozen();

            base.RemoveItem(index);
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param><param name="item">The new value for the element at the specified index. The value can be null for reference types.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
        protected override void SetItem(int index, WindowFilter item)
        {
            VerifyNotFrozen();

            base.SetItem(index, item);
        }

        private void VerifyNotFrozen()
        {
            if (IsFrozen)
                throw new InvalidOperationException("Window filters cannot be changed once the application is running");
        }
    }
}
