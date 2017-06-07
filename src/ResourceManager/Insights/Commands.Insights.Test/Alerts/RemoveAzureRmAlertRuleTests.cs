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

using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Azure.Commands.Insights.Alerts;
using Microsoft.Azure.Management.Monitor.Management;
using Microsoft.WindowsAzure.Commands.Common.Test.Mocks;
using Microsoft.WindowsAzure.Commands.ScenarioTest;
using Moq;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.Azure.Commands.Insights.Test.Alerts
{
    public class RemoveAzureRmAlertRuleTests
    {
        private readonly RemoveAzureRmAlertRuleCommand cmdlet;
        private readonly Mock<MonitorManagementClient> insightsManagementClientMock;
        private readonly Mock<IAlertRulesOperations> insightsAlertRuleOperationsMock;
        private MockCommandRuntime commandRuntimeMock;
        private Microsoft.Rest.Azure.AzureOperationResponse response;
        private string resourceGroup;
        private string ruleNameOrTargetUri;

        public RemoveAzureRmAlertRuleTests(Xunit.Abstractions.ITestOutputHelper output)
        {
            ServiceManagemenet.Common.Models.XunitTracingInterceptor.AddToContext(new ServiceManagemenet.Common.Models.XunitTracingInterceptor(output));
            insightsAlertRuleOperationsMock = new Mock<IAlertRulesOperations>();
            insightsManagementClientMock = new Mock<MonitorManagementClient>();
            commandRuntimeMock = new MockCommandRuntime();
            cmdlet = new RemoveAzureRmAlertRuleCommand()
            {
                CommandRuntime = commandRuntimeMock,
                MonitorManagementClient = insightsManagementClientMock.Object
            };

            response = new Microsoft.Rest.Azure.AzureOperationResponse()
            {
                RequestId = Guid.NewGuid().ToString(),
                Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK    
                }
            };

            insightsAlertRuleOperationsMock.Setup(f => f.DeleteWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(response))
                .Callback((string resourceGrp, string ruleName, Dictionary<string, List<string>> h, CancellationToken t) =>
                {
                    resourceGroup = resourceGrp;
                    ruleNameOrTargetUri = ruleName;
                });

            insightsManagementClientMock.SetupGet(f => f.AlertRules).Returns(this.insightsAlertRuleOperationsMock.Object);
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void RemoveAlertRuleCommandParametersProcessing()
        {
            cmdlet.ResourceGroup = Utilities.ResourceGroup;
            cmdlet.Name = Utilities.Name;

            cmdlet.ExecuteCmdlet();
            Assert.Equal(Utilities.ResourceGroup, this.resourceGroup);
            Assert.Equal(Utilities.Name, this.ruleNameOrTargetUri);
        }
    }
}
