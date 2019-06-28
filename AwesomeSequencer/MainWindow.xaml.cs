using AwesomeSequencer.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AwesomeSequencer
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Result();
            //            const string programText =
            //@"using System;
            //using System.Collections;
            //using System.Linq;
            //using System.Text;
            //using System.Windows.Forms;

            //namespace HelloWorld
            //{
            //    class Program
            //    {
            //        static void Main(string[] args)
            //        {
            //            Console.WriteLine(""Hello, World!"");
            //            MessageBox.Show(""Hello"");
            //        }
            //        static public string myword = ""Myword"";
            //        static void Test()
            //        {
            //            MessageBox.Show(myword);
            //        }
            //    }
            //}";

            //            const string programText =
            //@"using System;
            //using System.Collections;
            //using System.Linq;
            //using System.Text;
            //using System.Windows.Forms;

            //namespace HelloWorld
            //{
            //    public class Program
            //    {
            //        public void TestSetup()
            //        {
            //            Console.WriteLine(""Hello, World!"");
            //            MessageBox.Show(""Hello"");
            //        }
            //        public string myword = ""Myword"";
            //        public void TestRun()
            //        {
            //            MessageBox.Show(myword);
            //        }

            //        public void TestRun(string word)
            //        {
            //            MessageBox.Show(word);
            //        }

            //        public void TestRun(string word, string word2)
            //        {
            //            MessageBox.Show(word+word2);
            //        }
            //    }
            //}";

            string programText =
@"using AwesomeSequencer.Core;

    [StepClass]
	public class MyClass
	{
	    [StepSetUp]
	    public void SetupLogic()
	    {
	        //program setup logic
	    }

	    [StepMain]
	    public void MainLogic()
	    {
	        //program main logic
	    }

	    [StepTearDown]
	    public void TearDownLogic()
	    {
	        //program teardown logic
	    }
	}";
            editor.Text = programText;
        }

        private SyntaxTree tree;

        private void DiagnosticButton_Click(object sender, RoutedEventArgs e)
        {
            tree = CSharpSyntaxTree.ParseText(editor.Text);
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
                references: new[] { mscorlib, systemCorelib, systemWindowsFormslib, AwesomeSequencerCorelib },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                );

            var disanostics = compilation.GetDiagnostics();

            string DiagnosticMessage = string.Empty;
            foreach (var diagnostic in disanostics)
            {
                if (diagnostic.Severity == DiagnosticSeverity.Error)
                {
                    DiagnosticMessage += diagnostic.ToString() + "\n";
                }
            }

            if (disanostics.Length == 0)
            {
                DiagnosticMessage = "Complie Successful.";
            }
            //compilation.Emit
            DiagnosticTextBox.Text = DiagnosticMessage;
            //using (var stream = new MemoryStream())
            //{
            //    var compileResult = compilation.Emit(stream);
            //    var assembly = Assembly.Load(stream.GetBuffer());
            //    Type method = assembly.GetType("HelloWorld.Program");

            //    MethodInfo TestRunParametermethod = method.GetMethod("TestRun", new Type[] { typeof(string) });
            //    MethodInfo TestRunParametermethod2 = method.GetMethod("TestRun", new Type[] { typeof(string), typeof(string) });
            //    MethodInfo TestRunmethod = method.GetMethod("TestRun", new Type[] { });
            //    MethodInfo TestSetupmethod = method.GetMethod("TestSetup");
            //    var o = Activator.CreateInstance(method);
            //    TestRunmethod.Invoke(o, null);
            //    TestSetupmethod.Invoke(o, null);
            //    TestRunParametermethod.Invoke(o, new object[] { "Hello Parameter" });
            //    TestRunParametermethod2.Invoke(o, new object[] { "2Hello Parameter2", "2Hello Parameter2" });
            //}
        }

        public void Test()
        {
            //var compilation = CSharpCompilation.Create("MyCompilation",
            //    syntaxTrees: new[] { tree }, references: new[] { mscorlib });

            //Emit to stream
            //var ms = new MemoryStream();
            //var emitResult = compilation.Emit(ms);
        }
    }
}