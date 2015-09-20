RIT SG Bikeshare
================
This is the official open source software project backing bikeshare.rit.edu. This site is intended to manage all parts of the administration of a bike share program, including, but not limited to:
* Maintenance of bikes
* Legal compliance
* User management
* Checkout of bikes

License
=======
This application was developed by Nathan Castle and released under the MIT license. For more information, consult License.md.

About
=====
This Bike Share application is developed using ASP.NET MVC, with a MSSQL database and Bootstrap for styling.

Installation/Deployment
=======================
In order to install/use this software, you will need:
* Visual Studio 2013 (free for students on Dreamspark!)
* .NET 4.5
* Nuget

Setting up the server
=====================
When setting up the server, make sure the following Roles are installed
* Web Server (IIS)
  * Web Server
    * Application Development
      * Application Initialization
      * ASP.NET 4.5
Make sure the following features are installed if you want to send emails from the server.
* SMTP Server

Step-by-Step
============
* Clone the desired branch (generic or branded) to your local machine.
* Ensure that Nuget package restore is enabled and retrieve all packages
* Navigate to build/publish
* Publish to web deploy package. Don't include App_Data. Do set build config to release.
* Copy the web deploy package to the destination server
* In IIS, import package, then select the web deploy zip.
* In the deployed website directory, update web.config with desired database settings. 
* Update web.config with email settings (there are two places)
* Update nJupiter.DataAccess.ldap.config in bin with your ldap information
* Create Content/Images/Racks, and give write access to IIS_IUsers so that users can upload files.
