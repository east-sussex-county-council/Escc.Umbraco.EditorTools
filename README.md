#Escc.Umbraco.EditorTools

Adds a new section to the Umbraco back office that displays additional information and tools for editors.

After cloning navigate to the file en.xml at Umbraco/Config/Lang/en.xml
Search the file for `<area alias="sections">` 
and add a new key `<key alias="EditorTools">Editor Tools</key>`

This makes the name of the section be displayed as Editor Tools instead of [EditorTools]

#Tools:
`Document Type Usage:` Displays a list group of all your document types and how many of each type you currently have. If you click on one, a table is generated below the list displaying the heading, type and url to the edit page for each page of that document type.
  
`Current Users:` Displays a table of all your current active users. The table gives each users name , username , email and user type. You can select multiple users and disable them all at once.  

`Disabled Users:` The exact opposite of the Current Users tab. Displays all disabled users and allows you to select multiple users and re-enable them.  

`Page Expiry Report:` Displays a table of all your pages without an expiry date, showing the pages heading and edit url.  

`Export To CSV:` Allows you to export your content tree to a csv file. The csv file contains each pages Header, Template type, Document type, Expiry Date, Edit url and live url.
  
`Media Search:` Provides a more in depth search for your media files. While still not perfect it can find more than the standard umbraco search bar.