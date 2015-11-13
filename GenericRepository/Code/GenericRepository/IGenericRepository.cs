using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ToB.Db.GenericRepository
{
    public interface IGenericRepository<Entity>  where Entity : class
    {
        void Add(Entity entity);

        void Delete(object id);

        void Delete(Entity id);

        void Delete(Func<Entity, Boolean> where);

        void Update(Entity entity);

        int Count(Expression<Func<Entity, bool>> filter = null);

        IEnumerable<Entity> GetAll();
        
        Entity GetById(object id);

        IQueryable<Entity> GetQueryable(Func<Entity, bool> where);

        bool Exists(object primaryKey);

        Entity GetSingle(Func<Entity, bool> predicate);

        Entity GetSingleOrDefault(Func<Entity, bool> predicate);

        Entity GetFirst(Func<Entity, bool> predicate);

        Entity GetFirstOrDefault(Func<Entity, bool> predicate);

        IEnumerable<Entity> Get(
            int? pageNo = null, 
            int? pageSize = null,    
            Expression<Func<Entity,Entity>> selector = null,
            Expression<Func<Entity, bool>> filter = null,
            Func<IQueryable<Entity>, IOrderedQueryable<Entity>> orderBy = null,
            string includeProperties = "");
    }
}
