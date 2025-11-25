using Microsoft.EntityFrameworkCore;
using Kanini.LMP.Data.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Reflection;
using Kanini.LMP.Data.Data;

namespace Kanini.LMP.Data.Repositories.Implementations
{
    public class LMPRepositoy<T, TId> : ILMPRepository<T, TId> where T : class
    {
        protected readonly LmpDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public LMPRepositoy(LmpDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null)
        {
            var query = _dbSet.AsNoTracking();

            if (predicate != null)
                query = query.Where(predicate);

            return await query.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(TId id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<T> UpdateAsync(T entity, Expression<Func<T, bool>>? predicate = null)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            T? existingEntity;

            if (predicate != null)
            {
                // Update by predicate
                existingEntity = await _dbSet.FirstOrDefaultAsync(predicate);
                if (existingEntity == null)
                    throw new ArgumentException($"{typeof(T)} not found with the given predicate");
            }
            else
            {
                // Update by ID (default behavior)
                var keyProperty = GetKeyProperty();
                var keyValue = keyProperty.GetValue(entity);
                existingEntity = await _dbSet.FindAsync(keyValue);
                if (existingEntity == null)
                    throw new ArgumentException($"{typeof(T)} with ID {keyValue} not found");
            }

            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return existingEntity;
        }

        public virtual async Task DeleteAsync(TId id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            var entities = await _dbSet.Where(predicate).ToListAsync();
            if (entities.Any())
            {
                _dbSet.RemoveRange(entities);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null
                ? await _dbSet.CountAsync()
                : await _dbSet.CountAsync(predicate);
        }

        private PropertyInfo GetKeyProperty()
        {
            var keyProperty = typeof(T).GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true).Length > 0);

            return keyProperty ?? throw new InvalidOperationException($"Entity does not have a Key attribute");
        }
    }
}