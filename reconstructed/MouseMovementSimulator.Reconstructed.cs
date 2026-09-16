// RECONSTRUCTED. Exact IL: reconstructed/evidence/MouseMovementSimulator.il.txt
using System; using System.Diagnostics; using System.Drawing; using System.Threading;
namespace ModernAutoClicker.Reconstructed
{
    internal static class MouseMovementSimulator
    {
        internal static void MoveSmoothly(Point start,Point end,int durationMs,Func<bool> keepRunning){if(keepRunning!=null&&!keepRunning())return;if(start==Point.Empty){if(NativeMethods.GetCursorPos(out NativeMethods.POINT cp))start=new Point(cp.X,cp.Y);else start=end;}if(end==Point.Empty){if(durationMs>0)Thread.Sleep(durationMs);return;}if(durationMs<=20||start==end){NativeMethods.SetCursorPos(end.X,end.Y);if(durationMs>0)Thread.Sleep(durationMs);return;}var sw=Stopwatch.StartNew();while(sw.ElapsedMilliseconds<durationMs){if(keepRunning!=null&&!keepRunning())return;float t=(float)sw.ElapsedMilliseconds/durationMs;if(t>1f)t=1f;float eased=t*t*(3f-2f*t);int x=(int)(start.X+(end.X-start.X)*eased),y=(int)(start.Y+(end.Y-start.Y)*eased);NativeMethods.SetCursorPos(x,y);Thread.Sleep(10);}sw.Stop();if(keepRunning==null||keepRunning())NativeMethods.SetCursorPos(end.X,end.Y);}
    }
}
