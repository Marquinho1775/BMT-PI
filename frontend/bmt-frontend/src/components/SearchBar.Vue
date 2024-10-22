<template>
  <div class="test_container">
    <v-container fluid class="searchbar-container">
      <v-row no-gutters align="center">
        <v-col cols="8">
          <v-text-field
            class="search-field"
            v-model="searchCriteria.searchText"
            :loading="loading"
            append-inner-icon="mdi-magnify"
            label="Buscar producto..."
            variant="solo"
            hide-details
            single-line
            @click:append-inner="Search"
          ></v-text-field>
        </v-col>
        <v-col cols="4">
          <v-select
            class="category-select"
            v-model="searchCriteria.categories"
            :items="categories"
            label="Categorías"
            multiple
            chips
          ></v-select>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>


<script>
  import axios from 'axios';
  import { API_URL } from '@/main.js';

  export default {
    data: () => ({
      searchCriteria: {
        searchText: '',
        categories: [],
      },
      showCategories: false,
      categories: [],
      loaded: false,
      loading: false,
    }),

    methods: {
      Search() {
        this.showSpinner()
        // llamar a backend, crear nueva pagina con los resultados, la nueva pagina renderiza el componente global de grid de productos
      },
      ShowCategories() {
        this.showCategories = !this.showCategories
      },
      showSpinner() {
        this.loading = true
        setTimeout(() => {
          this.loading = false
        }, 2000)
      },
    },
    async created() {
      try {
        const response = await axios.get(API_URL + '/Product/get-tags');
        this.categories = response.data;
      } catch (error) {
        console.error('Error al obtener las categorias:', error);
      }
    },
  }
</script>

<style scoped>
.searchbar-container {
  width: 100%;
}

.test_container {
  display: flex;
  justify-content: center;
  max-width: 600px;
  margin: 0 auto;
}

.search-field,
.category-select {
  width: 100%;
  height: 40px;
}

.v-input__control {
  height: 100%;
}

.search-field {
  margin-right: 4px;
}

.category-select {
  margin-left: 4px;
}

</style>
