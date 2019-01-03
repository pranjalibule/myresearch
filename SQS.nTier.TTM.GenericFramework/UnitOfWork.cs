/******************************************************************************
 *                          © 2017 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * AKS 1Dec2016 Created the class
 *******************************************************************************/

namespace SQS.nTier.TTM.GenericFramework
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Threading;
    using System.Threading.Tasks;

    public class UnitOfWork : IUnitOfWork
    {
        #region Private variables

        private bool disposed = false;
        private readonly DbContext dataContext;
        private ObjectContext objectContext;
        private DbTransaction transaction;

        #endregion

        #region Default Constructor

        /// <summary>
        /// 
        /// </summary>
        public UnitOfWork(DbContext dataContext, LoginSession loginSession)
        {
            this.LoginSession = loginSession;
            this.dataContext = dataContext;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public LoginSession LoginSession { get; }

        /// <summary>
        /// Repository
        /// </summary>
        /// <typeparam name="T">class</typeparam>
        /// <returns>IGenericDataRepository</returns>
        public IGenericDataRepository<T> GetRepository<T>() where T : class, IBaseEntity
        {
            var type = typeof(T).Name;

            var repositoryType = typeof(GenericDataRepository<>);

            return (IGenericDataRepository<T>)Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), dataContext, this);

        }

        ///// <summary>
        ///// Save
        ///// </summary>
        //public void Save()
        //{
        //    dataContext.SaveChanges();
        //}

        /// <summary>
        /// SaveAsync
        /// </summary>
        public async Task SaveAsync()
        {
            int x = await (dataContext.SaveChangesAsync());
        }

        /// <summary>
        /// SaveAsync
        /// </summary>
        /// <param name="token">CancellationToken</param>
        public async Task SaveAsync(CancellationToken token)
        {
            int x = await (dataContext.SaveChangesAsync(token));
        }

        /// <summary>
        /// BeginTransaction
        /// </summary>
        /// <param name="isolationLevel">IsolationLevel</param>
        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            objectContext = ((IObjectContextAdapter)dataContext).ObjectContext;
            if (objectContext.Connection.State != ConnectionState.Open)
            {
                objectContext.Connection.Open();
            }

            transaction = objectContext.Connection.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Commit
        /// </summary>
        /// <returns></returns>
        public bool Commit()
        {
            transaction.Commit();
            return true;
        }

        /// <summary>
        /// Rollback
        /// </summary>
        public void Rollback()
        {
            transaction.Rollback();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">bool</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only

                try
                {
                    if (objectContext != null && objectContext.Connection.State == ConnectionState.Open)
                    {
                        objectContext.Connection.Close();
                    }
                }
                catch (ObjectDisposedException)
                {
                    // do nothing, the objectContext has already been disposed
                }

                if (dataContext != null)
                {
                    dataContext.Dispose();
                }
            }

            // release any unmanaged objects
            // set the object references to null

            disposed = true;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
