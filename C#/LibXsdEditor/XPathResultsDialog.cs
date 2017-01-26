// TODO this file needs commenting/rewriting with better function names

using System.Collections;
using System.Windows.Forms;
using System.Xml;

namespace LibXsdEditor
{
    /// <summary>
    /// Summary description for dlgXPathResult.
    /// </summary>
    public class DlgXPathResult : Form
    {
        private ListView lvResults;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        // These are all used by the recursive routines:
        //	ProcessChildHeaders
        //	ProcessChildData
        private readonly Hashtable _columns = new Hashtable();
        private readonly Hashtable _columnData = new Hashtable();
        private readonly Hashtable _columnName = new Hashtable();
        private int _columnIndex;
        private ListViewItem _listViewItem = null;

        private readonly ArrayList _newRow;

        public DlgXPathResult(XmlNodeList nodeList, string newRowOn)
        {
            InitializeComponent();
            var columnCreatesNewRow = newRowOn.Split(' ');
            _newRow = new ArrayList(columnCreatesNewRow);

            // get a list of all the column headers in the node list
            foreach (var node in nodeList)
                ProcessChildHeaders(null, (XmlNode) node); //, node.FirstChild);


            var newRowRequired = false;
            foreach (XmlNode node in nodeList)
            {
                if (newRowRequired)
                {
                    _listViewItem = CreateLvi(_columns.Count);
                    newRowRequired = false;
                }
                ProcessAttributes(node);
                newRowRequired = ProcessChildData1(node, node.FirstChild, newRowRequired);
            }
        }

        #region Process Headers

        private void ProcessChildHeaders(XmlNode node, XmlNode child)
        {
            while (child != null)
            {
                // process attributes
                if (child.Attributes != null)
                {
                    foreach (XmlAttribute attr in child.Attributes)
                    {
                        if (!_columns.Contains(attr.Name))
                        {
                            _columns.Add(attr.Name, _columnIndex);
                            lvResults.Columns.Add(attr.Name, -2, HorizontalAlignment.Left);
                            ++_columnIndex;
                        }
                    }
                }

                // if this child is an element, get its children
                if (child.FirstChild is XmlElement)
                {
                    ProcessChildHeaders(node, child.FirstChild);
                }
                else
                {
                    // if not, then either it or its child is a text element
                    var name = child.Name == "#text" ? node.Name : child.Name;
                    if (!_columns.Contains(name))
                    {
                        _columns.Add(name, _columnIndex);
                        lvResults.Columns.Add(name, -2, HorizontalAlignment.Left);
                        ++_columnIndex;
                    }
                }
                child = child.NextSibling;
            }
        }

        #endregion

        #region Process Data

        private bool ProcessChildData1(XmlNode node, XmlNode child, bool newRowRequired)
        {
            if (newRowRequired)
            {
                _listViewItem = CreateLvi(_columns.Count);
                newRowRequired = false;
            }
            while (child != null)
            {
                ProcessAttributes(child);

                // if this child is an element, get its children
                if (child.FirstChild is XmlElement)
                {
                    newRowRequired |= ProcessChildData1(child, child.FirstChild, newRowRequired);
                    ProcessChildData2();
                }
                else
                {
                    // if not, then either it or its child is a text element.
                    var name = child.Name == "#text" ? node.Name : child.Name;
                    _columnData[name] = child.InnerText;
                    if (_newRow.Contains(name))
                        newRowRequired = true;
                }
                ProcessChildData2();
                child = child.NextSibling;
            }
            return newRowRequired;
        }

        private void ProcessChildData2()
        {
            if (_listViewItem == null)
                _listViewItem = CreateLvi(_columns.Count);
            var iter = _columnData.GetEnumerator();
            while (iter.MoveNext())
            {
                var key = (string) iter.Key;
                var n = (int) _columns[key];
                if (_listViewItem.SubItems[n].Text != "")
                {
                    _listViewItem = CreateLvi(_columns.Count);
                    break;
                }
            }

            iter.Reset();
            while (iter.MoveNext())
            {
                var key = (string) iter.Key;
                var val = (string) iter.Value;
                var n = (int) _columns[key];
                _listViewItem.SubItems[n].Text = val;
            }

            _columnData.Clear();
        }

        #endregion

        #region Process Attributes

        private void ProcessAttributes(XmlNode node)
        {
            if (node.Attributes == null)
                return;
            foreach (XmlAttribute attr in node.Attributes)
                _columnData[attr.Name] = attr.Value;
        }

        #endregion

        private ListViewItem CreateLvi(int n)
        {
            var listView = new ListViewItem();
            for (var i = 0; i < n; i++)
                listView.SubItems.Add("");
            lvResults.Items.Add(listView);
            return listView;
        }

        /// <summary>
        /// Clean up any resources being used.
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
            this.lvResults = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // lvResults
            // 
            this.lvResults.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                      | System.Windows.Forms.AnchorStyles.Left)
                                     | System.Windows.Forms.AnchorStyles.Right);
            this.lvResults.AutoArrange = false;
            this.lvResults.GridLines = true;
            this.lvResults.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvResults.Location = new System.Drawing.Point(8, 8);
            this.lvResults.MultiSelect = false;
            this.lvResults.Name = "lvResults";
            this.lvResults.Size = new System.Drawing.Size(672, 264);
            this.lvResults.TabIndex = 0;
            this.lvResults.View = System.Windows.Forms.View.Details;
            // 
            // DlgXPathResult
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(688, 278);
            this.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.lvResults
            });
            this.Name = "DlgXPathResult";
            this.Text = "XPath Result";
            this.ResumeLayout(false);
        }

        #endregion
    }
}

