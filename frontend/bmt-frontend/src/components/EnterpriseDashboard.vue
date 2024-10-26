<template>
  <v-app class="d-flex flex-column">
    <AppHeader/>
    <v-main class="flex-grow-1">
      <v-container>
        <v-card class="pa-5 mb-4">
          <v-row>
            <v-col>
              <h2 class="text-center">{{ enterprise.name }}</h2>
            </v-col>
          </v-row>
          <v-card-text>
            <p><strong>Cédula del emprendimiento:</strong> {{ enterprise.identificationNumber ? formatIdentification(enterprise.identificationNumber) : 'N/A' }}</p>
            <p><strong>Correo empresarial:</strong> {{ enterprise.email || 'N/A' }}</p>
            <p><strong>Número de teléfono:</strong> {{ enterprise.phoneNumber || 'N/A' }}</p>
            <h3>Emprendedores asociados</h3>
            <v-btn append-icon="mdi-plus" variant="outlined" @click="handleInviteEntrepreneur">
              Invitar colaborador 
            </v-btn>
            <div v-if="enterprise.staff && enterprise.staff.length">
              <ul>
                <li v-for="staff in enterprise.staff" :key="staff.id">
                  {{ staff.name }} {{ staff.lastName }}
                </li>
              </ul>
            </div>
          </v-card-text>
          <v-card-title>
            <h3 class="text-left">Productos</h3>
          </v-card-title>
          <v-card-text>
            <v-btn append-icon="mdi-plus" variant="outlined" @click="handleRegisterProduct">
              Registrar nuevo producto
            </v-btn>
            <div v-if="products.length">
              <v-row>
                <v-col cols="12" sm="6" md="4" v-for="product in paginatedProducts" :key="product.id">
                  <v-card class="product-card">
                    <v-carousel show-arrows="hover" height="200px" hide-delimiters>
                      <v-carousel-item v-for="(image, index) in product.imagesURLs" :key="index">
                        <v-img :src="image" height="200px" aspect-ratio="16/9" cover></v-img>
                      </v-carousel-item>
                    </v-carousel>
                    <v-card-title>{{ product.name }}</v-card-title>
                    <v-card-subtitle>Precio: {{ product.price }} colones</v-card-subtitle>
                    <v-card-text>
                      <p>{{ product.description }}</p>
                      <p><strong>Peso:</strong> {{ product.weight }} kg</p>
                      <!-- Display Tags if Available -->
                      <div v-if="product.tags && product.tags.length">
                        <p><strong>Tags:</strong></p>
                        <v-chip-group>
                          <v-chip
                            v-for="(tag, index) in product.tags"
                            :key="index"
                            class="ma-1"
                            color="primary"
                            outlined>
                            {{ tag }}
                          </v-chip>
                        </v-chip-group>
                      </div>
                    </v-card-text>
                  </v-card>
                </v-col>
              </v-row>
              <div class="pagination-container">
                <v-btn icon @click="prevPage" :disabled="currentPage === 1">
                  <v-icon>mdi-chevron-left</v-icon>
                </v-btn>
                <span>Página {{ currentPage }} de {{ totalPages }}</span>
                <v-btn icon @click="nextPage" :disabled="currentPage === totalPages">
                  <v-icon>mdi-chevron-right</v-icon>
                </v-btn>
              </div>
            </div>
          </v-card-text>
        </v-card>
      </v-container>
    </v-main>
    <AppFooter/>
    <AppSidebar/>
  </v-app>
</template>

<script>
import axios from 'axios';
import { API_URL, URL } from '@/main.js';
import { getToken } from '@/helpers/auth';

export default {
  data() {
    return {
      enterprise: {},
      products: [],
      currentPage: 1,
      productsPerPage: 3
    };
  },
  computed: {
    paginatedProducts() {
      const start = (this.currentPage - 1) * this.productsPerPage;
      const end = start + this.productsPerPage;
      return this.products.slice(start, end);
    },
    totalPages() {
      return Math.ceil(this.products.length / this.productsPerPage);
    }
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
    formatIdentification(identification) {
      if (identification && identification.length === 9) {
        return `${identification.slice(0, 1)}-${identification.slice(1, 5)}-${identification.slice(5)}`;
      }
      return identification || 'N/A';
    },
    goBack() {
      this.$router.push('/enterprises');
    },
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
        } else {
          console.warn(`El producto con ID ${product.id} no tiene una propiedad imagesURLs válida.`);
        }
      });
    },
    nextPage() {
      if (this.currentPage < this.totalPages) {
        this.currentPage += 1;
      }
    },
    prevPage() {
      if (this.currentPage > 1) {
        this.currentPage -= 1;
      }
    },
    handleInviteEntrepreneur() {
      this.$router.push(`/enterprise/${this.enterprise.id}/invite`);
    },
    handleRegisterProduct() {
      this.$router.push(`${this.enterprise.id}/new-product`);
    }
  }
};
</script>

<style scoped>
.v-app-bar {
  background-color: #9FC9FC;
}

.v-footer {
  height: 50px;
  background-color: #9FC9FC;
}

.v-card {
  background-color: #A9C5FF;
}

.text-center {
  text-align: center;
}

ul {
  padding-left: 20px;
  list-style-type: disc;
  margin-top: 10px;
}

.product-card {
  background-color: #f5f5f5;
  margin-bottom: 20px;
  padding: 10px;
  border-radius: 8px;
}

.pagination-container {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-top: 20px;
}

span {
  margin: 0 10px;
}

.ma-1 {
  margin: 4px;
}
</style>
