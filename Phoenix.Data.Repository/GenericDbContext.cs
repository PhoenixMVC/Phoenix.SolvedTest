using Phoenix.Pluralization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Phoenix.Data.Repository.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TypeExtensions = Phoenix.Data.Repository.Internal.TypeExtensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;
using Phoenix.Common.Utilities;

namespace Phoenix.Data.Repository
{
    public class GenericDbContext : DbContext, IDbContext
    {
        public GenericDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var repositoryOptions = this.GetInfrastructure().GetRequiredService<IOptions<RepositoryOptions>>().Value;
            var modelConfigurations = this.GetInfrastructure().GetRequiredService<IEnumerable<IModelConfiguration>>();

            base.OnModelCreating(modelBuilder);

            var entityConfigurations = repositoryOptions
                .Assemblies
                .SelectMany(a => a.GetTypes())
                .Where(TypeExtensions.IsEntityConfigure);

            var method = typeof(ModelBuilder)
                                .GetMethods()
                                .First(m => m.Name == nameof(ModelBuilder.ApplyConfiguration) &&
                                                            m.GetParameters().Any(p => p.ParameterType.IsGenericType &&
                                                            p.ParameterType.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

            foreach (var configureType in entityConfigurations)
            {
                var entityType = configureType.ExtractEntityType();
                method.MakeGenericMethod(entityType).Invoke(modelBuilder, new[] { Activator.CreateInstance(configureType) });
            }

            foreach (var configure in modelConfigurations)
            {
                configure.Configure(modelBuilder);
            }

            foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()))
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            var pluralizer = new EnglishPluralizationService();
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                // var tableName = entity.Relational().TableName;
                if (!pluralizer.IsPlural(tableName))
                {
                    entity.SetTableName(pluralizer.Pluralize(tableName));
                    // entity.Relational().TableName = pluralizer.Pluralize(tableName);
                }
            }
        }

        public IDbContextTransaction BeginTransaction() => Database.BeginTransaction();

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
            Database.BeginTransactionAsync(cancellationToken);

        public IQueryable<TEntity> DbSet<TEntity>() where TEntity : class => Set<TEntity>();

        public void Migrate() => Database.Migrate();

        public Task MigrateAsync(CancellationToken cancellationToken = default) => Database.MigrateAsync(cancellationToken);

        public void SetCommandTimeout(TimeSpan timeout) => Database.SetCommandTimeout(timeout);

        public void SetCommandTimeout(int? timeout) => Database.SetCommandTimeout(timeout);

        public Task RawSqlQueryAsync(string query, Action<DbDataReader> action)
        {
            return RawSqlQueryAsync(query, null, action);
        }

        public async Task RawSqlQueryAsync(string query, Dictionary<string, object> parameters, Action<DbDataReader> action)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException(nameof(query));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            using var connection = Database.GetDbConnection();
            using var command = Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            if (parameters != null)
            {
                DbParameter parameter;
                foreach (var item in parameters)
                {
                    parameter = command.CreateParameter();
                    parameter.ParameterName = item.Key;
                    parameter.Value = item.Value ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                }
            }

            await connection.OpenAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                action(reader);
                reader.Close();
            }

            connection.Close();

        }

        //Task<EntityEntry> IDbContext.AddAsync(object entity, CancellationToken cancellationToken)
        //{
        //    return AddAsync(entity, cancellationToken).AsTask();
        //}

        Task<TEntity> IDbContext.FindAsync<TEntity>(params object[] keyValues)
        {
            return FindAsync<TEntity>(keyValues).AsTask();
        }

        Task<object> IDbContext.FindAsync(Type entityType, object[] keyValues, CancellationToken cancellationToken)
        {
            return FindAsync(entityType, keyValues, cancellationToken).AsTask();
        }

        Task<object> IDbContext.FindAsync(Type entityType, params object[] keyValues)
        {
            return FindAsync(entityType, keyValues).AsTask();
        }

        Task<TEntity> IDbContext.FindAsync<TEntity>(object[] keyValues, CancellationToken cancellationToken)
        {
            return FindAsync<TEntity>(keyValues, cancellationToken).AsTask();
        }


        int IDbContext.SaveChanges()
        {
            _cleanString();
            return base.SaveChanges();
        }

        int IDbContext.SaveChanges(bool acceptAllChangesOnSuccess)
        {
            _cleanString();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        Task<int> IDbContext.SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
        {
            _cleanString();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        Task<int> IDbContext.SaveChangesAsync(CancellationToken cancellationToken)
        {
            _cleanString();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void _cleanString()
        {
            var changedEntities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
            foreach (var item in changedEntities)
            {
                if (item.Entity == null)
                    continue;

                var properties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

                foreach (var property in properties)
                {
                    var propName = property.Name;
                    var val = (string)property.GetValue(item.Entity, null);

                    if (val.HasValue())
                    {
                        var newVal = val.Fa2En().FixPersianChars();
                        if (newVal == val)
                            continue;
                        property.SetValue(item.Entity, newVal, null);
                    }
                }
            }
        }

    }

}
