﻿// ----------------------------------------------------------------------------------
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

using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Insights.OutputClasses;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading;
using Microsoft.Azure.Insights;
using Microsoft.Azure.Insights.Models;
using Microsoft.Rest.Azure.OData;

namespace Microsoft.Azure.Commands.Insights.Metrics
{
    /// <summary>
    /// Get the list of metric definitions for a resource.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureRmMetricDefinition"), OutputType(typeof(MetricDefinition[]))]
    public class GetAzureRmMetricDefinitionCommand : MonitorClientCmdletBase
    {
        /// <summary>
        /// Gets or sets the ResourceId parameter of the cmdlet
        /// </summary>
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource Id")]
        [ValidateNotNullOrEmpty]
        public string ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the metricnames parameter of the cmdlet
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "The metric names of the query")]
        [ValidateNotNullOrEmpty]
        public string[] MetricNames { get; set; }

        /// <summary>
        /// Gets or sets the detailedoutput parameter of the cmdlet
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "Return object with all the details of the records (the default is to return only some attributes, i.e. no detail)")]
        public SwitchParameter DetailedOutput { get; set; }

        /// <summary>
        /// Process the general parameters (i.e. defined in this class) and the particular parameters (i.e. the parameters added by the descendants of this class).
        /// </summary>
        /// <returns>The final query filter to be used by the cmdlet</returns>
        protected string ProcessParameters()
        {
            var buffer = new StringBuilder();
            if (this.MetricNames != null)
            {
                var metrics = this.MetricNames
                    .Select(n => string.Concat("name.value eq '", n, "'"))
                    .Aggregate((a, b) => string.Concat(a, " or ", b));
                buffer.Append(metrics);
            }

            return buffer.ToString().Trim();
        }

        /// <summary>
        /// Execute the cmdlet
        /// </summary>
        protected override void ProcessRecordInternal()
        {
            WriteWarning("This cmdlet has changed from the previous release: the call pattern and the output might be different.");

            string queryFilter = this.ProcessParameters();
            bool fullDetails = this.DetailedOutput.IsPresent;

            var response = this.MonitorClient.MetricDefinitions.ListAsync(resourceUri: this.ResourceId, odataQuery: new ODataQuery<MetricDefinition>(queryFilter), cancellationToken: CancellationToken.None).Result;

            // If fullDetails is present full details of the records are displayed, otherwise only a summary of the records is displayed
            var records = response.Select<MetricDefinition, MetricDefinition>(e => fullDetails ? (MetricDefinition)new PSMetricDefinition(e) : new PSMetricDefinitionNoDetails(e)).ToArray();

            WriteObject(sendToPipeline: records, enumerateCollection: true);
        }
    }
}
