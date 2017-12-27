﻿using System;
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
        static object _sync = new object();

        public override void OnEntry(MethodExecutionArgs args)
        {
            lock (_sync)
            {
                var arguments = args.Arguments;
                var method = args.Method.Name;

                AddLog(arguments, method);
            }
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            lock (_sync)
            {
                var result = args.ReturnValue;

                AddLogResult(result);
            }
        }

        private void AddLog(Arguments arguments, string methodName)
        {
            string path = @"d:\TESTPRO\AspectOrientedProgramming\MessageQueueServices\TestFolder\ClientLog.txt";
            var currentUtc = DateTime.UtcNow;
            var argumentValues = arguments.Select(argument => SerializeObject(argument)).ToList();

            if (!File.Exists(path))
            {
                File.Create(path);
                using (StreamWriter tw = new StreamWriter(path))
                {
                    AddLogText(tw, currentUtc, argumentValues, methodName);
                }
            }
            else if (File.Exists(path))
            {
                using (StreamWriter tw = File.AppendText(path))
                {
                    AddLogText(tw, currentUtc, argumentValues, methodName);
                }
            }
        }

        private void AddLogResult(object result)
        {
            string path = @"d:\TESTPRO\AspectOrientedProgramming\MessageQueueServices\TestFolder\ClientLog.txt";
            var resultValue = SerializeObject(result);

            if (!File.Exists(path))
            {
                File.Create(path);
                using (StreamWriter tw = new StreamWriter(path))
                {
                    AddLogTextResult(tw, resultValue);
                    tw.Close();
                }
            }
            else if (File.Exists(path))
            {
                using (StreamWriter tw = File.AppendText(path))
                {
                    AddLogTextResult(tw, resultValue);
                    tw.Close();
                }
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
