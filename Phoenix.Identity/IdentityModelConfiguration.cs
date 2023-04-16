using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Phoenix.Data.Repository;
using Phoenix.RouteAuthorization;
using System;
using System.Linq;

namespace Phoenix.Identity
{
    public class IdentityModelConfiguration<TUser, TRole> : IdentityModelConfiguration<TUser, TRole, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
        where TUser : UserBase
        where TRole : RoleBase
    {
        public IdentityModelConfiguration(IServiceProvider serviceProvider, IDbContextOptions dbContextOptions)
            : base(serviceProvider, dbContextOptions)
        {
        }
    }

    
    public class IdentityModelConfiguration<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IModelConfiguration
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDbContextOptions _dbContextOptions;

        public IdentityModelConfiguration(IServiceProvider serviceProvider, IDbContextOptions dbContextOptions)
        {
            _serviceProvider = serviceProvider;
            _dbContextOptions = dbContextOptions;
        }

        public void Configure(ModelBuilder builder)
        {
            var storeOptions = GetStoreOptions();
            var maxKeyLength = storeOptions?.MaxLengthForKeys ?? 0;
            var encryptPersonalData = storeOptions?.ProtectPersonalData ?? false;
            PersonalDataConverter converter = null;

            builder.Entity<TUser>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(u => u.NormalizedUserName).HasName("UserNameIndex").IsUnique();
                b.HasIndex(u => u.NormalizedEmail).HasName("EmailIndex");
                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                b.Property(u => u.UserName).HasMaxLength(256);
                b.Property(u => u.NormalizedUserName).HasMaxLength(256);
                b.Property(u => u.Email).HasMaxLength(256);
                b.Property(u => u.NormalizedEmail).HasMaxLength(256);

                if (encryptPersonalData)
                {
                    converter = new PersonalDataConverter(_serviceProvider.GetService<IPersonalDataProtector>());
                    var personalDataProps = typeof(TUser).GetProperties().Where(
                                    prop => Attribute.IsDefined(prop, typeof(ProtectedPersonalDataAttribute)));
                    foreach (var p in personalDataProps)
                    {
                        if (p.PropertyType != typeof(string))
                        {
                            throw new InvalidOperationException("[ProtectedPersonalData] only works strings by default.");
                        }
                        b.Property(typeof(string), p.Name).HasConversion(converter);
                    }
                }

             //   b.HasMany<TUserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
                b.HasMany<TUserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();
                b.HasMany<TUserLogin>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();
                b.HasMany<TUserToken>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();

            });

            builder.Entity<TRole>(b =>
            {
                b.HasKey(r => r.Id);
                b.HasIndex(r => r.NormalizedName).HasName("RoleNameIndex").IsUnique();
                b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

                b.Property(u => u.Name).HasMaxLength(256);
                b.Property(u => u.NormalizedName).HasMaxLength(256);

                b.HasMany<TUserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
                b.HasMany<TRoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
            });

            builder.Entity<TRoleClaim>(b =>
            {
                b.HasKey(rc => rc.Id);
            });

            builder.Entity<TUserRole>(b =>
            {
                b.HasKey(r => new { r.UserId, r.RoleId });
            });

            builder.Entity<TUserClaim>(b =>
            {
                b.HasKey(uc => uc.Id);
            });

            builder.Entity<TUserLogin>(b =>
            {
                b.HasKey(l => new { l.LoginProvider, l.ProviderKey });

                if (maxKeyLength > 0)
                {
                    b.Property(l => l.LoginProvider).HasMaxLength(maxKeyLength);
                    b.Property(l => l.ProviderKey).HasMaxLength(maxKeyLength);
                }
            });

            builder.Entity<TUserToken>(b =>
            {
                b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

                if (maxKeyLength > 0)
                {
                    b.Property(t => t.LoginProvider).HasMaxLength(maxKeyLength);
                    b.Property(t => t.Name).HasMaxLength(maxKeyLength);
                }

                if (encryptPersonalData)
                {
                    var tokenProps = typeof(TUserToken).GetProperties().Where(
                                    prop => Attribute.IsDefined(prop, typeof(ProtectedPersonalDataAttribute)));
                    foreach (var p in tokenProps)
                    {
                        if (p.PropertyType != typeof(string))
                        {
                            throw new InvalidOperationException("[ProtectedPersonalData] only works strings by default.");
                        }
                        b.Property(typeof(string), p.Name).HasConversion(converter);
                    }
                }
            });


            // پیکربندی احرازهویت
            //builder.Entity<RoleAuthorizedRoute>(b =>
            //{
            //    b.HasKey(x => new { x.RoleId, x.RouteId });
            //   b.ToTable("RoleAuthorizedRoutes");
            //});

            //builder.Entity<UserAuthorizedRoute>(b =>
            //{
            //    b.HasKey(x => new { x.UserId, x.RouteId });
            //    b.ToTable("UserAuthorizedRoutes");
            //});
            //builder.Entity<Route>(b =>
            //{
            //    b.HasKey(x => x.Id);

            //    b.HasMany(x => x.AuthorizedRoles).WithOne().HasForeignKey(x => x.RouteId).IsRequired();
            //    b.HasMany(x => x.AuthorizedUsers).WithOne().HasForeignKey(x => x.RouteId).IsRequired();

            //});


            var routeEntityConfigure = new RouteAuthorizationEntityConfigure();
            routeEntityConfigure.Configure(builder);

            builder.Entity<UserAuthorizedRoute>(b =>
            {
                b.HasOne<TUser>().WithMany().HasForeignKey(x => x.UserId).IsRequired();
            });

            builder.Entity<RoleAuthorizedRoute>(b =>
            {
                b.HasOne<TRole>().WithMany().HasForeignKey(x => x.RoleId).IsRequired();
            });
            builder.Entity<UserNameBanList>(b =>
            {
                b.HasIndex(x => x.Id).IsUnique();
            });
            builder.Entity<EmailBanList>(b =>
            {
                b.HasIndex(x => x.Id).IsUnique();
            });
            builder.Entity<PasswordBanList>(b =>
            {
                b.HasIndex(x => x.Id).IsUnique();
            });
            builder.Entity<LoginHistory>(b =>
            {
                b.HasIndex(x => x.Id).IsUnique();
            });

            //builder.Entity<UserRouteAuthorization>(b =>
            //{
            //    b.HasKey(x => new { x.RouteId, x.UserId });
            //});

            //builder.Entity<RoleRouteAuthorization>(b =>
            //{
            //    b.HasKey(x => new { x.RouteId, x.RoleId });
            //});

            //builder.Entity<UserOperationAuthorization>(b =>
            //{
            //    b.HasKey(x => new { x.RouteId, x.UserId });
            //});

            //builder.Entity<RoleOperationAuthorization>(b =>
            //{
            //    b.HasKey(x => new { x.RouteId, x.RoleId });
            //});

            //builder.Entity<Route>(b =>
            //{
            //    b.HasKey(x => x.Id);
            //    b.HasIndex(x => x.HashCode).IsUnique();
            //    b.Property(x => x.HashCode).HasMaxLength(250).IsRequired();
            //    b.Property(x => x.RequestRate).HasDefaultValue(5);

            //    b.HasMany(x => x.AuthorizedRoles).WithOne().HasForeignKey(x => x.RouteId).IsRequired();
            //    b.HasMany(x => x.AuthorizedUsers).WithOne().HasForeignKey(x => x.RouteId).IsRequired();

            //    b.HasMany(x => x.UserOperations).WithOne().HasForeignKey(x => x.RouteId).IsRequired();
            //    b.HasMany(x => x.RoleOperations).WithOne().HasForeignKey(x => x.RouteId).IsRequired();
            //});

        }


        private StoreOptions GetStoreOptions() => _dbContextOptions
                           .Extensions.OfType<CoreOptionsExtension>()
                           .FirstOrDefault()?.ApplicationServiceProvider
                           ?.GetService<IOptions<IdentityOptions>>()
                           ?.Value?.Stores;

        private class PersonalDataConverter : ValueConverter<string, string>
        {
            public PersonalDataConverter(IPersonalDataProtector protector) : base(s => protector.Protect(s), s => protector.Unprotect(s), default)
            { }
        }

    }

}
