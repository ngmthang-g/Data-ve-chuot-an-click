// RECONSTRUCTED. Exact IL: reconstructed/evidence/MacroRunner.il.txt and worker closure token 0x060004DE.
// This is readable reference source, not claimed original source.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ModernAutoClicker.Reconstructed.Advanced
{
    internal sealed class MacroRunner
    {
        private volatile bool _isRunning;
        private Thread _workerThread;
        private DateTime _startTime;
        private int _totalCyclesCompleted;
        public bool IsRunning => _isRunning;
        public int TotalCyclesCompleted => _totalCyclesCompleted;
        public event Action<int,string> OnStepExecuting;
        public event Action<int,TimeSpan> OnProgressUpdated;
        public event Action OnStopped;

        // 0x0600001A RVA 0x3830 + closure 0x060004DE RVA 0x3690.
        public void Start(MacroProfileSnapshot profile, IEnumerable<MacroProfileSnapshot> subProfiles, bool freeMouseMode, bool smoothMouseMove)
        {
            if (_isRunning || profile == null || profile.Steps == null || profile.Steps.Count == 0) return;
            var steps = new List<MacroStepSnapshot>(); foreach (var s in profile.Steps) steps.Add(s.Clone());
            if (steps.Count == 0) { _isRunning = false; OnStopped?.Invoke(); return; }
            var scripts = new Dictionary<string,List<MacroStepSnapshot>>(StringComparer.OrdinalIgnoreCase);
            if (subProfiles != null) foreach (var p in subProfiles) { if (p == null || p.IsCombine || p.Steps == null) continue; var copy=new List<MacroStepSnapshot>(); foreach(var s in p.Steps) copy.Add(s.Clone()); scripts[p.Name ?? ""] = copy; }
            _isRunning=true; _totalCyclesCompleted=0; _startTime=DateTime.Now;
            int targetLoops=profile.LoopCount, randomInterval=Math.Max(0,profile.RandomIntervalMs), randomJitter=Math.Max(0,profile.RandomJitterPx);
            _workerThread=new Thread(()=>{ Thread.Sleep(60); int cycle=0; while(_isRunning && (targetLoops<=0 || cycle<targetLoops)){ ExecuteStepList(steps,scripts,freeMouseMode,randomInterval,randomJitter,smoothMouseMove); if(!_isRunning)break; _totalCyclesCompleted=++cycle; OnProgressUpdated?.Invoke(_totalCyclesCompleted,DateTime.Now-_startTime);} _isRunning=false; OnStopped?.Invoke(); }) { IsBackground=true, Name="MacroRunnerThread"};
            _workerThread.Start();
        }

        private void ExecuteStepList(List<MacroStepSnapshot> steps, Dictionary<string,List<MacroStepSnapshot>> scripts, bool freeMouse, int randomInterval, int randomJitter, bool smooth)
        { for(int i=0;i<steps.Count;i++){ if(!_isRunning)return; var step=steps[i]; if(!step.Enabled)continue; OnStepExecuting?.Invoke(i,DescribeStep(step)); ExecuteSingleStep(step,steps,ref i,scripts,freeMouse,randomInterval,randomJitter,smooth);} }
        private static string DescribeStep(MacroStepSnapshot step){ if(step.ActionType==12)return string.Format("Run {0} (x{1})",step.KeyData,step.RepeatCount); if(step.ActionType==7)return string.Format("Delay {0}ms",step.DelayMs); return step.Name; }

        // 0x0600001C RVA 0x3ABC.
        private void ExecuteSingleStep(MacroStepSnapshot step, List<MacroStepSnapshot> current, ref int index, Dictionary<string,List<MacroStepSnapshot>> scripts, bool freeMouse, int randomInterval, int randomJitter, bool smooth)
        {
            if(step.ActionType==12){ if(scripts!=null && scripts.TryGetValue(step.KeyData??"",out var nested)&&nested!=null&&nested.Count>0){int loops=Math.Max(1,step.RepeatCount);for(int r=0;r<loops&&_isRunning;r++)ExecuteStepList(nested,scripts,freeMouse,randomInterval,randomJitter,smooth);} SleepAppliedDelay(step.DelayMs,randomInterval); return; }
            if(step.ActionType==8){WaitForColor(step,randomInterval);return;}
            if(step.ActionType==11){WaitForChange(step,randomInterval);return;}
            if(step.ActionType==9||step.ActionType==10){EvaluateColorBranch(step,current,ref index,freeMouse,randomInterval,randomJitter,smooth);return;}
            int repeatCount=Math.Max(1,step.RepeatCount);
            for(int repeat=0;repeat<repeatCount;repeat++){ if(!_isRunning)return; ActionExecutor.Execute(step,freeMouse,randomInterval,randomJitter); int delay=ActionExecutor.ApplyDelayInterval(step.DelayMs,randomInterval); if(delay<=0||!_isRunning)continue; SleepOrSmoothAfterAction(step,current,index,repeat,repeatCount,freeMouse,delay,smooth); }
        }

        private void WaitForColor(MacroStepSnapshot step,int randomInterval){int tolerance=step.Tolerance>0?step.Tolerance:10;while(_isRunning){Point p=ActionExecutor.ResolveActualScreenPoint(step,step.StartPoint);Color actual=NativeMethods.GetPixelColorReference(p.X,p.Y);if(ActionExecutor.MatchesColor(actual,step.TargetColor,tolerance))break;Thread.Sleep(20);}SleepAppliedDelay(step.DelayMs,randomInterval);}
        private void WaitForChange(MacroStepSnapshot step,int randomInterval){int tolerance=step.Tolerance>0?step.Tolerance:10;Point p=ActionExecutor.ResolveActualScreenPoint(step,step.StartPoint);Color initial=NativeMethods.GetPixelColorReference(p.X,p.Y);while(_isRunning){p=ActionExecutor.ResolveActualScreenPoint(step,step.StartPoint);Color current=NativeMethods.GetPixelColorReference(p.X,p.Y);if(!ActionExecutor.MatchesColor(current,initial,tolerance))break;Thread.Sleep(20);}SleepAppliedDelay(step.DelayMs,randomInterval);}

        private void EvaluateColorBranch(MacroStepSnapshot step,List<MacroStepSnapshot> current,ref int index,bool freeMouse,int randomInterval,int randomJitter,bool smooth)
        {
            int tolerance=step.Tolerance>0?step.Tolerance:10; bool matched; Point matchedPoint=Point.Empty;
            if(step.ActionType==10){Point a=ActionExecutor.ResolveActualScreenPoint(step,step.StartPoint),b=ActionExecutor.ResolveActualScreenPoint(step,step.EndPoint);var area=new Rectangle(Math.Min(a.X,b.X),Math.Min(a.Y,b.Y),Math.Max(1,Math.Abs(b.X-a.X)),Math.Max(1,Math.Abs(b.Y-a.Y)));Point found=ActionExecutor.FindMatchingPixelInArea(area,step.TargetColor,tolerance);if(found!=Point.Empty){matched=true;matchedPoint=found;}else{matched=false;matchedPoint=a;}}
            else{matchedPoint=ActionExecutor.ResolveActualScreenPoint(step,step.StartPoint);Color actual=NativeMethods.GetPixelColorReference(matchedPoint.X,matchedPoint.Y);matched=ActionExecutor.MatchesColor(actual,step.TargetColor,tolerance);}
            int target=matched?step.IfTrueStep:step.IfFalseStep; if(target==-1){_isRunning=false;return;}
            int nextIndex;
            if(target==-2){ if(matched&&matchedPoint!=Point.Empty){MacroStepSnapshot click=step.Clone();click.ActionType=0;click.HoldMs=Math.Max(10,step.HoldMs);if(step.ActionType==10){if(step.RelativeToWindow&&!string.IsNullOrEmpty(step.ProcessName)){IntPtr hwnd=NativeMethods.FindWindowByTarget(step.ProcessName,step.WindowTitle);if(hwnd!=IntPtr.Zero){var p=new NativeMethods.POINT{X=matchedPoint.X,Y=matchedPoint.Y};if(NativeMethods.ScreenToClient(hwnd,ref p))click.StartPoint=new Point(p.X,p.Y);else click.StartPoint=matchedPoint;}else click.StartPoint=matchedPoint;}else click.StartPoint=matchedPoint;}ActionExecutor.Execute(click,freeMouse,randomInterval,randomJitter);} nextIndex=(index+1<current.Count)?index+1:0; }
            else if(target>0)nextIndex=target-1; else nextIndex=(index+1<current.Count)?index+1:0;
            if(matched){int delay=ActionExecutor.ApplyDelayInterval(step.DelayMs,randomInterval);if(delay>0&&_isRunning){if(smooth&&!freeMouse&&delay>20){Point destination=ResolveEnabledStepPoint(current,nextIndex);Point from=matchedPoint!=Point.Empty?matchedPoint:Cursor.Position;if(destination!=Point.Empty)MouseMovementSimulator.MoveSmoothly(from,destination,delay,()=>_isRunning);else Thread.Sleep(delay);}else Thread.Sleep(delay);}}
            if(nextIndex==0&&_isRunning)Thread.Sleep(1); if(nextIndex>=0&&nextIndex<current.Count)index=nextIndex-1;else index=current.Count;
        }

        private Point ResolveEnabledStepPoint(List<MacroStepSnapshot> steps,int candidateIndex){if(candidateIndex<0||candidateIndex>=steps.Count)return Point.Empty;var s=steps[candidateIndex];if(!s.Enabled||s.StartPoint==Point.Empty)return Point.Empty;return ActionExecutor.ResolveActualScreenPoint(s,s.StartPoint);}
        private void SleepOrSmoothAfterAction(MacroStepSnapshot step,List<MacroStepSnapshot> current,int index,int repeat,int repeatCount,bool freeMouse,int delay,bool smooth){if(!(smooth&&!freeMouse&&delay>20)){Thread.Sleep(delay);return;}Point destination=Point.Empty;if(repeat<repeatCount-1)destination=ActionExecutor.ResolveActualScreenPoint(step,step.StartPoint);else{for(int i=index+1;i<current.Count;i++){var candidate=current[i];if(!candidate.Enabled||candidate.StartPoint==Point.Empty)continue;destination=ActionExecutor.ResolveActualScreenPoint(candidate,candidate.StartPoint);break;}if(destination==Point.Empty&&current.Count>1){for(int i=0;i<=index&&i<current.Count;i++){var candidate=current[i];if(!candidate.Enabled||candidate.StartPoint==Point.Empty)continue;destination=ActionExecutor.ResolveActualScreenPoint(candidate,candidate.StartPoint);break;}}}if(destination!=Point.Empty){Point from=ActionExecutor.ResolveActualScreenPoint(step,step.StartPoint);MouseMovementSimulator.MoveSmoothly(from,destination,delay,()=>_isRunning);}else Thread.Sleep(delay);}
        private void SleepAppliedDelay(int delayMs,int randomInterval){int delay=ActionExecutor.ApplyDelayInterval(delayMs,randomInterval);if(delay>0&&_isRunning)Thread.Sleep(delay);}
        public void Stop(){_isRunning=false;try{if(_workerThread!=null&&_workerThread.IsAlive)_workerThread.Join(200);}catch{} _workerThread=null;}
    }

    internal sealed class MacroProfileSnapshot
    { public string Name="New Macro Profile",DefaultWindowTitle="",DefaultProcessName=""; public bool IsCombine=false,DefaultRelativeToWindow=false; public int LoopCount=0,RandomIntervalMs=0,RandomJitterPx=0; public List<MacroStepSnapshot> Steps=new List<MacroStepSnapshot>(); }
    internal sealed class MacroStepSnapshot
    {
        public string Id=Guid.NewGuid().ToString("N"),Name="Step",KeyData="Space",Note="",ProcessName="",WindowTitle="",ColorHex="#00FF00"; public bool Enabled=true,RelativeToWindow=false; public int ActionType=0,HoldMs=10,DelayMs=240,RepeatCount=1,ScrollStep=0,Tolerance=10,IfTrueStep=-2,IfFalseStep=0; public Point StartPoint=Point.Empty,EndPoint=Point.Empty; public Color TargetColor=Color.FromArgb(0,255,0);
        public MacroStepSnapshot Clone()=>new MacroStepSnapshot{Id=Guid.NewGuid().ToString("N"),Name=Name,KeyData=KeyData,Note=Note,ProcessName=ProcessName,WindowTitle=WindowTitle,ColorHex=ColorHex,Enabled=Enabled,RelativeToWindow=RelativeToWindow,ActionType=ActionType,HoldMs=HoldMs,DelayMs=DelayMs,RepeatCount=RepeatCount,ScrollStep=ScrollStep,Tolerance=Tolerance,IfTrueStep=IfTrueStep,IfFalseStep=IfFalseStep,StartPoint=StartPoint,EndPoint=EndPoint,TargetColor=TargetColor};
    }
}
