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
                            model.quantity = (int)reader["quantity"];
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
                        quantity = reader.GetInt32(0)
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
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand("AddProduct", connection, transaction))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@product", model.product);
                        command.Parameters.AddWithValue("@price", model.price);
                        command.Parameters.AddWithValue("@quantity", model.quantity);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    connection.Close();
                    return model;
                }
                catch
                {
                    transaction.Rollback();
                    connection.Close();
                    return null!;
                }
            }
        }

        [HttpPut("{id}")]
        public Itemmodel UpdateItem(int id, Itemmodel model)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand command = new SqlCommand("UpdateProduct", connection, transaction))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@id", id);
                            command.Parameters.AddWithValue("@product", model.product);
                            command.Parameters.AddWithValue("@price", model.price);
                            command.Parameters.AddWithValue("@quantity", model.quantity);
                            int rows = command.ExecuteNonQuery();
                            if (rows == 0)
                            {
                                transaction.Rollback();
                                return null!;
                            }
                        }
                        transaction.Commit();
                        return model;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return null!;
                    }
                }
            }
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