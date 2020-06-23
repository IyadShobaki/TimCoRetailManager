using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;
using Microsoft.Extensions.Configuration;

namespace TRMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IConfiguration _config;

        public InventoryController(IConfiguration config)
        {
            _config = config;
        }
        [Authorize(Roles = "Manager,Admin")] //Manager (or) Admin can get the inventory info
        [HttpGet]
        public List<InventoryModel> Get()
        {
            InventoryData data = new InventoryData(_config);
            return data.GetInventory();
        }
        //if you have Warehouse Worker and you want to make it join mission
        //[Authorize(Roles = "WarehouseWorker")] //Admin (and) WarehouseWorker can add to the inventory
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void Post(InventoryModel item)
        {
            InventoryData data = new InventoryData(_config);
            data.SaveInventoryRecord(item);
        }
    }
}
