param(
    [string]$ProcessName,
    [int]$TargetPid = 0,
    [string]$WindowTitleContains = "",
    [Parameter(Mandatory=$true)][int]$ClientX,
    [Parameter(Mandatory=$true)][int]$ClientY,
    [ValidateSet("Left","Right","Middle")][string]$Button = "Left",
    [ValidateSet("NativeDirect","NativeScreen","AdvancedDirect","AdvancedScreen")][string]$TransportVariant = "NativeDirect",
    [int]$HoldMs = 25,
    [string]$TestId = "manual-probe",
    [string]$StateTag = "unspecified",
    [ValidateSet("success","no_effect","partial","unknown")][string]$ObservedResult = "unknown",
    [string]$Notes = "",
    [switch]$AllowIconic,
    [string]$Output = ".\runtime-probe.jsonl"
)

$ErrorActionPreference = "Stop"

Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
using System.Text;

public static class ProbeNative {
    [StructLayout(LayoutKind.Sequential)] public struct POINT { public int X; public int Y; }
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int Left; public int Top; public int Right; public int Bottom; }
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")] public static extern bool EnumWindows(EnumWindowsProc cb, IntPtr lParam);
    [DllImport("user32.dll")] public static extern bool IsWindow(IntPtr hWnd);
    [DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr hWnd);
    [DllImport("user32.dll")] public static extern bool IsIconic(IntPtr hWnd);
    [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint pid);
    [DllImport("user32.dll", CharSet=CharSet.Unicode)] public static extern int GetWindowText(IntPtr hWnd, StringBuilder sb, int maxCount);
    [DllImport("user32.dll")] public static extern int GetWindowTextLength(IntPtr hWnd);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
    [DllImport("user32.dll")] public static extern bool GetClientRect(IntPtr hWnd, out RECT rect);
    [DllImport("user32.dll")] public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);
    [DllImport("user32.dll")] public static extern bool ScreenToClient(IntPtr hWnd, ref POINT point);
    [DllImport("user32.dll")] public static extern IntPtr WindowFromPoint(POINT point);
    [DllImport("user32.dll")] public static extern IntPtr RealChildWindowFromPoint(IntPtr hwndParent, POINT ptParentClientCoords);
    [DllImport("user32.dll", SetLastError=true)] public static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")] public static extern bool GetCursorPos(out POINT point);
    [DllImport("user32.dll", EntryPoint="GetDpiForWindow")] public static extern uint GetDpiForWindow(IntPtr hWnd);

    public static string Title(IntPtr hWnd) {
        int len = GetWindowTextLength(hWnd);
        var sb = new StringBuilder(Math.Max(1, len + 1));
        GetWindowText(hWnd, sb, sb.Capacity);
        return sb.ToString();
    }
    public static IntPtr PackLParam(int x, int y) {
        int packed = ((y & 0xffff) << 16) | (x & 0xffff);
        return new IntPtr(packed);
    }
}
'@

function Resolve-TargetWindow {
    $matches = New-Object System.Collections.Generic.List[object]
    [ProbeNative]::EnumWindows({
        param($hWnd, $lParam)
        if (-not [ProbeNative]::IsWindow($hWnd)) { return $true }
        [uint32]$wpid = 0
        [void][ProbeNative]::GetWindowThreadProcessId($hWnd, [ref]$wpid)
        $title = [ProbeNative]::Title($hWnd)
        if ($TargetPid -gt 0 -and $wpid -ne [uint32]$TargetPid) { return $true }
        if ($ProcessName) {
            try { $p = Get-Process -Id $wpid -ErrorAction Stop } catch { return $true }
            if ($p.ProcessName -ne $ProcessName) { return $true }
        }
        if ($WindowTitleContains -and $title.IndexOf($WindowTitleContains, [StringComparison]::OrdinalIgnoreCase) -lt 0) { return $true }
        if (-not [ProbeNative]::IsWindowVisible($hWnd)) { return $true }
        if (-not $AllowIconic -and [ProbeNative]::IsIconic($hWnd)) { return $true }
        $matches.Add([pscustomobject]@{ Hwnd=$hWnd; Pid=$wpid; Title=$title })
        return $true
    }, [IntPtr]::Zero) | Out-Null

    if ($matches.Count -eq 0) { throw "No visible top-level window matched PID/process/title filters." }
    if ($matches.Count -gt 1) {
        $desc = ($matches | ForEach-Object { "PID=$($_.Pid) HWND=$($_.Hwnd) Title='$($_.Title)'" }) -join "`n"
        throw "Multiple windows matched. Add -TargetPid or -WindowTitleContains.`n$desc"
    }
    return $matches[0]
}

function PointObj([ProbeNative+POINT]$p) { [pscustomobject]@{ x=$p.X; y=$p.Y } }
function RectObj([ProbeNative+RECT]$r) { [pscustomobject]@{ left=$r.Left; top=$r.Top; right=$r.Right; bottom=$r.Bottom; width=($r.Right-$r.Left); height=($r.Bottom-$r.Top) } }

try {
    $target = Resolve-TargetWindow
} catch {
    $failure = [ordered]@{
        timestamp_utc = [DateTime]::UtcNow.ToString("o")
        test_id = $TestId
        state_tag = $StateTag
        target = [ordered]@{ process_name=$ProcessName; requested_pid=$TargetPid; title_contains=$WindowTitleContains; client_x=$ClientX; client_y=$ClientY; button=$Button; hold_ms=$HoldMs; transport_variant=$TransportVariant; allow_iconic=[bool]$AllowIconic }
        resolved = [ordered]@{ status="no_match_or_ambiguous"; error=$_.Exception.Message }
        before = [ordered]@{}
        after = [ordered]@{}
        messages = @()
        observed_result = $ObservedResult
        notes = $Notes
    }
    $dir = Split-Path -Parent $Output
    if ($dir -and -not (Test-Path $dir)) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
    ($failure | ConvertTo-Json -Depth 8 -Compress) | Add-Content -Encoding UTF8 -Path $Output
    $failure | ConvertTo-Json -Depth 8
    exit 2
}
$top = [IntPtr]$target.Hwnd
if ($top -eq [IntPtr]::Zero) { throw "Resolved HWND was zero." }

$parentClient = New-Object ProbeNative+POINT
$parentClient.X = $ClientX; $parentClient.Y = $ClientY
$requestedScreen = $parentClient
[void][ProbeNative]::ClientToScreen($top, [ref]$requestedScreen)

$child = $top
$effective = $top
$effectivePoint = $parentClient
$sendMove = $true

switch ($TransportVariant) {
    "NativeDirect" {
        $child = [ProbeNative]::RealChildWindowFromPoint($top, $parentClient)
        if ($child -eq [IntPtr]::Zero) { $child = $top }
        if ($child -ne $top) {
            $effective = $child
            $effectivePoint = $requestedScreen
            [void][ProbeNative]::ScreenToClient($child, [ref]$effectivePoint)
        }
    }
    "NativeScreen" {
        $effective = [ProbeNative]::WindowFromPoint($requestedScreen)
        if ($effective -eq [IntPtr]::Zero) { throw "WindowFromPoint returned zero for NativeScreen." }
        $effectivePoint = $requestedScreen
        [void][ProbeNative]::ScreenToClient($effective, [ref]$effectivePoint)
        $child = [ProbeNative]::RealChildWindowFromPoint($effective, $effectivePoint)
        if ($child -ne [IntPtr]::Zero -and $child -ne $effective) {
            $effective = $child
            $effectivePoint = $requestedScreen
            [void][ProbeNative]::ScreenToClient($child, [ref]$effectivePoint)
        } else { $child = $effective }
    }
    "AdvancedDirect" {
        $child = $top; $effective = $top; $effectivePoint = $parentClient
    }
    "AdvancedScreen" {
        $effective = [ProbeNative]::WindowFromPoint($requestedScreen)
        if ($effective -eq [IntPtr]::Zero) { throw "WindowFromPoint returned zero for AdvancedScreen." }
        $child = $effective
        $effectivePoint = $requestedScreen
        [void][ProbeNative]::ScreenToClient($effective, [ref]$effectivePoint)
        $sendMove = $false
    }
}

$buttonMap = @{
    Left   = @{ Down=0x0201; Up=0x0202; WParam=0x0001 }
    Right  = @{ Down=0x0204; Up=0x0205; WParam=0x0002 }
    Middle = @{ Down=0x0207; Up=0x0208; WParam=0x0010 }
}
$m = $buttonMap[$Button]
$lParam = [ProbeNative]::PackLParam($effectivePoint.X, $effectivePoint.Y)

$cursorBefore = New-Object ProbeNative+POINT; [void][ProbeNative]::GetCursorPos([ref]$cursorBefore)
$wr = New-Object ProbeNative+RECT; [void][ProbeNative]::GetWindowRect($top, [ref]$wr)
$cr = New-Object ProbeNative+RECT; [void][ProbeNative]::GetClientRect($top, [ref]$cr)
$fgBefore = [ProbeNative]::GetForegroundWindow()
$dpi = $null
try { $dpi = [ProbeNative]::GetDpiForWindow($top) } catch { $dpi = $null }

$events = @()
if ($sendMove) {
    $r1 = [ProbeNative]::PostMessage($effective, 0x0200, [IntPtr]::Zero, $lParam)
    $events += [pscustomobject]@{ name="WM_MOUSEMOVE"; value=0x0200; wparam=0; lparam=$lParam.ToInt64(); posted=$r1; at_utc=[DateTime]::UtcNow.ToString("o") }
}
$r2 = [ProbeNative]::PostMessage($effective, [uint32]$m.Down, [IntPtr]$m.WParam, $lParam)
$events += [pscustomobject]@{ name="DOWN"; value=$m.Down; wparam=$m.WParam; lparam=$lParam.ToInt64(); posted=$r2; at_utc=[DateTime]::UtcNow.ToString("o") }
if ($HoldMs -gt 0) { Start-Sleep -Milliseconds $HoldMs }
$r3 = [ProbeNative]::PostMessage($effective, [uint32]$m.Up, [IntPtr]::Zero, $lParam)
$events += [pscustomobject]@{ name="UP"; value=$m.Up; wparam=0; lparam=$lParam.ToInt64(); posted=$r3; at_utc=[DateTime]::UtcNow.ToString("o") }

$cursorAfter = New-Object ProbeNative+POINT; [void][ProbeNative]::GetCursorPos([ref]$cursorAfter)
$fgAfter = [ProbeNative]::GetForegroundWindow()

$record = [ordered]@{
    timestamp_utc = [DateTime]::UtcNow.ToString("o")
    test_id = $TestId
    state_tag = $StateTag
    target = [ordered]@{ process_name=$ProcessName; requested_pid=$TargetPid; title_contains=$WindowTitleContains; client_x=$ClientX; client_y=$ClientY; button=$Button; hold_ms=$HoldMs; transport_variant=$TransportVariant; allow_iconic=[bool]$AllowIconic }
    resolved = [ordered]@{ pid=$target.Pid; title=$target.Title; top_hwnd=$top.ToInt64(); child_hwnd=$child.ToInt64(); effective_hwnd=$effective.ToInt64(); parent_client=(PointObj $parentClient); effective_client=(PointObj $effectivePoint); requested_screen=(PointObj $requestedScreen); visible=[ProbeNative]::IsWindowVisible($top); iconic=[ProbeNative]::IsIconic($top); window_rect=(RectObj $wr); client_rect=(RectObj $cr); dpi=$dpi }
    before = [ordered]@{ cursor=(PointObj $cursorBefore); foreground_hwnd=$fgBefore.ToInt64() }
    after = [ordered]@{ cursor=(PointObj $cursorAfter); foreground_hwnd=$fgAfter.ToInt64(); cursor_unchanged=($cursorBefore.X -eq $cursorAfter.X -and $cursorBefore.Y -eq $cursorAfter.Y); foreground_unchanged=($fgBefore -eq $fgAfter) }
    messages = $events
    observed_result = $ObservedResult
    notes = $Notes
}

$dir = Split-Path -Parent $Output
if ($dir -and -not (Test-Path $dir)) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
($record | ConvertTo-Json -Depth 8 -Compress) | Add-Content -Encoding UTF8 -Path $Output
$record | ConvertTo-Json -Depth 8
