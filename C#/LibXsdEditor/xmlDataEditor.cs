/**
 * This project was originaly downloaded from https://www.codeproject.com/articles/3645/a-dynamically-generated-xml-data-editor,
 * under the title "A Dynamically Generated XML Data Editor". It's purpose is to dynamically generate
 * an editor for XML documents given the XSD that represents the structure of those documents, it does
 * this using WinForms.
 * 
 * LICENSE: Unknown / The Code Project Open License? https://en.wikipedia.org/wiki/Code_Project_Open_License
 * AUTHOR:  Marc Clifton, 15 Oct 2003
 * 
 * The code in this project has been modified from the original, by Richard James Howe, it is
 * not the original code.
 * 
 * TODO:
 *  - Refactor (renaming, proper comments, clean up code and make more robust)
 *  - Once refactored, and the essence deduced, a complete ground up rewrite as a library
 *  should be made.
 *  - Again, once refactored, a Perl/Tk prototype would also be useful.
 *  - Add references
 *  - Many of the magic numbers relating to GUI element placement need to be factored out (this
 *  would aide in making the interface more customizable as well).
 *  - Split the logic and display code into two separate things.
 *  - This form should be split into a (optional) top level form and (optional) subforms.
 *  - Which way around do the X and Y axis go?
 *  - The internals need updating
 *  - Configuration options (the editor for which could be this XSD editor - talk about "dog fooding")
 *  - More complex examples need creating
 *  - Assertions and ways of automatically testing things need doing (how to perform unit tests
 *  on GUI components?).
 *  
 *  
 * ===================================================================================================================================
 * Original Description:
 * 
 * An XSD-driven dynamic form creator and XML data editor.
 * Using an Xml Schema Definition (XSD) document, this utility dynamically generates a data entry form to create and edit XML data.
 * 
 * Things to do:
 * 
 * Implement a "Close" menu item (closes both XSD and XML)
 * Look at XmlSchemaSimpleType and derived classes like XmlSchemaSimpleTypeList
 * 
 * Bugs:
 * 
 * A simple type that references a simple type with enumerations doesn't show up as a combo box.  Why?
 * A local element ref'ing a global element type'ing a complex element caused an assertion.  Why?
 * An element type'd as a complex type is causing problems
 * 
 * In the Schema Editor:
 *	1.	Can't remove elements of simple type (simple types that are a children to a complex type)
 *	2.	missing feature: DEL key to remove nodes
 *	3.	Renaming a complex type of a complex type caused the child complex type to have a name, instead of the parent element of complex type
 *	4.	Did min inclusive create a max inclusive node instead?
 *	5.	If a simple type has a global simple type restriction base (instead of a built-in one) it doesn't show up in the global list
 * ===================================================================================================================================
 */

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace LibXsdEditor
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class XmlDataEditorForm : Form
    {
        private Panel _dynamicForm;
        private MainMenu _mainMenu;

        private OpenFileDialog _openFileDialog;
        private SaveFileDialog _saveXmlAsDlg;

        private MenuItem _menuFile;
        private MenuItem _menuOpenXsd;
        private MenuItem _menuOpenXml;
        private MenuItem _menuInferXsd;
        private MenuItem _menuItemDivider1;
        private MenuItem _menuSaveXml;
        private MenuItem _menuSaveXmlAs;
        private MenuItem _menuItemDivider2; 
        private MenuItem _menuExit;

        private MenuItem _menuAbout;

        private XmlSchema _schema;
        private XmlDataDocument _doc;
        private string _xmlFileName;
        private Hashtable _tableInfo = new Hashtable();

        private Label _xPathLabel; 
        private Button _buttonXPathQuery;
        private TextBox _editableXPath;
        private Label _newRowOnLabel; 
        private TextBox _editableNewRowOn;

        private System.ComponentModel.IContainer components;

        public XmlDataEditorForm()
        {
            InitializeComponent(); // Required for Windows Form Designer support
        }

        /// <summary>
        /// Clean up any resources being used, this needs to
        /// be called.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._dynamicForm = new System.Windows.Forms.Panel();
            this._mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this._menuFile = new System.Windows.Forms.MenuItem();
            this._menuOpenXsd = new System.Windows.Forms.MenuItem();
            this._menuInferXsd = new System.Windows.Forms.MenuItem();
            this._menuOpenXml = new System.Windows.Forms.MenuItem();
            this._menuItemDivider1 = new System.Windows.Forms.MenuItem();
            this._menuSaveXml = new System.Windows.Forms.MenuItem();
            this._menuSaveXmlAs = new System.Windows.Forms.MenuItem();
            this._menuItemDivider2 = new System.Windows.Forms.MenuItem();
            this._menuExit = new System.Windows.Forms.MenuItem();
            this._menuAbout = new System.Windows.Forms.MenuItem();
            this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this._saveXmlAsDlg = new System.Windows.Forms.SaveFileDialog();
            this._xPathLabel = new System.Windows.Forms.Label();
            this._editableXPath = new System.Windows.Forms.TextBox();
            this._buttonXPathQuery = new System.Windows.Forms.Button();
            this._newRowOnLabel = new System.Windows.Forms.Label();
            this._editableNewRowOn = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _dynamicForm
            // 
            this._dynamicForm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dynamicForm.AutoScroll = true;
            this._dynamicForm.Location = new System.Drawing.Point(0, 80);
            this._dynamicForm.Name = "_dynamicForm";
            this._dynamicForm.Size = new System.Drawing.Size(528, 704);
            this._dynamicForm.TabIndex = 0;
            // 
            // _mainMenu
            // 
            this._mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._menuFile,
            this._menuAbout});
            // 
            // _menuFile
            // 
            this._menuFile.Index = 0;
            this._menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._menuOpenXsd,
            this._menuInferXsd,
            this._menuOpenXml,
            this._menuItemDivider1,
            this._menuSaveXml,
            this._menuSaveXmlAs,
            this._menuItemDivider2,
            this._menuExit});
            this._menuFile.Text = "File";
            // 
            // _menuOpenXsd
            // 
            this._menuOpenXsd.Index = 0;
            this._menuOpenXsd.Shortcut = System.Windows.Forms.Shortcut.CtrlD;
            this._menuOpenXsd.Text = "Open XS&D";
            this._menuOpenXsd.Click += new System.EventHandler(this.mnuOpenXSD_Click);
            // 
            // _menuInferXsd
            // 
            this._menuInferXsd.Enabled = false;
            this._menuInferXsd.Index = 1;
            this._menuInferXsd.Text = "&Infer XSD";
            this._menuInferXsd.Click += new System.EventHandler(this.mnuInferXSD_Click);
            // 
            // _menuOpenXml
            // 
            this._menuOpenXml.Index = 2;
            this._menuOpenXml.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this._menuOpenXml.Text = "Open XM&L";
            this._menuOpenXml.Click += new System.EventHandler(this.mnuOpenXML_Click);
            // 
            // _menuItemDivider1
            // 
            this._menuItemDivider1.Index = 3;
            this._menuItemDivider1.Text = "-";
            // 
            // _menuSaveXml
            // 
            this._menuSaveXml.Index = 4;
            this._menuSaveXml.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this._menuSaveXml.Text = "&Save XML";
            this._menuSaveXml.Click += new System.EventHandler(this.mnuSaveXML_Click);
            // 
            // _menuSaveXmlAs
            // 
            this._menuSaveXmlAs.Index = 5;
            this._menuSaveXmlAs.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
            this._menuSaveXmlAs.Text = "Save XML &As";
            this._menuSaveXmlAs.Click += new System.EventHandler(this.mnuSaveXmlAs_Click);
            // 
            // _menuItemDivider2
            // 
            this._menuItemDivider2.Index = 6;
            this._menuItemDivider2.Text = "-";
            // 
            // _menuExit
            // 
            this._menuExit.Index = 7;
            this._menuExit.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
            this._menuExit.Text = "E&xit";
            this._menuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // _menuAbout
            // 
            this._menuAbout.Index = 1;
            this._menuAbout.Text = "&About";
            // 
            // _openFileDialog
            // 
            this._openFileDialog.DefaultExt = "xsd";
            // 
            // _saveXmlAsDlg
            // 
            this._saveXmlAsDlg.DefaultExt = "xml";
            // 
            // _xPathLabel
            // 
            this._xPathLabel.Location = new System.Drawing.Point(8, 16);
            this._xPathLabel.Name = "_xPathLabel";
            this._xPathLabel.Size = new System.Drawing.Size(48, 16);
            this._xPathLabel.TabIndex = 1;
            this._xPathLabel.Text = "XPath:";
            this._xPathLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // _editableXPath
            // 
            this._editableXPath.Location = new System.Drawing.Point(56, 16);
            this._editableXPath.Name = "_editableXPath";
            this._editableXPath.Size = new System.Drawing.Size(400, 20);
            this._editableXPath.TabIndex = 2;
            // 
            // _buttonXPathQuery
            // 
            this._buttonXPathQuery.Location = new System.Drawing.Point(464, 16);
            this._buttonXPathQuery.Name = "_buttonXPathQuery";
            this._buttonXPathQuery.Size = new System.Drawing.Size(48, 20);
            this._buttonXPathQuery.TabIndex = 3;
            this._buttonXPathQuery.Text = "Query";
            this._buttonXPathQuery.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this._buttonXPathQuery.Click += new System.EventHandler(this.btnXpathQuery_Click);
            // 
            // _newRowOnLabel
            // 
            this._newRowOnLabel.Location = new System.Drawing.Point(8, 48);
            this._newRowOnLabel.Name = "_newRowOnLabel";
            this._newRowOnLabel.Size = new System.Drawing.Size(98, 16);
            this._newRowOnLabel.TabIndex = 4;
            this._newRowOnLabel.Text = "New Row On:";
            this._newRowOnLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // _editableNewRowOn
            // 
            this._editableNewRowOn.Location = new System.Drawing.Point(96, 48);
            this._editableNewRowOn.Name = "_editableNewRowOn";
            this._editableNewRowOn.Size = new System.Drawing.Size(144, 20);
            this._editableNewRowOn.TabIndex = 5;
            // 
            // XmlDataEditorForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(536, 793);
            this.Controls.Add(this._editableNewRowOn);
            this.Controls.Add(this._newRowOnLabel);
            this.Controls.Add(this._buttonXPathQuery);
            this.Controls.Add(this._editableXPath);
            this.Controls.Add(this._xPathLabel);
            this.Controls.Add(this._dynamicForm);
            this.Menu = this._mainMenu;
            this.Name = "XmlDataEditorForm";
            this.Text = "XML Data Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public class TypeInfo
        {
            public TypeInfo(string typeName)
            {
                TypeName = typeName;
            }

            public string TypeName = "";

            // facet information
            public string Length = "-1";
            public string MinLength = "-1";
            public string MaxLength = "-1";
            public string Pattern = "";
            public string MaxInclusive;
            public string MaxExclusive;
            public string MinInclusive;
            public string MinExclusive;
            public string FractionDigits = "-1";
            public string TotalDigits = "-1";
            public string WhiteSpace = ""; // "preserve", "replace", or "collapse"
            public ArrayList Enumeration = new ArrayList();
        }

        public class TableInfo
        {

            public TextBox Tb;
            public int Rows;
            public int Pos;
            public DataRow[] DataRows;
        }

        #region Menu And Button Events

        #region Open XSD

        private void mnuOpenXSD_Click(object sender, EventArgs e)
        {
            _openFileDialog.DefaultExt = "xsd";
            _openFileDialog.CheckFileExists = true;
            _openFileDialog.Filter = "XSD Files (*.xsd)|*.xsd|All Files (*.*)|*.*";
            var res = _openFileDialog.ShowDialog();
            if (res != DialogResult.OK)
                return;

            try
            {
                var tr = new StreamReader(_openFileDialog.FileName);
                _schema = XmlSchema.Read(tr, SchemaValidationHandler);
                tr.Close();
                CompileSchema(); // report any problems with the schema by compiling it
                _doc = new XmlDataDocument();
                tr = new StreamReader(_openFileDialog.FileName);
                _doc.DataSet.ReadXmlSchema(tr);
                tr.Close();
                ConstructGUI(_doc.DataSet);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Error processing XSD schema:");
                _schema = null;
                _doc = null;
            }
        }

        #endregion

        #region Infer XSD

        private void mnuInferXSD_Click(object sender, EventArgs e)
        {
            _openFileDialog.DefaultExt = "xml";
            _openFileDialog.CheckFileExists = true;
            _openFileDialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
            var res = _openFileDialog.ShowDialog();
            if (res != DialogResult.OK)
                return;

            // read into XmlSchema because we want the information in that format
            try
            {
                _doc = new XmlDataDocument();
                var tr = new StreamReader(_openFileDialog.FileName);
                _doc.DataSet.InferXmlSchema(tr, null);
                tr.Close();
                string strSchema = _doc.DataSet.GetXmlSchema();

                var sw = new StreamWriter("temp.txt", false, System.Text.Encoding.UTF8);
                sw.Write(strSchema);
                sw.Flush();
                sw.Close();

                var sr = new StringReader(strSchema);
                _schema = XmlSchema.Read(sr, SchemaValidationHandler);
                sr.Close();
                CompileSchema();
                ConstructGUI(_doc.DataSet);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Error inferring schema:");
                _schema = null;
                _doc = null;
            }

        }

        #endregion

        #region Open XML

        private void mnuOpenXML_Click(object sender, EventArgs e)
        {
            if (_schema == null)
            {
                MessageBox.Show("Please load an XSD first.", "Cannot load XML");
                return;
            }
            _openFileDialog.DefaultExt = "xml";
            _openFileDialog.CheckFileExists = true;
            _openFileDialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
            var res = _openFileDialog.ShowDialog();
            if (res != DialogResult.OK)
                return;
            
            _xmlFileName = _openFileDialog.FileName;
            try
            {
                var tr = new XmlTextReader(_openFileDialog.FileName);
                _doc.Load(tr);
                tr.Close();

                // setup to point to the first record in the root table
                foreach (DataTable dt in _doc.DataSet.Tables)
                {
                    // but we're only interested in the toplevel tables
                    if (dt.ParentRelations.Count == 0)
                    {
                        var ti = _tableInfo[dt] as TableInfo;
                        ti.Pos = 1;
                        ti.Rows = dt.Rows.Count;
                        FirstRecord(dt, ti);
                        UpdateRecordCountInfo(dt);
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Error processing XML document:");
            }
        }

        #endregion

        #region Save XML

        private void mnuSaveXML_Click(object sender, System.EventArgs e)
        {
            if (_doc.DataSet == null)
            {
                MessageBox.Show("No XML data to save.");
                return;
            }

            if (_xmlFileName != null)
            {
                _doc.DataSet.AcceptChanges();
                StreamWriter sw = new StreamWriter(_xmlFileName, false, System.Text.Encoding.UTF8);
                _doc.DataSet.WriteXml(sw);
                sw.Flush();
                sw.Close();
            }
            else
            {
                mnuSaveXmlAs_Click(sender, e);
            }
        }

        private void mnuSaveXmlAs_Click(object sender, System.EventArgs e)
        {
            if (_doc.DataSet == null)
            {
                MessageBox.Show("No XML data to save.");
                return;
            }

            _doc.DataSet.AcceptChanges();
            DialogResult res = _saveXmlAsDlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                _xmlFileName = _saveXmlAsDlg.FileName;
                StreamWriter sw = new StreamWriter(_xmlFileName, false, System.Text.Encoding.UTF8);
                _doc.DataSet.WriteXml(sw);
                sw.Flush();
                sw.Close();
            }
        }

        #endregion

        #region Exit Program

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Close(); // signal form to close
        }

        #endregion

        #region XPath Query

        private void btnXpathQuery_Click(object sender, EventArgs e)
        {
            if (_doc == null)
            {
                MessageBox.Show("Please load a schema and XML data set first.");
                return;
            }

            try
            {
                var nodeList = _doc.SelectNodes(_editableXPath.Text);
                var xPathDialog = new DlgXPathResult(nodeList, _editableNewRowOn.Text);
                xPathDialog.ShowDialog(this);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error processing XPath query:");
                return;
            }


        }

        #endregion

        #endregion

        #region Schema Validation Handler

        // report any errors to the schema error edit box
        private void SchemaValidationHandler(object sender, ValidationEventArgs args)
        {
            MessageBox.Show(args.Message, "Schema compilation error:");
        }

        #endregion

        #region CompileSchema

        // Compile the schema as it exists in the XmlSchema structure, displaying any
        // errors in the schema error edit box and displaying the schema itself in
        // the schema edit box.
        private void CompileSchema()
        {
            _schema.Compile(SchemaValidationHandler);
        }

        #endregion

        #region Functions That Inspect The Schema

        private XmlSchemaAttribute GetLocalAttribute(XmlSchemaComplexType ct, string name)
        {
            foreach (var ctAttr in ct.Attributes)
            {
                var attr = ctAttr as XmlSchemaAttribute;
                if (attr != null && attr.QualifiedName.Name == name)
                    return attr;
            }
            return null;
        }

        private XmlSchemaElement GetLocalElement(XmlSchemaComplexType ct, string name)
        {
            var seq = ct.Particle as XmlSchemaSequence;
            if (seq == null)
                return null;
            foreach (var item in seq.Items)
            {
                var el = item as XmlSchemaElement;
                if (el != null && el.QualifiedName.Name == name)
                    return el; 
            }
            return null;
        }

        private XmlSchemaComplexType GetLocalComplexType(XmlSchemaElement el)
        {
            return el.SchemaType as XmlSchemaComplexType;
        }

        private XmlSchemaElement GetGlobalElement(string name)
        {
            var qname = new XmlQualifiedName(name, _schema.TargetNamespace);
            var obj = _schema.Elements[qname];
            return obj as XmlSchemaElement;
        }

        private XmlSchemaSimpleType GetGlobalSimpleType(string name)
        {
            foreach (var item in _schema.Items)
            {
                var obj = item as XmlSchemaSimpleType;
                if (obj != null && obj.Name == name)
                    return obj;
            }
            return null;
        }

        private XmlSchemaAttribute GetGlobalAttribute(string name)
        {
            return _schema.Attributes[new XmlQualifiedName(name, _schema.TargetNamespace)] as XmlSchemaAttribute;
        }

        private XmlSchemaComplexType GetGlobalComplexType(string name)
        {
            foreach(var item in _schema.Items)
            {
                var obj = item as XmlSchemaComplexType;
                if (obj != null && obj.Name == name)
                        return obj;

                var el2 = item as XmlSchemaElement;
                if (el2 != null && el2.SchemaType is XmlSchemaComplexType)
                {
                    obj = el2.SchemaType as XmlSchemaComplexType;
                    if (((XmlSchemaElement)item).Name == name) // *** The returning complex type does not have any associated qualified name!!! ****
                        return obj;
                }
            }
            return null;
        }

        private void GetFacetInfo(TypeInfo ti, XmlSchemaSimpleTypeRestriction rs)
        {
            foreach (XmlSchemaFacet facet in rs.Facets)
            {

                // Replace with dictionary and delegates (see https://stackoverflow.com/questions/7542793/how-to-use-switch-case-on-a-type)
                switch (facet.GetType().ToString())
                {
                    case "System.Xml.Schema.XmlSchemaLengthFacet":
                        ti.Length = facet.Value;
                        break;
                    case "System.Xml.Schema.XmlSchemaMinLengthFacet":
                        ti.MinLength = facet.Value;
                        break;
                    case "System.Xml.Schema.XmlSchemaMaxLengthFacet":
                        ti.MaxLength = facet.Value;
                        break;
                    case "System.Xml.Schema.XmlSchemaPatternFacet":
                        ti.Pattern = facet.Value;
                        break;
                    case "System.Xml.Schema.XmlSchemaEnumerationFacet":
                        ti.Enumeration.Add(facet.Value);
                        break;
                    case "System.Xml.Schema.XmlSchemaMaxInclusiveFacet":
                        ti.MaxInclusive = facet.Value;
                        break;
                    case "System.Xml.Schema.XmlSchemaMaxExclusiveFacet":
                        ti.MaxExclusive = facet.Value;
                        break;
                    case "System.Xml.Schema.XmlSchemaMinInclusiveFacet":
                        ti.MinInclusive = facet.Value;
                        break;
                    case "System.Xml.Schema.XmlSchemaMinExclusiveFacet":
                        ti.MinExclusive = facet.Value;
                        break;
                    case "System.Xml.Schema.XmlSchemaFractionDigitsFacet":
                        ti.FractionDigits = facet.Value;
                        break;
                    case "System.Xml.Schema.XmlSchemaTotalDigitsFacet":
                        ti.TotalDigits = facet.Value;
                        break;
                    case "System.Xml.Schema.XmlSchemaWhiteSpaceFacet":
                        ti.WhiteSpace = facet.Value;
                        break;
                }
            }
        }

        // TODO Split up function
        private TypeInfo GetTypeInfo(XmlSchemaComplexType ct, string name)
        {
            TypeInfo ti = null;
            XmlSchemaElement el = GetLocalElement(ct, name); // is it an element?

            // *** 5/10/2003 *** Search global element list also
            if (el == null)
                el = GetGlobalElement(name);

            // or is it an attribute?
            XmlSchemaAttribute attr = null;
            if (ct != null)
                attr = GetLocalAttribute(ct, name);

            if (el != null)
            {
                // is it an element of simple type?
                if (el.SchemaType is XmlSchemaSimpleType)
                {
                    // (paths have been tested)
                    // yes...get the restriction base to find the type
                    var st = el.SchemaType as XmlSchemaSimpleType;
                    var rest = st.Content as XmlSchemaSimpleTypeRestriction;
                    if (rest != null)
                    {
                        if (rest.BaseTypeName.Namespace == "")
                        {
                            st = GetGlobalSimpleType(rest.BaseTypeName.Name);
                            rest = st.Content as XmlSchemaSimpleTypeRestriction;
                            ti = new TypeInfo(rest.BaseTypeName.Name);
                            GetFacetInfo(ti, rest);
                        }
                        else
                        {
                            ti = new TypeInfo(rest.BaseTypeName.Name);
                            GetFacetInfo(ti, rest);
                        }
                    }
                }
                else
                {
                    // no...get the attribute type
                    // does it reference a global element?
                    if (el.RefName.Name != "")
                    {
                        // (path has been tested)
                        el = GetGlobalElement(el.RefName.Name);
                        if (el != null)
                            ti = new TypeInfo(el.SchemaTypeName.Name);
                    }
                    else
                    {
                        // (path has been tested)
                        // no...the type is specified in the element

                        // Does this reference a global simple type?
                        string typename = el.SchemaTypeName.Name;
                        if (el.SchemaTypeName.Namespace == "")
                        {
                            var st = GetGlobalSimpleType(typename);
                            var rest = st.Content as XmlSchemaSimpleTypeRestriction;
                            if (rest != null)
                            {
                                typename = rest.BaseTypeName.Name;
                                ti = new TypeInfo(typename);
                                GetFacetInfo(ti, rest);
                            }
                        }
                        else
                        {
                            ti = new TypeInfo(typename);
                        }
                    }
                }
            }
            else if (attr != null)
            {
                // does it reference a global attribute?
                if (attr.RefName.Name != "")
                {
                    // yes...get the type of the reference
                    // !!! PATH HAS NOT BEEN TESTED !!!
                    attr = GetGlobalAttribute(attr.RefName.Name);
                    if (attr != null)
                    {
                        string typename = attr.SchemaTypeName.Name;
                        ti = new TypeInfo(typename);
                    }
                }
                else
                {
                    // no...get the type of the attribute
                    // is it of simple type defined globally?
                    if (attr.AttributeType is XmlSchemaSimpleType)
                    {
                        // (path has been tested)
                        var st = GetGlobalSimpleType(attr.SchemaTypeName.Name);
                        if (st != null)
                        {
                            var rest = st.Content as XmlSchemaSimpleTypeRestriction;
                            if (rest != null)
                            {
                                ti = new TypeInfo(rest.BaseTypeName.Name);
                                GetFacetInfo(ti, rest);
                            }
                        }
                    }
                    else
                    {
                        // (path has been tested)
                        string typename = attr.SchemaTypeName.Name;
                        ti = new TypeInfo(typename);
                    }
                }
            }
            return ti;
        }

        #endregion

        #region Construct GUI

        private void ConstructGUI(DataSet dataSet)
        {
            // clear the GUI
            // not implemented

            var pos = new Point(10, 10);
            // get all tables in the dataset
            foreach (DataTable dt in dataSet.Tables)
            {
                // but we're only interested in the toplevel tables
                if (dt.ParentRelations.Count == 0)
                {
                    /*
						* Rule 1:
						* A top level table will be a top level element in the schema that
						* is of a complex type.  The element name will be the table name.
						* What we want to identify is the complex type that the table references,
						* so that we can determine the data types of the columns.
						* 
						* Any other rules???
						*/
                    var el = GetGlobalElement(dt.TableName);
                    if (el != null)
                    {
                        XmlSchemaComplexType ct;
                        // references a global complex type?
                        if (el.SchemaTypeName.Name != "")
                        {
                            string name = el.RefName.Name != "" ? el.RefName.Name : el.SchemaTypeName.Name;
                            ct = GetGlobalComplexType(name);
                        }
                        else // contains a complex type?
                        {
                            ct = el.SchemaType as XmlSchemaComplexType;
                        }
                        if (ct != null)
                        {
                            var p2 = ConstructGUI(pos.X, pos.Y, dt, _dynamicForm, ct);
                            pos.Y += p2.Y;
                        }
                    }
                }
            }
        }

        // TODO Split this up, this function is far too large
        private Point ConstructGUI(int absx, int absy, DataTable dt, Control gbParent, XmlSchemaComplexType ct)
        {
            // for this data table, construct a groupbox as a container for
            //	record scrollbar
            //	table columns
            var groupBox = new GroupBox
            {
                Text = dt.TableName,
                Location = new Point(absx, absy),
                Parent = gbParent,
                Visible = true
            };

            groupBox.Font = new Font(groupBox.Font, FontStyle.Bold);

            _tableInfo[dt] = new TableInfo();
            CreateRecordNavigationBar(10, 15, groupBox, dt);

            var relx = 10; // and indented by 10 pixels
            var rely = 40; // and 30 pixels from top of groupbox

            // For each column in the table...
            foreach (DataColumn col in dt.Columns)
            {
                // if it's not an internal ID...
                if (col.ColumnMapping != MappingType.Hidden)
                {
                    // *** 5/10/2003 *** Check for a simple content type.
                    var sc = ct.ContentModel as XmlSchemaSimpleContent;
                    if (sc != null)
                    {
                        CreateLabel(relx, rely, dt.TableName, groupBox);
                        var sce = sc.Content as XmlSchemaSimpleContentExtension;
                        if (sce != null)
                            CreateEditControl(relx + 120, rely, new TypeInfo(sce.BaseTypeName.Name), groupBox, dt, col);
                    }
                    else
                    {
                        // display its name
                        CreateLabel(relx, rely, col.ColumnName, groupBox);
                        /* INACCESSIBLE INFORMATION THAT WOULD HAVE BEEN REALLY USEFUL!!!
						* col.XmlDataType is the data type, simple or global
						* col.SimpleType contains the facets and XmlBaseType of the simple type
						* These properties are INTERNAL classes!  Since we have to get this
						* information ourselves... */
                        var ti = GetTypeInfo(ct, col.ColumnName);
                        if (ti != null)
                            CreateEditControl(relx + 120, rely, ti, groupBox, dt, col); /*Control editCtrl=*/
                    }
                }

                if (col.ColumnMapping == MappingType.Hidden)
                {
                    // *** columns that define relationships do not need to be displayed ***

                    var label = CreateLabel(relx, rely, col.ColumnName, groupBox);
                    var editCtrl = CreateEditControl(relx + 120, rely, new TypeInfo("string"), groupBox, dt, col);
                    editCtrl.Size = new Size(50, 20);
                    ((TextBox) editCtrl).ReadOnly = true;

                    // These are columns that are created when the schema is loaded into a dataset.
                    // They are not part of the schema itself, but an artifact of the "tablization"
                    // of the schema.


                    // TODO Turn these two into a single function

                    // identify child relations
                    // Does this column have a relationship with a child table?
                    // data relations are always 1:1
                    if (dt.ChildRelations.Count != 0)
                    {
                        var dr = dt.ChildRelations[0];
                        for (var i = 0; i < dr.ChildColumns.Length; i++)
                            if (dr.ChildColumns[i].ColumnName == col.ColumnName)
                            {
                                label.ForeColor = Color.Blue;
                                break;
                            }  
                    }

                    // identify parent relations.
                    // Does this column have a relationship with a parent table?
                    // data relations are always 1:1
                    if (dt.ParentRelations.Count != 0)
                    {
                        var dr = dt.ParentRelations[0];
                        for (var i = 0; i < dr.ParentColumns.Length; i++)
                        {
                            if (dr.ParentColumns[i].ColumnName == col.ColumnName)
                            {
                                label.ForeColor = Color.Red;
                                break;
                            }
                        }
                    }
                }
                rely += 20;
            }

            var rx = 0; // assume no children
            var choiceIdx = 0;
            // Get child relationships, which are displayed as indented groupboxes
            foreach (DataRelation childRelation in dt.ChildRelations)
            {
                DataTable dt2 = childRelation.ChildTable;

                /* Rule 1: The data table references a global complex type.
					* The table name is the element name of complex type in the
					* current complex type object collection.  As with the root
					* table, we need to find the element, get the type, then get
					* the complex type.
					* 
					* Rule 2: If this isn't a global complex type (ct2==null), then
					* the element is a local complex type!
					*/
                XmlSchemaElement el2 = null;
                XmlSchemaElement el3 = null;
                XmlSchemaComplexType ct2 = null;

//				if (dt2.TableName=="Author")
//				{
//					MessageBox.Show("Searching: "+dt2.TableName);
//				}

                // *** 5/10/2003 *** Handle complex types with "choice" schemas.
                // A "choice" is represented as separate tables, each indexing the same choice list,
                // and the child relation index tracks the choice items!
                var choice = ct.ContentTypeParticle as XmlSchemaChoice;
                if (choice != null)
                {
//					for (int i=0; i<choice.Items.Count; i++)
                    {
                        var elChoice = choice.Items[choiceIdx] as XmlSchemaElement;
                        if (elChoice != null)
                        {
                            ct2 = GetGlobalComplexType(elChoice.RefName.Name);
                            var p = ConstructGUI(relx + 20, rely + 20, dt2, groupBox, ct2);
                            if (p.X > rx)
                                rx = p.X; // indent level
                            rely += p.Y + 20; // vertical spacing between child tables
//							break;
                        }
                    }
                    ++choiceIdx;
                    continue;
                }
                else if (ct != null) // TODO: NULL check on previously deferenced variable, this is wrong
                {
                    // get the local element associated with the table name in this complex type
                    el2 = GetLocalElement(ct, dt2.TableName);

                    // if it exists...
                    if (el2 != null)
                    {
                        // get either the refname or the schema type name
                        string name = el2.RefName.Name != "" ? el2.RefName.Name : el2.SchemaTypeName.Name;

                        // find the complex type defining the element type.
                        ct2 = GetGlobalComplexType(name);
                        if (ct2 == null)
                        {
                            // if the complex type is not defined globally, then check if it's defnied locally
                            ct2 = GetLocalComplexType(el2);

                            // if it's not defined locally, check if the element is referring to a global ELEMENT
                            if (ct2 == null)
                            {
                                el3 = GetGlobalElement(name);
                            }
                        }
                    }
                }

                if (ct2 != null)
                {
                    var p = ConstructGUI(relx + 20, rely + 20, dt2, groupBox, ct2);
                    if (p.X > rx)
                        rx = p.X; // indent level
                    rely += p.Y + 20; // vertical spacing between child tables
                }

                // *** 5/10/2003 *** This handles the case where a local element references a global element!
                // This structure must be created as a child node, complete with groupbox support.
                else if (el3 != null)
                {
                    // *** 5/12/2003 *** Create a tableInfo entry.
                    // NOTE THAT A SEPARATE GROUPBOX WITH RECORD NAVIGATION IS NOT CREATED!
                    _tableInfo[dt2] = new TableInfo();
                    CreateLabel(relx, rely, dt2.TableName, groupBox);
                    CreateEditControl(relx + 120, rely, new TypeInfo(el3.SchemaTypeName.Name), groupBox, dt2, dt2.Columns[0]);
                    rely += 20; // vertical spacing between rows
                }
                else
                {
                    MessageBox.Show("Not found: " + dt.TableName + "." + dt2.TableName);
                }
            }

            // set our size based on number of child tables (indents)
            groupBox.Size = new Size(300 + rx, rely + 10);

            // return the size of this groupbox, which includes all columns and child tables
            return new Point(40 + rx, rely + 10);
        }

        #endregion

        /// <summary>
        /// Create a label at given coordinates with an (optional?)
        /// parent.
        /// </summary>
        /// <param name="x">position along x-axis</param>
        /// <param name="y">position along y-axi</param>
        /// <param name="name">name of the lable</param>
        /// <param name="parent">parent of this label</param>
        /// <returns>new label, should not return null</returns>
        private Label CreateLabel(int x, int y, string name, Control parent)
        {
            var label = new Label
            {
                Location = new Point(x, y + 2),
                Size = new Size(120, 15),
                Text = name,
                Visible = true,
                Parent = parent
            };
            Font = new Font(label.Font, FontStyle.Regular);
            return label;
        }

        private Control CreateRecordNavigationBar(int x, int y, Control parent, DataTable tag)
        {
            var navigationBar = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(280, 19),
                Visible = true,
                Parent = parent,
                BorderStyle = BorderStyle.FixedSingle
            };

            CreateNavigationButton("<<", 0, 0, FirstRecord, navigationBar, tag);
            CreateNavigationButton("<", 30, 0, PrevRecord, navigationBar, tag);
            CreateNavigationButton("*", 60, 0, NewRecord, navigationBar, tag);
            CreateNavigationButton(">", 90, 0, NextRecord, navigationBar, tag);
            CreateNavigationButton(">>", 120, 0, LastRecord, navigationBar, tag);

            var tb = new TextBox
            {
                Location = new Point(150, 2),
                Size = new Size(100, 18),
                Font = new Font("Tahoma", 8, FontStyle.Regular),
                Parent = navigationBar,
                Visible = true,
                Text = "   record 0 of 0",
                BorderStyle = BorderStyle.None
            };
            ((TableInfo) _tableInfo[tag]).Tb = tb;

            CreateNavigationButton("X", 250, 0, DeleteRecord, navigationBar, tag);

            return navigationBar;
        }

        /// <summary>
        /// Create a button, used for navigation around records, at given
        /// coordinates.
        /// </summary>
        /// <param name="text">The text of the button, for example "=", or "forward"</param>
        /// <param name="x">position along the x-axis for the button</param>
        /// <param name="y">position along the y-axis for the button</param>
        /// <param name="ev">behaviour on button click</param>
        /// <param name="parent">parent of button</param>
        /// <param name="tag">any data for the button</param>
        private void CreateNavigationButton(string text, int x, int y, EventHandler ev, Control parent, object tag)
        {
            var btnNewRecord = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(30, 17),
                Parent = parent,
                Visible = true,
                Font = new Font("Tahoma", 8, FontStyle.Bold),
                Tag = tag
            };
            btnNewRecord.Click += ev;
        }

        private Control CreateEditControl(int x, int y, TypeInfo typeInfo, Control parent, DataTable dataTable, DataColumn dataColumn)
        {
            Control ctrl;

            // if the schema has an enumeration, then display as a combo box
            if (typeInfo.Enumeration.Count != 0)
            {
                var comboBox = new ComboBox
                {
                    Location = new Point(x, y),
                    Size = new Size(140, 200),
                    Visible = true,
                    Parent = parent
                };
                comboBox.Font = new Font(comboBox.Font, FontStyle.Regular);
                foreach (object obj in typeInfo.Enumeration)
                    comboBox.Items.Add(obj);
                ctrl = comboBox;
                ctrl.DataBindings.Add("Text", dataTable, dataColumn.ColumnName);
            }
            else if ((typeInfo.MinInclusive != null) && (typeInfo.MaxInclusive != null))
            {
                // if the schema has a min/max inclusive set, then display as an up-down control
                var upDown = new NumericUpDown
                {
                    Location = new Point(x, y),
                    Size = new Size(80, 20),
                    Visible = true,
                    Parent = parent,
                    Minimum = Convert.ToInt32(typeInfo.MinInclusive),
                    Maximum = Convert.ToInt32(typeInfo.MaxInclusive)
                };

                upDown.Font = new Font(upDown.Font, FontStyle.Regular);
                ctrl = upDown;
                ctrl.DataBindings.Add("Text", dataTable, dataColumn.ColumnName);
            }
            else
            {
                // use MaxLength
                // use Tag???
                // NumericUpDown???
                switch (typeInfo.TypeName)
                {
                    case "boolean":
                    {
                        // boolean is implemented as a checkbox
                        var ck1 = new CheckBox
                        {
                            Location = new Point(x, y),
                            Size = new Size(140, 20),
                            Visible = true,
                            Text = "",
                            Parent = parent,
                            AutoCheck = true,
                        };
                        ck1.Font = new Font(ck1.Font, FontStyle.Regular);
                        ctrl = ck1;

                        var b = new Binding("Checked", dataTable, dataColumn.ColumnName);
                        b.Format += TrueFalseToBool;
                        ctrl.DataBindings.Add(b);
                        break;
                    }

                    case "string":
                    {
                        var tb = new TextBox
                        {
                            Location = new Point(x, y),
                            Size = new Size(140, 20),
                            Visible = true,
                            Parent = parent,
                        };
                        
                        tb.Font = new Font(tb.Font, FontStyle.Regular);
                        ctrl = tb;
                        ctrl.DataBindings.Add("Text", dataTable, dataColumn.ColumnName);
                        break;
                    }

                    case "decimal":
                    { // Right justified.

                        var tb = new TextBox
                        {
                            Location = new Point(x, y),
                            Size = new Size(140, 20),
                            Visible = true,
                            Parent = parent,
                            TextAlign = HorizontalAlignment.Right
                        };

                        tb.Font = new Font(tb.Font, FontStyle.Regular);
                        ctrl = tb;
                        ctrl.DataBindings.Add("Text", dataTable, dataColumn.ColumnName);
                        break;
                    }
                    case "positiveInteger":
                    { // Right justified.
                        var tb = new TextBox
                        {
                            Location = new Point(x, y),
                            Size = new Size(140, 20),
                            Visible = true,
                            Parent = parent,
                            TextAlign = HorizontalAlignment.Right
                        };

                        tb.Font = new Font(tb.Font, FontStyle.Regular);
                        ctrl = tb;
                        ctrl.DataBindings.Add("Text", dataTable, dataColumn.ColumnName);
                        break;
                    }
                    default:
                    {
                        // use a text box for the default data type
                        var tb = new TextBox
                        {
                            Location = new Point(x, y),
                            Size = new Size(140, 20),
                            Parent = parent,
                            Visible = true
                        };

                        ctrl = tb;
                        tb.Font = new Font(tb.Font, FontStyle.Regular);
                        ctrl.DataBindings.Add("Text", dataTable, dataColumn.ColumnName);
                        break;
                    }
                }
            }
            return ctrl;
        }

        private void TrueFalseToBool(object sender, ConvertEventArgs args)
        {
            if (args.Value.GetType() == typeof (System.DBNull))
                args.Value = false;
        }

        private void UpdateRecordCountInfo(DataTable dt)
        {
            // update the text box to reflect the record #
            var info = _tableInfo[dt] as TableInfo;
            if (dt.ParentRelations.Count == 0)
                info.Pos = BindingContext[dt].Position + 1;
            // *** 5/12/2003 *** If a child element references a global element, no navigational record is created.  Is this
            // a problem?
            if (info.Tb != null)
                info.Tb.Text = "   record " + info.Pos + " of " + info.Rows;
        }

        private int GetMatchingRows(DataTable dt)
        {
            var info = _tableInfo[dt] as TableInfo;
            var parentRelation = dt.ParentRelations[0];
            var oarentData = parentRelation.ParentTable;
            var dcParent = parentRelation.ParentColumns[0];
            // get the current record # of the parent
            int n = BindingContext[oarentData].Position;
            if (n != -1)
            {
                // get the ID
                string val = oarentData.Rows[n][dcParent].ToString();
                // search the child for all records where child.parentID=parent.ID
                string expr = dcParent.ColumnName + "=" + val;
                // save the rows, as we'll use them later on when navigating the child
                info.DataRows = dt.Select(expr);
            }
            // return the length
            if (info.DataRows == null)
                return 0;
            return info.DataRows.Length;
        }

        #region Navigation Bar Event Handlers

        // Create a new record in the associated table.
        // This function also creates new records in all child tables and
        // sets the child-parent relationship key in the child tables.
        private void NewRecord(object sender, EventArgs e)
        {
            var button = sender as Button;
            var data = button.Tag as DataTable;
            data.Rows.Add(data.NewRow());
            int newRow = data.Rows.Count - 1;
            BindingContext[data].Position = newRow;

            var info = _tableInfo[data] as TableInfo;

            // Set the child relationship ID's to the parent!
            // There will be only one parent relationship except
            // for the root table.
            if (data.ParentRelations.Count != 0)
            {
                var parentRelation = data.ParentRelations[0];
                var dt2 = parentRelation.ParentTable;
                int n = BindingContext[dt2].Position;

                // this is always a 1:1 relationship
                var dcParent = parentRelation.ParentColumns[0];
                var dcChild = parentRelation.ChildColumns[0];
                string val = dt2.Rows[n][dcParent].ToString();
                data.Rows[newRow][dcChild] = val;

                n = GetMatchingRows(data);
                info.Pos = n;
                info.Rows = n;
            }
            else
            {
                info.Pos = newRow + 1;
                info.Rows = newRow + 1;
            }

            UpdateRecordCountInfo(data);

            // for each child, also create a new row in the child's table
            foreach (DataRelation childRelation in data.ChildRelations)
                NewRecord(data, childRelation.ChildTable, childRelation);
        }

        private void NewRecord(DataTable parent, DataTable child, DataRelation dr)
        {
            child.Rows.Add(child.NewRow());

            // get the last row of the parent (this is the new row)
            // and the new row in the child (also the last row)
            int newParentRow = parent.Rows.Count - 1;
            int newChildRow = child.Rows.Count - 1;

            // go to this record
            BindingContext[child].Position = newChildRow;

            // get the parent and child columns
            // copy the parent ID (auto sequencing) to the child to establish
            // the relationship.  This is always a 1:1 relationship
            var dcParent = dr.ParentColumns[0];
            var dcChild = dr.ChildColumns[0];
            string val = parent.Rows[newParentRow][dcParent].ToString();
            child.Rows[newChildRow][dcChild] = val;

            ((TableInfo) _tableInfo[child]).Pos = 1;
            ((TableInfo) _tableInfo[child]).Rows = 1;
            UpdateRecordCountInfo(child);

            // recurse into children of this child
            foreach (DataRelation childRelation in child.ChildRelations)
                NewRecord(child, childRelation.ChildTable, childRelation);
        }

        private void FirstRecord(object sender, EventArgs e)
        {
            var button = sender as Button;
            var data = button.Tag as DataTable;
            var info = _tableInfo[data] as TableInfo;
            info.Pos = 1;
            FirstRecord(data, info);
            UpdateRecordCountInfo(data);
        }

        private void PrevRecord(object sender, EventArgs e)
        {
            var button = sender as Button;
            var data = button.Tag as DataTable;
            var info = _tableInfo[data] as TableInfo;
            if (info.Pos > 1)
            {
                ((TableInfo) _tableInfo[data]).Pos--;
                PrevRecord(data, info);
                UpdateRecordCountInfo(data);
            }
        }

        private void NextRecord(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var data = button.Tag as DataTable;
            var info = _tableInfo[data] as TableInfo;
            if (info.Pos < info.Rows)
            {
                ((TableInfo) _tableInfo[data]).Pos++;
                NextRecord(data, info);
                UpdateRecordCountInfo(data);
            }
        }

        private void LastRecord(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var data = button.Tag as DataTable;
            var info = _tableInfo[data] as TableInfo;
            info.Pos = info.Rows;
            LastRecord(data, info);
            UpdateRecordCountInfo(data);
        }
        
        private void DeleteRecord(object sender, EventArgs e)
        {
            var button = sender as Button;
            var data = button.Tag as DataTable;
            int n = BindingContext[data].Position;
            data.Rows.RemoveAt(n);

            // if a child table...
            n = (data.ParentRelations.Count != 0)
                ? n = GetMatchingRows(data) // ...get all rows matching the parent ID
                : n = data.Rows.Count; // ...otherwise get all rows of the root table

            var info = _tableInfo[data] as TableInfo;
            info.Rows = n;

            if (info.Pos > info.Rows) // if position is now past # of rows...
                info.Pos = info.Rows; // set position to last row

            // update the display
            UpdateRecordCountInfo(data);

            if (n != 0) // table has records?
            {
                // set to the next available row
                if (data.ParentRelations.Count != 0)
                    SetPositionToRow(data, info.DataRows[info.Pos - 1]);
                else
                    BindingContext[data].Position = info.Pos - 1;
            }

            // reset all children to record #1
            ResetAllChildren(data);

            // Cascading delete automatically implemented
            // by the table constraints.
        }

        private void FirstRecord(DataTable dt, TableInfo ti)
        {
            if (dt.ParentRelations.Count == 0)
                BindingContext[dt].Position = 0;
            else
                SetPositionToRow(dt, ti.DataRows[ti.Pos - 1]); // get the first row in the record set that matches the parent
            ResetAllChildren(dt);
        }

        private void PrevRecord(DataTable dt, TableInfo ti)
        {
            if (dt.ParentRelations.Count == 0)
                BindingContext[dt].Position--;
            else
                SetPositionToRow(dt, ti.DataRows[ti.Pos - 1]); // get the previous row that matches the parent ID
            ResetAllChildren(dt);
        }

        private void NextRecord(DataTable dt, TableInfo ti)
        {
            if (dt.ParentRelations.Count == 0)
                BindingContext[dt].Position++;
            else
                SetPositionToRow(dt, ti.DataRows[ti.Pos - 1]); // get the next row that matches the parent ID
            ResetAllChildren(dt);
        }

        private void LastRecord(DataTable dt, TableInfo ti)
        {
            if (dt.ParentRelations.Count == 0)
                BindingContext[dt].Position = dt.Rows.Count - 1;
            else
                SetPositionToRow(dt, ti.DataRows[ti.Pos - 1]);                // get the last row that matches the parent ID
            ResetAllChildren(dt);
        }

        private void ResetAllChildren(DataTable dt)
        {
            foreach (DataRelation dr in dt.ChildRelations) // update all children of the table to match our new ID
                ResetChildRecords(dr.ChildTable);
        }

        private void ResetChildRecords(DataTable dt)
        {
            int n = GetMatchingRows(dt);
            var info = _tableInfo[dt] as TableInfo;
            info.Pos = 1;
            info.Rows = n;
            UpdateRecordCountInfo(dt);
            if (n != 0)
                SetPositionToRow(dt, info.DataRows[0]);

            foreach (DataRelation dr in dt.ChildRelations)
                ResetChildRecords(dr.ChildTable);
        }

        private void SetPositionToRow(DataTable dt, DataRow row)
        {
            for (var i = 0; i < dt.Rows.Count; i++)
                if (dt.Rows[i] == row)
                {
                    BindingContext[dt].Position = i;
                    break;
                }
        }

        #endregion

    }
}
