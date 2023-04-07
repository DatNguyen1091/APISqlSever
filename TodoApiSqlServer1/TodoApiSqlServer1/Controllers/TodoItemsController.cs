using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Text;
using TodoApiSqlServer1.Models;

namespace TodoApiSqlServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemController : ControllerBase
    {
        private readonly string connectionString = "Server=DATNGUYEN\\SQLEXPRESS;Database=testdb;Integrated Security=True;";

        [HttpGet]
        public List<Itemmodel> GetAllItems()
        {
            List<Itemmodel> itemmodels = new List<Itemmodel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM ProductView", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Itemmodel model = new Itemmodel();
                            model.id = (int)reader["id"];
                            model.product = (string)reader["product"];
                            model.price = (double)reader["price"];
                            itemmodels.Add(model);
                        }
                    }
                }
                connection.Close();
            }
            return itemmodels;
        }

        [HttpGet("{id}")]
        public Itemmodel GetItemsId(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("getproductID", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Itemmodel item = new Itemmodel
                    {
                        id = reader.GetInt32(0),
                        product = reader.GetString(1),
                        price = reader.GetDouble(2),
                    };
                    return item;
                }
                connection.Close();
                return null!;
            }
        }

        [HttpPost]
        public Itemmodel AddItem(Itemmodel model)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("AddProduct", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id", model.id);
                        command.Parameters.AddWithValue("@product", model.product);
                        command.Parameters.AddWithValue("@price", model.price);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return model;
            }
            catch 
            {
                return null!;
            }
        }

        [HttpPut("{id}")]
        public Itemmodel UpdateItem(int id, Itemmodel model)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();              
                using (SqlCommand command = new SqlCommand("UpdateProduct", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@product", model.product);
                    command.Parameters.AddWithValue("@price", model.price); 
                    int rows = command.ExecuteNonQuery();
                    if (rows == 0)
                    {
                        return null!;
                    }
                }
                connection.Close();
            }
            return model;
        }

        [HttpDelete("{id}")]
        public string RemoveItem(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("DeleteProduct", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", id);
                    int rows = command.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        return "Item deleted successfully.";
                    }
                }
                connection.Close();
            }
            return "Failed to delete item.";
        }
    }
}