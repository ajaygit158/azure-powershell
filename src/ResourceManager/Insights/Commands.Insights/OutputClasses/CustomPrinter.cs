// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Microsoft.Azure.Commands.Insights.OutputClasses
{
    public static class CustomPrinter
    {
        /// <summary>
        /// Customized printing for the result of the powershell commands. It opens all the nested properties and collection elements.
        /// </summary>
        /// <param name="obj">The object</param>
        public static string Print(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            Print(obj, 0, sb);

            var output = sb.ToString();
            return output.EndsWith("\r\n") ? output.Substring(0, output.Length - 2) : output;
        }

        private static void Print(object obj, int currentIndent, StringBuilder sb)
        {
            if (obj == null)
            {
                sb.AppendLine();
                return;
            }

            //Handles the basic types
            if (obj is DateTime)
            {
                var objAsDateTime = (DateTime)obj;
                sb.AppendLine(objAsDateTime.Kind != DateTimeKind.Utc
                    ? objAsDateTime.ToUniversalTime().ToString("O")
                    : objAsDateTime.ToString("O"));

                return;
            }

            if (obj is TimeSpan)
            {
                var objAsTimeSpan = (TimeSpan)obj;
                sb.AppendLine(XmlConvert.ToString(objAsTimeSpan));
                return;
            }

            if (obj is System.String || obj is System.ValueType)
            {
                sb.AppendLine(obj.ToString());
                return;
            }

            if (obj is ICollection)
            {
                var atLeastOne = false;
                sb.Append("{");
                foreach (var item in ((ICollection)obj))
                {
                    if (!atLeastOne)
                    {
                        atLeastOne = true;
                        sb.AppendLine();
                    }

                    sb.AppendRepeated(' ', currentIndent);
                    Print(item, currentIndent + 3, sb);
                }

                if (atLeastOne)
                {
                    sb.AppendRepeated(' ', currentIndent);
                }

                sb.AppendLine("}");
                return;
            }

            // Handle an arbitrary object (no simple type, no collection)
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();
            int nameLength = properties.Max(x => x.Name.Length);

            sb.AppendLine("[");
            foreach (PropertyInfo property in properties)
            {
                sb.AppendRepeated(' ', currentIndent);
                sb.Append(property.Name);
                sb.AppendRepeated(' ', nameLength - property.Name.Length + 1);
                sb.Append(": ");
                Print(property.GetValue(obj), currentIndent + nameLength + 3, sb);
            }

            sb.AppendRepeated(' ', currentIndent);
            sb.AppendLine("]");
        }

        private static void AppendRepeated(this StringBuilder sb, char c, int count)
        {
            if (sb != null)
            {
                for (int i = 0; i < count; i++)
                {
                    sb.Append(c);
                }
            }
        }
    }
}
