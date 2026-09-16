// RECONSTRUCTED from Auto Clicker by Max(1).exe IL/metadata.
// Exact evidence: reconstructed/evidence/NativeMethods.il.txt
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ModernAutoClicker.Reconstructed
{
    internal static class NativeMethods
    {
        internal const int WM_MOUSEMOVE = 0x0200;
        internal const int WM_LBUTTONDOWN = 0x0201, WM_LBUTTONUP = 0x0202;
        internal const int WM_RBUTTONDOWN = 0x0204, WM_RBUTTONUP = 0x0205;
        internal const int WM_MBUTTONDOWN = 0x0207, WM_MBUTTONUP = 0x0208;
        internal const uint MK_LBUTTON = 0x0001, MK_RBUTTON = 0x0002, MK_MBUTTON = 0x0010;
        internal const uint MOUSEEVENTF_LEFTDOWN = 0x0002, MOUSEEVENTF_LEFTUP = 0x0004;
        internal const uint MOUSEEVENTF_RIGHTDOWN = 0x0008, MOUSEEVENTF_RIGHTUP = 0x0010;
        internal const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020, MOUSEEVENTF_MIDDLEUP = 0x0040;
        internal const uint GA_ROOT = 2;

        [StructLayout(LayoutKind.Sequential)] internal struct POINT { public int X, Y; }
        [StructLayout(LayoutKind.Sequential)] internal struct RECT { public int Left, Top, Right, Bottom; }
        [StructLayout(LayoutKind.Sequential)] internal struct MOUSEINPUT { public int dx, dy; public uint mouseData, dwFlags, time; public IntPtr dwExtraInfo; }
        [StructLayout(LayoutKind.Sequential)] internal struct KEYBDINPUT { public ushort wVk, wScan; public uint dwFlags, time; public IntPtr dwExtraInfo; }
        [StructLayout(LayoutKind.Explicit)] internal struct InputUnion { [FieldOffset(0)] public MOUSEINPUT mi; [FieldOffset(0)] public KEYBDINPUT ki; }
        [StructLayout(LayoutKind.Sequential)] internal struct INPUT { public uint type; public InputUnion u; }

        [DllImport("user32.dll")] internal static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")] internal static extern uint SendInput(uint count, INPUT[] inputs, int size);
        [DllImport("user32.dll")] internal static extern bool GetCursorPos(out POINT point);
        [DllImport("user32.dll")] internal static extern IntPtr WindowFromPoint(POINT point);
        [DllImport("user32.dll")] internal static extern IntPtr RealChildWindowFromPoint(IntPtr parent, POINT point);
        [DllImport("user32.dll")] internal static extern bool ScreenToClient(IntPtr hwnd, ref POINT point);
        [DllImport("user32.dll")] internal static extern bool ClientToScreen(IntPtr hwnd, ref POINT point);
        [DllImport("user32.dll")] internal static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")] internal static extern IntPtr GetAncestor(IntPtr hwnd, uint flags);
        [DllImport("user32.dll")] internal static extern bool IsWindow(IntPtr hwnd);
        [DllImport("user32.dll")] internal static extern bool IsWindowVisible(IntPtr hwnd);
        [DllImport("user32.dll")] internal static extern bool IsIconic(IntPtr hwnd);
        [DllImport("user32.dll")] internal static extern bool EnumWindows(EnumWindowsProc proc, IntPtr lParam);
        [DllImport("user32.dll")] internal static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint pid);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] internal static extern int GetWindowText(IntPtr hwnd, StringBuilder text, int max);
        [DllImport("user32.dll")] internal static extern int GetWindowTextLength(IntPtr hwnd);
        [DllImport("user32.dll")] internal static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("user32.dll")] internal static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
        [DllImport("gdi32.dll")] internal static extern int GetPixel(IntPtr hdc, int x, int y);
        [DllImport("user32.dll", EntryPoint="GetWindowLong")] private static extern IntPtr GetWindowLong32(IntPtr hwnd, int index);
        [DllImport("user32.dll", EntryPoint="GetWindowLongPtr")] private static extern IntPtr GetWindowLongPtr64(IntPtr hwnd, int index);
        [DllImport("user32.dll", EntryPoint="SetWindowLong")] private static extern IntPtr SetWindowLong32(IntPtr hwnd, int index, IntPtr value);
        [DllImport("user32.dll", EntryPoint="SetWindowLongPtr")] private static extern IntPtr SetWindowLongPtr64(IntPtr hwnd, int index, IntPtr value);
        internal delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        private sealed class TargetWindowCacheEntry { public IntPtr Hwnd; public DateTime Timestamp; }
        private static readonly Dictionary<string, TargetWindowCacheEntry> _targetWindowCache = new Dictionary<string, TargetWindowCacheEntry>(StringComparer.OrdinalIgnoreCase);
        private static IntPtr Pack(int x, int y) => new IntPtr(((y & 0xffff) << 16) | (x & 0xffff));

        internal static IntPtr GetTopLevelWindow(IntPtr hwnd){if(hwnd==IntPtr.Zero)return IntPtr.Zero;IntPtr root=GetAncestor(hwnd,GA_ROOT);return root!=IntPtr.Zero?root:hwnd;}
        internal static void SendPhysicalClick(int button,int holdMs){uint down=MOUSEEVENTF_LEFTDOWN,up=MOUSEEVENTF_LEFTUP;if(button==1){down=MOUSEEVENTF_RIGHTDOWN;up=MOUSEEVENTF_RIGHTUP;}else if(button==2){down=MOUSEEVENTF_MIDDLEDOWN;up=MOUSEEVENTF_MIDDLEUP;}var input=new[]{new INPUT{type=0,u=new InputUnion{mi=new MOUSEINPUT{dwFlags=down}}}};SendInput(1,input,Marshal.SizeOf(typeof(INPUT)));if(holdMs>0)Thread.Sleep(holdMs);input[0].u.mi.dwFlags=up;SendInput(1,input,Marshal.SizeOf(typeof(INPUT)));}

        internal static void SendBackgroundClick(int screenX,int screenY,int button,int holdMs){var screen=new POINT{X=screenX,Y=screenY};IntPtr hwnd=WindowFromPoint(screen);if(hwnd==IntPtr.Zero)return;var client=screen;ScreenToClient(hwnd,ref client);IntPtr child=RealChildWindowFromPoint(hwnd,client);if(child!=IntPtr.Zero&&child!=hwnd){var childClient=screen;ScreenToClient(child,ref childClient);hwnd=child;client=childClient;}SelectMessages(button,out int down,out int up,out IntPtr downWParam);IntPtr lp=Pack(client.X,client.Y);PostMessage(hwnd,WM_MOUSEMOVE,IntPtr.Zero,lp);PostMessage(hwnd,down,downWParam,lp);if(holdMs>0)Thread.Sleep(holdMs);PostMessage(hwnd,up,IntPtr.Zero,lp);}

        internal static void PerformClickDirectToWindow(IntPtr hwnd,int clientX,int clientY,int button,int holdMs){if(hwnd==IntPtr.Zero)return;var point=new POINT{X=clientX,Y=clientY};IntPtr child=RealChildWindowFromPoint(hwnd,point);if(child!=IntPtr.Zero&&child!=hwnd){var converted=point;ClientToScreen(hwnd,ref converted);ScreenToClient(child,ref converted);hwnd=child;point=converted;}SelectMessages(button,out int down,out int up,out IntPtr downWParam);IntPtr lp=Pack(point.X,point.Y);PostMessage(hwnd,WM_MOUSEMOVE,IntPtr.Zero,lp);PostMessage(hwnd,down,downWParam,lp);if(holdMs>0)Thread.Sleep(holdMs);PostMessage(hwnd,up,IntPtr.Zero,lp);}

        internal static void PostMouseSequence(IntPtr hwnd,int x,int y,int button,int holdMs){SelectMessages(button,out int down,out int up,out IntPtr wp);IntPtr lp=Pack(x,y);PostMessage(hwnd,WM_MOUSEMOVE,IntPtr.Zero,lp);PostMessage(hwnd,down,wp,lp);if(holdMs>0)Thread.Sleep(holdMs);PostMessage(hwnd,up,IntPtr.Zero,lp);}
        internal static void SendPhysicalMouseDown(int button){uint flag=button==1?MOUSEEVENTF_RIGHTDOWN:button==2?MOUSEEVENTF_MIDDLEDOWN:MOUSEEVENTF_LEFTDOWN;var a=new[]{new INPUT{type=0,u=new InputUnion{mi=new MOUSEINPUT{dwFlags=flag}}}};SendInput(1,a,Marshal.SizeOf(typeof(INPUT)));}
        internal static void SendPhysicalMouseUp(int button){uint flag=button==1?MOUSEEVENTF_RIGHTUP:button==2?MOUSEEVENTF_MIDDLEUP:MOUSEEVENTF_LEFTUP;var a=new[]{new INPUT{type=0,u=new InputUnion{mi=new MOUSEINPUT{dwFlags=flag}}}};SendInput(1,a,Marshal.SizeOf(typeof(INPUT)));}
        internal static void SendPhysicalWheel(int steps){uint wheel=0x0800;int sign=Math.Sign(steps);for(int i=0;i<Math.Abs(steps);i++){var a=new[]{new INPUT{type=0,u=new InputUnion{mi=new MOUSEINPUT{mouseData=unchecked((uint)(sign*120)),dwFlags=wheel}}}};SendInput(1,a,Marshal.SizeOf(typeof(INPUT)));}}
        internal static INPUT CreateUnicodeKeyInput(char c,bool up)=>new INPUT{type=1,u=new InputUnion{ki=new KEYBDINPUT{wScan=c,dwFlags=(uint)(0x0004|(up?0x0002:0))}}};
        internal static INPUT CreateVirtualKeyInput(ushort vk,bool up,bool extended)=>new INPUT{type=1,u=new InputUnion{ki=new KEYBDINPUT{wVk=vk,dwFlags=(uint)((up?0x0002:0)|(extended?0x0001:0))}}};
        internal static Color GetPixelColorReference(int x,int y){IntPtr dc=GetDC(IntPtr.Zero);try{int c=GetPixel(dc,x,y);if(c==-1)return Color.Black;return Color.FromArgb(c&255,(c>>8)&255,(c>>16)&255);}catch{return Color.Black;}finally{ReleaseDC(IntPtr.Zero,dc);}}
        internal static IntPtr GetWindowLongPtrReference(IntPtr hwnd,int index)=>IntPtr.Size==8?GetWindowLongPtr64(hwnd,index):GetWindowLong32(hwnd,index);
        internal static IntPtr SetWindowLongPtrReference(IntPtr hwnd,int index,IntPtr value)=>IntPtr.Size==8?SetWindowLongPtr64(hwnd,index,value):SetWindowLong32(hwnd,index,value);
        internal static string ReadWindowTitleReference(IntPtr hwnd)=>ReadWindowTitle(hwnd);
        private static void SelectMessages(int button,out int down,out int up,out IntPtr wParam){if(button==1){down=WM_RBUTTONDOWN;up=WM_RBUTTONUP;wParam=new IntPtr(MK_RBUTTON);}else if(button==2){down=WM_MBUTTONDOWN;up=WM_MBUTTONUP;wParam=new IntPtr(MK_MBUTTON);}else{down=WM_LBUTTONDOWN;up=WM_LBUTTONUP;wParam=new IntPtr(MK_LBUTTON);}}

        internal static IntPtr FindWindowByTarget(string processName,string windowTitle)
        {
            string proc=(processName??"").Trim();if(proc.EndsWith(".exe",StringComparison.OrdinalIgnoreCase))proc=proc.Substring(0,proc.Length-4);string title=(windowTitle??"").Trim().TrimEnd('.').Trim();if(proc.Length==0&&title.Length==0)return IntPtr.Zero;
            string key=proc+"|"+title;lock(_targetWindowCache){TargetWindowCacheEntry e;if(_targetWindowCache.TryGetValue(key,out e)&&e!=null&&(DateTime.Now-e.Timestamp).TotalMilliseconds<1000){if(e.Hwnd==IntPtr.Zero)return IntPtr.Zero;if(IsWindow(e.Hwnd)&&IsWindowVisible(e.Hwnd)&&!IsIconic(e.Hwnd))return e.Hwnd;}}
            var pids=new HashSet<int>();if(proc.Length!=0){try{foreach(Process p in Process.GetProcessesByName(proc)){pids.Add(p.Id);p.Dispose();}}catch{}if(pids.Count==0&&title.Length==0)return IntPtr.Zero;}
            IntPtr exactMatch=IntPtr.Zero,processMatch=IntPtr.Zero;EnumWindows((h,_)=>{if(!IsWindowVisible(h)||IsIconic(h))return true;GetWindowThreadProcessId(h,out uint pid);bool procOk=pids.Count==0||pids.Contains((int)pid);if(!procOk)return true;string actual=ReadWindowTitle(h);if(title.Length>0&&string.Equals(actual.Trim(),title,StringComparison.OrdinalIgnoreCase)){exactMatch=h;return false;}if(processMatch==IntPtr.Zero&&(title.Length==0||actual.IndexOf(title,StringComparison.OrdinalIgnoreCase)>=0))processMatch=h;return true;},IntPtr.Zero);
            IntPtr result=exactMatch!=IntPtr.Zero?exactMatch:processMatch;lock(_targetWindowCache)_targetWindowCache[key]=new TargetWindowCacheEntry{Hwnd=result,Timestamp=DateTime.Now};return result;
        }
        private static string ReadWindowTitle(IntPtr hwnd){int n=GetWindowTextLength(hwnd);var sb=new StringBuilder(n+1);GetWindowText(hwnd,sb,sb.Capacity);return sb.ToString();}
    }
}
