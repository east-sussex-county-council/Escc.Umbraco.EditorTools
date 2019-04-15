# Escc.Umbraco.EditorTools

Adds a new section to the Umbraco back office that displays additional information and tools for editors.

## Tools

### Users

The 'Users' tool is designed to be a quick an easy way to view all of the active and disabled users in your Umbraco installation.

This is particularly useful if a user is struggling to log in. For example they may have forgotten their username or believes their account to be disabled for some reason (Umbraco disables a user account after several failed login attempts). When not being used to troubleshoot login problems it is also a useful audit tool to see how many users you have.

### Content

The 'Content' tool queries the examine indexes and the content service for your Umbraco installation and creates a table of all of your published and unpublished content. This tool is particularly handy for audit purposes. If your site is particularly large the generation of data can take at least a minute, but is then cached for future use.

On each table for your content you are provided with the ID, Name, published URL and a link to the edit page for each content node.

You can see some basic statistics about all your content pages such as your total pages and a table of your document types. 

The process then goes a step further and generates a table for each document type to allow you to quickly view all the pages that fall under that type. On each table you can view basic information such as the pages name, id and published URL. Your also provided with a link to each pages edit page.

### Media

The 'Media' tool queries Examine for all the media files it can find and returns them to you in a table.

You can view the name, date created, the creator and the edit link for media in this table.

You can view some basic statistics about your media files, such as the total media count and a table of your file types.

### Page Expiry

The 'Page Expiry' tool allows you to view all the pages in your Umbraco installation that are published and have either an expiry date or are set never to expire (they have no expiry date). It can also tell you which pages have recently expired.

The first time the tool is opened it might take some time to load as the tool queries the content service to find expiry dates. However, once loaded, the data is cached to avoid having to do the query again. If you believe the tables to out of date you can click refresh to update the cache.

This tool is particularly useful for audit purposes and to quickly find out which pages are expiring and when, which pages will never expire, and which pages have recently expired.

Developers: This tool also makes use of Examine to speed up the generation of the report. It will add 2 fields to your InternalIndex "customIsPublished" and "customExpireDate".

### CSV Export

The 'CSV Export' tool generates a CSV file of your entire Umbraco content tree that you can download.

The CSV file contains the following properties for each page: Header, Document Type, Expiry Date, Edit URL and Live URL.

## Installation

1. Clone this repository.

2. Open in Visual Studio 2015 and up.

3. Use the `web.example.config` to create a `web.config` file.

4. Right click on the solution and click 'Restore Nuget Packages'.

5. Right click on the solution and click 'Manage Nuget Packages for Solution'.

6. Search for and install a copy of 'UmbracoCms'. (If you want to use an existing Umbraco database, make sure you install the same version of the UmbracoCms)

7. Right click on the solution and click 'Rebuild Solution'.

8. Use a web server such as IIS to host the project.

9. Navigate to your Umbraco installation in a browser eg https://localhost:8888/umbraco and follow the installation/upgrade process if it appears.

10. Go To the Users Tab

11. Find a User you want to be able to use the tools

12. Tick the 'Editor Tools' check box

13. Click Save. The user will see the new section when they refresh the page.