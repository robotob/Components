using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq.Expressions;

namespace ToB.Db.GenericRepository
{

    public class GenericRepository<Entity>
        : IGenericRepository<Entity> where Entity : class
    {
        private IDbSet<Entity> _entitySet;

        private string _errorMessage = String.Empty;
        private readonly BaseDbContext _context;

        public GenericRepository(BaseDbContext context)
        {
            this._context = context;
            this._entitySet = _context.Set<Entity>();
        }
        
        public virtual void Add(Entity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                // _dbContext.Entry(entity).State = EntityState.Detached;

                ///TODO check this line >> this._entitySet.AsNoTracking<Entity>();
                this._entitySet.Add(entity);
           
            }
            catch (DbEntityValidationException dbEx)
            {

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _errorMessage += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                    }
                }
                throw new Exception(_errorMessage, dbEx);
            }
            catch
            {
                throw;
            }

        }

        public virtual void Delete(object id)
        {
            var currentEntity = this._entitySet.Find(id);

            if (currentEntity != null)
            {
                Delete(currentEntity);
            }
        }

        public virtual void Delete(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (_context.Entry(entity).State == EntityState.Detached)
            {
                this._entitySet.Attach(entity);
            }
            this._entitySet.Remove(entity);
        }

        /// <summary>
        /// generic delete method , deletes data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual void Delete(Func<Entity, Boolean> where)
        {
            IQueryable<Entity> objects = this._entitySet.Where<Entity>(where).AsQueryable();
            foreach (Entity obj in objects)
            {
                this._entitySet.Remove(obj);
            }
        }

        public virtual void Update(Entity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }

                _entitySet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
            catch (DbEntityValidationException dbEx)
            {

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _errorMessage += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                    }
                }
                throw new Exception(_errorMessage, dbEx);
            }
            catch
            {
                throw;
            }
        }

        public virtual int Count(Expression<Func<Entity, bool>> filter = null)
        {
            IQueryable<Entity> query = _entitySet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.Count();
        }

        public virtual IEnumerable<Entity> GetAll()
        {
            IQueryable<Entity> query = _entitySet;
            return query.ToList();
        }
        
        public virtual Entity GetById(object entityId)
        {
            return this._entitySet.Find(entityId);
        }

        /// <summary>
        /// generic method to get many record on the basis of a condition but query able.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual IQueryable<Entity> GetQueryable(Func<Entity, bool> where)
        {
            return this._entitySet.Where(where).AsQueryable();
        }

        /// <summary>
        /// Inclue multiple
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public IQueryable<Entity> GetWithInclude(System.Linq.Expressions.Expression<Func<Entity, bool>> predicate, params string[] include)
        {
            IQueryable<Entity> query = this._entitySet;
            query = include.Aggregate(query, (current, inc) => current.Include(inc));
            return query.Where(predicate);
        }

        /// <summary>
        /// Generic method to check if entity exists
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public bool Exists(object primaryKey)
        {
            return this._entitySet.Find(primaryKey) != null;
        }

        /// <summary>
        /// Gets a single record by the specified criteria (usually the unique identifier)
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record that matches the specified criteria</returns>
        public Entity GetSingle(Func<Entity, bool> predicate)
        {
            return this._entitySet.Single<Entity>(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Entity GetSingleOrDefault(Func<Entity, bool> predicate)
        {
            return this._entitySet.SingleOrDefault<Entity>(predicate);
        }

        /// <summary>
        /// The first record matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record containing the first record matching the specified criteria</returns>
        public Entity GetFirst(Func<Entity, bool> predicate)
        {
            return this._entitySet.First<Entity>(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Entity GetFirstOrDefault(Func<Entity, bool> predicate)
        {
            return this._entitySet.FirstOrDefault<Entity>(predicate);
        }

        public IEnumerable<Entity> Get(
            int? pageNo = null,
            int? pageSize = null,
            Expression<Func<Entity, Entity>> selector = null,
            Expression<Func<Entity, bool>> filter = null, 
            Func<IQueryable<Entity>, IOrderedQueryable<Entity>> orderBy = null,
            string includeProperties = "")
        {
            // cannot have pageNo and pageSize with out orderBy.
            if (pageNo != null && pageSize != null && orderBy == null)
            {
                throw new Exception("cannot have pageNo and pageSize with out orderBy.");
            }

            IQueryable<Entity> query = _entitySet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            
            if (selector != null)
            {
                query = query.Select(selector);
            }                                  

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            

            if (orderBy != null)
            {
                //return orderBy(query).ToList();
                int excludedRows = (pageNo.Value - 1) * pageSize.Value;
                query = orderBy(query).Skip(excludedRows).Take(pageSize.Value);

                return query.ToList();
            }
            else
            {
                return query.ToList();
            }
        }
    }
}
