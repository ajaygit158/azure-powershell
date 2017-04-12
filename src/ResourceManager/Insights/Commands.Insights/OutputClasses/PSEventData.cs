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

using Microsoft.Azure.Insights.Models;

namespace Microsoft.Azure.Commands.Insights.OutputClasses
{
    /// <summary>
    /// Wrapps around the EventData and exposes all the localized strings as invariant/localized properties
    /// </summary>
    public class PSEventData : EventData
    {
        /// <summary>
        /// Initializes a new instance of the EventData class.
        /// </summary>
        public PSEventData(EventData eventData)
        {
            this.Authorization = eventData.Authorization;
            this.Caller = eventData.Caller;
            this.Claims = eventData.Claims;
            this.CorrelationId = eventData.CorrelationId;
            this.Description = eventData.Description;
            this.EventDataId = eventData.EventDataId;
            this.EventName = new PSLocalizableString(eventData.EventName);
            this.Category = new PSLocalizableString(eventData.Category);
            this.EventTimestamp = eventData.EventTimestamp;
            this.HttpRequest = eventData.HttpRequest;
            this.Id = eventData.Id;
            this.Level = eventData.Level;
            this.OperationId = eventData.OperationId;
            this.OperationName = new PSLocalizableString(eventData.OperationName); ;
            this.Properties = eventData.Properties;
            this.ResourceGroupName = eventData.ResourceGroupName;
            this.ResourceProviderName = new PSLocalizableString(eventData.ResourceProviderName);
            this.ResourceId = eventData.ResourceId;
            this.Status = new PSLocalizableString(eventData.Status);
            this.SubmissionTimestamp = eventData.SubmissionTimestamp;
            this.SubscriptionId = eventData.SubscriptionId;
            this.SubStatus = new PSLocalizableString(eventData.SubStatus);
        }
    }
}
