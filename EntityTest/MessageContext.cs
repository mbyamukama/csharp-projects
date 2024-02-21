using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityTest
{
    internal class MessageContext : DbContext
    {
        public DbSet<Message>? Messages { get; set; }

        public DbSet<MessageCollection>? MessageCollections { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;database=messagedb;user=root;password=ctzlzrzc");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.MsgID);
                entity.Property(e => e.Text);
                entity.Property(e => e.DateReceived).IsRequired();
                entity.HasOne(d => d.MessageCollection).WithMany(p => p.Messages);
            });

            modelBuilder.Entity<MessageCollection>(entity =>
            {
                entity.HasKey(e => e.UserID);
            });
        }

    }
}
