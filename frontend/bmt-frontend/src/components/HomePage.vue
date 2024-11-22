<template>
  <v-main class="flex-grow-1">
    <UserDashboard v-if="isUserLoggedIn" />
    <productSearchGrid :products="products" v-else />
  </v-main>
</template>

<script>
import { getToken } from '@/helpers/auth';
import axios from 'axios';
import { API_URL } from '@/main.js';

export default {
  data() {
    return {
      isUserLoggedIn: false,
      products: [],
    };
  },
  created() {
    this.isUserLoggedIn = getToken();
    if (!this.isUserLoggedIn) {
      this.getProducts();
    }
  },
  methods: {
    async getProducts() {
      try {
        const response = await axios.get(`${API_URL}/Product/GetProductsDetails`);
        this.products = response.data.data;
        this.URLImage();
      } catch (error) {
        console.error('Error fetching products:', error);
      }
    },
  },
};
</script>

<style scoped>
.flex-grow-1 {
  flex-grow: 1;
}
</style>
