using System;
using System.Collections.Generic;
using System.IO;
using Jurassic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Engine
{
    public class GlobalScripts
    {
        private static readonly System.Text.Encoding ISO_8859_1 = System.Text.Encoding.GetEncoding("iso-8859-1");
        private static Dictionary<string, bool> _required = new Dictionary<string, bool>();

        public static void BindToEngine(ScriptEngine engine)
        {
            engine.SetGlobalFunction("RequireScript", new Action<string>(RequireScript));
            engine.SetGlobalFunction("RequireSystemScript", new Action<string>(RequireSystemScript));
            engine.SetGlobalFunction("EvaluateScript", new Action<string>(EvaluateScript));
            engine.SetGlobalFunction("EvaluateSystemScript", new Action<string>(EvaluateSystemScript));

            engine.Execute("Object.defineProperty(Object.prototype, \"__defineGetter__\", { value: function(name, func) {" +
                "Object.defineProperty(this, name, { get: func, configurable: true }); } });");

            engine.Execute("Object.defineProperty(Object.prototype, \"__defineSetter__\", { value: function(name, func) {" +
                "Object.defineProperty(this, name, { set: func, configurable: true }); } });");
        }

        public static void RequireScript(string filename)
        {
            if (_required.ContainsKey(filename) && _required[filename])
                return;
            EvaluateScript(filename);
            _required[filename] = true;
        }

        static void EvaluateScript(string filename)
        {
            filename = Program.ParseSpherePath(filename, "scripts");
            string text = File.ReadAllText(filename, ISO_8859_1);
            StringScriptSource source = new StringScriptSource(text, filename);
            RunCode(source);
        }

        static void RequireSystemScript(string filename)
        {
            if (_required.ContainsKey(filename) && _required[filename])
                return;
            EvaluateSystemScript(filename);
            _required[filename] = true;
        }

        static void EvaluateSystemScript(string filename)
        {
            filename = GlobalProps.EnginePath + "/system/scripts/" + filename;
            string text = File.ReadAllText(filename, ISO_8859_1);
            StringScriptSource source = new StringScriptSource(text, filename);
            RunCode(source);
        }

        public static void RunCode(StringScriptSource source)
        {
            try
            {
                Program._engine.Execute(source);
            }
            catch (JavaScriptException ex)
            {
                Console.WriteLine(string.Format("Script error in \'{0}\', line: {1}\n{2}", ex.SourcePath, ex.LineNumber, ex.Message));
            }
            catch (Exception e)
            {
                var st = new StackTrace(e);
                var frame = st.GetFrame(0);
                Console.WriteLine("Fatal Error: " + e.Message + " @ " + frame.GetFileLineNumber());
                Console.WriteLine(frame.GetFileName());
            }
        }
    }
}
