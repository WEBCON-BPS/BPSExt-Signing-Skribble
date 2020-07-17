using System;
using System.Data;
using System.Text;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using System.Linq;
using WebCon.WorkFlow.SDK.Tools.Data;
using WebCon.WorkFlow.SDK.Documents;
using WebCon.WorkFlow.SDK.Documents.Model;
using System.Diagnostics;
using WebCon.BpsExt.Signing.Skribble.CustomActions.Helpers;
using System.Collections.Generic;

namespace WebCon.BpsExt.Signing.Skribble.CustomActions.CheckAll
{
    public class CheckAllDocStatusAction : CustomAction<CheckAllDocStatusActionConfig>
    {
        public override ActionTriggers AvailableActionTriggers => ActionTriggers.Recurrent;

        public override void RunWithoutDocumentContext(RunCustomActionWithoutContextParams args)
        {
            var log = new StringBuilder();

            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var response = new SkribbleHelper(log, Configuration.ApiConfig).ChecAllkDocumentStatus();

                log.AppendLine($"API returned {response.Count()} envelopes");

                var sqlQuery = $"SELECT WFD_ID, {Configuration.Workflow.OperationFieldName} " +
                               $"from WFElements where WFD_STPID = {Configuration.Workflow.StepId}";

                var dt = SqlExecutionHelper.GetDataTableForSqlCommandOutsideTransaction(sqlQuery);
                CheckSignStatus(dt, response, sw);
            }
            catch (Exception e)
            {
                log.AppendLine(e.ToString());
                args.HasErrors = true;
                args.Message = e.Message;
            }
            finally
            {
                args.LogMessage = log.ToString();
                args.Context.PluginLogger.AppendInfo(log.ToString());
            }
        }

        private void CheckSignStatus(DataTable dt, List<Models.Documents> response, Stopwatch sw)
        {
            var time = TimeSpan.FromSeconds(Configuration.Workflow.ExecutionTime);

            foreach (DataRow row in dt.Rows)
            {
                if (sw.ElapsedMilliseconds > time.TotalMilliseconds)
                    break;

                var wfdId = Convert.ToInt32(row["WFD_ID"]);
                var docId = row[Configuration.Workflow.OperationFieldName].ToString();

                if (response.Any(x => x.document_id == docId))
                {
                    var signStatus = response.FirstOrDefault(x => x.document_id == docId)?.status_overall;
                    switch (signStatus)
                    {
                        case Models.Statuses.Signed:
                            MoveDocument(wfdId, Configuration.Workflow.SuccessPathId);
                            break;
                        case Models.Statuses.Open:
                            break;
                        default:
                            MoveDocument(wfdId, Configuration.Workflow.ErrorPathId);
                            break;
                    }
                }
            }
        }

        private void MoveDocument(int wfdId, int pathId)
        {
            var document = DocumentsManager.GetDocumentByID(wfdId, true);
            DocumentsManager.MoveDocumentToNextStep(new MoveDocumentToNextStepParams(document, pathId)
            {
                ForceCheckout = true,
                SkipPermissionsCheck = true
            });
        }
    }
}