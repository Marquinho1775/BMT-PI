<template>
  <v-main class="flex-grow-1">
    <v-container>
      <productSearchGrid :products="products" />
    </v-container>
    <v-container>
      <reports-table :titles="['Nombre', 'Edad', 'Ciudad']" :reports="[
        ['Juan', 30, 'Madrid'],
        ['Ana', 25, 'Barcelona'],
        ['Luis', 28, 'Valencia']
      ]" />
    </v-container>

    <pending-orders-reports />

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
        const response = await axios.get(`${API_URL}/Product/GetProductsDetails`);
        this.products = response.data.data;
        console.log('Products:', this.products);
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
