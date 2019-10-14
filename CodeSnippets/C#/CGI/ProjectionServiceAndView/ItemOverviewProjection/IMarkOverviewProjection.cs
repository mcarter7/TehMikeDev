using Cgi.Infrastructure.Messaging;
using CGI.Aware.IMarkOracleProjectionService.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGI.Aware.TagRegistry.Contracts;
using Newtonsoft.Json;
using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;

namespace CGI.Aware.IMarkOracleProjectionService.Projections
{
    class IMarkOverviewProjection : AbstractProjection
    {
        private const string InsertSql = @"
INSERT INTO IMARK_OVERVIEW (    id,    item_Id,    plan_Id,    is_serialized,    serial_number,    nsn,    description,    location,    location_Id,    parent_location,    parent_location_id,    qty,    created_by_name,    created_by_id,    created_date) VALUES (    :id,    :item_id,    :plan_Id,    :is_serialized,    :serial_number,    :nsn,    :description,    :location,    :location_Id,    :parent_location,    :parent_location_id,    :qty,    :created_by_name,    :created_by_id,    :created_date)";

        private const string DeleteSql = @"
DELETE FROM IMARK_OVERVIEW
WHERE id = :id
";

        private const string UpdateItemSql = @"
UPDATE IMARK_OVERVIEW
SET id = :id,    item_Id = :item_id,    plan_Id = :plan_Id,    is_serialized = :is_serialized,    serial_number = :serial_number,    nsn = :nsn,    description = :description,    location = :location,    location_Id = :location_Id,    parent_location = :parent_location,    parent_location_id = :parent_location_id,    qty = :qty,    created_by_name = :created_by_name,    created_by_id = :created_by_id,    created_date = :created_date
WHERE id = :id
";
        private const string FetchItemByItemIdSql = @"
SELECT item_Id,    plan_Id,    serial_number,    description
FROM IMARK_OVERVIEW
WHERE item_id = :item_id
";

        private string connectionString = string.Empty;

        public IMarkOverviewProjection(string connString) : base("imark_overview", connString)
        {
            connectionString = connString;
        }

        private void PopulateParameters(OracleCommand cmd, Guid itemGroupId, string itemId, string payload)
        {
            var item = JsonConvert.DeserializeObject<IMarkOverviewItem>(payload);

            item = PopulateLocations(item);

            cmd.Parameters.AddHashId(Constants.Parameters.Id, itemGroupId, itemId);
            cmd.Parameters.AddWithValue(Constants.Parameters.ItemId, itemId);
            cmd.Parameters.AddWithValue(Constants.Parameters.PlanId, item.Detail.PlanId);
            cmd.Parameters.AddWithValue(Constants.Parameters.IsSerialized, item.Detail.IsSerialized());
            cmd.Parameters.AddWithValue(Constants.Parameters.SerialNumber, item.Detail.SerialNumber.GetValue(32));
            cmd.Parameters.AddWithValue(Constants.Parameters.NSN, item.Detail.Fsc + item.Detail.Niin);
            cmd.Parameters.AddWithValue(Constants.Parameters.Description, item.Detail.Description.GetValue(128));
            cmd.Parameters.AddWithValue(Constants.Parameters.Location, item.Location.GetLocationName());
            cmd.Parameters.AddWithValue(Constants.Parameters.LocationId, item.Location.GetLocationId());
            cmd.Parameters.AddWithValue(Constants.Parameters.ParentLocation, item.ParentLocation.GetLocationName());
            cmd.Parameters.AddWithValue(Constants.Parameters.ParentLocationId, item.ParentLocation.GetLocationId());
            cmd.Parameters.AddWithValue(Constants.Parameters.QTY, item.Detail.Quantity * item.Detail.QuantityPerIssue);
            cmd.Parameters.AddWithValue(Constants.Parameters.CreatedByName, item.MarkerInfo.Name.GetValue(64));
            cmd.Parameters.AddWithValue(Constants.Parameters.CreatedById, item.MarkerInfo.UserId.GetValue(20));
            cmd.Parameters.AddWithValue(Constants.Parameters.CreatedDate, item.MarkerInfo.CreatedDate);
        }

        private IMarkOverviewItem PopulateLocations(IMarkOverviewItem item)
        {
            if (item.Location != null)
            {
                var g = Guid.Empty;
                var isGuid = Guid.TryParse(item.Location.Name, out g);

                if (isGuid && g != Guid.Empty)
                    if (item.Detail.SerialNumber != string.Empty)
                        item.Location.Name = item.Detail.SerialNumber.GetValue(32);
                    else
                        item.Location.Name = item.Detail.Description.GetValue(128);
            }

            if(item.ParentLocation != null)
            {
                var g = Guid.Empty;
                var isGuid = Guid.TryParse(item.ParentLocation.Name, out g);

                if (isGuid && g != Guid.Empty)
                {
                    var fetchItem = FetchItembyItemId(g);

                    if (fetchItem.SerialNumber != string.Empty)
                        item.ParentLocation.Name = fetchItem.SerialNumber;
                    else
                        item.ParentLocation.Name = fetchItem.Description;
                }
            }

            return item;
        }

        private IMarkFlatItem FetchItembyItemId(Guid itemId)
        {
            IMarkFlatItem flatItem = new IMarkFlatItem();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand(FetchItemByItemIdSql, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue(Constants.Parameters.ItemId, itemId.ToString().Replace("-",""));

                    using (OracleDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            flatItem.ItemId = rdr.GetString(0);
                            flatItem.PlanId = rdr.GetString(1);
                            flatItem.SerialNumber = (rdr.IsDBNull(2)) ? string.Empty : rdr.GetString(2);
                            flatItem.Description = rdr.GetString(3);
                        }
                    }
                }

                connection.Close();
            }

            return flatItem;
        }

        private void Handle(Envelope<ItemRegistered> e)
        {

            Execute(e.Metadata, cmd =>
            {

                cmd.CommandText = InsertSql;
                PopulateParameters(cmd, e.Body.ItemGroupId, e.Body.ItemId, e.Body.Payload);

                cmd.ExecuteNonQuery();
            });
        }

        private void Handle(IEnvelope<ItemReRegistered> e)
        {
            Execute(e.Metadata, cmd =>
            {
                cmd.CommandText = InsertSql;
                PopulateParameters(cmd, e.Body.ItemGroupId, e.Body.ItemId, e.Body.Payload);

                cmd.ExecuteNonQuery();
            });
        }

        private void Handle(IEnvelope<ItemRemoved> e)
        {
            Execute(e.Metadata, cmd =>
            {
                cmd.CommandText = DeleteSql;
                cmd.Parameters.AddHashId(Constants.Parameters.Id, e.Body.ItemGroupId, e.Body.ItemId);

                cmd.ExecuteNonQuery();
            });
        }

        private void Handle(IEnvelope<ItemPayloadChanged> e)
        {
            Execute(e.Metadata, cmd =>
            {
                cmd.CommandText = UpdateItemSql;
                PopulateParameters(cmd, e.Body.ItemGroupId, e.Body.ItemId, e.Body.Payload);

                cmd.ExecuteNonQuery();
            });
        }

    }
}
