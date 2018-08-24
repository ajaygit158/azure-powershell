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
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Xml;
using Microsoft.Azure.Commands.Insights.OutputClasses;
using Microsoft.Azure.Management.Monitor;
using Microsoft.Azure.Management.Monitor.Models;
using Microsoft.Rest.Azure.OData;
using System.Globalization;

namespace Microsoft.Azure.Commands.Insights.ScheduledQueryRules
{
    /// <summary>
    /// Add or Update a Scheduled Query Rule
    /// </summary>
    [Cmdlet("Add", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "ScheduledQueryRule", SupportsShouldProcess = true), OutputType(typeof(PSLogSearchRuleResource))]
    class AddAzureRmScheduledQueryRuleCommand : ManagementCmdletBase
    {
        #region Cmdlet parameters

        public string Description { get; set; }


        #endregion

        protected override void ProcessRecordInternal()
        {
            throw new NotImplementedException();
        }
    }
}
