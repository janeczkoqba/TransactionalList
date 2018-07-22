namespace TransactionalList
{
    public interface ITransactionalList<T>
        where T : class, ITransactionalListEquals<T>
    {
        void Commit();
        void Cancel();
    }
}
