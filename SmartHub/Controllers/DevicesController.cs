using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SmartHub.Clients;
using SmartHub.Models;
using SmartHub.Services;

namespace SmartHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly SmartHomeContext _context;
        private readonly IMqttClientService mqttClientService;

        public DevicesController(SmartHomeContext context, MqttClientServiceProvider provider)
        {
            _context = context;
            mqttClientService = provider.MqttClientService;
        }

        // GET: api/Devices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevices()
        {
            return await _context.Devices.ToListAsync();
        }

        // GET: api/Devices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Device>> GetDevice(string id)
        {
            var device = await _context.Devices.FindAsync(id);

            if (device == null)
            {
                return NotFound();
            }

            return device;
        }

        // PUT: api/Devices/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDevice(string id, Device device)
        {
            if (id != device.DeviceId)
            {
                return BadRequest();
            }

            _context.Entry(device).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                DeviceConfig deviceConfig = new DeviceConfig
                {
                    DeviceId = device.DeviceId,
                    DeviceTypeId = device.DeviceTypeId,
                    Pins = device.Pins.Split(',').Select(Int32.Parse).ToList(),
                    Enabled = device.Enabled
                };

                MqttConfigMessage configMessage = new MqttConfigMessage
                {
                    GatewayId = device.GatewayId,
                    DeviceConfigs = new List<DeviceConfig>()
                };
                configMessage.DeviceConfigs.Add(deviceConfig);

                string payload = JsonConvert.SerializeObject(configMessage);
                await mqttClientService.PublishMessageAsync($"{device.GatewayId}/update/config", payload);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Devices
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Device>> PostDevice(Device device)
        {
            _context.Devices.Add(device);
            try
            {
                await _context.SaveChangesAsync();
                DeviceConfig deviceConfig = new DeviceConfig
                {
                    DeviceId = device.DeviceId,
                    DeviceTypeId = device.DeviceTypeId,
                    Pins = device.Pins.Split(',').Select(Int32.Parse).ToList()
                };

                MqttConfigMessage configMessage = new MqttConfigMessage
                {
                    GatewayId = device.GatewayId,
                    DeviceConfigs = new List<DeviceConfig>()
                };
                configMessage.DeviceConfigs.Add(deviceConfig);

                string payload = JsonConvert.SerializeObject(configMessage);
                await mqttClientService.PublishMessageAsync($"{device.GatewayId}/update/config", payload);
            }
            catch (DbUpdateException)
            {
                if (DeviceExists(device.DeviceId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            

            return CreatedAtAction("GetDevice", new { id = device.DeviceId }, device);
        }

        // DELETE: api/Devices/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Device>> DeleteDevice(string id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            DeviceConfig deviceConfig = new DeviceConfig
            {
                DeviceId = device.DeviceId,
                DeviceTypeId = device.DeviceTypeId,
                Delete = true
            };

            MqttConfigMessage configMessage = new MqttConfigMessage
            {
                GatewayId = device.GatewayId,
                DeviceConfigs = new List<DeviceConfig>()
            };
            configMessage.DeviceConfigs.Add(deviceConfig);

            string payload = JsonConvert.SerializeObject(configMessage);
            await mqttClientService.PublishMessageAsync($"{device.GatewayId}/update/config", payload);

            return device;
        }

        private bool DeviceExists(string id)
        {
            return _context.Devices.Any(e => e.DeviceId == id);
        }
    }
}
