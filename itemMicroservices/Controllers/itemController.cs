using itemMicroservices.Model;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace itemMicroservices.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class itemController : ControllerBase
    {
        private readonly SqlConnection _connection;



        public itemController(IConfiguration config) =>
            //this a db instance
            _connection = new SqlConnection(config.GetConnectionString("str"));

        [HttpGet]
        public async Task<JsonResult> Get()
        {
            List<ItemRecords> list = new List<ItemRecords>();
            using (SqlConnection connection = _connection)
            {
                connection.Open();
                string fetch = "GetAllItemRecords";
                using (SqlCommand command = new SqlCommand(fetch, connection))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        ItemRecords itemrecords = new ItemRecords()
                        {
                            ItemCode = (int)reader.GetInt64(0),
                            ItemName = reader.GetString(1),
                            BuyingPrice = (double)reader.GetDecimal(2),
                            SellingPrice = (double)reader.GetDecimal(3),
                            Terminus = reader.GetString(5)

                        };
                        list.Add(itemrecords);
                    }
                }
            }
            JsonResult result = new JsonResult(list);
            return result;
        }

        [HttpPost]
        public void Post([FromBody] ItemRecords value)
        {

            using (SqlConnection connection = _connection)
            {
                connection.Open();
               string fetch = "InsertItemRecords";
                using (SqlCommand command = new SqlCommand(fetch, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                   command.Parameters.AddWithValue("@ItemCode", SqlDbType.Int).Value = value.ItemCode;
                    command.Parameters.AddWithValue("@ItemName", SqlDbType.NVarChar).Value = value.ItemName;
                    command.Parameters.AddWithValue("@BuyingPrice", SqlDbType.Money).Value = value.BuyingPrice;
                    command.Parameters.AddWithValue("@SellingPrice", SqlDbType.Money).Value = value.SellingPrice;
                    command.Parameters.AddWithValue("@Terminus", SqlDbType.NVarChar).Value = value.Terminus;


                    command.ExecuteReader();

               }


                

            }
        }
           
        

        

        // PUT api/<itemController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<itemController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
