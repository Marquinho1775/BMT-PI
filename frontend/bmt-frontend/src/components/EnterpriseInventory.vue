<template>
    <v-app class="d-flex flex-column">
      <AppHeader />
      <v-main class="flex-grow-1">
        <v-container>
          <v-card flat>
            <v-card-title class="d-flex align-center pe-2">
              <v-icon icon="mdi-warehouse"></v-icon> &nbsp;
              Inventario de Productos
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
  
            <table class="custom-table">
              <thead>
                <tr>
                  <th>Nombre</th>
                  <th>Tipo</th>
                  <th>Peso</th>
                  <th>Precio</th>
                  <th>Cantidad en Inventario</th>
                  <th>Límite</th>
                  <th>Días Disponibles</th>
                  <th>Tags</th>
                  <th>Imágenes</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(product, index) in filteredProducts" :key="index">
                  <td>{{ product.name }}</td>
                  <td>{{ formatProductType(product.type) }}</td>
                  <td>{{ product.weight }} kg</td>
                  <td>₡{{ product.price }}</td>
                  <td>{{ product.stock }}</td>
                  <td>{{ product.limit }}</td>
                  <td>{{ formatWeekDays(product.weekDaysAvailable) }}</td>
                  <td>
                    <v-chip-group>
                      <v-chip v-for="(tag, i) in product.tags" :key="i" class="ma-1" color="primary" outlined>
                        {{ tag }}
                      </v-chip>
                    </v-chip-group>
                  </td>
                  <td>
                    <v-carousel show-arrows="hover" height="100" width="200" hide-delimiters>
                      <v-carousel-item v-for="(image, i) in product.imagesURLs" :key="i">
                        <v-img :src="image" height="200" width="100"></v-img>
                      </v-carousel-item>
                    </v-carousel>
                  </td>
                </tr>
              </tbody>
            </table>
          </v-card>
        </v-container>
      </v-main>
      <AppFooter />
      <AppSidebar />
    </v-app>
  </template>
  
  
  <script>
  import axios from 'axios';
  import { API_URL, URL } from '@/main.js';
  import { getToken } from '@/helpers/auth';
  
  export default {
    data() {
      return {
        search: '',
        products: [],
      };
    },
    computed: {
      filteredProducts() {
        return this.products.filter((product) => {
          return product.name.toLowerCase().includes(this.search.toLowerCase());
        });
      },
    },
    async created() {
      const enterpriseId = this.$route.params.id;
      try {
        const token = getToken();
        const enterpriseResponse = await axios.get(API_URL + `/Enterprise/${enterpriseId}`, {
          headers: { Authorization: `Bearer ${token}` }
        });
        this.enterprise = enterpriseResponse.data;
        await this.getProducts();
      } catch (error) {
        console.error('Error al cargar la empresa:', error);
        if (error.response) {
          console.error('Detalles del error:', error.response.data);
        }
      }
    },
    methods: {
      async getProducts() {
        try {
          const response = await axios.get(`${API_URL}/Product/${this.enterprise.name}`);
          this.products = response.data;
          this.URLImage();
        } catch (error) {
          console.error('Error al obtener productos:', error);
        }
      },
      URLImage() {
        this.products.forEach(product => {
          if (Array.isArray(product.imagesURLs)) {
            product.imagesURLs = product.imagesURLs.map(image => `${URL}${image}`);
          }
        });
      },
      formatWeekDays(weekDaysString) {
        if (!weekDaysString) {
            return "Disponible todos los días";
        }

        const daysMap = ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"];

        return weekDaysString
            .split("")
            .map(day => daysMap[Number(day.trim())])
            .filter(Boolean)
            .join(", ");
      },
      formatProductType(type) {
        return type === "NonPerishable" ? "No perecedero" : type === "Perishable" ? "Perecedero" : "Desconocido";
      },
    }
  };
  </script>
  
  <style scoped>
  .custom-table {
    width: 100%;
    border-collapse: collapse;
  }
  .custom-table th, .custom-table td {
    padding: 16px;
    border-bottom: 1px solid #e0e0e0;
    text-align: left;
  }
  </style>
  