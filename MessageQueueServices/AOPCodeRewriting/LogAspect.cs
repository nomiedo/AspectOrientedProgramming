using System;
using System.Collections.Generic;
using System.Linq;
using PostSharp.Aspects;
using System.IO;
using System.Xml.Serialization;

namespace AOPCodeRewriting
{
    [Serializable]
    public class LogAspect : OnMethodBoundaryAspect
    {
        static object _sync2 = new object();
        private const string path = @"d:\TESTPRO\AspectOrientedProgramming\MessageQueueServices\TestFolder\ClientLog.txt";

        public override void OnEntry(MethodExecutionArgs args)
        {
            if (!File.Exists(@"d:\TESTPRO\AspectOrientedProgramming\MessageQueueServices\TestFolder\ClientLog.txt"))
            {
                File.Create(@"d:\TESTPRO\AspectOrientedProgramming\MessageQueueServices\TestFolder\ClientLog.txt");
            }

            lock (_sync2)
            {
                var arguments = args.Arguments;
                var method = args.Method.Name;

                AddLog(arguments, method);
            }
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            lock (_sync2)
            {
                var result = args.ReturnValue;

                AddLogResult(result);
            }
        }

        private void AddLog(Arguments arguments, string methodName)
        {              
            var currentUtc = DateTime.UtcNow;
            var argumentValues = arguments.Select(argument => SerializeObject(argument)).ToList();

            if (!File.Exists(path)) return;

            using (StreamWriter tw = File.AppendText(path))
            {
                AddLogText(tw, currentUtc, argumentValues, methodName);
            }
        }

        private void AddLogResult(object result)
        {
            var resultValue = SerializeObject(result);

            if (!File.Exists(path)) return;

            using (StreamWriter tw = File.AppendText(path))
            {
                AddLogTextResult(tw, resultValue);
            }
        }

        private void AddLogText(StreamWriter tw, DateTime currentUtc, List<string> argumentValues, string methodName)
        {

                tw.WriteLine($"Date Time: {currentUtc}");
                tw.WriteLine($"Method: {methodName}");
                foreach (var argumentValue in argumentValues)
                {
                    tw.WriteLine($"Argument: {argumentValue}");
                }
                tw.Close();
        }

        private void AddLogTextResult(StreamWriter tw, string resultValue)
        {

                tw.WriteLine($"Result: {resultValue}");
                tw.Close();

        }


        public string SerializeObject<T>(T toSerialize, bool isResult = false)
        {
            try
            {
                if (toSerialize == null)
                    return isResult ? "No Arguments" : "No Result";

                XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, toSerialize);
                    return textWriter.ToString();
                }
            }
            catch (Exception e)
            {
                return "Not serializable";
            }
        }
    }
}
