-- missing real logins
select  campusid, firstname, lastname from mydts.dbo.my_users where campusid not in (select id from dts_webdata.dbo.dts_cars_profiles)
and len(campusid) = 6
order by campusid

-- all missing users
select  campusid, firstname, lastname from mydts.dbo.my_users where campusid not in (select id from dts_webdata.dbo.dts_cars_profiles)
order by campusid



-- FIND username conflicts

select 
   count(*)
from
 mydts.dbo.forums_users fu
  inner join mydts.dbo.my_users mu
    on fu.userid = mu.userid
  inner join dts_webdata.dbo.dts_cars_profiles cu
    on mu.campusid = cu.id
where 
  fu.username <> cu.username
and cu.username <> ''


-- FIND usernames to simply port over
select 
   count(*)
from
 mydts.dbo.forums_users fu
  inner join mydts.dbo.my_users mu
    on fu.userid = mu.userid
  inner join dts_webdata.dbo.dts_cars_profiles cu
    on mu.campusid = cu.id
where 
cu.username = ''


-- FIND users with a current class (email these folks their new login information)
select 
   count(*)
from
 mydts.dbo.forums_users fu
  inner join mydts.dbo.my_users mu
    on fu.userid = mu.userid
  inner join dts_webdata.dbo.dts_cars_profiles cu
    on mu.campusid = cu.id
where 
cu.username = ''
and fu.userid in (select userid from mydts.dbo.my_courseuserlinks where courseid in (Select courseid from my_courses where sessionid = 36))

/*
-- logic
 (1) for users with a new login (johndyer) who have both accounts, simply update the forums_user.username
 (2) for users without a new login, create a new MembershipUser, then email it to them if they have a class this semester (spring 2011 = 36)

-- backend
 (1) Create new login with *.dts.edu and ID/username/email login possibilities
 (2) Double check what happens when my.dts.edu creates a user -> aspnet_users and stuff!

 */
