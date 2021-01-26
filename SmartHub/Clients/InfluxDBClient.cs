using System;
using SmartHub.Models;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using InfluxDB.Client.Writes;
using System.Threading.Tasks;

namespace SmartHub.Clients
{
    public class InfluxDBClient
    {
        private static readonly char[] Token = "".ToCharArray();

        public InfluxDBClient()
        { 
        }

        public async Task WritePoint(DataEntity dataEntity)
        {
            var influxDBClient = InfluxDBClientFactory.Create("http://localhost:8086");

            //
            // Write Data
            //
            using (var writeApi = influxDBClient.GetWriteApi())
            {
                //
                // Write by Point
                //
                var point = PointData.Measurement(dataEntity.Unit)
                    .Tag("deviceid", dataEntity.DeviceID.ToString())
                    .Tag("gatewayid", dataEntity.GatewayID.ToString())
                    .Field("value", dataEntity.Value)
                    .Timestamp(DateTime.UtcNow.AddSeconds(-10), WritePrecision.Ns);

                writeApi.WritePoint("smarthome", "org_id", point);
            }

            

            influxDBClient.Dispose();
        }

        public async Task<DataEntity> WritePoint()
        {
            var influxDBClient = InfluxDBClientFactory.Create("http://localhost:8086");
            DataEntity dataEntity = new DataEntity();
            //
            // Query data
            //
            var flux = "from(bucket:\"temperature-sensors\") |> range(start: 0)";

            var fluxTables = await influxDBClient.GetQueryApi().QueryAsync(flux, "org_id");
            fluxTables.ForEach(fluxTable =>
            {
                var fluxRecords = fluxTable.Records;
                fluxRecords.ForEach(fluxRecord =>
                {
                    
                });
            });
            influxDBClient.Dispose();
            return dataEntity;
        } 
    }
}
