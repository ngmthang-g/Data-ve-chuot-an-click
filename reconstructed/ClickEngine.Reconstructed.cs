// RECONSTRUCTED. Exact IL: reconstructed/evidence/ClickEngine.il.txt plus closure token 0x06000507 in raw full IL.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace ModernAutoClicker.Reconstructed
{
    internal sealed class ClickEngine
    {
        private volatile bool _isRunning;
        private Thread _workerThread;
        private long _totalClicks;
        private DateTime _startTime;
        private static readonly Random _rng = new Random();
        public event Action<long, TimeSpan> OnProgressUpdated;
        public event Action<int> OnPointExecuting;
        public event Action OnStopped;
        public bool IsRunning => _isRunning;
        public long TotalClicks => _totalClicks;

        internal Point ApplyJitter(Point p, int radius) { if (radius <= 0) return p; return new Point(p.X + _rng.Next(-radius, radius + 1), p.Y + _rng.Next(-radius, radius + 1)); }
        internal Point ResolveActualPoint(Point p, string processName, string windowTitle, bool relativeToWindow) { if (!relativeToWindow || (string.IsNullOrEmpty(processName) && string.IsNullOrEmpty(windowTitle))) return p; IntPtr hwnd=NativeMethods.FindWindowByTarget(processName,windowTitle); if(hwnd==IntPtr.Zero)return p; var np=new NativeMethods.POINT{X=p.X,Y=p.Y}; if(!NativeMethods.ClientToScreen(hwnd,ref np))return p; return new Point(np.X,np.Y); }

        public void Start(ClickConfigSnapshot config) { if(_isRunning)return; _isRunning=true; _totalClicks=0; _startTime=DateTime.Now; _workerThread=new Thread(()=>Worker(config)){IsBackground=true}; _workerThread.Start(); }
        private void Worker(ClickConfigSnapshot c)
        {
            int interval=Math.Max(1,c.IntervalMs), holdMs=Math.Min(10,interval), remainingDelay=Math.Max(0,interval-holdMs), pointIndex=0, loopsCompleted=0;
            bool hasPoints=c.ClickMode==0 && c.PointsList!=null && c.PointsList.Count>0; DateTime lastProgress=DateTime.MinValue;
            while(_isRunning)
            {
                if(c.FreeMouseMode)
                {
                    if(hasPoints)
                    {
                        int index=pointIndex%c.PointsList.Count; OnPointExecuting?.Invoke(index); Point p=ApplyJitter(c.PointsList[index],c.JitterPx);
                        IntPtr hwnd=c.RelativeToWindow?NativeMethods.FindWindowByTarget(c.TargetProcessName,c.TargetWindowTitle):IntPtr.Zero;
                        if(hwnd!=IntPtr.Zero) NativeMethods.PerformClickDirectToWindow(hwnd,p.X,p.Y,c.MouseButton,holdMs);
                        else { Point screen=ResolveActualPoint(p,c.TargetProcessName,c.TargetWindowTitle,c.RelativeToWindow); NativeMethods.SendBackgroundClick(screen.X,screen.Y,c.MouseButton,holdMs); }
                        pointIndex=(pointIndex+1)%c.PointsList.Count; if(pointIndex==0)loopsCompleted++;
                    }
                    else if(NativeMethods.GetCursorPos(out NativeMethods.POINT cp)) { Point p=ApplyJitter(new Point(cp.X,cp.Y),c.JitterPx); NativeMethods.SendBackgroundClick(p.X,p.Y,c.MouseButton,holdMs); loopsCompleted++; }
                }
                else
                {
                    if(hasPoints) { int index=pointIndex%c.PointsList.Count; OnPointExecuting?.Invoke(index); Point p=ApplyJitter(c.PointsList[index],c.JitterPx); p=ResolveActualPoint(p,c.TargetProcessName,c.TargetWindowTitle,c.RelativeToWindow); NativeMethods.SetCursorPos(p.X,p.Y); pointIndex=(pointIndex+1)%c.PointsList.Count; if(pointIndex==0)loopsCompleted++; }
                    else if(c.JitterPx>0 && NativeMethods.GetCursorPos(out NativeMethods.POINT cp)) { Point p=ApplyJitter(new Point(cp.X,cp.Y),c.JitterPx); NativeMethods.SetCursorPos(p.X,p.Y); loopsCompleted++; }
                    NativeMethods.SendPhysicalClick(c.MouseButton,holdMs);
                }
                Interlocked.Increment(ref _totalClicks); DateTime now=DateTime.Now;
                if(interval>=10 || (now-lastProgress).TotalMilliseconds>=25 || _totalClicks==1){ lastProgress=now; OnProgressUpdated?.Invoke(_totalClicks,now-_startTime); }
                if(c.Loops>0 && loopsCompleted>=c.Loops){ _isRunning=false; OnProgressUpdated?.Invoke(_totalClicks,DateTime.Now-_startTime); OnPointExecuting?.Invoke(-1); OnStopped?.Invoke(); return; }
                if(remainingDelay>0)
                {
                    if(c.SmoothMouseMove && !c.FreeMouseMode && hasPoints && c.PointsList.Count>1 && remainingDelay>20){ int nextIndex=pointIndex%c.PointsList.Count; Point next=ResolveActualPoint(c.PointsList[nextIndex],c.TargetProcessName,c.TargetWindowTitle,c.RelativeToWindow); MouseMovementSimulator.MoveSmoothly(Point.Empty,next,remainingDelay,()=>_isRunning); }
                    else Thread.Sleep(remainingDelay);
                }
            }
        }
        public void Stop(){ _isRunning=false; try{if(_workerThread!=null&&_workerThread.IsAlive)_workerThread.Join(200);}catch{} _workerThread=null; }
    }
    internal sealed class ClickConfigSnapshot { public int IntervalMs,ClickMode,JitterPx,MouseButton,Loops; public bool FreeMouseMode,RelativeToWindow,SmoothMouseMove; public string TargetProcessName,TargetWindowTitle; public List<Point> PointsList=new List<Point>(); }
}
