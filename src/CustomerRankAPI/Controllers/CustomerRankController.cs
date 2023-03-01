using CustomerRankAPI.Service;
using Microsoft.AspNetCore.Mvc;

namespace CustomerRankAPI.Controllers
{
    [ApiController]
    public class CustomerRankController : ControllerBase
    {
        private readonly IRankService _service;
        public CustomerRankController(IRankService rankService)
        {
            _service = rankService;
        }      

        [HttpPost("customer/{customerid}/score/{score}")]
        public IActionResult UpdateScore(long customerid, int score)
        {
            if (customerid <= 0)
            {
                return BadRequest("customer id error.");
            }
            if (score == 0)
            {
                return BadRequest("score cannot be equal to 0.");
            }
            if (_service.UpdateCustomerScore(customerid, score))
            {
                return new OkObjectResult("success");
            }
            else
            {
                return BadRequest("update failed,please retry.");
            }
        }

        [HttpGet("leaderboard")]
        public IActionResult GetCustomersByRank(int start, int end)
        {
            var ret = _service.GetCustomersByRank(start, end);
            return Ok(ret);
        }

        [HttpGet("leaderboard/{customerid}")]
        public IActionResult GetCustomersByScore(long customerid, int high, int low)
        {
            var ret = _service.GetCustomerNearRank(customerid, high, low);
            return Ok(ret);
        }
    }
}
