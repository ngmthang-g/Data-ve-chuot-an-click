// RECONSTRUCTED. Exact IL: reconstructed/evidence/UnfocusClickFilter.il.txt
using System.Windows.Forms;
namespace ModernAutoClicker.Reconstructed
{
    internal sealed class UnfocusClickFilter:IMessageFilter
    {
        private readonly Form _form; internal UnfocusClickFilter(Form form){_form=form;}
        public bool PreFilterMessage(ref Message m){if(m.Msg!=0x201&&m.Msg!=0x204&&m.Msg!=0x0A1)return false;if(_form==null||_form.IsDisposed||!_form.IsHandleCreated)return false;Control active=GetDeepActiveControl(_form);if(!(active is TextBox)&&!(active is NumberInput))return false;Control clicked=Control.FromHandle(m.HWnd);if(clicked!=null&&(clicked==active||IsChildOf(clicked,active)))return false;if(clicked!=null&&clicked.CanFocus&&!(clicked is Panel)&&!(clicked is Label))clicked.Focus();else _form.ActiveControl=null;return false;}
        internal static Control GetDeepActiveControl(ContainerControl c){if(c==null)return null;Control current=c.ActiveControl;while(current is ContainerControl cc&&cc.ActiveControl!=null)current=cc.ActiveControl;return current;}
        internal static bool IsChildOf(Control child,Control parent){for(Control c=child;c!=null;c=c.Parent)if(c==parent)return true;return false;}
    }
    internal class NumberInput:Control{}
}
