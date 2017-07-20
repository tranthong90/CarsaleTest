using CarShop.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CarShop
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext()
        //: base("AuthContext")
        {
            Database.SetInitializer(new ApplicationDbInitializer());
        }
        public class ApplicationDbInitializer : DropCreateDatabaseAlways<AuthContext>
        {
            protected override void Seed(AuthContext context)
            {
                InitializeIdentityForEF(context);
                base.Seed(context);
            }

            public static void InitializeIdentityForEF(AuthContext db)
            {

                if (!db.Users.Any())
                {
                    var roleStore = new RoleStore<IdentityRole>(db);
                    var roleManager = new RoleManager<IdentityRole>(roleStore);
                    var userStore = new UserStore<IdentityUser>(db);
                    var userManager = new UserManager<IdentityUser>(userStore);

                    // Add roles
                    var adminRole = roleManager.FindByName("AdminGroup");
                    if (adminRole == null)
                    {
                        adminRole = new IdentityRole("AdminGroup");
                        roleManager.Create(adminRole);
                    }

                    var dealerRole = roleManager.FindByName("DealerGroup");
                    if (dealerRole == null)
                    {
                        dealerRole = new IdentityRole("DealerGroup");
                        roleManager.Create(dealerRole);
                    }

                    // Create admin users
                    var adminUser = userManager.FindByName("Admin1");
                    if (adminUser == null)
                    {

                        adminUser = new IdentityUser
                        {
                            UserName = "Admin1",

                        };

                        userManager.Create(adminUser, "123456");
                        userManager.SetLockoutEnabled(adminUser.Id, false);
                        userManager.AddToRole(adminUser.Id, "AdminGroup");
                    }

                    for (int i = 1; i <= 100; i++)
                    {
                        var dealerUser = userManager.FindByName("Dealer" + i);
                        if (dealerUser == null)
                        {

                            dealerUser = new IdentityUser
                            {
                                UserName = "Dealer" + i,

                            };

                            userManager.Create(dealerUser, "123456");
                            userManager.SetLockoutEnabled(dealerUser.Id, false);
                            userManager.AddToRole(dealerUser.Id, "DealerGroup");
                        }
                    }

                }
            }

        }
    }
}