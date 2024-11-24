<template>
  <v-main class="flex-grow-1">
    <v-container>
      <v-card flat>
        <v-card-title class="d-flex align-center pe-2">
          <v-icon icon="mdi-cart"></v-icon> &nbsp;
          Tu Carrito de Compras

          <v-spacer></v-spacer>

          <v-text-field
            v-model="search"
            density="compact"
            label="Buscar"
            prepend-inner-icon="mdi-magnify"
            variant="solo-filled"
            flat
            hide-details
            single-line
          ></v-text-field>
        </v-card-title>

        <v-divider></v-divider>

        <!-- Tabla personalizada -->
        <table class="custom-table">
          <thead>
            <tr>
              <th>Imagen</th>
              <th>Producto</th>
              <th class="text-right">Precio</th>
              <th class="text-center">Cantidad</th>
              <th class="text-right">Subtotal</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, index) in filteredCartProducts" :key="index">
              <td>
                <v-card class="my-2" elevation="2" rounded>
                  <v-img :src="item.image" height="64" cover></v-img>
                </v-card>
              </td>
              <td>{{ item.product.name }}</td>
              <td class="text-right">₡{{ item.product.price }}</td>
              <td class="text-center">
                <div class="d-flex align-center justify-center">
                  <v-text-field
                    v-model.number="item.newQuantity"
                    type="number"
                    min="1"
                    hide-details
                    dense
                    solo
                    style="width: 60px;"
                  ></v-text-field>
                  <v-btn
                    icon
                    x-small
                    color="green"
                    @click="updateQuantity(item)"
                    class="ml-1"
                    v-if="item.newQuantity !== item.quantity"
                    :disabled="item.newQuantity < 1"
                  >
                    <v-icon x-small>mdi-check</v-icon>
                  </v-btn>
                </div>
              </td>
              <td class="text-right">₡{{ item.subtotal }}</td>
              <td>
                <v-btn icon x-small color="red" @click="confirmDelete(item)">
                  <v-icon x-small>mdi-delete</v-icon>
                </v-btn>
              </td>
            </tr>
          </tbody>
        </table>

        <v-divider></v-divider>
        <v-card-subtitle class="text-h6">Total: ₡{{ cartTotal }}</v-card-subtitle>

        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="primary" @click="clearCart">Vaciar Carrito</v-btn>
          <v-btn color="primary" @click="checkOut">Pagar</v-btn>
        </v-card-actions>

        <!-- Diálogo de confirmación para eliminación -->
        <v-dialog v-model="dialog" max-width="500">
          <v-card>
            <v-card-title class="headline">Confirmar Eliminación</v-card-title>
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
    </v-container>
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
      cartTotal: 0,
      dialog: false,
      productToDelete: null,
    };
  },
  computed: {
    filteredCartProducts() {
      return this.cartProducts.filter((item) => {
        return item.product.name.toLowerCase().includes(this.search.toLowerCase());
      });
    },
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
            image:
              item.product.imagesURLs != null && item.product.imagesURLs.length > 0
                ? URL + item.product.imagesURLs[0]
                : 'https://via.placeholder.com/64',
          }));
          this.cartTotal = shoppingCart.cartTotal.toFixed(2);
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
.custom-table {
  width: 100%;
  border-collapse: collapse;
}
.custom-table th,
.custom-table td {
  padding: 16px;
  border-bottom: 1px solid #e0e0e0;
}
.custom-table th {
  text-align: left;
}
.text-right {
  text-align: right;
}
.text-center {
  text-align: center;
}
.ml-1 {
  margin-left: 4px;
}
</style>
