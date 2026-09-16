#!/usr/bin/env python3
import re
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]

class DeepResearchTests(unittest.TestCase):
    def text(self, rel):
        return (ROOT / rel).read_text(encoding='utf-8', errors='replace')

    def test_runtime_probe_declares_window_from_point(self):
        s = self.text('runtime/windows/BackgroundClickProbe.ps1')
        self.assertRegex(s, r'extern\s+IntPtr\s+WindowFromPoint\s*\(')

    def test_action_executor_execute_matches_four_argument_il_signature(self):
        s = self.text('reconstructed/ActionExecutor.Reconstructed.cs')
        m = re.search(r'internal static void Execute\s*\((.*?)\)\s*\{', s, re.S)
        self.assertIsNotNone(m)
        params = [p.strip() for p in m.group(1).split(',') if p.strip()]
        self.assertEqual(4, len(params), params)
        self.assertIn('MacroStepSnapshot step', params[0])
        self.assertIn('bool freeMouseMode', params[1])
        self.assertIn('int randomInterval', params[2])
        self.assertIn('int randomJitter', params[3])

    def test_macrorunner_condition_branches_are_reconstructed_not_placeholders(self):
        s = self.text('reconstructed/MacroRunner.Reconstructed.cs')
        self.assertNotIn('ReconstructionBoundary(', s)
        self.assertNotIn('throw new NotSupportedException', s)
        for name in ('WaitForColor', 'WaitForChange', 'EvaluateColorBranch'):
            self.assertRegex(s, rf'private .*\b{name}\s*\(')
        self.assertIn('FindMatchingPixelInArea', s)
        self.assertIn('IfTrueStep', s)
        self.assertIn('IfFalseStep', s)

    def test_runtime_matrix_runner_covers_documented_core_variants(self):
        s = self.text('runtime/windows/Run-Matrix.ps1')
        for case in ('BG-RESIZED','BG-MOVED','BG-HWND-RECREATE','BG-CHILD-LEFT','BG-HOLD-250','DPI-100','DPI-125','DPI-150','MULTIMON-SECONDARY'):
            self.assertIn(case, s)

if __name__ == '__main__':
    unittest.main(verbosity=2)
