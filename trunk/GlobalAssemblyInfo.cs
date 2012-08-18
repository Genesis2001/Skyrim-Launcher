using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

[assembly: AssemblyProduct("Skyrim Launcher")]
[assembly: AssemblyCompany("UnifiedTech.org")]
[assembly: AssemblyCopyright("\x00a9 UnifiedTech.org. All rights reserved.")]

[assembly: AssemblyVersion("1.0.0")]

#if DEBUG
[assembly : AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
