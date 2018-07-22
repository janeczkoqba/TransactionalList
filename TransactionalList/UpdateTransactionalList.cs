using System;
using System.Collections.Generic;
using System.Linq;

namespace TransactionalList
{
    public class ActionTransactionalList<T>: BaseTransactionalList<T>
        where T : class, ITransactionalListEquals<T>
    {
        private Action<T, T> _updateAction;
        private Action<T> _deleteAction;

        public List<T> CurrentList
        {
            get
            {
                var result = new List<T>(_toAdd);
                var baseListWithoutDeleted = BaseList.Where(x => !_toDelete.Any(y => y.TransactionalListEquals(x)));
                result.AddRange(_toUpdate.Where(x => baseListWithoutDeleted.Any(y => y.TransactionalListEquals(x))));
                result.AddRange(baseListWithoutDeleted.Where(x => !_toUpdate.Any(y => y.TransactionalListEquals(x))));

                return result;
            }
        }

        public ActionTransactionalList(Action<T, T> updateAction) : this(new CtorParams(updateAction))
        {
        }

        public ActionTransactionalList(ICollection<T> baseList, Action<T, T> updateAction) : this(new CtorParams(baseList, updateAction))
        {
            _updateAction = updateAction;
        }

        private ActionTransactionalList(CtorParams pars) : base(pars.baseList)
        {
            _updateAction = pars.updateAction;
        }

        public void AddOrUpdate(T obj)
        {
            _toDelete.RemoveAll(x => x.TransactionalListEquals(obj)); // clear from delete if exists
            _toUpdate.RemoveAll(x => x.TransactionalListEquals(obj)); // clear from update if exists
            _toAdd.RemoveAll(x => x.TransactionalListEquals(obj)); // clear from update if exists

            if(BaseList.Any(x => x.TransactionalListEquals(obj))) // check if update or add new
            {
                _toUpdate.Add(obj);
            }
            else
            {
                _toAdd.Add(obj);
            }
        }

        public void Remove(T obj)
        {
            _toDelete.RemoveAll(x => obj.TransactionalListEquals(obj));
            _toUpdate.RemoveAll(x => obj.TransactionalListEquals(obj));
            _toAdd.RemoveAll(x => obj.TransactionalListEquals(obj));

            if(BaseList.Any(x => x.TransactionalListEquals(obj)))
                _toDelete.Add(obj);
        }

        public void Commit()
        {
            if (_updateAction == null) throw new InvalidOperationException("Update action cannot be null.");

            BaseList.AddRange(_toAdd);
            BaseList.RemoveAll(x => _toDelete.Any(y => y.TransactionalListEquals(x)));

            foreach (var item in _toUpdate)
            {
                var baseItem = BaseList.Single(x => x.TransactionalListEquals(item));
                _updateAction(baseItem, item);
            }

            _toAdd.Clear();
            _toDelete.Clear();
            _toUpdate.Clear();
        }

        public void Cancel()
        {
            _toDelete.Clear();
            _toAdd.Clear();
            _toUpdate.Clear();
        }


        private class CtorParams
        {
            public ICollection<T> baseList { get; set; }
            public Action<T, T> updateAction { get; set; }

            public CtorParams(ICollection<T> baseList, Action<T, T> updateAction)
            {
                this.baseList = baseList;
                this.updateAction = updateAction;
            }

            public CtorParams(Action<T, T> updateAction)
            {
                baseList = new List<T>();
                this.updateAction = updateAction;
            }
        }
    }
}
