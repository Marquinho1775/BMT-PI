<template>
  <v-app class="d-flex flex-column">
    <AppHeader />
    <v-main class="flex-grow-1" style="overflow-x: hidden;">
      <h1 class="title1">Pedidos pendientes</h1>
      <template v-if="items && items.length > 0">
        <v-virtual-scroll :items="items" height="100%" style="overflow-x: hidden">
          <template v-slot:default="{ item }">
            <v-row class="order-card mb-4 p-1 bg-light-grey rounded" justify="space-between">
              <v-col style="padding-left: 5rem;">
                <ul class="order-list" v-if="item.products && item.products.length > 0">
                  <li v-for="(products, enterpriseName) in groupProductsByEnterprise(item.products)"
                    :key="enterpriseName">
                    <h4 class="enterprise-name" style="padding-bottom: 0.5rem;">
                      {{ enterpriseName }}</h4>
                    <ul class="product-list">
                      <li v-for="product in products" :key="product.productId"
                        style="padding-bottom: 0.3rem; padding-top: 0.3rem;">
                        <span class="quantity-box">{{ product.quantity }}</span>
                        {{ product.productName }}
                      </li>
                    </ul>
                  </li>
                </ul>
                <p>Peso: {{ item.order.weight }} kg</p>
                <p>{{ getTotalProductQuantity(item.products || []) }} artículos
                  • Costo: ₡{{ (item.order.orderCost ??
                    0).toFixed(2) }} + ₡{{ (item.order.deliveryFee ?? 0).toFixed(2) }}
                  de envío</p>
                <p v-if="item.order.orderDate"> Fecha de creación del pedido: {{ new
                  Date(item.order.orderDate).toLocaleDateString() }}</p>
                <p v-if="item.order.deliveryDate"> Fecha de entrega: {{ new
                  Date(item.order.deliveryDate).toLocaleDateString() }}</p>
              </v-col>
              <v-col class="d-flex flex-column align-center justify-center" cols="auto">
                <v-btn size="x-large" class="custom-btn" :style="{ backgroundColor: '#9fc9fc', color: 'black' }"
                  @click="denyOrder(item.order.orderId)">
                  Cancelar pedido
                </v-btn>
              </v-col>
            </v-row>
          </template>
        </v-virtual-scroll>
      </template>
    </v-main>
    <AppFooter />
    <AppSidebar />
  </v-app>
</template>

<script>
import axios from 'axios';
import { getUser } from '@/helpers/auth';
import { API_URL } from '@/main.js';

export default {
  data() {
    return {
      orders: [],
    };
  },
  computed: {
    items() {
      return this.orders;
    }
  },
  methods: {
    async fetchOrders() {
      try {
        const response = await axios.get(API_URL + "/User/GetToConfirmOrders", {
          params: { userId: getUser().id },
        });
        this.orders = response.data.data;
      } catch (error) {
        console.error("Error fetching orders:", error);
      }
    },
    async denyOrder(orderId) {
      this.$swal.fire({
        title: '¿Cancelar pedido?',
        text: "¿Estás seguro de que quieres cancelar este pedido?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d0eda0',
        cancelButtonColor: '#9fc9fc',
        confirmButtonText: 'Cancelar pedido',
        cancelButtonText: 'Volver'
      }).then(async (result) => {
        if (result.isConfirmed) {
          try {
            const response = await axios.put(`${API_URL}/User/DenyOrder`, null, {
              params: { orderID: orderId },
            });

            if (response.data.success) {
              this.$swal.fire('Cancelado', `El pedido ${orderId} ha sido cancelado.`, 'success');
              this.orders = this.orders.filter((order) => order.order.orderId !== orderId);
            } else {
              this.$swal.fire('Error', `No se pudo cancelar el pedido: ${response.data.Message}`, 'error');
            }
          } catch (error) {
            this.$swal.fire('Error', 'Ocurrió un problema al cancelar el pedido.', 'error');
          }
        }
      });
    },
    getTotalProductQuantity(products) {
      if (!Array.isArray(products)) return 0;
      return products.reduce((total, product) => total + product.quantity, 0);
    },
    groupProductsByEnterprise(products) {
      if (!products || !Array.isArray(products)) return {};
      return products.reduce((grouped, product) => {
        (grouped[product.enterpriseName] = grouped[product.enterpriseName] || []).push(product);
        return grouped;
      }, {});
    },
  },
  async mounted() {
    await this.fetchOrders();
  },
};
</script>

<style scoped>
.order-card {
  border-bottom: 2px solid #ebebeb;
}

.custom-btn {
  min-width: 100%;
  margin-right: 80%;
  font-weight: 700;
}

.product-list {
  padding-left: 1rem;
  list-style-type: none;
  padding-bottom: 0.7rem;
}

.order-list {
  padding-left: 0;
  list-style-type: none;
}

.enterprise-name {
  font-weight: bold;
  margin: 0;
}

.quantity-box {
  display: inline-block;
  width: 24px;
  height: 24px;
  line-height: 24px;
  text-align: center;
  border: 1px solid #ccc;
  border-radius: 4px;
  margin-right: 8px;
  font-weight: bold;
  background-color: #f9f9f9;
}

.title1 {
  text-align: left;
  padding: 1rem;
  padding-left: 2rem;
  font-weight: 700;
}

.order-card p,
.order-card h4 {
  margin: 0;
  padding: 0.1rem 0;
}

.order-card h4 {
  font-weight: 700;
}
</style>
