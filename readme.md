# aspnetcore-angular-ad
This is a template for quick start a aspnetcore with angular framework with Active Directory, based on default aspnetcore 2.0 template.

It demos the following:
1. Azure AD Authentication
2. Angular Multi-lang
3. Basic CRUD using posts only, instead of the normal get/post/put/delete
4. Refresh staying on the current page

Currently using:

    Angular 4.2.5

Road map:

    1. Upgrade to Angular 6, may reference the following: https://github.com/MarkPieszak/aspnetcore-angular2-universal 

To get demo work:

1. Change appsettings.json to use your own Azure AD authentication settings. Please see https://github.com/Azure-Samples/active-directory-dotnet-webapp-openidconnect-aspnetcore for directions.

2. Right click project name and choose manage user secrete, to give secrete such as following:

        {
            "Authentication": {
                "ClientSecret": "xxxxxxxxxxxxxxx="
            },
            "EmailConfiguration": {
                "SmtpPassword": "xxxxxxxxxxxxxx"
            }
        }

    Note, EmailConfiguration is not necessary for the demo to work.

3. Open aspnetcore-angular-ad\Data\DbInitializer.cs, change the following "live.com#xxxx@outlook.com" to your own Microsoft account, such as "live.com#myalias@outlook.com", to initialize yourself as admin during test database initialization.

        var users = new MisUser[]
        {
            new MisUser{Name="test2", IdentityName="live.com#xxxx@outlook.com", IsActive= true, IsAdmin=true },
        };

4. Run the app through VS. It should initialize the local database by its own. It should bring you to Azure AD login page.  After login, it should redirect you to the localhost's web.
