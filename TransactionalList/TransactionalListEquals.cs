namespace TransactionalList
{
    public interface ITransactionalListEquals<T>
         where T : ITransactionalListEquals<T>
    {
        bool TransactionalListEquals(T obj);
    }
}
