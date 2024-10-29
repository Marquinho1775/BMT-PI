<template>
  <v-app class="d-flex flex-column">
    <AppHeader />
    <v-main class="flex-grow-1" style="overflow-x: hidden;">
      <h1 class="title1">Pedidos por revisar</h1>
      <template v-if="items && items.length > 0">
      <v-virtual-scroll :items="items" height="100%">
        <template v-slot:default="{ item }">
          <v-row class="order-card mb-4 p-1 bg-light-grey rounded" justify="space-between">
            <v-col style="padding-left: 5rem">
              <h4>{{ item.UserName }}</h4>
              <p>{{ item.Direction }} - {{ item.OtherSigns }}</p>
              <ul>
                <li v-for="(products, enterpriseName) in groupProductsByEnterprise(item.Products)" :key="enterpriseName">
                  <strong>{{ enterpriseName }}</strong>
                  <ul class="product-list">
                    <li v-for="product in products" :key="product.ProductId" style="padding-bottom: 0.3rem; padding-top: 0.3rem;">
                      <span class="quantity-box">{{ product.Quantity }}</span> {{ product.ProductName }}
                    </li>
                  </ul>
                </li>
              </ul>
              <p>Peso: {{ item.Weight }} kg</p>
              <p>{{getTotalProductQuantity(item.Products)}} artículos • Costo: ₡{{ item.OrderCost.toFixed(2) }} + ₡{{ item.DeliveryFee.toFixed(2) }} de envío</p>
              <p>{{ item.OrderDate.toLocaleDateString() }}</p>
            </v-col>
            <v-col class="d-flex flex-column align-center justify-center" cols="auto">
              <v-btn size="x-large" class="mb-3 custom-btn" :style="{ backgroundColor: '#d0eda0', color: 'black' }" @click="confirmOrder(item.OrderId)">
                Aceptar pedido
              </v-btn>
              <v-btn size="x-large" class="custom-btn" :style="{ backgroundColor: '#9fc9fc', color: 'black' }" @click="denyOrder(item.OrderId)">
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
        const response = await axios.post(API_URL + '/Developer/getToConfirmOrders');
        this.orders = response.data;
      } catch (error) {
        console.error("Error fetching orders:", error);
      }
    },
    async confirmOrder(orderId) {
      try {
        const response = await axios.post(API_URL + '/Developer/ConfirmOrder', null, {
          params: { orderID: orderId }
        });
        
        if (response.status === 200) {
          console.log('Order ${orderId} confirmed successfully');
          this.orders = this.orders.filter(order => order.OrderId !== orderId);
        } else {
          console.error('Failed to confirm order ${orderId}');
        }
      } catch (error) {
        console.error("Error confirming order:", error);
      }
    },
    getTotalProductQuantity(products) {
      return products.reduce((total, product) => total + product.Quantity, 0);
    }
  },
  async denyOrder(orderId) {
    try {
      const response = await axios.post(API_URL + '/Developer/DenyOrder', null, {
        params: { orderID: orderId }
      });
      
      if (response.status === 200) {
        console.log('Order ${orderId} denied successfully');
        this.orders = this.orders.filter(order => order.OrderId !== orderId);
      } else {
        console.error('Failed to deny order ${orderId}');
      }
    } catch (error) {
      console.error("Error denying order:", error);
    }
  },
  getTotalProductQuantity(products) {
    return products.reduce((total, product) => total + product.Quantity, 0);
  },
  groupProductsByEnterprise(products) {
    return products.reduce((grouped, product) => {
      (grouped[product.EnterpriseName] = grouped[product.EnterpriseName] || []).push(product);
      return grouped;
    }, {});
  },
  async mounted() {
    await this.fetchOrders();
  }
};
</script>

<style scoped>
.order-card {
  border-bottom: 2px solid #ffffff;
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
  margin: 0; /* Elimina el margen por defecto */
  padding: 0.1rem 0; /* Ajusta el padding para un espaciado más pequeño */
}

.order-card h4 {
  font-weight: 700;
}
</style>
