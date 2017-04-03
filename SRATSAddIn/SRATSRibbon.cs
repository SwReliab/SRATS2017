using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Office.Interop.Excel;
using SRATS2017AddIn.Views;

namespace SRATSAddIn
{
    public partial class SRATSRibbon
    {
        private void SRATSRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            Worksheet objSheet;
            Range range;

            new ExcelOperations();

            objSheet = Globals.ThisAddIn.Application.ActiveSheet as Worksheet;
            range = Globals.ThisAddIn.Application.Selection as Range;

            string v = objSheet.Name + "!"
                + range.get_Address(Type.Missing, Type.Missing, XlReferenceStyle.xlA1, Type.Missing, Type.Missing);

            SRATSMainView fm = new SRATSMainView(v);
            fm.Show();
        }
    }
}
