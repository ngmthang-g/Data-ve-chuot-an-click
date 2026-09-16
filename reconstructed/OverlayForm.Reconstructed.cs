// RECONSTRUCTED. Exact IL: reconstructed/evidence/OverlayForm.il.txt
using System; using System.Drawing; using System.Windows.Forms;
namespace ModernAutoClicker.Reconstructed
{
    internal class OverlayForm : Form
    {
        private const int WM_NCHITTEST=0x84, WM_MOUSEACTIVATE=0x21, HTTRANSPARENT=-1, HTCLIENT=1, MA_NOACTIVATE=3, GWL_EXSTYLE=-20, WS_EX_TRANSPARENT=0x20;
        private bool _isClickThrough;
        protected override bool ShowWithoutActivation => true;
        protected override CreateParams CreateParams { get { var cp=base.CreateParams; cp.ExStyle|=0x08000088; return cp; } }
        internal void SetClickThrough(bool enabled){_isClickThrough=enabled;if(!IsHandleCreated)return;long style=NativeMethods.GetWindowLongPtrReference(Handle,GWL_EXSTYLE).ToInt64();style=enabled?(style|WS_EX_TRANSPARENT):(style&~WS_EX_TRANSPARENT);NativeMethods.SetWindowLongPtrReference(Handle,GWL_EXSTYLE,new IntPtr(style));}
        protected override void WndProc(ref Message m){if(_isClickThrough){base.WndProc(ref m);return;}if(m.Msg==WM_MOUSEACTIVATE){m.Result=new IntPtr(MA_NOACTIVATE);return;}if(m.Msg==WM_NCHITTEST){int raw=m.LParam.ToInt32(),sx=(short)(raw&0xffff),sy=(short)((raw>>16)&0xffff);Point client=PointToClient(new Point(sx,sy));m.Result=new IntPtr(IsInteractiveMarker(client)?HTCLIENT:HTTRANSPARENT);return;}base.WndProc(ref m);}
        private bool IsInteractiveMarker(Point p)=>GetPointAtReference(p)>=0;
        private int GetPointAtReference(Point p)=>throw new NotSupportedException("Exact marker hit-testing is in reconstructed/evidence/OverlayForm.il.txt (GetPointAt).");
        internal void ShowOverlayReference(){Show();Invalidate();} internal void ClearAndHideReference(){Hide();Invalidate();}
    }
}
