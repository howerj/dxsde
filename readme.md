# XML Data editor

The goal of this project is to provide a dynamically generated editor
for a [XML][] files that is generated from an [XSD][] describing the data.

## To Do

* Refactor current project and turn it into a library so it
can be reused. The refactored project will be the basis of
a rewrite.
* Rewrite project, with a sensible license, and so it is
*far* more robust and functional.
* Port project to Perl/Tk Prototype
* The dynamically generated form should look *okay* by default
(it looks terrible at the moment) and should have a degree of
customizability in terms of its look; for example the color
of integer only or string only fields, or whether a new
form pops open for a child element or whether the current form
is divided up further for new children (or other views). It
should not be too customizable as this would overwhelm the
user of the library.
* The library, when made, needs its interface rethinking. Functions
should wrap around the forms currently provided and accept and
[XSD][] and optionally an [XML][] file, and return a new [XML][]
file (or null on failure).
* Create a better icon for the desktop application version
of the project (see <http://www.xiconeditor.com/>, perhaps).
* Appropriate shortcuts, tooltips, help sections, need to
be included in the program.
* It should be possible to infer an [XSD][] from an [XML][] file, it
should also be possible to infer a new [XSD][] from a new [XML][]
file and a previously produced [XSD][]. This could be used as a quick
way to generate an [XSD][] from an unknown source. See 
<http://xmlgrid.net/xml2xsd.html> and <http://www.freeformatter.com/xsd-generator.html>
* Update **AssemblyInfo.cs** (see <https://stackoverflow.com/questions/13180543/what-is-assemblyinfo-cs-used-for>)
* Make an installer for the example program.
* Create a [CHM][] file for the project: <https://stackoverflow.com/questions/2993166/how-to-create-a-chm-file>
* A Makefile for compilation under Linux could be made, using something
like the following command:

	mcs xmlDataEditor.cs XsdEditor.cs XPathResultsDialog.cs ../EntryPoint.cs\
	/r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Data\
	/r:System.Core\
	/r:System.Data.DataSetExtensions

## License

It is believed that the original project was released under the
["Code Project Open License"][]. Future versions will rewritten
for a different license.

## Notes

This project has been modified from a project available at 
<https://www.codeproject.com/articles/3645/a-dynamically-generated-xml-data-editor>,
it has been refactored and will eventually be rewritten. The
orginal author was Marc Clifton, circa 15 Oct 2003.

- Xsd2Code, Generate C# code from an XSD, <https://xsd2code.codeplex.com/>
- Xsd.exe, Generate an XSD from C# code, <https://stackoverflow.com/questions/10017139/how-to-create-a-xsd-schema-from-a-class>
- <https://stackoverflow.com/questions/1922604/creating-a-wpf-editor-for-xml-file-based-on-schema>
- A fully featured editor (XML not XML editor from XSD?) for WPF: <https://wpfxmleditor.codeplex.com/>

### Complimentary Projects

The idea of this module would be to make very generic, reusable components for GUI
projects, instead of making custom editors for internal data structures, a single
editor can be created and potentially customized.

Other "Complimentary" Projects and "Synergy" include:

* A user drag-and-drop customizable form. The user could create their own
displays and hook them up to internal signals, they would connect displays
to text fields, buttons, dials, graphs, etcetera. The user could then save
this form, or *view*.
* Embeddable scripting languages can be combined with this editor, and the
drag-and-drop form, to make an environment that a user could customized
with ease, a very [RAD][] tool.
* Abstract classes for [XML][] backed datastructures could be made, that
automatically synchronize their contents with that on the disk at program
start up / exit.
* The built in [XML][] serialization libraries should work well this project,
and projects using [XML][] throughout.

[XSD]: https://en.wikipedia.org/wiki/XML_Schema_(W3C)
[XML]: https://en.wikipedia.org/wiki/XML
["Code Project Open License"]:  https://en.wikipedia.org/wiki/Code_Project_Open_License
[RAD]: https://en.wikipedia.org/wiki/Rapid_application_development
[CHM]: https://en.wikipedia.org/wiki/Microsoft_Compiled_HTML_Help
