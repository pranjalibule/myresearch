namespace SQS.nTier.TTM.GenericFramework
{

    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// IUnitOfWork
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IGenericDataRepository<T> GetRepository<T>() where T : class, IBaseEntity;

        /// <summary>
        /// Save
        /// </summary>
        //void Save();

        /// <summary>
        /// SaveAsync
        /// </summary>
        Task SaveAsync();

        /// <summary>
        /// SaveAsync
        /// </summary>
        /// <param name="token">CancellationToken</param>
        Task SaveAsync(CancellationToken token);

        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        bool Commit();
        void Rollback();

        LoginSession LoginSession { get; }
    }

}
