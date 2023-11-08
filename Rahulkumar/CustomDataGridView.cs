using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rahulkumar
{
    public partial class CustomDataGridView : DataGridView
    {
        public CustomDataGridView()
        {
            InitializeComponent();
           // this.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        DisableSelectWindow disableSelectWindow;

        protected override void OnEditingControlShowing(DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox textBox = e.Control as TextBox; // var tbox = (TextBox)e.Control;
            if (textBox != null)
            {
                disableSelectWindow = new DisableSelectWindow(textBox);
                textBox.ContextMenuStrip = new ContextMenuStrip(); // disable contect menu 
            }
            // except starting column ie.  0  , all columns must type from right to left 
            if (this.CurrentCell.ColumnIndex > 0 && this.CurrentCell.RowIndex >= 0)
            {                
                textBox.RightToLeft = RightToLeft.Yes;
                textBox.TextAlign = HorizontalAlignment.Right;
            }
            base.OnEditingControlShowing(e);
        }

        protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
        {
            if (disableSelectWindow != null)
            {
                disableSelectWindow.ReleaseHandle();
                disableSelectWindow = null;
            }            
            base.OnCellEndEdit(e);
        }

        protected override void OnCellValidating(DataGridViewCellValidatingEventArgs e)
        {
            //let the user type alphabets but dont let him go 
            if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3)
            {
                string value = e.FormattedValue.ToString();
                foreach (char c in value)
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, "^[0-9,.]*$"))
                    {
                        e.Cancel = true;                       
                        break;
                    }
                }
            }
            base.OnCellValidating(e);
        }

        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            //convert the value of with 2 decimal places 
            if (e.RowIndex >= 0 && (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3))
            {
                string originalValue = e.Value as string;

                if (!string.IsNullOrEmpty(originalValue))
                {
                    string numericPart = string.Empty;
                    string nonNumericPart = string.Empty;

                    foreach (char c in originalValue)
                    {
                        if (char.IsDigit(c) || c == '.' || c == ',')
                        {
                            numericPart += c;
                        }
                        else
                        {
                            nonNumericPart += c;
                        }
                    }

                    if (!string.IsNullOrEmpty(numericPart))
                    {
                        if (decimal.TryParse(numericPart, out decimal parsedValue))
                        {
                            e.Value = parsedValue.ToString("N2") + nonNumericPart;
                            e.FormattingApplied = true;
                        }
                    }
                }
            }
            base.OnCellFormatting(e);
        }


    }
}
