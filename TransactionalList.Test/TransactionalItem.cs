using System;

namespace TransactionalList.Test
{
    public class TransactionalItem : ITransactionalListEquals<TransactionalItem>
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

        public bool TransactionalListEquals(TransactionalItem obj)
        {
            return ID == obj.ID;
        }
    }
}
