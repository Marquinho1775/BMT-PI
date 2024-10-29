<template>
  <v-main>
    <v-card flat>
      <v-card-title>
        <v-icon left>mdi-cart</v-icon>
        Your Shopping Cart
      </v-card-title>
      <v-data-table :headers="headers" :items="cartProducts" class="elevation-1">

        <!-- Template for the quantity column -->
        <template #[`item.quantity`]="{ item }">
          <div class="d-flex align-center">
            <v-text-field v-model.number="item.newQuantity" type="number" min="1" hide-details dense solo
              style="width: 50px;"></v-text-field>
            <!-- Confirmation button that only appears when there are changes -->
            <v-btn icon x-small color="green" @click="updateQuantity(item)" class="ml-1"
              v-if="item.newQuantity !== item.quantity" :disabled="item.newQuantity < 1">
              <v-icon x-small>mdi-check</v-icon>
            </v-btn>
          </div>
        </template>

        <!-- Template for the actions column -->
        <template #[`item.actions`]="{ item }">
          <v-btn icon x-small color="red" @click="confirmDelete(item)">
            <v-icon x-small>mdi-delete</v-icon>
          </v-btn>
        </template>
      </v-data-table>

      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="primary" @click="clearCart">Clear Cart</v-btn>
        <v-btn color="primary">Checkout</v-btn>
      </v-card-actions>

      <!-- Confirmation dialog for deletion -->
      <v-dialog v-model="dialog" max-width="500">
        <v-card>
          <v-card-title class="headline">
            Confirm Deletion
          </v-card-title>
          <v-card-text>
            Are you sure you want to remove this product from your cart?
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue darken-1" text @click="dialog = false">
              Cancel
            </v-btn>
            <v-btn color="blue darken-1" text @click="deleteProduct">
              Confirm
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-card>
  </v-main>
</template>

<script>
import axios from 'axios';
import { API_URL } from '@/main';

export default {
  data() {
    return {
      userId: '',
      shoppingCartId: '',
      cartProducts: [],
      headers: [
        { text: 'Product', value: 'product.name' },
        { text: 'Price', value: 'product.price', align: 'right' },
        { text: 'Quantity', value: 'quantity', align: 'center' },
        { text: 'Subtotal', value: 'subtotal', align: 'right' },
        { text: 'Actions', value: 'actions', sortable: false, align: 'center' },
      ],
      dialog: false,
      productToDelete: null,
    };
  },
  created() {
    const user = JSON.parse(localStorage.getItem('user')) || {};
    this.userId = user.id;
    this.fetchShoppingCart();
  },
  methods: {
    fetchShoppingCart() {
      axios
        .get(`${API_URL}/ShoppingCart`, { params: { userId: this.userId } })
        .then((response) => {
          const shoppingCart = response.data;
          this.shoppingCartId = shoppingCart.id;
          this.cartProducts = shoppingCart.cartProducts.map((item) => ({
            ...item,
            subtotal: item.subtotal.toFixed(2),
            newQuantity: item.quantity,
          }));
        })
        .catch((error) => {
          console.error('Error fetching shopping cart', error);
        });
    },
    updateQuantity(item) {
      if (item.newQuantity < 1) {
        item.newQuantity = 1;
      }
      axios
        .put(`${API_URL}/ShoppingCart/ChangeProductQuantity`, null, {
          params: {
            shoppingCartId: this.shoppingCartId,
            productId: item.product.id,
            quantity: item.newQuantity,
          },
        })
        .then(() => {
          item.quantity = item.newQuantity;
          item.subtotal = (item.product.price * item.quantity).toFixed(2);
        })
        .catch((error) => {
          console.error('Error updating quantity:', error);
        });
    },
    confirmDelete(item) {
      this.productToDelete = item;
      this.dialog = true;
    },
    deleteProduct() {
      axios
        .delete(`${API_URL}/ShoppingCart/DeleteProductFromCart`, {
          params: {
            shoppingCartId: this.shoppingCartId,
            productId: this.productToDelete.product.id,
          },
        })
        .then(() => {
          const index = this.cartProducts.indexOf(this.productToDelete);
          if (index > -1) {
            this.cartProducts.splice(index, 1);
          }
          this.dialog = false;
          this.productToDelete = null;
        })
        .catch((error) => {
          console.error('Error deleting product:', error);
        });
    },
    clearCart() {
      axios
        .delete(`${API_URL}/ShoppingCart/ClearShoppingCart`, {
          params: { shoppingCartId: this.shoppingCartId },
          headers: { Accept: 'text/plain' },
        })
        .then(() => {
          this.cartProducts = [];
        })
        .catch((error) => {
          console.error('Error clearing cart:', error);
        });
    },
  },
};
</script>

<style scoped>
.v-data-table th {
  text-align: center;
}

.ml-1 {
  margin-left: 4px;
}
</style>
