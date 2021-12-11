﻿using IPB.LogicApp.Standard.Testing.Model.WorkflowRunActionDetails;
using IPB.LogicApp.Standard.Testing.Model.WorkflowRunOverview;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace LogicApp.Testing.Example
{
    [TestClass]
    public class NonSpecflowExampleTest
    {
        [TestMethod]
        public void GreenPath()
        {
            var workflowName = "Hello-World-1";

            var logicAppTestManager = LogicAppTestManagerBuilder.Build(workflowName);

            //I have a message to send to the workflow
            var message = new Dictionary<string, object>();
            message.Add("first_name", "mike");
            message.Add("last_name", "stephenson");
            var requestJson = JsonConvert.SerializeObject(message);

            //Send a message to the workflow
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var workflowResponse = logicAppTestManager.TriggerLogicAppWithPost(content);

            //If we get a run id then we know the logic app got the message
            Assert.IsNotNull(workflowResponse.WorkFlowRunId);

            //If the logic app started running we can load the run history at this point to start checking it later
            logicAppTestManager.LoadWorkflowRunHistory();

            //We can check the trigger status was successful
            var triggerStatus = logicAppTestManager.GetTriggerStatus();
            Assert.AreEqual(triggerStatus, TriggerStatus.Succeeded);

            //Check that an action was successful
            var actionStatus = logicAppTestManager.GetActionStatus("Compose - Log Message Received");
            Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

            //Check that another action was successful
            actionStatus = logicAppTestManager.GetActionStatus("Response");
            Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

            //Check the workflow run was successful
            var workflowRunStatus = logicAppTestManager.GetWorkflowRunStatus();
            Assert.AreEqual(WorkflowRunStatus.Succeeded, workflowRunStatus);
        }
    }
}