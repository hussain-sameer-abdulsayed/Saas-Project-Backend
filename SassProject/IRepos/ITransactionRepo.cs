namespace SassProject.IRepos
{
    public interface ITransactionRepo
    {
        Task CommitTransactionAsync();
        Task BeginTransactionAsync();
        Task RollBackTransactionAsync();
    }
}
