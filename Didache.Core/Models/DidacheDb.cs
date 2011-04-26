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

		// course files
		public DbSet<CourseFile> CourseFiles { get; set; }
		public DbSet<CourseFileGroup> CourseFileGroups { get; set; }
		public DbSet<CourseFileAssociation> CourseFileAssociations { get; set; }

		// studnet and graded files
		public DbSet<StudentFile> StudentFiles { get; set; }
		public DbSet<GradedFile> GradedFiles { get; set; }


		// grading
		public DbSet<GradeGroup> GradeGroups { get; set; }
		public DbSet<GradeItem> GradeItems { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder) {


			modelBuilder.Entity<User>()
				.ToTable("dts_cars_Users");


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



			// FOURMS
			modelBuilder.Entity<Forum>()
				.ToTable("oe_Forums");

			modelBuilder.Entity<Thread>()
				.ToTable("oe_Forums_Threads");

			modelBuilder.Entity<Post>()
				.ToTable("oe_Forums_Posts");


			// interactions
			modelBuilder.Entity<InteractionPost>()
				.ToTable("oe_Interactions_Posts");
			modelBuilder.Entity<InteractionThread>()
				.ToTable("oe_Interactions_Threads");


			// course files
			modelBuilder.Entity<CourseFile>()
				.ToTable("oe_CourseFiles");
			modelBuilder.Entity<CourseFileGroup>()
				.ToTable("oe_CourseFileGroups");
			modelBuilder.Entity<CourseFileAssociation>()
				.HasKey(cfa => new { cfa.GroupID, cfa.FileID })
				.ToTable("oe_CourseFileGroups_Files");

			// student and graded
			modelBuilder.Entity<StudentFile>()
				.ToTable("oe_StudentFiles");
			modelBuilder.Entity<GradedFile>()
				.ToTable("oe_GradedFiles");


			// grading system
			modelBuilder.Entity<GradeGroup>()
				.ToTable("oe_GradeGroups");
			modelBuilder.Entity<GradeItem>()
				.ToTable("oe_GradeItems");

			// map example
			/*
			modelBuilder.Entity<WatchList>().HasMany(w => w.Securities)
				   .WithMany()
				   .Map(map => map.ToTable("WatchListSecurity")
				   .MapRightKey("SecurityId")
				   .MapLeftKey("WatchListId"));
			*/

			//http://johnpapa.net/silverlight/upgrading-to-entity-framework-4-1-rc/
			// possible silliness
			//modelBuilder.Entity<Security>().ToTable("Securities"); 
		}
	}



}
