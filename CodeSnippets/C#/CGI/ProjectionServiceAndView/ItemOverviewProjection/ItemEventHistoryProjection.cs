using Cgi.Infrastructure.Messaging;
using CGI.Aware.TagRegistry.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGI.Aware.IMarkOracleProjectionService.Projections
{
    class ItemEventHistoryProjection : AbstractProjection
    {

        private const string InsertSql = @"
INSERT INTO item_event_history(    id,    item_group_id,    item_id,    display_name,            modified_date,    payload,    changes_before,    changes_after,    event,    modified_by,    modified_by_id) VALUES (    :id,    :item_group_id,    :item_id,    :display_name,    :modified_date,    :payload,    :changes_before,    :changes_after,    :event,    :modified_by,    :modified_by_id)";
        private const string FetchLastEventForItemSql = @"
SELECT ITEM_ID, DISPLAY_NAME, EVENT, PAYLOAD
FROM ITEM_EVENT_HISTORY
WHERE item_id = (
SELECT Item_Id
FROM (
SELECT Item_Id, MAX(Modified_Date) FROM ITEM_EVENT_HISTORY WHERE ITEM_ID = :item_id GROUP BY ITEM_ID)
)
";


        private string connectionString = string.Empty;

        public ItemEventHistoryProjection(string connString) : base("item_event_history", connString)
        {
            connectionString = connString;
        }

        private ItemEventHistory FetchLastEventForItem(string itemId)
        {
            ItemEventHistory itemEvent = new ItemEventHistory();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand(FetchLastEventForItemSql, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue(Constants.Parameters.ItemId, itemId);

                    using (OracleDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            itemEvent.ItemId = rdr.GetString(0);
                            itemEvent.DisplayName = (rdr.IsDBNull(1)) ? string.Empty : rdr.GetString(1);
                            itemEvent.Event = rdr.GetOracleBlob(2);
                            itemEvent.PayloadBlob = rdr.GetOracleBlob(3);

                            if( itemEvent.PayloadBlob != null)
                            {
                                Byte[] payloadByte = (byte[])itemEvent.PayloadBlob.Value;
                                //string payload = System.Text.Encoding.UTF8.GetString(payloadByte);

                                itemEvent.PayloadJSON = System.Text.Encoding.UTF8.GetString(payloadByte);
                                //itemEvent.Payload = JsonConvert.DeserializeObject<IMarkOverviewItem>(payload);
                            }
                        }
                    }
                }

                connection.Close();
            }

            return itemEvent;
        }

        private void PopulateParameters(OracleCommand cmd, Guid itemGroupId, string itemId, string displayName, string payload, IDictionary<string, object> metadata, 
                                        string itemPayloadChangesBefore, string itemPayloadChangesAfter, string itemEvent, string modifiedBy, string modifiedById)
        {
            EventStore.ClientAPI.ResolvedEvent resolvedInfo = (EventStore.ClientAPI.ResolvedEvent)metadata["es:ResolvedEvent"];

            cmd.Parameters.AddWithValue(Constants.Parameters.Id, Guid.NewGuid());
            cmd.Parameters.AddWithValue(Constants.Parameters.ItemGroupId, itemGroupId);
            cmd.Parameters.AddWithValue(Constants.Parameters.ItemId, itemId);
            cmd.Parameters.AddWithValue(Constants.Parameters.DisplayName, displayName.GetValue(128));
            cmd.Parameters.AddWithValue(Constants.Parameters.ModifiedDate, DateTimeFromUnixTimestampMillis(long.Parse(resolvedInfo.Event.CreatedEpoch.ToString())));
            cmd.Parameters.AddWithValue(Constants.Parameters.Payload, payload.ToByteArray());
            cmd.Parameters.AddWithValue(Constants.Parameters.ChangesBefore, itemPayloadChangesBefore.ToByteArray());
            cmd.Parameters.AddWithValue(Constants.Parameters.ChangesAfter, itemPayloadChangesAfter.ToByteArray());
            cmd.Parameters.AddWithValue(Constants.Parameters.Event, itemEvent.ToByteArray());
            cmd.Parameters.AddWithValue(Constants.Parameters.ModifiedBy, modifiedBy.GetValue(64));
            cmd.Parameters.AddWithValue(Constants.Parameters.ModifiedById, modifiedById.GetValue(20));
        }

        private void Handle(Envelope<ItemRegistered> e)
        {
            Execute(e.Metadata, cmd =>
            {
                cmd.CommandText = InsertSql;

                var item = JsonConvert.DeserializeObject<IMarkOverviewItem>(e.Body.Payload);

                PopulateParameters(cmd, e.Body.ItemGroupId, e.Body.ItemId, e.Body.DisplayName,
                    e.Body.Payload, e.Metadata, string.Empty, string.Empty, ItemEvents.ItemRegistered, item.ModifiedInfo.Name, item.ModifiedInfo.UserId);

                cmd.ExecuteNonQuery();
            });
        }

        private void Handle(Envelope<ItemReRegistered> e)
        {
            Execute(e.Metadata, cmd =>
            {
                cmd.CommandText = InsertSql;

                var item = JsonConvert.DeserializeObject<IMarkOverviewItem>(e.Body.Payload);

                PopulateParameters(cmd, e.Body.ItemGroupId, e.Body.ItemId, e.Body.DisplayName,
                    e.Body.Payload, e.Metadata, string.Empty, string.Empty, ItemEvents.ItemReRegistered, item.ModifiedInfo.Name, item.ModifiedInfo.UserId);

                cmd.ExecuteNonQuery();
            });
        }

        private void Handle(Envelope<ItemRemoved> e)
        {
            Execute(e.Metadata, cmd =>
            {
                cmd.CommandText = InsertSql;

                PopulateParameters(cmd, e.Body.ItemGroupId, e.Body.ItemId, string.Empty,string.Empty,
                    e.Metadata, string.Empty, string.Empty, ItemEvents.ItemRemoved, "Admin", "");

                cmd.ExecuteNonQuery();
            });
        }

        private void Handle(IEnvelope<ItemDisplayNameChanged> e)
        {
            Execute(e.Metadata, cmd =>
            {
                cmd.CommandText = InsertSql;

                var sourceItem = FetchLastEventForItem(e.Body.ItemId);
                var convertedItem = JsonConvert.DeserializeObject<dynamic>(sourceItem.PayloadJSON);

                convertedItem.Detail.Description = e.Body.DisplayName;

                var updatedItem = JsonConvert.SerializeObject(convertedItem);

                PopulateParameters(cmd, e.Body.ItemGroupId, e.Body.ItemId, e.Body.DisplayName, updatedItem.ToString(),
                    e.Metadata, string.Empty, string.Empty, ItemEvents.DescriptionChanged, "", "");

                cmd.ExecuteNonQuery();
            });
        }

        private void Handle(Envelope<ItemPayloadChanged> e)
        {
            Execute(e.Metadata, cmd =>
            {
                cmd.CommandText = InsertSql;

                var sourceItem = FetchLastEventForItem(e.Body.ItemId);

                JObject sourceJObject = JsonConvert.DeserializeObject<JObject>(sourceItem.PayloadJSON);
                JObject targetJObject = JsonConvert.DeserializeObject<JObject>(e.Body.Payload);
                List<string> updatedValues = new List<string>();
                List<string> oldValues = new List<string>();
                string itemEvent = string.Empty;
                
                if (!JToken.DeepEquals(sourceJObject, targetJObject))
                {
                    foreach (KeyValuePair<string, JToken> sourceProperty in sourceJObject)
                    {
                        JProperty targetProperty = targetJObject.Property(sourceProperty.Key);

                        if(targetProperty == null)
                        {
                            Console.WriteLine(string.Format("{0} property value was removed", sourceProperty.Key));
                        }
                        else if (sourceProperty.Key.ToString() == "ModifiedInfo")
                        {
                            Console.WriteLine(string.Format("{0} property value ModifiedInfo not logged", sourceProperty.Key));
                        }
                        else if (!JToken.DeepEquals(sourceProperty.Value, targetProperty.Value))
                        {
                            itemEvent += FormatEventBase(sourceProperty.Key, ItemEvents.Changed);
                            Console.WriteLine(string.Format("{0} property value is changed", sourceProperty.Key));

                            bool targetPropsRemoved = false;
                            bool sourcePropsNonexistent = false;
                            Dictionary<string, dynamic> propValueTargetDictionary = new Dictionary<string, dynamic>();
                            Dictionary<string, dynamic> propValueSourceDictionary = new Dictionary<string, dynamic>();

                            if (!String.IsNullOrEmpty(targetProperty.Value.ToString()) && targetProperty.Value.ToString() != "[]")
                            {
                                if (sourceProperty.Key.ToString() == "Attributes" || sourceProperty.Key.ToString() == "Epcs" || sourceProperty.Key.ToString() == "SourceItems")
                                {
                                    var propTargetArray = targetProperty.Value.ToArray();

                                    for(int i = 0; i < propTargetArray.Count(); i++)
                                    {
                                        propValueTargetDictionary.Add(sourceProperty.Key.ToString() + "Payload" + i.ToString(), propTargetArray[i].ToString());
                                    }
                                }
                                else
                                {
                                    propValueTargetDictionary = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(targetProperty.Value.ToString());
                                }
                            }
                            else
                            {
                                targetPropsRemoved = true;
                            }

                            if (!String.IsNullOrEmpty(sourceProperty.Value.ToString()) && sourceProperty.Value.ToString() != "[]")
                            {
                                if (sourceProperty.Key.ToString() == "Attributes" || sourceProperty.Key.ToString() == "Epcs" || sourceProperty.Key.ToString() == "SourceItems")
                                {
                                    var propSourceArray = sourceProperty.Value.ToArray();

                                    for (int i = 0; i < propSourceArray.Count(); i++)
                                    {
                                        propValueSourceDictionary.Add(sourceProperty.Key.ToString() + "Payload" + i.ToString(), propSourceArray[i].ToString());
                                    }
                                }
                                else
                                {
                                    propValueSourceDictionary = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(sourceProperty.Value.ToString());
                                }
                            }
                            else
                            {
                                sourcePropsNonexistent = true;
                            }

                            if (targetPropsRemoved)
                            {
                                Console.WriteLine(string.Format("{0} property value is removed", sourceProperty.Key));
                                itemEvent += FormatEventBranches(sourceProperty.Key, ItemEvents.Removed);
                            }
                            if(sourcePropsNonexistent)
                            {
                                Console.WriteLine(string.Format("{0} property value is added", sourceProperty.Key));
                                itemEvent += FormatEventBranches(sourceProperty.Key, ItemEvents.Added);
                            }

                            //Differences Between Payloads Through Dictionaries 
                            if (propValueTargetDictionary.Count > 0 && propValueSourceDictionary.Count > 0 && propValueTargetDictionary != propValueSourceDictionary)
                            {
                                var sourceDiff = propValueSourceDictionary.Except(propValueTargetDictionary).ToDictionary(t => t.Key, t => t.Value);
                                var targetDiff = propValueTargetDictionary.Except(propValueSourceDictionary).ToDictionary(t => t.Key, t => t.Value);

                                foreach(var prop in targetDiff)
                                {
                                    if (prop.Key.ToString().Contains("Payload"))
                                    {
                                        if (propValueSourceDictionary.ContainsKey(prop.Key.ToString()))
                                        {
                                            var sourcePropPayload = propValueSourceDictionary[prop.Key.ToString()];
                                            var payloadTargetDictionary = new Dictionary<string, dynamic>();
                                            var payloadSourceDictionary = new Dictionary<string, dynamic>();

                                            if (prop.Key.ToString().Contains("Epcs"))
                                            {
                                                payloadTargetDictionary.Add(prop.Key.ToString(), prop.Value.ToString());
                                                payloadSourceDictionary.Add(prop.Key.ToString(), sourcePropPayload.ToString());
                                            }
                                            else
                                            {
                                                payloadTargetDictionary = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(prop.Value.ToString());
                                                payloadSourceDictionary = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(sourcePropPayload.ToString());
                                            }

                                            var sourcePayloadDiff = payloadSourceDictionary.Except(payloadTargetDictionary).ToDictionary(t => t.Key, t => t.Value);
                                            var targetPayloadDiff = payloadTargetDictionary.Except(payloadSourceDictionary).ToDictionary(t => t.Key, t => t.Value);

                                            foreach (var payloadProp in targetPayloadDiff)
                                            {
                                                var keyExist = sourcePayloadDiff.ContainsKey(payloadProp.Key);

                                                if (keyExist)
                                                {
                                                    oldValues.Add(FormatPropertyChange(sourceProperty.Key, payloadProp.Key, sourcePayloadDiff[payloadProp.Key]));

                                                    if (sourcePayloadDiff[payloadProp.Key] == null)
                                                    {
                                                        itemEvent += FormatEventBranches(payloadProp.Key, ItemEvents.Added);
                                                    }
                                                    else if (sourcePayloadDiff[payloadProp.Key].ToString() == "[]")
                                                    {
                                                        itemEvent += FormatEventBranches(payloadProp.Key, ItemEvents.Added);
                                                    }
                                                    else
                                                    {
                                                        itemEvent += FormatEventBranches(payloadProp.Key, ItemEvents.Updated);
                                                    }
                                                }
                                                else
                                                {
                                                    itemEvent += FormatEventBranches(payloadProp.Key, ItemEvents.Added);
                                                }

                                                updatedValues.Add(FormatPropertyChange(sourceProperty.Key, payloadProp.Key, payloadProp.Value));
                                            }
                                        }
                                        else
                                        {
                                            updatedValues.Add(FormatPropertyChange(sourceProperty.Key + ItemEvents.Added, prop.Key, prop.Value));
                                            itemEvent += FormatEventBranches(prop.Key, ItemEvents.Added);
                                        }
                                    }
                                    else
                                    {
                                        var keyExist = sourceDiff.ContainsKey(prop.Key);

                                        if (keyExist)
                                        {
                                            oldValues.Add(FormatPropertyChange(sourceProperty.Key, prop.Key, sourceDiff[prop.Key]));

                                            if(sourceDiff[prop.Key] == null)
                                            {
                                                itemEvent += FormatEventBranches(prop.Key, ItemEvents.Added);
                                            }
                                            else if (sourceDiff[prop.Key].ToString() == "[]")
                                            {
                                                itemEvent += FormatEventBranches(prop.Key, ItemEvents.Added);
                                            }
                                            else
                                            {
                                                itemEvent += FormatEventBranches(prop.Key, ItemEvents.Updated);
                                            }
                                        }
                                        else
                                        {
                                            itemEvent += FormatEventBranches(prop.Key, ItemEvents.Added);
                                        }

                                        updatedValues.Add(FormatPropertyChange(sourceProperty.Key, prop.Key, prop.Value));
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(string.Format("{0} property value didn't change", sourceProperty.Key));
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Objects are the same");
                    itemEvent = ItemEvents.NoChanges;
                }

                var changesBefore = FormatChanges(oldValues);
                var changesAfter = FormatChanges(updatedValues);

                if(itemEvent != ItemEvents.NoChanges && !String.IsNullOrEmpty(itemEvent))
                {
                    var item = JsonConvert.DeserializeObject<IMarkOverviewItem>(e.Body.Payload);

                    PopulateParameters(cmd, e.Body.ItemGroupId, e.Body.ItemId, string.Empty, e.Body.Payload, e.Metadata, changesBefore, changesAfter, itemEvent, item.ModifiedInfo.Name, item.ModifiedInfo.UserId);

                    cmd.ExecuteNonQuery();
                }
            });
        }

        private string FormatChanges(List<string> values)
        {
            int count = 0;
            string changes = string.Empty;

            while (count < values.Count)
            {
                if (count != values.Count - 1)
                {
                    changes += values[count] + ",";
                }
                else
                {
                    changes += values[count];
                }

                count += 1;
            }

            return changes;
        }

        private string FormatPropertyChange(string sourcePropertyKey, string propKey, dynamic PropValue)
        {
            if(propKey == "Timestamp")
            {
                PropValue = DateTimeFromUnixTimestampSeconds(long.Parse(PropValue.ToString()));
            }


            return "[" + sourcePropertyKey + "] " + propKey + ": " + PropValue;
        }

        private string FormatEventBase(string key, string itemEvent)
        {
            return "[" + key + itemEvent + "] ";
        }

        private string FormatEventBranches(string key, string itemEvent)
        {
            return key + itemEvent + ",";
        }

#region UnixDateTimeConversion
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetCurrentUnixTimestampMillis()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
        }

        public static DateTime DateTimeFromUnixTimestampMillis(long millis)
        {
            return UnixEpoch.AddMilliseconds(millis);
        }

        public static long GetCurrentUnixTimestampSeconds()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
        }

        public static DateTime DateTimeFromUnixTimestampSeconds(long seconds)
        {
            return UnixEpoch.AddSeconds(seconds);
        }
#endregion

    }

    public class ItemEventHistory
    {
        public string Id { get; set; }

        public Guid ItemGroupId { get; set; }

        public string ItemId { get; set; }

        public string DisplayName { get; set; }

        public Oracle.ManagedDataAccess.Types.OracleBlob PayloadBlob { get; set; }

        public string PayloadJSON { get; set; }

        //public IMarkOverviewItem Payload { get; set; }

        public Oracle.ManagedDataAccess.Types.OracleBlob Event { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string DisplayChanges { get; set; }
    }
}
