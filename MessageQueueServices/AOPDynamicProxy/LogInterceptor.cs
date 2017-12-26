using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;


namespace AOPDynamicProxy
{
    public class LogInterceptor : IInterceptor
    {
        static object _sync = new object();

        public void Intercept(IInvocation invocation)
        {
            lock (_sync)
            {
                var arguments = invocation.Arguments;
                var method = invocation.Method.Name;
                var result = invocation.ReturnValue;

                AddLog(arguments, method, result);
            }
        }

        private void AddLog(object[] arguments, string methodName, object result)
        {
            string path = @"d:\TESTPRO\AspectOrientedProgramming\MessageQueueServices\TestFolder\ServerLog.txt";
            var currentUtc = DateTime.UtcNow;
            var argumentValues = arguments.Select(argument => SerializeObject(argument)).ToList();
            var resultValue = SerializeObject(result);

            if (!File.Exists(path))
            {
                File.Create(path);
                using (StreamWriter tw = new StreamWriter(path))
                {
                    AddLogText(tw, currentUtc, argumentValues, methodName, resultValue);
                }
            }
            else if (File.Exists(path))
            {
                using (StreamWriter tw = File.AppendText(path))
                {
                    AddLogText(tw, currentUtc, argumentValues, methodName, resultValue);
                }
            }
        }

        private void AddLogText(StreamWriter tw, DateTime currentUtc, List<string> argumentValues, string methodName,
            string resultValue)
        {
            tw.WriteLine($"Date Time: {currentUtc}");
            tw.WriteLine($"Method: {methodName}");
            foreach (var argumentValue in argumentValues)
            {
                tw.WriteLine($"Argument: {argumentValue}");
            }
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