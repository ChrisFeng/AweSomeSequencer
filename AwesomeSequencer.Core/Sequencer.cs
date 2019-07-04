using System.Collections.Generic;

namespace AwesomeSequencer.Core
{
    public class Sequencer
    {
        private List<ScriptStep> scriptSteps = new List<ScriptStep>();

        public Sequencer(List<ScriptStep> scriptSteps)
        {
            this.scriptSteps = scriptSteps;
        }

        public void Executer()
        {
        }
    }
}