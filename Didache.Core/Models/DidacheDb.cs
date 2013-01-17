using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
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
		public DbSet<ForumPost> ForumPosts { get; set; }
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
		public DbSet<UnitSurvey> UnitSurveys { get; set; }


		// grading
		public DbSet<GradeGroup> GradeGroups { get; set; }
		public DbSet<GradeItem> GradeItems { get; set; }

		// additional user data
		public DbSet<Student> Students { get; set; }
		public DbSet<Employee> Employees { get; set; }
		public DbSet<Degree> Degrees { get; set; }
		public DbSet<AlumniInfo> AlumniInfos { get; set; }

		// community
		public DbSet<UserRelationship> UserRelationships { get; set; }
		public DbSet<CarsCourse> CarsCourses { get; set; }
		public DbSet<UserAction> UserActions { get; set; }
		public DbSet<UserPost> UserPosts { get; set; }
		public DbSet<UserPostComment> UserPostComments { get; set; }

		public DbSet<DiscussionGroup> DiscussionGroups { get; set; }
		public DbSet<DiscussionGroupMember> DiscussionGroupMembers { get; set; }


		// DTS Stuff
		public DbSet<CarsRelationship> CarsRelationships { get; set; }
		public DbSet<FamilyMember> FamilyMember { get; set; }
		public DbSet<Workplace> Workplaces { get; set; }
		public DbSet<WorkplaceWorker> WorkplaceWorkers { get; set; }

		

		// quetsions
		public DbSet<Announcement> Announcements { get; set; }
		public DbSet<HelpQuestion> HelpQuestions { get; set; }
		public DbSet<HelpCategory> HelpCategories { get; set; }


		protected override void OnModelCreating(DbModelBuilder modelBuilder) {


			modelBuilder.Entity<User>()
				.ToTable("dts_cars_Users");

			modelBuilder.Entity<User>()
				.Property(u => u.UserID)
					.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

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
				.HasKey(cu => new { cu.CourseID, cu.UserID, cu.RoleID })
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

			modelBuilder.Entity<ForumPost>()
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
				.ToTable("oe_GradeGroups_Items");

			modelBuilder.Entity<UnitSurvey>()
				.ToTable("oe_UnitSurveys");

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


			modelBuilder.Entity<Employee>()
				.ToTable("dts_cars_Users_Employees");

			modelBuilder.Entity<Student>()
				.ToTable("dts_cars_Users_Students");
			modelBuilder.Entity<AlumniInfo>()
				.ToTable("dts_cars_Users_Alumni");


			/*
			modelBuilder.Entity<User>()
				.HasOptional(u => u.Students);
			modelBuilder.Entity<Student>()
				.HasRequired(s => s.User);*/

			modelBuilder.Entity<Degree>()
				.ToTable("dts_cars_Degrees");

			// awesome
			modelBuilder.Entity<UserRelationship>()
				.HasKey(ur => new { ur.RequesterUserID, ur.TargetUserID })
				.ToTable("oe_UserRelationships");

			modelBuilder.Entity<CarsCourse>()
				.HasKey(ur => new { ur.UserID, ur.CourseCode })
				.ToTable("dts_cars_Courses_Students");

			modelBuilder.Entity<UserAction>()
				.ToTable("oe_UserActions");

			// children, spouses
			modelBuilder.Entity<CarsRelationship>()
				.ToTable("dts_cars_Relationships");
			modelBuilder.Entity<FamilyMember>()
				.ToTable("dts_cars_Family");

			modelBuilder.Entity<Workplace>()
				.ToTable("dts_cars_Workplaces");
			modelBuilder.Entity<WorkplaceWorker>()
				.ToTable("dts_cars_Workplaces_Workers");


			// posts!

			modelBuilder.Entity<UserPost>()
				.ToTable("oe_UserPosts");
			modelBuilder.Entity<UserPostComment>()
				.ToTable("oe_UserPosts_Comments");

			modelBuilder.Entity<DiscussionGroup>()
				.ToTable("oe_DiscussionGroups");
			modelBuilder.Entity<DiscussionGroupMember>()
				.HasKey(dgu => new { dgu.UserID, dgu.GroupID })
				.ToTable("oe_DiscussionGroups_Users");



			modelBuilder.Entity<Announcement>()
				.ToTable("oe_Announcements");

			modelBuilder.Entity<HelpQuestion>()
				.ToTable("oe_HelpQuestions");
			
			modelBuilder.Entity<HelpCategory>()
				.ToTable("oe_HelpCategories");

		}
	}



}
