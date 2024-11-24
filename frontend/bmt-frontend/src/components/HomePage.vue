<template>
  <v-main class="flex-grow-1">
    <v-container>
      <productGrid :products="products" />
    </v-container>
    <v-container>

    </v-container>


  </v-main>
</template>

<script>
import axios from 'axios';
import { API_URL} from '@/main.js';
export default {
  data() {
    return {
      products: [],
    };
  },
  mounted() {
    this.getProducts();
  },
  methods: {
    async getProducts() {
      try {
        const response = await axios.get(`${API_URL}/Product/GetProductsDetails`);
        this.products = response.data.data;
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
