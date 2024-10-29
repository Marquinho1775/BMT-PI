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

        public bool CreateShoppingCart(string userName)
        {
            string cartQuery = "INSERT INTO ShoppingCarts (UserId, Total) " +
                   "SELECT Id, 0 FROM Users WHERE UserName = @userName";
            SqlCommand cartCommand = new SqlCommand(cartQuery, _conection);
            cartCommand.Parameters.AddWithValue("@userName", userName);
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
            SqlCommand cartProductCommand = new SqlCommand(cartProductQuery, _conection);
            cartProductCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            _conection.Open();
            DataTable cartProductsTable = new DataTable();
            cartProductsTable.Load(cartProductCommand.ExecuteReader());
            _conection.Close();
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

        private CartProductModel GetCartProduct(string shoppingCartId, string productId)
        {
            CartProductModel cartProductModel = null;
            string cartProductQuery = "SELECT ProductId, Quantity, Subtotal " +
                                      "FROM ShoppingCartProducts WHERE ShoppingCartId = @shoppingCartId AND ProductId = @productId";
            SqlCommand cartProductCommand = new SqlCommand(cartProductQuery, _conection);
            cartProductCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            cartProductCommand.Parameters.AddWithValue("@productId", productId);
            _conection.Open();
            DataTable cartProductsTable = new DataTable();
            cartProductsTable.Load(cartProductCommand.ExecuteReader());
            _conection.Close();
            if (cartProductsTable.Rows.Count > 0)
            {
                DataRow row = cartProductsTable.Rows[0];
                cartProductModel = new CartProductModel
                {
                    Product = _productHandler.GetProduct(row["ProductId"].ToString()),
                    Quantity = int.Parse(row["Quantity"].ToString()),
                    Subtotal = double.Parse(row["Subtotal"].ToString())
                };
            }
            return cartProductModel;
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

        public string AddProductToCart(string shoppingCartId, string productId)
        {
            try
            {
                _conection.Open();
                string checkQuery = @"
                    IF EXISTS (
                        SELECT 1 
                        FROM ShoppingCartProducts 
                        WHERE ShoppingCartId = @shoppingCartId 
                          AND ProductId = @productId
                    )
                    SELECT 'ProductExists'
                    ELSE
                    BEGIN
                        -- Obtener el precio del producto
                        DECLARE @productPrice FLOAT = (SELECT Price FROM Products WHERE Id = @productId);
                        DECLARE @subtotal FLOAT = @productPrice;

                        -- Insertar el producto en el carrito
                        INSERT INTO ShoppingCartProducts (ShoppingCartId, ProductId, Quantity, Subtotal)
                        VALUES (@shoppingCartId, @productId, 1, @subtotal);

                        -- Actualizar el total del carrito
                        UPDATE ShoppingCarts 
                        SET Total = Total + @subtotal 
                        WHERE Id = @shoppingCartId;

                        SELECT 'Success' AS Result;
                    END
                ";
                using (SqlCommand command = new SqlCommand(checkQuery, _conection))
                {
                    command.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
                    command.Parameters.AddWithValue("@productId", productId);
                    string result = (string)command.ExecuteScalar();
                    return result;
                }
            }
            catch (Exception ex) {
                return $"Error: {ex.Message}";
            }
            finally
            {
                if (_conection.State == System.Data.ConnectionState.Open)
                {
                    _conection.Close();
                }
            }
        }


        public bool ChangeProductQuantity(string shoppingCartId, string productId, int quantity)
        {
            CartProductModel cartProductModel = GetCartProduct(shoppingCartId, productId);
            double subtotal = cartProductModel.Product.Price * quantity;
            double difference = subtotal - cartProductModel.Subtotal;
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
            string updateTotalQuery = "UPDATE ShoppingCarts SET Total = Total + @difference " +
                                    "WHERE Id = @shoppingCartId";
            SqlCommand updateTotalCommand = new SqlCommand(updateTotalQuery, _conection);
            updateTotalCommand.Parameters.AddWithValue("@difference", difference);
            updateTotalCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            _conection.Open();
            rowsAffected += updateTotalCommand.ExecuteNonQuery();
            _conection.Close();
            return rowsAffected > 0;
        }

        public bool DeleteProductFromCart(string shoppingCartId, string productId)
        {
            CartProductModel cartProduct = GetCartProduct(shoppingCartId, productId);
            double substractTotal = cartProduct.Subtotal;
            string cartProductQuery = "DELETE FROM ShoppingCartProducts " +
                                      "WHERE ShoppingCartId = @shoppingCartId AND ProductId = @productId";
            SqlCommand cartProductCommand = new SqlCommand(cartProductQuery, _conection);
            cartProductCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            cartProductCommand.Parameters.AddWithValue("@productId", productId);
            _conection.Open();
            int rowsAffected = cartProductCommand.ExecuteNonQuery();
            _conection.Close();

            string substractQuery = "UPDATE ShoppingCarts SET Total = Total - @substractTotal " +
                            "WHERE Id = @shoppingCartId";
            SqlCommand substractCommand = new SqlCommand(substractQuery, _conection);
            substractCommand.Parameters.AddWithValue("@substractTotal", substractTotal);
            substractCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            _conection.Open();
            rowsAffected += substractCommand.ExecuteNonQuery();
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
            string clearTotalQuery = "UPDATE ShoppingCarts SET Total = 0 " +
                                    "WHERE Id = @shoppingCartId";
            SqlCommand clearTotalCommand = new SqlCommand(clearTotalQuery, _conection);
            clearTotalCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            _conection.Open();
            rowsAffected += clearTotalCommand.ExecuteNonQuery();
            _conection.Close();
            return rowsAffected > 0;
        }
    }
}
