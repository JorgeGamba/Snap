﻿/*
Snap v1.0

Copyright (c) 2010 Tyler Brinks

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using System;
using System.Reflection;
using System.Text;
using Fasterflect;

namespace Snap
{
    /// <summary>
    /// Formats a method signature into a string representation
    /// </summary>
    public class MethodSignatureFormatter
    {
        /// <summary>
        /// Gets the formatting strings representing a method.
        /// </summary>
        /// <param name="method">A <see cref="MethodBase"/>.</param>
        /// <param name="methodParameters">The method parameters.</param>
        /// <returns></returns>
        public static string Create(MethodBase method, object[] methodParameters)
        {
            bool methodIsGeneric;

            var stringBuilder = new StringBuilder();

            var typeFormat = GetTypeFormatString(method.DeclaringType);
            var typeIsGeneric = method.DeclaringType.IsGenericTypeDefinition;

            // Build the format string for the method name.
            stringBuilder.Length = 0;
            stringBuilder.Append("::");
            stringBuilder.Append(method.Name);

            if (method.IsGenericMethodDefinition)
            {
                methodIsGeneric = true;
                stringBuilder.Append("<");

                for (var i = 0; i < method.GetGenericArguments().Length; i++)
                {
                    if (i > 0)
                    {
                        stringBuilder.Append(", ");
                    }

                    stringBuilder.AppendFormat("{{{0}}}", i);
                }

                stringBuilder.Append(">");
            }
            else
            {
                methodIsGeneric = false;
            }

            var methodFormat = stringBuilder.ToString();

            // Build the format string for parameters.
            stringBuilder.Length = 0;
            var parameters = method.Parameters();
            stringBuilder.Append("(");

            for (var i = 0; i < parameters.Count; i++)
            {
                if (i > 0)
                {
                    stringBuilder.Append(", ");
                }

                stringBuilder.Append("{{{");
                stringBuilder.Append(i);
                stringBuilder.Append("}}}");
            }

            stringBuilder.Append(")");

            var parameterFormat = stringBuilder.ToString();

            var signature = new MethodSignature
                                {
                                    MethodFormat = methodFormat,
                                    MethodIsGeneric = methodIsGeneric,
                                    ParameterFormat = parameterFormat,
                                    TypeFormat = typeFormat,
                                    TypeIsGeneric = typeIsGeneric
                                };

            return Format(signature, method, methodParameters);
        }
        /// <summary>
        /// Gets a formatting string representing a <see cref="Type"/>.
        /// </summary>
        /// <param name="type">A <see cref="Type"/>.</param>
        /// <returns>A formatting string representing the type
        /// where each generic type argument is represented as a
        /// formatting argument (e.g. <c>Dictionary&lt;{0},P1}&gt;</c>.
        /// </returns>
        private static string GetTypeFormatString(Type type)
        {
            var stringBuilder = new StringBuilder();

            // Build the format string for the declaring type.
            stringBuilder.Append(type.FullName);

            if (type.IsGenericTypeDefinition)
            {
                stringBuilder.Append("<");

                for (var i = 0; i < type.GetGenericArguments().Length; i++)
                {
                    if (i > 0)
                    {
                        stringBuilder.Append(", ");
                    }

                    stringBuilder.AppendFormat("{{{0}}}", i);
                }

                stringBuilder.Append(">");
            }

            return stringBuilder.ToString();
        }
        /// <summary>
        /// Gets a string representing a concrete method invocation.
        /// </summary>
        /// <param name="signature">The signature.</param>
        /// <param name="method">Invoked method.</param>
        /// <param name="invocationParameters">Concrete invocation parameters.</param>
        /// <returns>
        /// A representation of the method invocation.
        /// </returns>
        private static string Format(MethodSignature signature, MethodBase method, object[] invocationParameters)
        {
            var typeValue = signature.TypeIsGeneric
                                ? string.Format(signature.TypeFormat, method.DeclaringType.GetGenericArguments())
                                : signature.TypeFormat;

            var methodValue = signature.MethodIsGeneric
                                  ? string.Format(signature.MethodFormat, method.GetGenericArguments())
                                  : signature.MethodFormat;
            var parts = new[]
                            {
                                typeValue,
                                methodValue,
                                string.Format(signature.ParameterFormat, invocationParameters)
                            };

            return string.Concat(parts);
        }
    }
}
