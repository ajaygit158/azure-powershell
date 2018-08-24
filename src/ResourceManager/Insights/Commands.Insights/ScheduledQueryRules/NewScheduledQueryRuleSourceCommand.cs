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

using Microsoft.Azure.Commands.Insights.OutputClasses;
using Microsoft.Azure.Management.Monitor.Models;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.Insights.ScheduledQueryRules
{
    /// <summary>
    /// Create a ScheduledQueryRule Source
    /// </summary>
    [Cmdlet(VerbsCommon.New, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "ScheduledQueryRuleSource"), OutputType(typeof(PSSource))]
    class NewScheduledQueryRuleSourceCommand : MonitorCmdletBase
    {

        #region Cmdlet parameters

        [ValidateNotNullOrEmpty]
        public string Query { get; set; }

        public IList<string> AuthorizedResources { get; set; }

        [ValidateNotNullOrEmpty]
        public string DataSourceId { get; set; }

        public string QueryType { get; set; }

        #endregion
        protected override void ProcessRecordInternal()
        {
            Source source = new Source(Query, DataSourceId, AuthorizedResources, QueryType);
            WriteObject(new PSSource(source));
        }
    }
}
