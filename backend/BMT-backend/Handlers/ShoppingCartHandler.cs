using BMT_backend.Models;
using System.Data.SqlClient;
using System.Data;

namespace BMT_backend.Handlers
{
    public class ShoppingCartHandler
    {
        private SqlConnection _conection;
        private string _conectionPath;
        private readonly ProductHandler _productHandler;

        public ShoppingCartHandler()
        {
            var builder = WebApplication.CreateBuilder();
            _conectionPath = builder.Configuration.GetConnectionString("BMTContext");
            _conection = new SqlConnection(_conectionPath);
            _productHandler = new ProductHandler();
        }

        private DataTable CreateQueryTable(string query)
        {
            SqlCommand queryCommand = new SqlCommand(query, _conection);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable tableFormatQuery = new DataTable();
            _conection.Open();
            tableAdapter.Fill(tableFormatQuery);
            _conection.Close();
            return tableFormatQuery;
        }

        public bool CreateShoppingCart(string userId)
        {
            string cartQuery = "INSERT INTO ShoppingCarts (UserId, Total) " +
                               "VALUES (@userId, 0)";
            SqlCommand cartCommand = new SqlCommand(cartQuery, _conection);
            cartCommand.Parameters.AddWithValue("@userId", userId);
            _conection.Open();
            int rowsAffected = cartCommand.ExecuteNonQuery();
            _conection.Close();
            return rowsAffected > 0;
        }

        public ShoppingCartModel GetShoppingCart(string userId)
        {
            string cartQuery = "SELECT Id, UserId, Total " +
                               "FROM ShoppingCarts WHERE UserId = @userId";
            SqlCommand cartCommand = new SqlCommand(cartQuery, _conection);
            cartCommand.Parameters.AddWithValue("@userId", userId);
            _conection.Open();
            DataTable cartTable = new DataTable();
            cartTable.Load(cartCommand.ExecuteReader());
            _conection.Close();
            if (cartTable.Rows.Count == 0)
            {
                return null;
            }
            DataRow row = cartTable.Rows[0];
            ShoppingCartModel shoppingCart = new ShoppingCartModel
            {
                Id = row["Id"].ToString(),
                UserId = row["UserId"].ToString(),
                CartProducts = GetCartProducts(row["Id"].ToString()),
                CartTotal = double.Parse(row["Total"].ToString())
            };
            return shoppingCart;
        }

        private List<CartProductModel> GetCartProducts(string shoppingCartId)
        {
            List<CartProductModel> cartProducts = new List<CartProductModel>();
            string cartProductQuery = "SELECT ProductId, Quantity, Subtotal " +
                                      "FROM ShoppingCartProducts WHERE ShoppingCartId = @shoppingCartId";
            DataTable cartProductsTable = CreateQueryTable(cartProductQuery);
            foreach (DataRow row in cartProductsTable.Rows)
            {
                CartProductModel cartProduct = new CartProductModel
                {
                    Product = _productHandler.GetProduct(row["ProductId"].ToString()),
                    Quantity = int.Parse(row["Quantity"].ToString()),
                    Subtotal = double.Parse(row["Subtotal"].ToString())
                };
                cartProducts.Add(cartProduct);
            }
            return cartProducts;
        }

        public string GetCartId(string userId)
        {
            string cartQuery = "SELECT Id FROM ShoppingCarts WHERE UserId = @userId";
            SqlCommand cartCommand = new SqlCommand(cartQuery, _conection);
            cartCommand.Parameters.AddWithValue("@userId", userId);
            _conection.Open();
            string cartId = cartCommand.ExecuteScalar().ToString();
            _conection.Close();
            return cartId;
        }

        public bool AddProductToCart(string shoppingCartId, string productId)
        {
            double productPrice = _productHandler.GetProductPrice(productId);
            double subtotal = productPrice;
            string cartProductQuery = "INSERT INTO ShoppingCartProducts (ShoppingCartId, ProductId, Quantity, Subtotal) " +
                                      "VALUES (@shoppingCartId, @productId, @quantity, @subtotal)";
            SqlCommand cartProductCommand = new SqlCommand(cartProductQuery, _conection);
            cartProductCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            cartProductCommand.Parameters.AddWithValue("@productId", productId);
            cartProductCommand.Parameters.AddWithValue("@quantity", 1);
            cartProductCommand.Parameters.AddWithValue("@subtotal", subtotal);
            _conection.Open();
            int rowsAffected = cartProductCommand.ExecuteNonQuery();
            _conection.Close();

            string cartTotalQuery = "UPDATE ShoppingCarts SET Total = Total + @subtotal " +
                                    "WHERE Id = @shoppingCartId";
            SqlCommand cartTotalCommand = new SqlCommand(cartTotalQuery, _conection);
            cartTotalCommand.Parameters.AddWithValue("@subtotal", subtotal);
            cartTotalCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            _conection.Open();
            rowsAffected += cartTotalCommand.ExecuteNonQuery();
            _conection.Close();
            return rowsAffected > 0;
        }

        public bool ChangeProductQuantity(string shoppingCartId, string productId, int quantity)
        {
            double pruductPrice = _productHandler.GetProductPrice(productId);
            double subtotal = pruductPrice * quantity;
            string cartProductQuery = "UPDATE ShoppingCartProducts SET Quantity = @quantity, Subtotal = @subtotal " +
                                      "WHERE ShoppingCartId = @shoppingCartId AND ProductId = @productId";
            SqlCommand cartProductCommand = new SqlCommand(cartProductQuery, _conection);
            cartProductCommand.Parameters.AddWithValue("@quantity", quantity);
            cartProductCommand.Parameters.AddWithValue("@subtotal", subtotal);
            cartProductCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            cartProductCommand.Parameters.AddWithValue("@productId", productId);
            _conection.Open();
            int rowsAffected = cartProductCommand.ExecuteNonQuery();
            _conection.Close();
            return rowsAffected > 0;
        }

        public bool DeleteProductFromCart(string shoppingCartId, string productId)
        {
            string cartProductQuery = "DELETE FROM ShoppingCartProducts " +
                                      "WHERE ShoppingCartId = @shoppingCartId AND ProductId = @productId";
            SqlCommand cartProductCommand = new SqlCommand(cartProductQuery, _conection);
            cartProductCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            cartProductCommand.Parameters.AddWithValue("@productId", productId);
            _conection.Open();
            int rowsAffected = cartProductCommand.ExecuteNonQuery();
            _conection.Close();
            return rowsAffected > 0;
        }

        public bool ClearShoppingCart(string shoppingCartId)
        {
            string cartProductQuery = "DELETE FROM ShoppingCartProducts " +
                                      "WHERE ShoppingCartId = @shoppingCartId";
            SqlCommand cartProductCommand = new SqlCommand(cartProductQuery, _conection);
            cartProductCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            _conection.Open();
            int rowsAffected = cartProductCommand.ExecuteNonQuery();
            _conection.Close();
            return rowsAffected > 0;
        }
    }
}
