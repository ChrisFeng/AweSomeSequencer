using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AwesomeSequencer.Core
{
    public class AllStepIterator //: IBaseIterator
    {
        private List<ScriptStep> scriptSteps = new List<ScriptStep>();

        public AllStepIterator(List<ScriptStep> scriptSteps)
        {
            this.scriptSteps = scriptSteps;
        }

        public event EventHandler AllStepLoggerEvent;

        public int current;
        public bool FailOverJumpExecution;

        public IList<ScriptStep> GetAllSteps()
        {
            return scriptSteps;
        }

        public async Task IteratorExecute()
        {
            await Task.Run(() =>
            {
                while (HasNext)//判斷是否還有下一個步驟
                {
                    ResultType result = scriptSteps[current].Execute();

                    switch (scriptSteps[current].stepTypes)
                    {
                        case StepTypes.ScriptStep:
                            {
                                SetNext();
                                Console.WriteLine($"下一個步驟:{scriptSteps[current].Number}");
                                AllStepLoggerEvent(this, new LogEventArgs() { Log = $"下一個步驟:{scriptSteps[current].Number}" });
                            }
                            break;

                        case StepTypes.JumpStep:
                            {
                                if (scriptSteps[current].NumberofExecution == scriptSteps[current].ExecutionCounter)
                                {
                                    FailOverJumpExecution = true;
                                }
                                else
                                {
                                    current = LoopUpJumpDestination(scriptSteps[current]);//檢查是否存在Jump步驟
                                }

                                Console.WriteLine($"下一個步驟:{scriptSteps[current].Number}");
                                AllStepLoggerEvent(this, new LogEventArgs() { Log = $"下一個步驟:{scriptSteps[current].Number}" });
                            }
                            break;

                        default:
                            break;
                    }
                }
            });
        }

        public bool HasNext
        {
            get
            {
                if (current >= scriptSteps.Count)
                {
                    return false;
                }
                if (scriptSteps.Count == 0)
                {
                    return false;
                }
                if (FailOverJumpExecution)
                {
                    return false;
                }
                return true;
            }
        }

        public void SetNext()
        {
            this.current = current + 1;
        }

        public void Initialize()
        {
            current = 0;
            FailOverJumpExecution = false;
        }

        private int LoopUpJumpDestination(ScriptStep scriptStep)//檢查是否存在Jump步驟
        {
            for (int i = 0; i < scriptSteps.Count; i++)
            {
                if (scriptStep.JumpToNumber == scriptSteps[i].Number)
                {
                    return i;
                }
            }
            throw new Exception("Jump Step Not found");
        }
    }

    public class LogEventArgs : EventArgs
    {
        public string Log;
    }

    [Flags]
    public enum StepTypes
    {
        None = 0,
        SequenceSetup = 1,
        SequenceTeardown = 2,
        SequenceAbort = 4,
        Sequence = 8,
        ScriptStep = 0x10,
        NullStep = 0x20,
        JumpStep = 0x40
    }

    public enum IteratorTypes
    {
        IteratorAllSteps,
        IteratorCyclic
    }
}