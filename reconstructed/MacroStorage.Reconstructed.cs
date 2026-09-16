// RECONSTRUCTED schema-oriented view. Exact serializer/parser IL: reconstructed/evidence/MacroStorage.il.txt
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace ModernAutoClicker.Reconstructed.Advanced
{
    internal static class MacroStorage
    {
        internal static string Escape(string s) => (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n");
        internal static string ExtractString(string json, string quotedKey){if(json==null)return null;var m=Regex.Match(json,Regex.Escape(quotedKey)+@"\s*:\s*\"\"((?:\\.|[^\"\"\\])*)\"\"");return m.Success?Regex.Unescape(m.Groups[1].Value):null;}
        internal static bool ExtractInt(string json,string quotedKey,out int value){value=0;if(json==null)return false;var m=Regex.Match(json,Regex.Escape(quotedKey)+@"\s*:\s*(-?\d+)");return m.Success&&int.TryParse(m.Groups[1].Value,out value);}
        internal static bool ExtractBool(string json,string quotedKey,out bool value){value=false;if(json==null)return false;var m=Regex.Match(json,Regex.Escape(quotedKey)+@"\s*:\s*(true|false)",RegexOptions.IgnoreCase);return m.Success&&bool.TryParse(m.Groups[1].Value,out value);}
        internal static List<string> SplitJsonObjects(string arrayBody){var result=new List<string>();if(string.IsNullOrWhiteSpace(arrayBody))return result;int depth=0,start=-1;bool quoted=false,escape=false;for(int i=0;i<arrayBody.Length;i++){char c=arrayBody[i];if(quoted){if(escape)escape=false;else if(c=='\\')escape=true;else if(c=='\"')quoted=false;continue;}if(c=='\"'){quoted=true;continue;}if(c=='{'){if(depth++==0)start=i;}else if(c=='}'&&depth>0&&--depth==0&&start>=0){result.Add(arrayBody.Substring(start,i-start+1));start=-1;}}return result;}
        internal static MacroProfileSnapshot FromJsonReference(string json)
        {
            var p=new MacroProfileSnapshot();p.Name=ExtractString(json,"\"Name\"")??"Macro";if(ExtractInt(json,"\"LoopCount\"",out int loops))p.LoopCount=loops;if(ExtractInt(json,"\"RandomIntervalMs\"",out int ri))p.RandomIntervalMs=ri;if(ExtractInt(json,"\"RandomJitterPx\"",out int rj))p.RandomJitterPx=rj;p.DefaultWindowTitle=ExtractString(json,"\"DefaultWindowTitle\"")??"";p.DefaultProcessName=ExtractString(json,"\"DefaultProcessName\"")??"";if(ExtractBool(json,"\"DefaultRelativeToWindow\"",out bool dr))p.DefaultRelativeToWindow=dr;int stepsAt=json.IndexOf("\"Steps\"",StringComparison.Ordinal);if(stepsAt<0)return p;int open=json.IndexOf('[',stepsAt),close=json.LastIndexOf(']');if(open<0||close<=open)return p;
            foreach(string obj in SplitJsonObjects(json.Substring(open+1,close-open-1))){var s=new MacroStepSnapshot{Id=ExtractString(obj,"\"Id\"")??Guid.NewGuid().ToString("N"),Name=ExtractString(obj,"\"Name\"")??"Step"};if(ExtractInt(obj,"\"ActionType\"",out int at))s.ActionType=at;if(ExtractBool(obj,"\"Enabled\"",out bool en)||ExtractBool(obj,"\"IsChecked\"",out en))s.Enabled=en;int sx=0,sy=0,ex=0,ey=0;ExtractInt(obj,"\"StartX\"",out sx);ExtractInt(obj,"\"StartY\"",out sy);ExtractInt(obj,"\"EndX\"",out ex);ExtractInt(obj,"\"EndY\"",out ey);s.StartPoint=new Point(sx,sy);s.EndPoint=new Point(ex,ey);if(ExtractInt(obj,"\"HoldMs\"",out int h))s.HoldMs=h;if(ExtractInt(obj,"\"DelayMs\"",out int d))s.DelayMs=d;if(ExtractInt(obj,"\"RepeatCount\"",out int r))s.RepeatCount=Math.Max(1,r);if(ExtractInt(obj,"\"ScrollStep\"",out int sc))s.ScrollStep=sc;s.KeyData=ExtractString(obj,"\"KeyData\"")??"Space";if(ExtractInt(obj,"\"Tolerance\"",out int tol))s.Tolerance=tol;s.Note=ExtractString(obj,"\"Note\"")??"";s.ColorHex=ExtractString(obj,"\"ColorHex\"")??"";s.ProcessName=ExtractString(obj,"\"ProcessName\"")??"";s.WindowTitle=ExtractString(obj,"\"WindowTitle\"")??"";if(ExtractBool(obj,"\"RelativeToWindow\"",out bool rel))s.RelativeToWindow=rel;if(!string.IsNullOrWhiteSpace(s.ColorHex)){try{s.TargetColor=ColorTranslator.FromHtml(s.ColorHex);}catch{}}if(ExtractInt(obj,"\"IfTrueStep\"",out int it))s.IfTrueStep=it;if(ExtractInt(obj,"\"IfFalseStep\"",out int iff))s.IfFalseStep=iff;p.Steps.Add(s);}return p;
        }
    }
}
