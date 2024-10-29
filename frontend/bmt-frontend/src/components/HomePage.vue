<template>
  <v-main class="flex-grow-1">
    <v-container>
      <productSearchGrid :products="products" />
    </v-container>
  </v-main>
</template>

<script>
import axios from 'axios';
import { API_URL, URL } from '@/main.js';
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
        const response = await axios.get(`${API_URL}/Product`);
        this.products = response.data;
        this.URLImage();
      } catch (error) {
        console.error('Error fetching products:', error);
      }
    },
    URLImage() {
      this.products.forEach(product => {
        if (Array.isArray(product.imagesURLs)) {
          product.imagesURLs = product.imagesURLs.map(image => `${URL}${image}`);
        } else {
          console.warn(`El producto con ID ${product.id} no tiene una propiedad imagesURLs válida.`);
        }
      });
    },
  },
};
</script>

<style scoped>
.flex-grow-1 {
  flex-grow: 1;
}
</style>
