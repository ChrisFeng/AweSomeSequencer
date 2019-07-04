using System;
using System.Reflection;
using System.Text;
using System.Threading;

namespace AwesomeSequencer.Core
{
    public class ScriptStep
    {
        public ScriptStep(string Code)
        {
            this.Code = Code;
        }

        public ResultType Result;//結果
        public double LowerLimit;//下限
        public double UpperLimit;//上限
        public double Value;//量測值
        public int Number;//步驟號碼
        public string Description;//步驟描述
        public int JumpToNumber = -1;//跳躍到哪個步驟
        public int NumberofExecution = -1;//設定的跳躍次數
        public StepTypes stepTypes = StepTypes.ScriptStep;
        public int Repeat = 1;//此步驟的重複次數
        public int Delay = 1000;//每次Repeat的延遲時間
        public int ExecutionCounter = 0;//此步驟的跳躍次數
        public string diagnosticMessage;
        public string Code;
        public bool JumpOnFail = false;//Fail的時候是否要跳躍
        public bool IsActive = true;//是否啟動

        //public int FailCounter = 0;//此步驟的Fail跳躍次數
        public EventHandler ScriptStepLoggerEvent;

        public void SetAsJumpStep()
        {
            stepTypes = StepTypes.JumpStep;
        }

        public void SetJumpOnFail()
        {
            JumpOnFail = !JumpOnFail;
        }

        public static string GetInitialScript()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[StepClass]\r\n");
            stringBuilder.Append("public class MyClass\r\n");
            stringBuilder.Append("{\r\n");
            stringBuilder.Append("    [StepSetUp]\r\n");
            stringBuilder.Append("    public void SetupLogic()\r\n");
            stringBuilder.Append("    {\r\n");
            stringBuilder.Append("        //program setup logic\r\n");
            stringBuilder.Append("    }\r\n");
            stringBuilder.Append("\r\n");
            stringBuilder.Append("    [StepMain]\r\n");
            stringBuilder.Append("    public void MainLogic()\r\n");
            stringBuilder.Append("    {\r\n");
            stringBuilder.Append("        //program main logic\r\n");
            stringBuilder.Append("    }\r\n");
            stringBuilder.Append("\r\n");
            stringBuilder.Append("    [StepTearDown]\r\n");
            stringBuilder.Append("    public void TearDownLogic()\r\n");
            stringBuilder.Append("    {\r\n");
            stringBuilder.Append("        //program teardown logic\r\n");
            stringBuilder.Append("    }\r\n");
            stringBuilder.Append("}\r\n");
            return stringBuilder.ToString();
        }

        public static string GenerateUsingSyntax()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("using System;\r\n");
            stringBuilder.Append("using System.Threading;\r\n");
            stringBuilder.Append("using System.Windows.Forms;\r\n");
            stringBuilder.Append("using System.Text;\r\n");
            stringBuilder.Append("using System.Globalization;\r\n");
            stringBuilder.Append("using System.Collections.Generic;\r\n");
            stringBuilder.Append("using System.Reflection;\r\n");
            stringBuilder.Append("using System.Linq;\r\n");
            stringBuilder.Append("using AwesomeSequencer.Core;\r\n");
            return stringBuilder.ToString();
        }

        public ResultType Execute()
        {
            var assembly = AssemblyGenerator.CompileCode(Code, out string Message);
            if (assembly == null)
            {
                diagnosticMessage = Message;//要有錯誤, 需要再修改的地方
                return ResultType.Fail;
            }
            var standMethod = CheckStandarMethed(assembly);
            if (standMethod.successful == false)
            {
                return ResultType.Fail;//要有錯誤, 需要再修改的地方
            }

            standMethod.InvokeSetupLogic();
            ScriptStepLoggerEvent(this, new LogEventArgs() { Log = $"InvokeSetupLogic" });
            for (int i = 0; i < Repeat; i++)
            {
                Console.WriteLine($"{Description} : Repeat {i + 1}");
                ScriptStepLoggerEvent(this, new LogEventArgs() { Log = $"{Description} : Repeat {i + 1}" });
                standMethod.InvokeMainLogic();
                ScriptStepLoggerEvent(this, new LogEventArgs() { Log = $"InvokeMainLogic" });
                if (Value <= UpperLimit && Value >= LowerLimit)
                {
                    Result = ResultType.Pass;
                    Console.WriteLine($"{Description} : Pass");
                    ScriptStepLoggerEvent(this, new LogEventArgs() { Log = $"{Description} : Pass" });
                    //return Result;
                }
                else
                {
                    Result = ResultType.Fail;
                }
                ExecutionCounter++;
                ScriptStepLoggerEvent(this, new LogEventArgs() { Log = $"Delay : {Delay}" });
                Thread.Sleep(Delay);
            }
            standMethod.InvokeTearDownLogic();
            ScriptStepLoggerEvent(this, new LogEventArgs() { Log = $"InvokeTearDownLogic" });
            return Result;
        }

        //public void Event

        private StandardMethod CheckStandarMethed(Assembly assembly)
        {
            Type myClass = assembly.GetType("MyClass");
            if (myClass == null)
            {
                return new StandardMethod();
            }
            if (myClass.GetCustomAttribute(typeof(StepClassAttribute)) == null)
            {
                return new StandardMethod();
            }

            MethodInfo mySetupLogic = myClass.GetMethod("SetupLogic", new Type[] { });
            if (mySetupLogic == null)
            {
                return new StandardMethod();
            }
            if (mySetupLogic.GetCustomAttribute(typeof(StepSetUpAttribute)) == null)
            {
                return new StandardMethod();
            }

            MethodInfo myMainLogic = myClass.GetMethod("MainLogic", new Type[] { });
            if (myMainLogic == null)
            {
                return new StandardMethod();
            }
            if (myMainLogic.GetCustomAttribute(typeof(StepMainAttribute)) == null)
            {
                return new StandardMethod();
            }

            MethodInfo myTearDownLogic = myClass.GetMethod("TearDownLogic", new Type[] { });
            if (myTearDownLogic == null)
            {
                return new StandardMethod();
            }
            if (myTearDownLogic.GetCustomAttribute(typeof(StepTearDownAttribute)) == null)
            {
                return new StandardMethod();
            }
            var myInstance = Activator.CreateInstance(myClass);
            return new StandardMethod(true, myInstance, mySetupLogic, myMainLogic, myTearDownLogic);
        }
    }

    public class StandardMethod
    {
        public StandardMethod()
        {
        }

        public StandardMethod(bool successful, object myInstance, MethodInfo mySetupLogic, MethodInfo myMainLogic, MethodInfo myTearDownLogic)
        {
            this.successful = successful;
            this.myInstance = myInstance;
            this.mySetupLogic = mySetupLogic;
            this.myMainLogic = myMainLogic;
            this.myTearDownLogic = myTearDownLogic;
        }

        public bool successful;
        private object myInstance;
        private MethodInfo mySetupLogic;
        private MethodInfo myMainLogic;
        private MethodInfo myTearDownLogic;

        public void InvokeSetupLogic()
        {
            mySetupLogic.Invoke(myInstance, new Type[] { });
        }

        public void InvokeMainLogic()
        {
            myMainLogic.Invoke(myInstance, new Type[] { });
        }

        public void InvokeTearDownLogic()
        {
            myTearDownLogic.Invoke(myInstance, new Type[] { });
        }
    }

    public enum ResultType
    {
        None = 0,
        Pass = 1,
        Fail = 2,
        Abort = 3,
        FailOnContinue = 4
    }
}