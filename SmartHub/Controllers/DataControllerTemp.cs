using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartHub.Models;
using SmartHub.Clients;
using Newtonsoft.Json;
using SmartHub.Services;

namespace SmartHub.Controllers
{
    public class DataControllerTemp : Controller
    {
        private readonly IMqttClientService mqttClientService;
        private readonly SmartHomeContext _context;
        private InfluxDBClient influxDBClient;

        public IActionResult Index()
        {
            return View();
        }

        public DataControllerTemp(SmartHomeContext context, MqttClientServiceProvider provider)
        {
            _context = context;
            influxDBClient = new InfluxDBClient();
            mqttClientService = provider.MqttClientService;
        }


        [HttpPost]
        [Route("api/BodyTypes/JsonStringBody")]
        public string JsonStringBody([FromBody] string content)
        {
            return content;
        }

        [HttpPost]
        [Route("api/PostDataEntity")]
        public async Task PostDataEntity([FromBody] DataEntity dataEntity)
        {
            var test = dataEntity;
            dataEntity.Value = (float)Math.Round(dataEntity.Value);
            var device = _context.Devices.Find(dataEntity.DeviceId);
            var newDevice = device;
            _context.Entry(device).CurrentValues.SetValues(newDevice);
            await _context.SaveChangesAsync();
            influxDBClient.WritePoint(dataEntity);

            MqttDataMessage mqttMessage = new MqttDataMessage
            {
                GatewayId = _context.Devices.Find(dataEntity.DeviceId).GatewayId,
                DataEntities = new List<DataEntity>()
            };
            mqttMessage.DataEntities.Add(dataEntity);
            string payload = JsonConvert.SerializeObject(mqttMessage);
            await mqttClientService.PublishMessageAsync($"{mqttMessage.GatewayId}/update/data", payload);
        }

        [HttpGet]
        [Route("api/GetLastValue/{deviceId}")]
        public async Task<float> GetLastValue(string deviceId)
        {
            return await influxDBClient.GetLastValue(deviceId);

        }
    }
}
