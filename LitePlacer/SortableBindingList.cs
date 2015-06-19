using System;
using System.Collections.Generic;
using System.ComponentModel;
using Be.Timvw.Framework.Collections.Generic;

namespace Be.Timvw.Framework.ComponentModel {
    public class SortableBindingList<T> : BindingList<T> {
        private readonly Dictionary<Type, PropertyComparer<T>> comparers;
        private bool isSorted;
        private ListSortDirection listSortDirection;
        private PropertyDescriptor propertyDescriptor;

        public SortableBindingList()
            : base(new List<T>()) {
            comparers = new Dictionary<Type, PropertyComparer<T>>();
        }

        public SortableBindingList(IEnumerable<T> enumeration)
            : base(new List<T>(enumeration)) {
            comparers = new Dictionary<Type, PropertyComparer<T>>();
        }

        protected override bool SupportsSortingCore {
            get { return true; }
        }

        protected override bool IsSortedCore {
            get { return isSorted; }
        }

        protected override PropertyDescriptor SortPropertyCore {
            get { return propertyDescriptor; }
        }

        protected override ListSortDirection SortDirectionCore {
            get { return listSortDirection; }
        }

        protected override bool SupportsSearchingCore {
            get { return true; }
        }

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction) {
            List<T> itemsList = (List<T>)Items;

            Type propertyType = property.PropertyType;
            PropertyComparer<T> comparer;
            if (!comparers.TryGetValue(propertyType, out comparer)) {
                comparer = new PropertyComparer<T>(property, direction);
                comparers.Add(propertyType, comparer);
            }

            comparer.SetPropertyAndDirection(property, direction);
            itemsList.Sort(comparer);

            propertyDescriptor = property;
            listSortDirection = direction;
            isSorted = true;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore() {
            isSorted = false;
            propertyDescriptor = base.SortPropertyCore;
            listSortDirection = base.SortDirectionCore;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override int FindCore(PropertyDescriptor property, object key) {
            int count = Count;
            for (int i = 0; i < count; ++i) {
                T element = this[i];
                if (property.GetValue(element).Equals(key)) {
                    return i;
                }
            }

            return -1;
        }
    }
}