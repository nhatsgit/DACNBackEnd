using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public AddressController(IWebHostEnvironment env)
        {
            _env = env;
        }
        [HttpGet("GetDistrictsByProvince")]
        public IActionResult GetDistricts(string provinceId)
        {
            var districts = ReadJsonFile<List<District>>("json/districts.json");
            var listShow = districts.Where(p => p.parent_code == provinceId).ToList();
            return Ok(listShow);
        }
        [HttpGet("GetProvinces")]
        public IActionResult GetProvinces()
        {
            var provinces = ReadJsonFile<List<Province>>("json/provinces.json");
            return Ok(provinces);
        }
        [HttpGet("GetWardsByDistrict")]

        public IActionResult GetWards(string districtId)
        {

            var wards = ReadJsonFile<List<Ward>>("json/wards.json");

            var listShow = wards.Where(p => p.parent_code == districtId).ToList();


            return Ok(listShow);
        }

        private T ReadJsonFile<T>(string filePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath, filePath);
            var jsonData = System.IO.File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
    public class Province
    {
        public string _id { get; set; }

        public string name_with_type { get; set; }
        public string code { get; set; }
    }

    public class District
    {
        public string _id { get; set; }
        public string name_with_type { get; set; }

        public string code { get; set; }
        public string parent_code { get; set; }
    }

    public class Ward
    {
        public string _id { get; set; }
        public string name_with_type { get; set; }
        public string path_with_type { get; set; }

        public string code { get; set; }
        public string parent_code { get; set; }
    }
}
