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
        private bool inGotoNextControl = false;
        public CustomDataGridView()
        {
            this.AllowUserToAddRows = false;
            InitializeComponent();
           // this.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        protected override bool ProcessKeyPreview(ref Message m)
        {
            KeyEventArgs args1 = new KeyEventArgs(((Keys)((int)m.WParam)) | Control.ModifierKeys);
            switch (args1.KeyCode)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:

                    return false;
            }
            return base.ProcessKeyPreview(ref m);
        }
      


        #region 2. Event when DataGridview gets focus 
        protected override void OnEnter(EventArgs e)
        {            
            // if no rows present on entering in control , add new row and set the first cell as the active cell 
            if (RowCount == 0)
            {
                TryAddRow();
                this.CurrentCell = this.Rows[0].Cells[0];
            }
            base.OnEnter(e);
        }
        #endregion

        #region 3. Event raised when focus is entered in cell 
        protected override void OnCellEnter(DataGridViewCellEventArgs e)
        {
            DataGridViewColumn column = Columns[e.ColumnIndex];
            if (!column.ReadOnly && column is DataGridViewTextBoxColumn)
            {
                BeginEdit(false); // false means don't select 
            }
            base.OnCellEnter(e);
        }
        #endregion

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter || keyData == Keys.Right)
            {
                var current = CurrentCell;
                if (current != null)
                {
                    if (IsCurrentCellDirty) // fires if the cell state is changed by typing 
                    {
                        CommitEdit(DataGridViewDataErrorContexts.Commit);
                        EndEdit(DataGridViewDataErrorContexts.Commit);
                    }
                    if (IsCurrentCellInEditMode)
                    {
                        EndEdit(DataGridViewDataErrorContexts.Commit);
                    }
                    int row = current.RowIndex;
                    int col = current.ColumnIndex;
                    if (row == LastRowIndex)
                    {
                        if (col == FirstVisibleColumn.Index)
                        {
                            if (Rows[row].IsNewRow || string.IsNullOrEmpty(CurrentCell.Value?.ToString()) || CurrentCell.Value.ToString() == "End of list")
                            {
                                return GotoNextControl();
                            }
                        }
                    }
                    if (col == LastVisibleColumn.Index) // if reached last cell of current row 
                    {
                        if (TryAddRow())
                        {
                            CurrentCell = this[FirstVisibleColumn.Index, RowCount - 1];
                            return true;
                        }

                    }
                }
                return ProcessTabKey(Keys.Tab);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }



        #region function to end edit mode of current cell and add new row 
        protected bool TryAddRow()
        {
            try
            {
                EndEdit();
                Rows.Add();
                return true;
            }
            catch (Exception ex)
            {                
                return false;
            }
        }

        #endregion


        protected int LastRowIndex
        {
            get
            {
                return Rows.GetLastRow(DataGridViewElementStates.Visible);
            }
        }

        #region Function to return first visible column in datagridview 
        protected DataGridViewColumn FirstVisibleColumn
        {
            get
            {
                return Columns.GetFirstColumn(
                    DataGridViewElementStates.Visible,
                    DataGridViewElementStates.None);
            }
        }
        #endregion

        #region Function to return Last visible column in datagridview
        protected DataGridViewColumn LastVisibleColumn
        {
            get
            {
                return Columns.GetLastColumn(
                    DataGridViewElementStates.Visible,
                    DataGridViewElementStates.None);
            }
        }
        #endregion

        #region Function to go to next control 
        protected bool GotoNextControl()
        {
            inGotoNextControl = true;
            bool result = Parent.SelectNextControl(this, true, true, true, true);
            inGotoNextControl = false;
            return result;
        }
        #endregion

    




    DisableSelectWindow disableSelectWindow;

        protected override void OnEditingControlShowing(DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox textBox = e.Control as TextBox; // var tbox = (TextBox)e.Control;
            if (textBox != null)
            {
                if (this.CurrentCell.ColumnIndex > 0 && this.CurrentCell.RowIndex >= 0)
                {
                    // no need 
                   // textBox.RightToLeft = RightToLeft.Yes;
                  //  textBox.TextAlign = HorizontalAlignment.Right;
                }
                disableSelectWindow = new DisableSelectWindow(textBox);
                textBox.ContextMenuStrip = new ContextMenuStrip(); // disable contect menu 
            }
            base.OnEditingControlShowing(e);
        }

        protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
        {
            if (disableSelectWindow != null)
            {
                disableSelectWindow.Dispose();         
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
