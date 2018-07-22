using System;
using System.Collections.Generic;
using System.Linq;

namespace TransactionalList
{
    public class SwapTransactionalList<T>: BaseTransactionalList<T>
        where T : class, ITransactionalListEquals<T>
    {
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

        public SwapTransactionalList() : base()
        {
        }

        public SwapTransactionalList(ICollection<T> baseList) : base(baseList)
        {
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
            BaseList.AddRange(_toAdd);
            BaseList.RemoveAll(x => _toDelete.Any(y => y.TransactionalListEquals(x)));

            foreach (var item in _toUpdate)
            {
                var ind = BaseList.FindIndex(x => x.TransactionalListEquals(item));
                BaseList[ind] = item;
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

    }
}
