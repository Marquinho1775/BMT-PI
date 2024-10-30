<template>
  <v-app class="d-flex flex-column">
    <AppHeader />
    <v-main class="flex-grow-1" style="overflow-x: hidden;">
      <h1 class="title1">Pedidos por revisar</h1>
      <template v-if="items && items.length > 0">
        <v-virtual-scroll :items="items" height="100%" style="overflow-x: hidden;">
          <template v-slot:default="{ item }">
            <v-row class="order-card mb-4 p-1 bg-light-grey rounded" justify="space-between">
              <v-col style="padding-left: 5rem">
                <h4>{{ item.userName }}</h4>
                <p>{{ item.direction }} - {{ item.otherSigns }}</p>
                <ul>
                  <li v-for="(products, enterpriseName) in groupProductsByEnterprise(item.products || [])"
                    :key="enterpriseName">
                    <strong>{{ enterpriseName }}</strong>
                    <ul class="product-list">
                      <li v-for="product in products" :key="product.productId"
                        style="padding-bottom: 0.3rem; padding-top: 0.3rem;">
                        <span class="quantity-box">{{ product.quantity }}</span> {{ product.productName }}
                      </li>
                    </ul>
                  </li>
                </ul>
                <p>Peso: {{ item.weight }} kg</p>
                <p>{{ getTotalProductQuantity(item.products || []) }} artículos • Costo: ₡{{ (item.orderCost ??
                  0).toFixed(2) }} + ₡{{ (item.deliveryFee ?? 0).toFixed(2) }} de envío</p>
                <p v-if="item.orderDate">{{ new Date(item.orderDate).toLocaleDateString() }}</p>
                <p v-else>Fecha no disponible</p>
              </v-col>
              <v-col class="d-flex flex-column align-center justify-center" cols="auto">
                <v-btn size="x-large" class="mb-3 custom-btn" :style="{ backgroundColor: '#d0eda0', color: 'black' }"
                  @click="confirmOrder(item.orderId)">
                  Aceptar pedido
                </v-btn>
                <v-btn size="x-large" class="custom-btn" :style="{ backgroundColor: '#9fc9fc', color: 'black' }"
                  @click="denyOrder(item.orderId)">
                  Rechazar pedido
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
import { API_URL } from '@/main.js';

export default {
  data() {
    return {
      orders: []
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
        const response = await axios.get(API_URL + '/Developer/getToConfirmOrders');
        this.orders = response.data;
        console.log(this.orders);
      } catch (error) {
        console.error("Error fetching orders:", error);
      }
    },
    async confirmOrder(orderId) {
      if (confirm("¿Estás seguro de que quieres aceptar este pedido?")) {
        try {
          const response = await axios.put(API_URL + '/Developer/ConfirmOrder', null, {
            params: { orderID: orderId }
          });

          if (response.status === 200) {
            console.log(`Order ${orderId} confirmed successfully`);
            this.orders = this.orders.filter(order => order.orderId !== orderId);
          } else {
            console.error(`Failed to confirm order ${orderId}`);
          }
        } catch (error) {
          console.error("Error confirming order:", error);
        }
      }
    },
    async denyOrder(orderId) {
      if (confirm("¿Estás seguro de que quieres rechazar este pedido?")) {
        try {
          const response = await axios.put(API_URL + '/Developer/DenyOrder', null, {
            params: { orderID: orderId }
          });

          if (response.status === 200) {
            console.log(`Order ${orderId} denied successfully`);
            this.orders = this.orders.filter(order => order.orderId !== orderId);
          } else {
            console.error(`Failed to deny order ${orderId}`);
          }
        } catch (error) {
          console.error("Error denying order:", error);
        }
      }
    },
    getTotalProductQuantity(products) {
      if (!Array.isArray(products)) return 0; // Verificar si `products` es un arreglo válido
      return products.reduce((total, product) => total + product.quantity, 0);
    },
    groupProductsByEnterprise(products) {
      if (!products || !Array.isArray(products)) return {}; // Verifica si `products` es un arreglo válido
      return products.reduce((grouped, product) => {
        (grouped[product.enterpriseName] = grouped[product.enterpriseName] || []).push(product);
        return grouped;
      }, {});
    }
  },
  async mounted() {
    await this.fetchOrders();
  }
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
