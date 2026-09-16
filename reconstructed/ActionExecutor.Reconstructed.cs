// RECONSTRUCTED. Exact IL: reconstructed/evidence/ActionExecutor.il.txt
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ModernAutoClicker.Reconstructed.Advanced
{
    internal static class ActionExecutor
    {
        [ThreadStatic] private static Random _random;
        private static Random Rng => _random ?? (_random = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId)));

        // 0x06000007 RVA 0x29C4
        internal static Point ApplyJitter(Point p, int radius)
        {
            if (p == Point.Empty || radius <= 0) return p;
            return new Point(Math.Max(0, p.X + Rng.Next(-radius, radius + 1)), Math.Max(0, p.Y + Rng.Next(-radius, radius + 1)));
        }

        // 0x06000008 / 0x06000009
        internal static int ApplyHoldInterval(int value, int randomInterval) => Math.Max(1, value + (randomInterval > 0 ? Rng.Next(-randomInterval, randomInterval + 1) : 0));
        internal static int ApplyDelayInterval(int value, int randomInterval) => Math.Max(0, value + (randomInterval > 0 ? Rng.Next(-randomInterval, randomInterval + 1) : 0));

        // 0x0600000A RVA 0x2A90
        internal static bool MatchesColor(Color actual, Color expected, int tolerance) =>
            Math.Abs(actual.R - expected.R) <= tolerance && Math.Abs(actual.G - expected.G) <= tolerance && Math.Abs(actual.B - expected.B) <= tolerance;

        // 0x0600000B RVA 0x2AF0. The original uses one CopyFromScreen + LockBits scan for speed.
        // This readable reconstruction keeps the same first-match/virtual-screen semantics using GetPixelColorReference.
        internal static Point FindMatchingPixelInArea(Rectangle area, Color target, int tolerance)
        {
            if (area.Width <= 0 || area.Height <= 0) return Point.Empty;
            Rectangle clipped = Rectangle.Intersect(area, SystemInformation.VirtualScreen);
            if (clipped.Width <= 0 || clipped.Height <= 0) return Point.Empty;
            for (int y=clipped.Top; y<clipped.Bottom; y++)
                for (int x=clipped.Left; x<clipped.Right; x++)
                    if (MatchesColor(NativeMethods.GetPixelColorReference(x,y), target, tolerance)) return new Point(x,y);
            return Point.Empty;
        }

        // 0x0600000C RVA 0x2D74. A MacroStep-relative point is converted from target-client to screen.
        internal static Point ResolveActualScreenPoint(MacroStepSnapshot step, Point point)
        {
            if (point == Point.Empty) return Point.Empty;
            if (step == null || !step.RelativeToWindow || string.IsNullOrEmpty(step.ProcessName)) return point;
            IntPtr hwnd = NativeMethods.FindWindowByTarget(step.ProcessName, step.WindowTitle);
            if (hwnd == IntPtr.Zero) return point;
            var origin = new NativeMethods.POINT { X = 0, Y = 0 };
            if (!NativeMethods.ClientToScreen(hwnd, ref origin)) return point;
            return new Point(origin.X + point.X, origin.Y + point.Y);
        }

        // 0x0600000E RVA 0x2FF8. IMPORTANT: this Advanced.ActionExecutor method is NOT
        // NativeMethods.PerformClickDirectToWindow. It does not probe/retarget a child HWND.
        // It posts MOVE/DOWN/UP directly to the already-known HWND with the supplied client point.
        internal static void PerformClickDirectToWindow(IntPtr hwnd, Point p, int button, int holdMs)
        {
            if (hwnd == IntPtr.Zero) return;
            SelectMessages(button, out int down, out int up, out IntPtr wp);
            IntPtr lp = Pack(p.X, p.Y);
            NativeMethods.PostMessage(hwnd, NativeMethods.WM_MOUSEMOVE, IntPtr.Zero, lp);
            NativeMethods.PostMessage(hwnd, down, wp, lp);
            if (holdMs > 0) Thread.Sleep(holdMs);
            NativeMethods.PostMessage(hwnd, up, IntPtr.Zero, lp);
        }

        // 0x0600000F RVA 0x30B8. The free-mouse SCREEN fallback differs from NativeMethods.SendBackgroundClick:
        // it hit-tests WindowFromPoint, converts to that HWND's client coordinates, then posts DOWN/UP only.
        // It does not issue WM_MOUSEMOVE and does not call RealChildWindowFromPoint in this method.
        internal static void PerformClick(Point screenPoint, int button, int holdMs, bool freeMouseMode)
        {
            if (screenPoint == Point.Empty) return;
            if (freeMouseMode)
            {
                var p = new NativeMethods.POINT { X = screenPoint.X, Y = screenPoint.Y };
                IntPtr hwnd = NativeMethods.WindowFromPoint(p);
                if (hwnd == IntPtr.Zero) return;
                NativeMethods.ScreenToClient(hwnd, ref p);
                SelectMessages(button, out int down, out int up, out IntPtr wp);
                IntPtr lp = Pack(p.X, p.Y);
                NativeMethods.PostMessage(hwnd, down, wp, lp);
                if (holdMs > 0) Thread.Sleep(holdMs);
                NativeMethods.PostMessage(hwnd, up, IntPtr.Zero, lp);
                return;
            }
            NativeMethods.SetCursorPos(screenPoint.X, screenPoint.Y);
            Thread.Sleep(5);
            NativeMethods.SendPhysicalClick(button, holdMs);
        }

        private static IntPtr Pack(int x, int y) => new IntPtr(((y & 0xffff) << 16) | (x & 0xffff));
        private static void SelectMessages(int button, out int down, out int up, out IntPtr wp)
        {
            if (button == 1) { down=NativeMethods.WM_RBUTTONDOWN; up=NativeMethods.WM_RBUTTONUP; wp=new IntPtr(2); }
            else if (button == 2) { down=NativeMethods.WM_MBUTTONDOWN; up=NativeMethods.WM_MBUTTONUP; wp=new IntPtr(16); }
            else { down=NativeMethods.WM_LBUTTONDOWN; up=NativeMethods.WM_LBUTTONUP; wp=new IntPtr(1); }
        }

        // 0x0600000D RVA 0x2E18. Conditions (ActionType 8..11) are handled by MacroRunner, not here.
        internal static void Execute(MacroStepSnapshot step, bool freeMouseMode, int randomInterval, int randomJitter)
        {
            if (step == null) return;
            int hold = ApplyHoldInterval(step.HoldMs, randomInterval);
            Point startClient = ApplyJitter(step.StartPoint, randomJitter);
            Point endClient = ApplyJitter(step.EndPoint, randomJitter);
            Point startScreen = ResolveActualScreenPoint(step, startClient);
            Point endScreen = ResolveActualScreenPoint(step, endClient);
            IntPtr bound = freeMouseMode && step.RelativeToWindow && !string.IsNullOrEmpty(step.ProcessName)
                ? NativeMethods.FindWindowByTarget(step.ProcessName, step.WindowTitle) : IntPtr.Zero;

            switch (step.ActionType)
            {
                case 0:
                case 1:
                    if (bound != IntPtr.Zero && startClient != Point.Empty) PerformClickDirectToWindow(bound,startClient,step.ActionType,hold);
                    else PerformClick(startScreen,step.ActionType,hold,freeMouseMode);
                    break;
                case 2: PerformMiddleScroll(startScreen,step.ScrollStep,hold,freeMouseMode); break;
                case 3:
                    if (bound != IntPtr.Zero && startClient != Point.Empty)
                    {
                        PerformClickDirectToWindow(bound,startClient,0,hold);
                        Thread.Sleep(Math.Max(20,SystemInformation.DoubleClickTime/3));
                        PerformClickDirectToWindow(bound,startClient,0,hold);
                    }
                    else
                    {
                        PerformClick(startScreen,0,hold,freeMouseMode);
                        Thread.Sleep(Math.Max(20,SystemInformation.DoubleClickTime/3));
                        PerformClick(startScreen,0,hold,freeMouseMode);
                    }
                    break;
                case 4: PerformDrag(startScreen,endScreen,hold); break;
                case 5: KeyboardSimulator.ExecuteKeyPress(step.KeyData,hold); break;
                case 6: KeyboardSimulator.ExecuteTypeText(step.KeyData,hold); break;
                case 7:
                    int slept=0; while (slept<hold) { int d=Math.Min(20,hold-slept); Thread.Sleep(d); slept+=d; }
                    break;
            }
        }

        // 0x06000010 RVA 0x3290: when scrollStep == 0 it degrades to middle click;
        // otherwise IL emits MOUSEEVENTF_WHEEL via SendInput in ±120-unit steps.
        internal static void PerformMiddleScroll(Point p, int scrollStep, int holdMs, bool freeMouseMode)
        {
            if (scrollStep == 0) { PerformClick(p, 2, holdMs, freeMouseMode); return; }
            NativeMethods.SetCursorPos(p.X, p.Y);
            NativeMethods.SendPhysicalWheel(scrollStep);
        }

        // 0x06000011 RVA 0x3450: drag is a physical path even if ordinary click can be background.
        internal static void PerformDrag(Point start, Point end, int holdMs)
        {
            NativeMethods.SetCursorPos(start.X, start.Y);
            NativeMethods.SendPhysicalMouseDown(0);
            if (holdMs > 0) Thread.Sleep(holdMs);
            MouseMovementSimulator.MoveSmoothly(start, end, 10, () => true);
            NativeMethods.SendPhysicalMouseUp(0);
        }
    }
}
