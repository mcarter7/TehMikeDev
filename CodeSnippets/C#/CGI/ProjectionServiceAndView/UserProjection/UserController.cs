using IMarkReporting.Services.Query;
using IMarkReporting.Services.Query.User;
using IMarkReporting.Web.Models.Item;
using IMarkReporting.Web.Models.Report;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace IMarkReporting.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IQueryService _queryService;

        public UserController(IQueryService queryService)
        {
            _queryService = queryService;
        }

        public ActionResult UserEventHistory()
        {
            var model = new UserEventHistoryModel();
            var today = DateTime.Now;

            model.StartDate = today.Date.ToShortDateString();
            model.EndDate = today.Date.AddDays(1).ToShortDateString();

            return View(model);
        }

        [HttpPost]
        public ActionResult UserEventHistory(UserEventHistoryModel model)
        {
            var startDate = DateTime.Parse(model.StartDate) + new TimeSpan(0, 0, 0);
            var endDate = DateTime.Parse(model.EndDate) + new TimeSpan(23, 59, 59);

            GetUserEventHistoryQuery historyQuery = new GetUserEventHistoryQuery(model.Name, startDate, endDate);

            var userEvents = _queryService.Execute(historyQuery);

            foreach (var historicItem in userEvents.OrderBy(x => x.ModifiedDate))
            {
                var itemEventHistory = new ItemHistoryEvent();
                var events = historicItem.Event.Split(',').ToList<string>();
                var changesBefore = historicItem.ChangesBefore.Split(',').ToList<string>();
                var changesAfter = historicItem.ChangesAfter.Split(',').ToList<string>();
                var formattedEvents = new List<string>();
                var formattedChangesBefore = new List<string>();
                var formattedChangesAfter = new List<string>();

                RemoveModifedInfoTimestamp(changesBefore);
                RemoveModifedInfoTimestamp(changesAfter);

                formattedEvents = FormatActions(events, formattedEvents);
                formattedChangesBefore = FormatActions(changesBefore, formattedChangesBefore);
                formattedChangesAfter = FormatActions(changesAfter, formattedChangesAfter);

 #region Updated payload versions with added properties that are arrays of some kind to be formatted here

                foreach (var e in formattedEvents)
                {
                    if (e.Contains("Changed"))
                    {
                        var index = formattedEvents.IndexOf(e);
                        var isSomethingBeingAdded = formattedEvents[index + 1].Contains("Added");

                        if (isSomethingBeingAdded)
                        {
                            var formatAddedPropertyToCheckLogs = e.Replace("Changed", "").Replace(" : ", ": ");
                            var alreadyLogged = formattedChangesAfter.Contains(formatAddedPropertyToCheckLogs);

                            if (!alreadyLogged)
                            {
                                var payload = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(historicItem.Payload);
                                var formatToCheckPayload = Regex.Replace(formatAddedPropertyToCheckLogs.Replace(": ", ""), @"\s+", "");

                                if (payload.ContainsKey(formatToCheckPayload))
                                {
                                    if (formatToCheckPayload == "Label")
                                    {
                                        var propertyToLog = payload[formatToCheckPayload];

                                        var LabelPayload = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>("{" + propertyToLog.Payload.ToString().Replace("{", "").Replace("}", "") + "}");
                                        var label_ItemId = LabelPayload["ItemId"];
                                        var label_NSN = LabelPayload["NSN"];
                                        var label_Description = LabelPayload["Description"];
                                        var label_RFID = LabelPayload["RFID"];
                                        var labelType = propertyToLog.LabelType;

                                        formattedChangesAfter.Add("Label: ");
                                        formattedChangesAfter.Add("-  Label Type: " + labelType.ToString().Replace("\"", ""));
                                        formattedChangesAfter.Add("-  Item Id: " + label_ItemId.ToString().Replace("\"", ""));
                                        formattedChangesAfter.Add("-  NSN: " + label_NSN.ToString().Replace("\"", ""));
                                        formattedChangesAfter.Add("-  Description: " + label_Description.ToString().Replace("\"", ""));
                                        formattedChangesAfter.Add("-  RFID: " + label_RFID.ToString().Replace("\"", ""));
                                    }
                                    if (formatToCheckPayload == "Epcs")
                                    {
                                        var propertyToLog = payload[formatToCheckPayload];

                                        String[] epcArray = propertyToLog.ToString().Split(',');

                                        formattedChangesAfter.Add("Epcs: ");
                                        for (int i = 0; i < epcArray.Count(); i++)
                                        {
                                            formattedChangesAfter.Add("- " + epcArray[i].ToString().Replace("\"", "").Replace("[", "").Replace("]", ""));
                                        }
                                    }
                                    if (formatToCheckPayload == "ParentLocation" || formatToCheckPayload == "Location")
                                    {
                                        var propertyToLog = payload[formatToCheckPayload];

                                        var parentL_Name = propertyToLog.Name;
                                        String[] parentL_Epcs = propertyToLog.Epcs.ToString().Split(',');
                                        String[] parentL_LabelIds = propertyToLog.LabelIds.ToString().Split(',');

                                        formattedChangesAfter.Add("Parent Location: ");
                                        formattedChangesAfter.Add("-  Name: " + parentL_Name.ToString());

                                        if (parentL_Epcs.Count() > 0)
                                        {
                                            formattedChangesAfter.Add("-  Epcs: ");
                                            for (int i = 0; i < parentL_Epcs.Count(); i++)
                                            {
                                                formattedChangesAfter.Add("-= " + parentL_Epcs[i].ToString().Replace("\"", "").Replace("[", "").Replace("]", ""));
                                            }
                                        }
                                        if (parentL_LabelIds.Count() > 0)
                                        {
                                            formattedChangesAfter.Add("-  Label Ids: ");
                                            for (int i = 0; i < parentL_LabelIds.Count(); i++)
                                            {
                                                formattedChangesAfter.Add("-= " + parentL_LabelIds[i].ToString().Replace("\"", "").Replace("[", "").Replace("]", ""));
                                            }
                                        }
                                    }
                                    if (formatToCheckPayload == "Attributes")
                                    {
                                        var propertyToLog = payload[formatToCheckPayload];

                                        String[] attributesArray = propertyToLog.ToString().Split(']');

                                        formattedChangesAfter.Add("Attributes: ");
                                        for (int i = 0; i < attributesArray.Count(); i++)
                                        {
                                            if (!string.IsNullOrEmpty(attributesArray[i].ToString()))
                                            {
                                                formattedChangesAfter.Add("- " + attributesArray[i].ToString().Replace("\"", "").Replace("[", "").Replace("]", ""));
                                            }
                                        }
                                    }

                                }
                            }
                        }

                    }
                }

#endregion

                itemEventHistory.Events = formattedEvents;
                itemEventHistory.ChangesBefore = formattedChangesBefore;
                itemEventHistory.ChangesAfter = formattedChangesAfter;
                itemEventHistory.ModifiedDate = historicItem.ModifiedDate.ToString();
                itemEventHistory.ModifiedBy = historicItem.ModifiedBy;

                model.reportEventItems.Add(itemEventHistory);
            }

            model.SearchStarted = true;
            return View("UserEventHistory", model);
        }

        [ActionName("ExportBtnClick")]
        public Object UserEventHistoryExport(string selectedStartDate, string selectedEndDate, string selectedName)
        {
            UserEventHistoryModel model = new UserEventHistoryModel();

            var startDate = DateTime.Parse(selectedStartDate) + new TimeSpan(0, 0, 0);
            var endDate = DateTime.Parse(selectedEndDate) + new TimeSpan(23, 59, 59);

            GetUserEventHistoryQuery historyQuery = new GetUserEventHistoryQuery(selectedName, startDate, endDate);

            var userEvents = _queryService.Execute(historyQuery);

            foreach (var historicItem in userEvents.OrderBy(x => x.ModifiedDate))
            {
                var itemEventHistory = new ItemHistoryEvent();
                var events = historicItem.Event.Split(',').ToList<string>();
                var changesBefore = historicItem.ChangesBefore.Split(',').ToList<string>();
                var changesAfter = historicItem.ChangesAfter.Split(',').ToList<string>();
                var formattedEvents = new List<string>();
                var formattedChangesBefore = new List<string>();
                var formattedChangesAfter = new List<string>();

                RemoveModifedInfoTimestamp(changesBefore);
                RemoveModifedInfoTimestamp(changesAfter);

                formattedEvents = FormatActions(events, formattedEvents);
                formattedChangesBefore = FormatActions(changesBefore, formattedChangesBefore);
                formattedChangesAfter = FormatActions(changesAfter, formattedChangesAfter);

                #region Updated payload versions with added properties that are arrays of some kind to be formatted here

                foreach (var e in formattedEvents)
                {
                    if (e.Contains("Changed"))
                    {
                        var index = formattedEvents.IndexOf(e);
                        var isSomethingBeingAdded = formattedEvents[index + 1].Contains("Added");

                        if (isSomethingBeingAdded)
                        {
                            var formatAddedPropertyToCheckLogs = e.Replace("Changed", "").Replace(" : ", ": ");
                            var alreadyLogged = formattedChangesAfter.Contains(formatAddedPropertyToCheckLogs);

                            if (!alreadyLogged)
                            {
                                var payload = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(historicItem.Payload);
                                var formatToCheckPayload = Regex.Replace(formatAddedPropertyToCheckLogs.Replace(": ", ""), @"\s+", "");

                                if (payload.ContainsKey(formatToCheckPayload))
                                {
                                    if (formatToCheckPayload == "Label")
                                    {
                                        var propertyToLog = payload[formatToCheckPayload];

                                        var LabelPayload = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>("{" + propertyToLog.Payload.ToString().Replace("{", "").Replace("}", "") + "}");
                                        var label_ItemId = LabelPayload["ItemId"];
                                        var label_NSN = LabelPayload["NSN"];
                                        var label_Description = LabelPayload["Description"];
                                        var label_RFID = LabelPayload["RFID"];
                                        var labelType = propertyToLog.LabelType;

                                        formattedChangesAfter.Add("Label: ");
                                        formattedChangesAfter.Add("-  Label Type: " + labelType.ToString().Replace("\"", ""));
                                        formattedChangesAfter.Add("-  Item Id: " + label_ItemId.ToString().Replace("\"", ""));
                                        formattedChangesAfter.Add("-  NSN: " + label_NSN.ToString().Replace("\"", ""));
                                        formattedChangesAfter.Add("-  Description: " + label_Description.ToString().Replace("\"", ""));
                                        formattedChangesAfter.Add("-  RFID: " + label_RFID.ToString().Replace("\"", ""));
                                    }
                                    if (formatToCheckPayload == "Epcs")
                                    {
                                        var propertyToLog = payload[formatToCheckPayload];

                                        String[] epcArray = propertyToLog.ToString().Split(',');

                                        formattedChangesAfter.Add("Epcs: ");
                                        for (int i = 0; i < epcArray.Count(); i++)
                                        {
                                            formattedChangesAfter.Add("- " + epcArray[i].ToString().Replace("\"", "").Replace("[", "").Replace("]", ""));
                                        }
                                    }
                                    if (formatToCheckPayload == "ParentLocation" || formatToCheckPayload == "Location")
                                    {
                                        var propertyToLog = payload[formatToCheckPayload];

                                        var parentL_Name = propertyToLog.Name;
                                        String[] parentL_Epcs = propertyToLog.Epcs.ToString().Split(',');
                                        String[] parentL_LabelIds = propertyToLog.LabelIds.ToString().Split(',');

                                        formattedChangesAfter.Add("Parent Location: ");
                                        formattedChangesAfter.Add("-  Name: " + parentL_Name.ToString());

                                        if (parentL_Epcs.Count() > 0)
                                        {
                                            formattedChangesAfter.Add("-  Epcs: ");
                                            for (int i = 0; i < parentL_Epcs.Count(); i++)
                                            {
                                                formattedChangesAfter.Add("-= " + parentL_Epcs[i].ToString().Replace("\"", "").Replace("[", "").Replace("]", ""));
                                            }
                                        }
                                        if (parentL_LabelIds.Count() > 0)
                                        {
                                            formattedChangesAfter.Add("-  Label Ids: ");
                                            for (int i = 0; i < parentL_LabelIds.Count(); i++)
                                            {
                                                formattedChangesAfter.Add("-= " + parentL_LabelIds[i].ToString().Replace("\"", "").Replace("[", "").Replace("]", ""));
                                            }
                                        }
                                    }
                                    if (formatToCheckPayload == "Attributes")
                                    {
                                        var propertyToLog = payload[formatToCheckPayload];

                                        String[] attributesArray = propertyToLog.ToString().Split(']');

                                        formattedChangesAfter.Add("Attributes: ");
                                        for (int i = 0; i < attributesArray.Count(); i++)
                                        {
                                            if (!string.IsNullOrEmpty(attributesArray[i].ToString()))
                                            {
                                                formattedChangesAfter.Add("- " + attributesArray[i].ToString().Replace("\"", "").Replace("[", "").Replace("]", ""));
                                            }
                                        }
                                    }

                                }
                            }
                        }

                    }
                }

                #endregion

                itemEventHistory.Events = formattedEvents;
                itemEventHistory.ChangesBefore = formattedChangesBefore;
                itemEventHistory.ChangesAfter = formattedChangesAfter;
                itemEventHistory.ModifiedDate = historicItem.ModifiedDate.ToString();
                itemEventHistory.ModifiedBy = historicItem.ModifiedBy;

                model.reportEventItems.Add(itemEventHistory);
            }

            //Prepare for Export

            foreach(var reportItem in model.reportEventItems)
            {
                foreach(var ev in reportItem.Events)
                {
                    reportItem.EventsForExport += ev + " ";
                }

                foreach (var chB in reportItem.ChangesBefore)
                {
                    reportItem.ChangesBeforeForExport += chB + " ";
                }

                foreach (var chA in reportItem.ChangesAfter)
                {
                    reportItem.ChangesAfterForExport += chA + " ";
                }
            }

            return ExcelExport(model);
        }


        public object ExcelExport(UserEventHistoryModel model)
        {
            string filename = "User Event Report_" + System.DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ".xlsx";
            string[] fields = { "ModifiedDate", "EventsForExport", "ChangesBeforeForExport", "ChangesAfterForExport", "ModifiedBy" };
            string[] headers = { "Event Date", "Events", "Before", "After", "Modified By" };

            using (MemoryStream ms = new MemoryStream())
            {
                Stanley.Common.OpenXml.CommonExcelSpreadsheet.ExportTo(ms, model.reportEventItems, "Report", fields, headers);

                return new ExcelExport_UserHistoryReport(ms.ToArray(), filename);
            }
        }


        public class ExcelExport_UserHistoryReport : FileStreamResult
        { 
            private const string ExcelMimeType = "application/vnd.ms-excel";
            public ExcelExport_UserHistoryReport(byte[] resultBytes, string filename) : base(new MemoryStream(resultBytes), ExcelMimeType)
            {
                FileDownloadName = filename;
            }

        }

        [HttpGet]
        public JsonResult FindName(string selectedName)
        {
            GetUsersQuery userQuery = new GetUsersQuery(selectedName);

            var users = _queryService.Execute(userQuery);

            return new JsonResult { Data = users, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private List<string> FormatActions(List<string> events, List<string> eventsToSeparate)
        {

            if (events.Count > 0)
            {
                foreach (var e in events)
                {
                    if (!string.IsNullOrEmpty(e))
                    {
                        var branchEvent = Regex.Replace(e, "([a-z])_?([A-Z])", "$1 $2");

                        if (branchEvent.Contains("["))
                        {
                            var baseEvent = branchEvent.Split(']').ToList<string>();

                            eventsToSeparate.Add(baseEvent[0].Replace("[", "").Replace("]", "") + ": ");
                            eventsToSeparate.Add("- " + baseEvent[1].Replace("[", "").Replace("]", "").Replace("\"", ""));
                        }
                        else if (branchEvent.Contains("Registered") || branchEvent.Contains("Removed"))
                        {
                            eventsToSeparate.Add(events.First());
                        }
                        else
                        {
                            eventsToSeparate.Add("- " + branchEvent.Replace("]", "").Replace("\"", ""));
                        }
                    }
                }
            }
            else
            {
                eventsToSeparate = new List<string>();
            }

            return eventsToSeparate;
        }

        private void RemoveModifedInfoTimestamp(List<string> changes)
        {
            var index = 0;
            bool exist = false;

            foreach (var change in changes)
            {
                if (change.Contains("[ModifiedInfo] Timestamp:"))
                {
                    index = changes.IndexOf(change);
                    exist = true;
                }
            }

            if (exist)
            {
                changes.RemoveAt(index);
            }
        }
    }
}