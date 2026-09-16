param(
    [string]$ProcessName,
    [int]$TargetPid = 0,
    [string]$WindowTitleContains = "",
    [int]$ClientX = 50,
    [int]$ClientY = 50,
    [string]$Output = ".\runtime\results\matrix.jsonl"
)

$probe = Join-Path $PSScriptRoot "BackgroundClickProbe.ps1"
$cases = @(
    @{ Id="BG-FOREGROUND-LEFT"; State="foreground"; Button="Left"; Variant="NativeDirect"; Hold=25; AllowIconic=$false; Prompt="Bring target window to foreground." },
    @{ Id="BG-BACKGROUND-LEFT"; State="background"; Button="Left"; Variant="NativeDirect"; Hold=25; AllowIconic=$false; Prompt="Put another application in foreground without minimizing target." },
    @{ Id="BG-COVERED-LEFT"; State="covered"; Button="Left"; Variant="NativeDirect"; Hold=25; AllowIconic=$false; Prompt="Cover the target point with another window." },
    @{ Id="RESOLVER-MINIMIZED"; State="minimized"; Button="Left"; Variant="NativeDirect"; Hold=25; AllowIconic=$false; Prompt="Minimize the target. Exact FindWindowByTarget-compatible behavior is no match / no dispatch." },
    @{ Id="TRANSPORT-MINIMIZED-DIRECT"; State="minimized_transport_only"; Button="Left"; Variant="NativeDirect"; Hold=25; AllowIconic=$true; Prompt="Keep target minimized. This isolates transport only and intentionally bypasses the sample resolver's IsIconic rejection." },
    @{ Id="BG-RESIZED"; State="resized"; Button="Left"; Variant="NativeDirect"; Hold=25; AllowIconic=$false; Prompt="Resize the target after choosing the client-relative test point; keep the same client coordinates." },
    @{ Id="BG-MOVED"; State="moved"; Button="Left"; Variant="NativeDirect"; Hold=25; AllowIconic=$false; Prompt="Move the target window to a different screen position without changing the client point." },
    @{ Id="BG-HWND-RECREATE"; State="hwnd_recreated"; Button="Left"; Variant="NativeDirect"; Hold=25; AllowIconic=$false; Prompt="Close and reopen the target, then ensure PID/title filters identify the live replacement HWND." },
    @{ Id="BG-CHILD-LEFT"; State="child_control"; Button="Left"; Variant="NativeDirect"; Hold=25; AllowIconic=$false; Prompt="Choose a point over a real child HWND/control so child retargeting can be observed." },
    @{ Id="VARIANT-NATIVE-SCREEN"; State="visible"; Button="Left"; Variant="NativeScreen"; Hold=25; AllowIconic=$false; Prompt="Keep target visible; test NativeMethods.SendBackgroundClick screen hit-test path." },
    @{ Id="VARIANT-ADV-DIRECT"; State="visible"; Button="Left"; Variant="AdvancedDirect"; Hold=25; AllowIconic=$false; Prompt="Keep target visible; test Advanced.ActionExecutor direct HWND path (no child retarget)." },
    @{ Id="VARIANT-ADV-SCREEN"; State="visible"; Button="Left"; Variant="AdvancedScreen"; Hold=25; AllowIconic=$false; Prompt="Keep target visible; test Advanced.ActionExecutor screen path (DOWN/UP only)." },
    @{ Id="BG-RIGHT"; State="visible"; Button="Right"; Variant="NativeDirect"; Hold=25; AllowIconic=$false; Prompt="Restore target to a normal visible state." },
    @{ Id="BG-MIDDLE"; State="visible"; Button="Middle"; Variant="NativeDirect"; Hold=25; AllowIconic=$false; Prompt="Keep target in a normal visible state." },
    @{ Id="BG-HOLD-250"; State="visible_hold_250"; Button="Left"; Variant="NativeDirect"; Hold=250; AllowIconic=$false; Prompt="Keep target visible; verify the logged DOWN-to-UP interval is approximately at least 250 ms." },
    @{ Id="DPI-100"; State="dpi_100"; Button="Left"; Variant="NativeDirect"; Hold=25; AllowIconic=$false; Prompt="Run with target on a display configured to 100% scale." },
    @{ Id="DPI-125"; State="dpi_125"; Button="Left"; Variant="NativeDirect"; Hold=25; AllowIconic=$false; Prompt="Run with target on a display configured to 125% scale." },
    @{ Id="DPI-150"; State="dpi_150"; Button="Left"; Variant="NativeDirect"; Hold=25; AllowIconic=$false; Prompt="Run with target on a display configured to 150% scale." },
    @{ Id="MULTIMON-SECONDARY"; State="secondary_monitor"; Button="Left"; Variant="NativeDirect"; Hold=25; AllowIconic=$false; Prompt="Move target to a non-primary monitor and run at the same client point." }
)

foreach ($c in $cases) {
    Write-Host ""
    Write-Host "[$($c.Id)] $($c.Prompt)"
    $ok = Read-Host "Press Enter to run, or type skip"
    if ($ok -eq "skip") { continue }
    $args = @{ ProcessName=$ProcessName; TargetPid=$TargetPid; WindowTitleContains=$WindowTitleContains; ClientX=$ClientX; ClientY=$ClientY; Button=$c.Button; TransportVariant=$c.Variant; HoldMs=$c.Hold; TestId=$c.Id; StateTag=$c.State; ObservedResult="unknown"; Output=$Output }
    if ($c.AllowIconic) { $args.AllowIconic = $true }
    & $probe @args
    $observed = Read-Host "Observed result: success / no_effect / partial / unknown"
    if ($observed -notin @("success","no_effect","partial","unknown")) { $observed = "unknown" }
    $note = Read-Host "Optional note (press Enter to leave blank)"
    Write-Host "To commit a human-observed result, re-run this case directly with -ObservedResult $observed -Notes '$note'."
}
