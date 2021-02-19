using System;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using InfluxDB.Client.Writes;
using SmartHub.Models;

namespace SmartHub.Clients
{
    public class InfluxDBClient
    {
        private InfluxDB.Client.InfluxDBClient Client;
        private string Bucket;
        private string Org;
        private string Token;

        public InfluxDBClient()
        {
            // You can generate a Token from the "Tokens Tab" in the UI
            Token = "zBEeGlQ0k5icC4zVr87uzys6pNaU9b41SUG7PyGHP1tjJ2vA4lecnNzghpI2ztThNx7DhWGQo_YCK2mV9pPqcA==";
            Bucket = "SmartHub";
            Org = "SmartHub";

            Client = InfluxDBClientFactory.Create("http://localhost:8086", Token.ToCharArray());
        }

        public void WritePoint(DataEntity dataEntity)
        {
            SmartHomeContext context = new SmartHomeContext();
            string unit = context.DeviceTypes.Find(dataEntity.DeviceTypeId).Unit;
            var point = PointData
                .Measurement(unit)
                .Tag("DeviceId", dataEntity.DeviceId)
                .Tag("Enabled", dataEntity.Enabled.ToString())
                .Field("Value", (float)(dataEntity.Value))
                .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

            using (var writeApi = Client.GetWriteApi())
            {
                writeApi.WritePoint(Bucket, Org, point);
            }
        }
        
        public async Task<float> GetLastValue(string deviceId)
        {
            //var query = $"from(bucket: \"SmartHub\") |> range(start: v.timeRangeStart, stop: v.timeRangeStop) |> filter(fn: (r) => r[\"_measurement\"] == \"bit\") |> filter(fn: (r) => r[\"DeviceId\"] == \"1\") |> last()";
            var query = $"from(bucket:\"{Bucket}\") |> range(start: -7d) |> filter(fn: (r) => r.DeviceId == \"{deviceId}\") |> last()";
            var fluxTables = await Client.GetQueryApi().QueryAsync(query, Org);
            double value = Convert.ToDouble(fluxTables[0].Records[0].GetValue());
            return (float)value;
        }
    }
}