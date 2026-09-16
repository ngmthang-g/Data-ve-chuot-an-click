// RECONSTRUCTED. Exact IL: reconstructed/evidence/WindowTracker.il.txt
using System; using System.Collections.Generic; using System.Text; using System.Windows.Forms; using ModernAutoClicker.Reconstructed.Advanced;
namespace ModernAutoClicker.Reconstructed
{
    internal sealed class WindowTracker:IDisposable
    {
        private readonly Timer _timer; private string _lastSignature=""; private bool _isPaused; public event Action OnPositionsChanged;
        public Func<bool> IsWindowActive,IsRunning,IsBasicTab,IsOverlayInteracting,IsShowMapChecked,SimpleRelativeToWindow; public Func<string> SimpleTargetProcessName,SimpleTargetWindowTitle; public Func<List<MacroStepSnapshot>> GetAdvancedSteps;
        internal WindowTracker(int intervalMs){_timer=new Timer{Interval=intervalMs};_timer.Tick+=Timer_Tick;} internal void Start()=>_timer.Start(); internal void Stop()=>_timer.Stop(); internal void Pause()=>_isPaused=true; internal void Resume()=>_isPaused=false; internal void ResetSignature()=>_lastSignature=string.Empty;
        internal void CheckNow(){if(_isPaused)return;string s=GetSignature();if(s!=_lastSignature){_lastSignature=s;OnPositionsChanged?.Invoke();}}
        private void Timer_Tick(object sender,EventArgs e){if(_isPaused)return;if(IsShowMapChecked!=null&&!IsShowMapChecked())return;if(IsWindowActive!=null&&!IsWindowActive())return;if(IsRunning!=null&&IsRunning())return;if(IsOverlayInteracting!=null&&IsOverlayInteracting())return;CheckNow();}
        internal string GetSignature(){bool basic=IsBasicTab!=null&&IsBasicTab();if(basic){bool relative=SimpleRelativeToWindow!=null&&SimpleRelativeToWindow();string proc=SimpleTargetProcessName?.Invoke()??"",title=SimpleTargetWindowTitle?.Invoke()??"";if(!relative&&string.IsNullOrEmpty(proc))return "Desktop";IntPtr hwnd=NativeMethods.FindWindowByTarget(proc,title);if(hwnd==IntPtr.Zero)return "NotFound";var p=new NativeMethods.POINT{X=0,Y=0};NativeMethods.ClientToScreen(hwnd,ref p);return string.Format("{0}:{1},{2}",hwnd,p.X,p.Y);}var steps=GetAdvancedSteps?.Invoke();if(steps==null||steps.Count==0)return "";var sb=new StringBuilder(128);var seen=new HashSet<string>(StringComparer.OrdinalIgnoreCase);foreach(var s in steps){string proc=s.ProcessName??"",title=s.WindowTitle??"";if(!s.RelativeToWindow||string.IsNullOrEmpty(proc))continue;string key=proc+"|"+title;if(!seen.Add(key))continue;IntPtr hwnd=NativeMethods.FindWindowByTarget(proc,title);var p=new NativeMethods.POINT{X=0,Y=0};if(hwnd!=IntPtr.Zero)NativeMethods.ClientToScreen(hwnd,ref p);sb.Append(hwnd).Append(':').Append(p.X).Append(',').Append(p.Y).Append(';');}return sb.ToString();}
        public void Dispose(){_timer.Stop();_timer.Dispose();}
    }
}
