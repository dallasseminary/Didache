using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations;


namespace Didache {
	public class DidacheDb : DbContext  {

		// uses "didache" connection string
		public DidacheDb() : base("didache") {

		}

		// course meta
		public DbSet<Session> Sessions { get; set; }
		public DbSet<Campus> Campuses { get; set; }

		// courses
		public DbSet<Course> Courses { get; set; }
		public DbSet<Unit> Units { get; set; }
		public DbSet<Task> Tasks { get; set; }
		public DbSet<CourseUserGroup> CourseUserGroups { get; set; }
		public DbSet<UserTaskData> UserTasks { get; set; }

		// people
		public DbSet<User> Users { get; set; }
		public DbSet<CourseUser> CourseUsers { get; set; }

		// forums
		public DbSet<Forum> Forums { get; set; }
		public DbSet<Post> Posts { get; set; }
		public DbSet<Thread> Threads { get; set; }


		// interactions
		public DbSet<InteractionPost> InteractionPosts { get; set; }
		public DbSet<InteractionThread> InteractionThreads { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			// TODO: Use Fluent API Here 

			modelBuilder.Entity<Session>()
				.ToTable("oe_Sessions");

			modelBuilder.Entity<Campus>()
				.ToTable("oe_Campuses");

			modelBuilder.Entity<Course>()
				.ToTable("oe_Courses");
			
			modelBuilder.Entity<Unit>()
				.ToTable("oe_Courses_Units");

			modelBuilder.Entity<Task>()
				.ToTable("oe_Courses_Tasks");



			//modelBuilder.Entity<CourseUser>()
			//	.Property(u => u.ID).HasColumnName("UserID");

			modelBuilder.Entity<CourseUser>()
				.HasKey(cu => new { cu.CourseID, cu.GroupID, cu.UserID, cu.RoleID })
				.ToTable("oe_Courses_Users");

			
			/*
			modelBuilder.Entity<CourseUserGroup>()
				.HasMany(g=>g.Students).WithMany()
				.Map(m =>
				{
					m.MapLeftKey(g => g.GroupID, "GroupID");
					m.MapRightKey(p => p.UserID, "UserID");
				});
			*/

			modelBuilder.Entity<CourseUserGroup>()
				.ToTable("oe_Courses_UserGroups");
	
			modelBuilder.Entity<UserTaskData>()
				.ToTable("oe_Courses_Tasks_UserData");

			/*
			modelBuilder.Entity<User>()
				.Property(u => u.UserID).HasColumnName("ID");
			modelBuilder.Entity<User>()
				.Property(u => u.LoginUserID).HasColumnName("UserID");
			*/
			modelBuilder.Entity<User>()
				.ToTable("dts_cars_Users");


			// FOURMS
			modelBuilder.Entity<Forum>()
				.ToTable("oe_Forums");

			modelBuilder.Entity<Thread>()
				.ToTable("oe_Forums_Threads");

			modelBuilder.Entity<Post>()
				.ToTable("oe_Forums_Posts");


			// interactions

			
		}
	}



}
