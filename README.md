#Escc.Umbraco.EditorTools

Adds a new section to the Umbraco back office that displays additional information and tools for editors.

After cloning navigate to the file en.xml at Umbraco/Config/Lang/en.xml
Search the file for `<area alias="sections">` 
and add a new key `<key alias="EditorTools">Editor Tools</key>`

This makes the name of the section be displayed as Editor Tools instead of [EditorTools]