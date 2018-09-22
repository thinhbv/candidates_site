using System.Collections;
using System.Collections.Generic;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class DynamicControlObjectCollection : ICollection<DynamicControlObject>, IControlFormProvider
    {
        private readonly IList<DynamicControlObject> list;
        private readonly IList<ControlFormAttribute> attributes;

        public DynamicControlObjectCollection()
        {
            list = new List<DynamicControlObject>();
            attributes = new List<ControlFormAttribute>();
        }

        public DynamicControlObjectCollection(IList<ControlFormAttribute> attributes)
        {
            list = new List<DynamicControlObject>();
            this.attributes = new List<ControlFormAttribute>(attributes);
        }

        #region ICollection<DynamicControlObject> Members

        public IEnumerator<DynamicControlObject> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(DynamicControlObject item)
        {
            list.Add(item);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(DynamicControlObject item)
        {
            return list.Contains(item);
        }

        public void CopyTo(DynamicControlObject[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public bool Remove(DynamicControlObject item)
        {
            return list.Remove(item);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion ICollection<DynamicControlObject> Members

        public IEnumerable<ControlFormAttribute> GetAttributes()
        {
            return attributes;
        }
    }
}