using AwesomeSequencer.Core;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            var programText = ScriptStep.GetInitialScript();
            editor.Text = programText;
        }

        private void DiagnosticButton_Click(object sender, RoutedEventArgs e)
        {
            var CompleteSyntax = ScriptStep.GenerateUsingSyntax() + editor.Text;
            AssemblyGenerator.CompileCode(CompleteSyntax, out string diagnosticMessage);
            DiagnosticTextBox.Text = diagnosticMessage;
        }

        private async void ExcuteButton_Click(object sender, RoutedEventArgs e)
        {
            ResultListBox.Items.Add("Hello-MyAwaitTask");
            var samples = SampleScripStep();
            var AllStep = new AllStepIterator(samples);
            AllStep.AllStepLoggerEvent += new EventHandler(AppendLog);
            foreach (var sample in samples)
            {
                sample.ScriptStepLoggerEvent += new EventHandler(AppendLog);
            }
            await AllStep.IteratorExecute();
            ResultListBox.Items.Add("Finished");
        }

        private void AppendLog(object sender, EventArgs e)
        {
            var arg = e as LogEventArgs;
            Application.Current.Dispatcher.Invoke(() =>
            {
                ResultListBox.Items.Add(arg.Log);
                ResultListBox.SelectedIndex = ResultListBox.Items.Count - 1;
                ResultListBox.Focus();
                var border = (Border)VisualTreeHelper.GetChild(ResultListBox, 0);
                var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            });
        }

        private List<ScriptStep> SampleScripStep()
        {
            var CompleteSyntax = ScriptStep.GenerateUsingSyntax() + editor.Text;
            List<ScriptStep> scriptSteps = new List<ScriptStep>();
            scriptSteps.Add(new ScriptStep(CompleteSyntax)
            {
                LowerLimit = 110,
                UpperLimit = 200,
                Value = 150,
                Number = 1,
                Description = "1 voltage",
                stepTypes = StepTypes.ScriptStep
            });
            scriptSteps.Add(new ScriptStep(CompleteSyntax)
            {
                LowerLimit = 120,
                UpperLimit = 200,
                Value = 210,
                Number = 2,
                Description = "2 voltage",
                stepTypes = StepTypes.JumpStep,
                NumberofExecution = 3,
                JumpToNumber = 1
            });
            scriptSteps.Add(new ScriptStep(CompleteSyntax)
            {
                LowerLimit = 130,
                UpperLimit = 200,
                Value = 150,
                Number = 3,
                Description = "3 voltage",
                stepTypes = StepTypes.ScriptStep
            });
            scriptSteps.Add(new ScriptStep(CompleteSyntax)
            {
                LowerLimit = 140,
                UpperLimit = 200,
                Value = 100,
                Number = 4,
                Description = "4 voltage",
                stepTypes = StepTypes.JumpStep,//jumpstep
                NumberofExecution = 3,
                JumpToNumber = 3
            });

            scriptSteps.Add(new ScriptStep(CompleteSyntax)
            {
                LowerLimit = 100,
                UpperLimit = 200,
                Value = 50,
                Number = 5,
                Description = "5 voltage",
                stepTypes = StepTypes.ScriptStep
            });
            return scriptSteps;
        }

        public static class Logger
        {
            //public static object ResultListBox { get; private set; }

            public static void AddLog(string log)
            {
                //ResultListBox.Items.Add(log);
            }
        }
    }
}