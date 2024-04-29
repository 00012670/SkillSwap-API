using BISP_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BISP_API.Context
{
    public class BISPdbContext : DbContext
    {
        public BISPdbContext(DbContextOptions<BISPdbContext> o) : base(o)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<SwapRequest> SwapRequests { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<SkillImage> SkillImages { get; set; }


        public override int SaveChanges()
        {
            HandleSoftDelete();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            HandleSoftDelete();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void HandleSoftDelete()
        {
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity is SwapRequest && entry.State == EntityState.Deleted))
            {
                entry.State = EntityState.Modified;
                entry.CurrentValues["IsDeleted"] = true;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<SwapRequest>()
                 .HasOne(sr => sr.Initiator)
                 .WithMany(u => u.SwapRequestsInitiated)
                 .HasForeignKey(sr => sr.InitiatorId)
                 .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SwapRequest>()
                .HasOne(sr => sr.Receiver)
                .WithMany(u => u.SwapRequestsReceived)
                .HasForeignKey(sr => sr.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SwapRequest>()
                .HasOne(sr => sr.SkillOffered)
                .WithMany(s => s.SwapRequestsOffered)
                .HasForeignKey(sr => sr.SkillOfferedId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SwapRequest>()
                .HasOne(sr => sr.SkillRequested)
                .WithMany(s => s.SwapRequestsExchanged)
                .HasForeignKey(sr => sr.SkillRequestedId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.FromUser)
                .WithMany(u => u.ReviewsSent)
                .HasForeignKey(r => r.FromUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.ToUser)
                .WithMany(u => u.ReviewsReceived)
                .HasForeignKey(r => r.ToUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Skill)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.SkillId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Request)
                .WithMany(sr => sr.Reviews)
                .HasForeignKey(r => r.RequestId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevents cascade delete

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Message)
                .WithMany(m => m.Notifications)
                .HasForeignKey(n => n.MessageId)
                .OnDelete(DeleteBehavior.Restrict);  

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.SwapRequest)
                .WithMany(sr => sr.Notifications)
                .HasForeignKey(n => n.SwapRequestId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

