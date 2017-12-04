using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Sockets;
using System;

namespace GlassZebra.Controllers
{
    [Route("api/[controller]")]
    public class PingController : Controller
    {
        // GET api/ping
        [HttpGet]
        public string Get()
        {
            return GetLocalIPAddress();
        }

        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

    }
}
