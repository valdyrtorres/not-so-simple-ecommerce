﻿using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SimpleEcommerceV2.IdentityServer.Domain.Models;

namespace SimpleEcommerceV2.IdentityServer.Domain.Repositories.Contexts
{
    public sealed class IdentityServerContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public IdentityServerContext(DbContextOptions<IdentityServerContext> options, IConfiguration configuration)
        : base(options)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public DbSet<UserEntity> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>().HasData(
                new UserEntity
                {
                    Id = 1,
                    Email = "admin@simple-ecommerce.com",
                    Password =Encoding.UTF8.GetString(SHA256.HashData(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Identity:User:Password"))))
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}