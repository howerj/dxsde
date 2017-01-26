using System;
using System.Windows.Forms;
using LibXsdEditor;

namespace xmlDataEditor
{
    public class EntryPoint
    {
        /// <summary>
        /// The main entry point for the application, this simply calls
        /// an example ready made editor from the LibXsdEditor library.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.Run(new XmlDataEditorForm());
        }
    }
}
