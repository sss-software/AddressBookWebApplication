using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AddressBook.Api.Data.Repositories
{
    public class DataRepository<T> : IDataRepository<T> where T : class
    {
        private readonly AddressBookApiContext context;

        public DataRepository(AddressBookApiContext context)
        {
            this.context = context;
        }

        public void Add(T entity)
        {
            context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }

        public async Task<T> SaveAsync(T entity)
        {
            await context.SaveChangesAsync();
            return entity;
        }
    }
}
