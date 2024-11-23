<template>
  <v-main>
    <v-container>
      <v-row>
        <v-col cols="12">
          <h1>Resultados de la búsqueda</h1>
          <v-card class="mb-4">
            <v-card-text>
              <v-row>
                <v-col cols="12" md="4">
                  <v-select
                    v-model="selectedTags"
                    :items="tags"
                    label="Filtrar por categorías"
                    multiple
                    chips
                  ></v-select>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>

          <h2>Productos</h2>
          <v-container>
            <div v-if="filteredProducts.length > 0">
              <productGrid :products="filteredProducts" />
            </div>
            <div v-else>
              No hay productos que coincidan con la búsqueda
            </div>
          </v-container>

          <h2>Empresas</h2>
          <div v-if="enterprises.length > 0">
            <v-row>
              <v-col
                v-for="enterprise in enterprises"
                :key="enterprise.id"
                cols="12"
                md="4"
              >
                <v-card>
                  <v-card-title>{{ enterprise.name }}</v-card-title>
                  <v-card-text>{{ enterprise.description }}</v-card-text>
                </v-card>
              </v-col>
            </v-row>
          </div>
          <div v-else>
            No hay empresas que coincidan con la búsqueda
          </div>
        </v-col>
      </v-row>
    </v-container>
  </v-main>
</template>

<script>
import axios from 'axios';
import { API_URL } from '@/main.js';

export default {
  data() {
    return {
      searchText: '',
      products: [],
      enterprises: [],
      tags: [],
      selectedTags: [], 
    };
  },

  watch: {
    '$route.params.searchText': {
      async handler() {
        this.searchText = this.$route.params.searchText.replace(/-/g, ' ');
        if (this.searchText && this.searchText.trim()) {
          await Promise.all([
            this.loadSearchResults(),
            this.loadTags()
          ]);
        }
      },
      immediate: true
    }
  },
  
  methods: {
    async loadSearchResults() {
      try {
        const response = await axios.get(API_URL + "/Search?userInput=" + this.searchText);
        this.products = response.data.products;
        this.enterprises = response.data.enterprises;
      }
      catch (error) {
        console.error('Error al cargar los resultados de la búsqueda:', error);
        if (error.response) {
          console.error('Detalles del error:', error.response.data);
        }
      }
    },
    
    async loadTags() {
      try {
        const response = await axios.get(API_URL + '/Tag');
        const array = response.data.data;
        for (let i = 0; i < array.length; i++) {
          this.tags.push(array[i].name);
        }
      } catch (error) {
        console.error('Error fetching tags:', error);
      }
    }
  },

  computed: {
    filteredProducts() {
      if (this.selectedTags.length === 0) {
        return this.products;
      } else {
        return this.products.filter((product) =>
          product.tags.some((tag) => this.selectedTags.includes(tag))
        );
      }
    },
  },
}
</script>