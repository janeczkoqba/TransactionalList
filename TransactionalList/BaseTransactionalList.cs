using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionalList
{
    public abstract class BaseTransactionalList<T>
        where T : class, ITransactionalListEquals<T>
    {
        public List<T> BaseList { get; }

        protected List<T> _toAdd;
        protected List<T> _toUpdate;
        protected List<T> _toDelete;

        protected BaseTransactionalList()
        {
            BaseList = new List<T>();
            _toAdd = new List<T>();
            _toUpdate = new List<T>();
            _toDelete = new List<T>();
        }

        protected BaseTransactionalList(ICollection<T> baseList) : this()
        {
            if (baseList == null) throw new ArgumentException("Base list cannot be null.", nameof(baseList));
            BaseList = new List<T>(baseList);
        }
    }
}
