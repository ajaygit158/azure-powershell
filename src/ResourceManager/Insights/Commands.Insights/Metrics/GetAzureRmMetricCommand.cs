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
using System;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Azure.Insights;
using Microsoft.Azure.Insights.Models;
using Microsoft.Rest.Azure.OData;

namespace Microsoft.Azure.Commands.Insights.Metrics
{
    /// <summary>
    /// Get the list of metric definition for a resource.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureRmMetric"), OutputType(typeof(Metric[]))]
    public class GetAzureRmMetricCommand : MonitorClientCmdletBase
    {
        internal const string GetAzureRmAMetricParamGroup = "Parameters for Get-AzureRmMetric cmdlet in the default mode";
        internal const string GetAzureRmAMetricFullParamGroup = "Parameters for Get-AzureRmMetric cmdlet in the full param set mode";

        /// <summary>
        /// Default value of the timerange to search for metrics
        /// </summary>
        public static readonly TimeSpan DefaultTimeRange = TimeSpan.FromHours(1);

        /// <summary>
        /// Gets or sets the ResourceId parameter of the cmdlet
        /// </summary>
        [Parameter(ParameterSetName = GetAzureRmAMetricParamGroup, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource Id")]
        [Parameter(ParameterSetName = GetAzureRmAMetricFullParamGroup, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource Id")]
        [ValidateNotNullOrEmpty]
        public string ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the timegrain parameter of the cmdlet
        /// </summary>
        [Parameter(ParameterSetName = GetAzureRmAMetricFullParamGroup, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The time grain of the query.")]
        [ValidateNotNullOrEmpty]
        public TimeSpan TimeGrain { get; set; }

        /// <summary>
        /// Gets or sets the aggregation type parameter of the cmdlet
        /// </summary>
        [Parameter(ParameterSetName = GetAzureRmAMetricFullParamGroup, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The aggregation type of the query")]
        [ValidateNotNullOrEmpty]
        public AggregationType? AggregationType { get; set; }

        /// <summary>
        /// Gets or sets the starttime parameter of the cmdlet
        /// </summary>
        [Parameter(ParameterSetName = GetAzureRmAMetricFullParamGroup, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The start time of the query")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the endtime parameter of the cmdlet
        /// </summary>
        [Parameter(ParameterSetName = GetAzureRmAMetricFullParamGroup, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The end time of the query")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the metricnames parameter of the cmdlet
        /// </summary>
        [Parameter(ParameterSetName = GetAzureRmAMetricParamGroup, Position = 1, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The metric names of the query")]
        [Parameter(ParameterSetName = GetAzureRmAMetricFullParamGroup, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The metric names of the query")]
        [ValidateNotNullOrEmpty]
        public string[] MetricNames { get; set; }

        /// <summary>
        /// Gets or sets the detailedoutput parameter of the cmdlet
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "Return object with all the details of the records (the default is to return only some attributes, i.e. no detail)")]
        public SwitchParameter DetailedOutput { get; set; }

        /// <summary>
        /// Process the general parameters (i.e. defined in this class). The particular parameters are a responsibility of the descendants after adding a method to process more parameters.
        /// </summary>
        /// <returns>The query filter to be used by the cmdlet</returns>
        protected string ProcessParameters()
        {
            var buffer = new StringBuilder();
            if (this.MetricNames != null)
            {
                var metrics = this.MetricNames.Select(n => string.Concat("name.value eq '", n, "'")).Aggregate((a, b) => string.Concat(a, " or ", b));

                buffer.Append("(");
                buffer.Append(metrics);
                buffer.Append(")");

                if (this.TimeGrain != default(TimeSpan))
                {
                    buffer.Append(" and timeGrain eq duration'");
                    buffer.Append(XmlConvert.ToString(this.TimeGrain));
                    buffer.Append("'");
                }

                // EndTime defaults to Now
                if (this.EndTime == default(DateTime))
                {
                    this.EndTime = DateTime.Now;
                }

                // StartTime defaults to EndTime - DefaultTimeRange  (NOTE: EndTime defaults to Now)
                if (this.StartTime == default(DateTime))
                {
                    this.StartTime = this.EndTime.Subtract(DefaultTimeRange);
                }

                buffer.Append(" and startTime eq ");
                buffer.Append(this.StartTime.ToString("O"));
                buffer.Append(" and endTime eq ");
                buffer.Append(this.EndTime.ToString("O"));

                if (this.AggregationType != null)
                {
                    buffer.Append(" and aggregationType eq '");
                    buffer.Append(this.AggregationType);
                    buffer.Append("'");
                }
            }

            string queryFilter = buffer.ToString();
            if (queryFilter.StartsWith(" and "))
            {
                queryFilter = queryFilter.Substring(4);
            }

            return queryFilter.Trim();
        }

        /// <summary>
        /// Execute the cmdlet
        /// </summary>
        protected override void ProcessRecordInternal()
        {
            WriteWarning("This cmdlet has changed from the previous release: the call pattern and the output are different.");
 
            string queryFilter = this.ProcessParameters();
            bool fullDetails = this.DetailedOutput.IsPresent;

            // Call the proper API methods to return a list of raw records.
            var response = this.MonitorClient.Metrics.ListAsync(resourceUri: this.ResourceId, odataQuery: new ODataQuery<Metric>(queryFilter), cancellationToken: CancellationToken.None).Result;
            var records = response.Select<Metric, Metric>(e => fullDetails ? (Metric)new PSMetric(e) : new PSMetricNoDetails(e)).ToArray();

            WriteObject(sendToPipeline: records, enumerateCollection: true);
        }
    }
}
