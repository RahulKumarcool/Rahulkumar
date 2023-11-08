using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rahulkumar
{
    public class caretbycastorixsir
    {
        public IntPtr CBTHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return CallNextHookEx(hHook, nCode, wParam, lParam);
            }
            else
            {
                //if (nCode == HCBT_CREATEWND)
                if (nCode == HCBT_SETFOCUS)
                {
                    if (lParam != IntPtr.Zero)
                    {
                        StringBuilder sClass = new StringBuilder(260);
                        GetClassName(wParam, sClass, (int)(sClass.Capacity));
                        if (sClass.ToString().Contains("EDIT"))
                        {

                            if (!EditWindows.Contains(wParam))
                            {
                                EditWindows.Add(wParam);
                                bool bRet = SetWindowSubclass(wParam, SubClassDelegate, 0, 0);
                            }
                        }
                    }
                }
                return CallNextHookEx(hHook, nCode, wParam, lParam);
            }
        }

        public SUBCLASSPROC SubClassDelegate = new SUBCLASSPROC(WindowSubClass);
        public static List<IntPtr> EditWindows = new List<IntPtr>();
        static IntPtr hHook = IntPtr.Zero;
        HookProc CBTHookProcedure;
        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        public void InstallHook()
        {
            if (hHook == IntPtr.Zero)
            {
                CBTHookProcedure = new HookProc(CBTHookProc);
                hHook = SetWindowsHookEx(WH_CBT, CBTHookProcedure, (IntPtr)0, AppDomain.GetCurrentThreadId());
                if (hHook == IntPtr.Zero)
                {
                    int nErrorCode = Marshal.GetLastWin32Error();
                    // ...
                }
            }
        }

        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ShowCaret(IntPtr hWnd);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetCaretPos(int X, int Y);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool DestroyCaret();

        public const int WM_SETFOCUS = 0x0007;
        public const int WM_KILLFOCUS = 0x0008;

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        public const int HCBT_MOVESIZE = 0;
        public const int HCBT_MINMAX = 1;
        public const int HCBT_QS = 2;
        public const int HCBT_CREATEWND = 3;
        public const int HCBT_DESTROYWND = 4;
        public const int HCBT_ACTIVATE = 5;
        public const int HCBT_CLICKSKIPPED = 6;
        public const int HCBT_KEYSKIPPED = 7;
        public const int HCBT_SYSCOMMAND = 8;
        public const int HCBT_SETFOCUS = 9;

        public const int WH_CBT = 5;

        public delegate int SUBCLASSPROC(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern bool SetWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern int DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern bool RemoveWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public RECT(int Left, int Top, int Right, int Bottom)
            {
                left = Left;
                top = Top;
                right = Right;
                bottom = Bottom;
            }
        }

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("Gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport("Gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("Gdi32.dll", SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        public const int SRCCOPY = 0x00CC0020; /* dest = source                   */
        public const int SRCPAINT = 0x00EE0086; /* dest = source OR dest           */
        public const int SRCAND = 0x008800C6; /* dest = source AND dest          */
        public const int SRCINVERT = 0x00660046; /* dest = source XOR dest          */
        public const int SRCERASE = 0x00440328; /* dest = source AND (NOT dest )   */
        public const int NOTSRCCOPY = 0x00330008; /* dest = (NOT source)             */
        public const int NOTSRCERASE = 0x001100A6; /* dest = (NOT src) AND (NOT dest) */
        public const int MERGECOPY = 0x00C000CA; /* dest = (source AND pattern)     */
        public const int MERGEPAINT = 0x00BB0226; /* dest = (NOT source) OR dest     */
        public const int PATCOPY = 0x00F00021; /* dest = pattern                  */
        public const int PATPAINT = 0x00FB0A09; /* dest = DPSnoo                   */
        public const int PATINVERT = 0x005A0049; /* dest = pattern XOR dest         */
        public const int DSTINVERT = 0x00550009; /* dest = (NOT dest)               */
        public const int BLACKNESS = 0x00000042; /* dest = BLACK                    */
        public const int WHITENESS = 0x00FF0062; /* dest = WHITE                    */
        public const int NOMIRRORBITMAP = unchecked((int)0x80000000); /* Do not Mirror the bitmap in this call */
        public const int CAPTUREBLT = 0x40000000; /* Include layered windows */

        [DllImport("Gdi32.dll", SetLastError = true)]
        public static extern bool PatBlt(IntPtr hdc, int x, int y, int w, int h, uint rop);

        [DllImport("User32", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("User32", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool FillRect(IntPtr hdc, [In] ref RECT rect, IntPtr hbrush);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateSolidBrush(int crColor);

        public static int WindowSubClass(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData)
        {
            IntPtr hBitmap = IntPtr.Zero;
            switch (uMsg)
            {
                case WM_SETFOCUS:
                    {
                        DefSubclassProc(hWnd, uMsg, wParam, lParam);
                        RECT rect = new RECT();
                        GetClientRect(hWnd, out rect);

                        StringBuilder sbBufferText = new StringBuilder(255);
                        GetWindowText(hWnd, sbBufferText, sbBufferText.Capacity);
                        if (sbBufferText.ToString().Length != 0)
                        {
                            // Test 10 pixels width Caret
                            CreateCaret(hWnd, IntPtr.Zero, 8, rect.bottom - rect.top);
                        }
                        else
                        {
                            int nWidth = 5; // ok 
                            int nHeight = rect.bottom - rect.top;
                            IntPtr hDC = GetDC(IntPtr.Zero);
                            IntPtr hMemDC = CreateCompatibleDC(hDC);
                            hBitmap = CreateCompatibleBitmap(hDC, nWidth, nHeight);
                            IntPtr hBitmapOld = SelectObject(hMemDC, hBitmap);

                            IntPtr hBrushBackground = CreateSolidBrush(ColorTranslator.ToWin32(System.Drawing.Color.White));
                            RECT rectCaret = new RECT(0, nHeight - 2, nWidth, nHeight - 1);
                            FillRect(hMemDC, ref rectCaret, hBrushBackground);
                            DeleteObject(hBrushBackground);

                            SelectObject(hMemDC, hBitmapOld);
                            DeleteDC(hMemDC);
                            ReleaseDC(IntPtr.Zero, hDC);

                            CreateCaret(hWnd, hBitmap, 0, 0);
                        }
                        //SetCaretPos(0, 0);
                        ShowCaret(hWnd);
                        return 0;
                    }
                    break;
                case WM_KILLFOCUS:
                    {
                        DestroyCaret();
                        DeleteObject(hBitmap);
                    }
                    break;
            }
            return DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }
    }
}

