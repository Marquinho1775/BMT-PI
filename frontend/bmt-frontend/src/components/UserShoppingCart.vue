<template>
  <v-main>
    <v-card flat>
      <v-card-title class="d-flex align-center pe-2">
        <v-icon icon="mdi-cart"></v-icon> &nbsp;
        Tu Carrito de Compras

        <v-spacer></v-spacer>

        <v-text-field v-model="search" density="compact" label="Buscar" prepend-inner-icon="mdi-magnify"
          variant="solo-filled" flat hide-details single-line></v-text-field>
      </v-card-title>

      <v-divider></v-divider>

      <v-data-table v-model:search="search" :items="cartProducts" :headers="headers" class="elevation-1">
        <!-- Template para la columna de imagen -->
        <template #[`item.image`]="{ item }">
          <v-card class="my-2" elevation="2" rounded>
            <v-img :src="item.image" height="64" cover></v-img>
          </v-card>
        </template>

        <!-- Template para la columna de calificación -->
        <template #[`item.rating`]="{ item }">
          <v-rating :model-value="item.product.rating" color="orange-darken-2" density="compact" size="small"
            readonly></v-rating>
        </template>

        <!-- Template para la columna de stock -->
        <template #[`item.stock`]="{ item }">
          <div class="text-end">
            <v-chip :color="item.product.stock ? 'green' : 'red'" :text="item.product.stock ? 'En stock' : 'Sin stock'"
              class="text-uppercase" size="small" label></v-chip>
          </div>
        </template>

        <!-- Template para la columna de cantidad -->
        <template #[`item.quantity`]="{ item }">
          <div class="d-flex align-center">
            <v-text-field v-model.number="item.newQuantity" type="number" min="1" hide-details dense solo
              style="width: 60px;"></v-text-field>
            <v-btn icon x-small color="green" @click="updateQuantity(item)" class="ml-1"
              v-if="item.newQuantity !== item.quantity" :disabled="item.newQuantity < 1">
              <v-icon x-small>mdi-check</v-icon>
            </v-btn>
          </div>
        </template>

        <!-- Template para la columna de acciones -->
        <template #[`item.actions`]="{ item }">
          <v-btn icon x-small color="red" @click="confirmDelete(item)">
            <v-icon x-small>mdi-delete</v-icon>
          </v-btn>
        </template>
      </v-data-table>

      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="primary" @click="clearCart">Vaciar Carrito</v-btn>
        <v-btn color="primary" @click="checkOut">Pagar</v-btn>
      </v-card-actions>

      <!-- Diálogo de confirmación para eliminación -->
      <v-dialog v-model="dialog" max-width="500">
        <v-card>
          <v-card-title class="headline">
            Confirmar Eliminación
          </v-card-title>
          <v-card-text>
            ¿Estás seguro de que deseas eliminar este producto de tu carrito?
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue darken-1" text @click="dialog = false">
              Cancelar
            </v-btn>
            <v-btn color="blue darken-1" text @click="deleteProduct">
              Confirmar
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-card>
  </v-main>
</template>

<script>
import axios from 'axios';
import { API_URL, URL } from '@/main';

export default {
  data() {
    return {
      search: '',
      userId: '',
      shoppingCartId: '',
      cartProducts: [],
      headers: [
        { text: 'Imagen', value: 'image', sortable: false },
        { text: 'Producto', value: 'product.name' },
        { text: 'Precio', value: 'product.price', align: 'right' },
        { text: 'Cantidad', value: 'quantity', align: 'center' },
        { text: 'Subtotal', value: 'subtotal', align: 'right' },
        { text: 'Acciones', value: 'actions', sortable: false, align: 'center' },
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
    checkOut() {
      this.$router.push('/checkout');
    },
    fetchShoppingCart() {
      axios
        .get(`${API_URL}/ShoppingCart`, { params: { userId: this.userId } })
        .then((response) => {
          const shoppingCart = response.data;
          this.shoppingCartId = shoppingCart.id;
          console.log('Carrito de compras:', shoppingCart);
          this.cartProducts = shoppingCart.cartProducts.map((item) => ({
            ...item,
            subtotal: (item.product.price * item.quantity).toFixed(2),
            newQuantity: item.quantity,
            image: item.product.imagesURLs != null ? URL + item.product.imagesURLs[0] : 'https://via.placeholder.com/64',            
          }));
        })
        .catch((error) => {
          console.error('Error al obtener el carrito de compras:', error);
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
          console.error('Error al actualizar la cantidad:', error);
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
          console.error('Error al eliminar el producto:', error);
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
          console.error('Error al vaciar el carrito:', error);
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
