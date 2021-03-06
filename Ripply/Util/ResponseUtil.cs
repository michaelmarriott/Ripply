﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Serilog;
using Serilog.Core;

namespace Ripply.Util
{
    public class ResponseUtil
    {
        private static readonly Logger Log = new LoggerConfiguration().CreateLogger();
        private double ExtractPrice(string text)
        {
            var decimalsEquation = @"[0-9.]+";
            var regex = new Regex(decimalsEquation);
            Match match = regex.Match(text);
            if (match.Success)
            {
                return Double.Parse(match.Value);
            }
            return 0.00;
        }

        public static string ReturnFirstMatch(string text, Expression[] expressions)
        {

            foreach (var expression in expressions)
            {
                Match match = Regex.Match(text, expression.Value,RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return expression.Name;
                }
            }
            return null;
        }

        public static int? ReturnFirstMatchName(string text, Expression[] expressions)
        {


            foreach (var expression in expressions)
            {

                var numericEquation = expression.Value;
                var regex = new Regex(numericEquation);
                Match match = regex.Match(text);
                if (match.Success)
                {
                    try
                    {
                        Console.WriteLine($"Text:{text} Expression: {expression.Value} Match:{match.Value}");
                        if (expression.Name != null)
                        {

                            return Int32.Parse(match.Value.Replace(expression.Name, ""));
                        }
                        return Int32.Parse(match.Value);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Match failed on Expression: {expression.Value} for text {text} with match result to int {match.Value}");
                        return null;
                    }
                }

            }
            return null;
        }


    }
}
