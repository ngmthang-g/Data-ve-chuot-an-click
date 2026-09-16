# PE / CLR Architecture

## PE layer
The sample is a PE32 GUI executable for x86 and contains a CLR header, so the native PE shell mainly transfers control into the .NET runtime via `_CorExeMain`.

Observed sections: `.text`, `.rsrc`, `.reloc`. The PE flags include ASLR (`DYNAMIC_BASE`) and DEP/NX compatibility. The image carries no native symbol table.

## CLR metadata layer
CLR metadata is rich enough to preserve names for application types, methods, fields, references and P/Invoke declarations. This is why the sample can be mapped structurally rather than treated as an opaque native binary.

Inventory for the frozen sample:
- 117 TypeDefs
- 1333 MethodDefs
- 775 Fields
- 257 TypeRefs
- 906 MemberRefs
- 6 AssemblyRefs

A static IL call extraction produced 12,257 call edges.

## Framework shape
The executable references classic .NET Framework 4-era assemblies (`mscorlib`, WinForms, Drawing, Core, Xml, System) and declares CLR metadata runtime `v4.0.30319`.

## Managed resources
Three JSON examples are embedded as managed resources. Their presence is especially useful because they document real serialization schema and feature combinations without inferring them solely from UI labels.

## Manifest implications
The app requests `asInvoker` and `uiAccess=false`. Therefore its normal architecture does not depend on privileged UIAccess injection. DPI awareness is configured, which matters because screen/client coordinate conversions can otherwise drift under scaling.

## Reverse-engineering consequence
For future builds, first compare SHA-256 and metadata inventory. If method names/tokens or resource layout change, regenerate indexes instead of assuming RVA/token stability.