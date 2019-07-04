using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AwesomeSequencer.Core
{
    public class AssemblyGenerator
    {
        public static Assembly CompileCode(string code, out string diagnosticMessage)
        {
            var tree = CSharpSyntaxTree.ParseText(code);
            var path = typeof(object).Assembly.Location;
            var test = System.IO.Path.Combine(path, @"..\");
            var netPath = System.IO.Path.GetFullPath(test);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var systemCorelib = MetadataReference.CreateFromFile(netPath + "System.Core.dll");
            var systemWindowsFormslib = MetadataReference.CreateFromFile(netPath + "System.Windows.Forms.dll");

            var customTest = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\");
            string customPath = System.IO.Path.GetFullPath(customTest);
            var AwesomeSequencerCorelib = MetadataReference.CreateFromFile(customPath + @"AwesomeSequencer.Core\bin\Debug\AwesomeSequencer.Core.dll");
            DirectoryInfo dir = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory);
            var s = dir.Parent;
            CSharpCompilation compilation = CSharpCompilation.Create("Modle",
                syntaxTrees: new[] { tree },
                references: BuildReferences(),
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                );

            diagnosticMessage = string.Empty;
            Assembly assembly = null;
            using (var ms = new MemoryStream())
            {
                var compilationResult = compilation.Emit(ms);
                if (compilationResult.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    diagnosticMessage = "Complie Successful.";
                    assembly = Assembly.Load(ms.ToArray());
                }
                else
                {
                    foreach (var diagnostic in compilationResult.Diagnostics)
                    {
                        if (diagnostic.Severity == DiagnosticSeverity.Error)
                        {
                            diagnosticMessage += diagnostic.ToString() + "\n";
                        }
                    }
                }
            }
            return assembly;
        }

        private static List<MetadataReference> BuildReferences()
        {
            var test = System.IO.Path.Combine(typeof(object).Assembly.Location, @"..\");
            var netPath = System.IO.Path.GetFullPath(test);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var systemCorelib = MetadataReference.CreateFromFile(netPath + "System.Core.dll");
            var systemWindowsFormslib = MetadataReference.CreateFromFile(netPath + "System.Windows.Forms.dll");
            var customTest = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\");
            string customPath = System.IO.Path.GetFullPath(customTest);
            var AwesomeSequencerCorelib = MetadataReference.CreateFromFile(customPath + @"AwesomeSequencer.Core\bin\Debug\AwesomeSequencer.Core.dll");
            List<MetadataReference> references = new List<MetadataReference>();
            references.Add(mscorlib);
            references.Add(systemCorelib);
            references.Add(systemWindowsFormslib);
            references.Add(AwesomeSequencerCorelib);
            return references;
        }
    }
}