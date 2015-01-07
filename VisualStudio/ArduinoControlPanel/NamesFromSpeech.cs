using SpeechLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoControlPanel
{
    public class NamesFromSpeech
    {
        private Form1 m_parent;
        private List<string> m_names;
        private SpSharedRecoContext m_context;
        private ISpeechRecoGrammar m_grammar;
        private int m_lastRuleId = 0;

        public NamesFromSpeech(Form1 parent, List<string> names)
        {
            // SAPI 5.4 Overview: http://msdn.microsoft.com/en-us/library/ee125077%28v=vs.85%29.aspx
            // Some code from https://github.com/kring/Voodoo-Voice/blob/master/VoiceRecognition/CommandRecognizer.cs

            m_parent = parent;
            m_names = new List<string>();

            m_context = new SpSharedRecoContext();
            m_context.EventInterests = SpeechRecoEvents.SRERecognition;
            m_context.Recognition += context_Recognition;

            m_grammar = m_context.CreateGrammar();
            m_grammar.Reset();

            foreach (string name in names)
            {
                AddRuleForName(name);
            }

            CommitAndActivate();
        }

        public void AddName(string name)
        {
            if (AddRuleForName(name))
            {
                CommitAndActivate();
            }
        }

        private bool AddRuleForName(string name)
        {
            if (!m_names.Contains(name))
            {
                var rule = m_grammar.Rules.Add(name, SpeechRuleAttributes.SRATopLevel, ++m_lastRuleId);
                object propertyValue = name;
                try
                {
                    rule.InitialState.AddWordTransition(null, name, " ", SpeechGrammarWordType.SGLexical, name, 0, ref propertyValue, 1);
                    m_names.Add(name);
                    return true;
                }
                catch
                {
                    int x = 0;
                    // TODO: Ignore duplicate phrase error, re-throw all others
                }
            }
            return false;
        }

        private void CommitAndActivate()
        {
            m_grammar.Rules.Commit();
            m_grammar.State = SpeechGrammarState.SGSExclusive;
            m_context.State = SpeechRecoContextState.SRCS_Enabled;
            foreach (string name in m_names)
            {
                m_grammar.CmdSetRuleState(name, SpeechRuleState.SGDSActive);
            }
        }

        private void context_Recognition(int StreamNumber, object StreamPosition, SpeechRecognitionType RecognitionType, ISpeechRecoResult Result)
        {
            m_parent.onSpeech(Result.PhraseInfo.Rule.Name);
        }
    }
}
