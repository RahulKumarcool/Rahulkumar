using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rahulkumar
{
    class DisableSelectWindow : NativeWindow
    {
        private bool MouseIsDown = false;

        public DisableSelectWindow(TextBox owner)
        {
            AssignHandle(owner.Handle);
        }

        const int
            WM_KEYDOWN = 0x0100,
            WM_CHAR = 0x0102,
            WM_IME_CHAR = 0x0286,
            WM_SYSCHAR = 0x0106,
            WM_MOUSEMOVE = 0x0200,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_LBUTTONUP = 0x0202,
            WM_CAPTURECHANGED = 0x0215,
            EM_SETSEL = 0x00B1;

        const char CTRL_A = '\x01';

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_KEYDOWN:
                    Keys keyCode = (Keys)(int)(m.WParam.ToInt64() & 0xffff);
                    if (Control.ModifierKeys.HasFlag(Keys.Shift))
                    {
                        switch (keyCode)
                        {
                            case Keys.Up:
                            case Keys.Down:
                            case Keys.Right:
                            case Keys.Left:
                                return;
                        }
                    }
                    base.WndProc(ref m);
                    break;
                case WM_CHAR:
                case WM_IME_CHAR:
                case WM_SYSCHAR:
                    char c = (char)(m.WParam.ToInt64() & 0xffff);
                    if (c == CTRL_A)
                    {
                        return;
                    }
                    base.WndProc(ref m);
                    break;
                case WM_CAPTURECHANGED:
                    MouseIsDown = false;
                    base.WndProc(ref m);
                    break;
                case WM_LBUTTONDOWN:
                    MouseIsDown = true;
                    base.WndProc(ref m);
                    break;
                case WM_LBUTTONUP:
                    MouseIsDown = false;
                    base.WndProc(ref m);
                    break;
                case WM_LBUTTONDBLCLK:
                    break;
                case WM_MOUSEMOVE:
                    if (!MouseIsDown)
                    {
                        base.WndProc(ref m);
                    }
                    break;
                case EM_SETSEL:
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
