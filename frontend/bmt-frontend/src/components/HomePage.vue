<template>
  <v-app class="d-flex flex-column">

    <!-- HEADER: App Bar -->
    <v-app-bar :elevation="10" app color="#9FC9FC" scroll-behavior="hide" dark>
      <v-btn icon @click="togglesidebarDrawer">
        <v-icon>mdi-menu</v-icon>
      </v-btn>
      <v-toolbar-title>Business Tracker</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn v-if="!isLoggedIn" color="primary" @click="handleLogin">Iniciar Sesión</v-btn>
      <v-btn v-if="!isLoggedIn" color="secondary" @click="handleRegister">Registrarse</v-btn>
      <v-btn v-if="isLoggedIn" color="primary" @click="handleLogout">Cerrar Sesión</v-btn>
    </v-app-bar>

    <!-- SIDEBAR: Navigation Drawer -->
    <AppSidebar v-model:sidebarDrawer="sidebarDrawer" :user-role="userRole" />

    <!-- MAIN CONTENT: Mostrar productos en cards -->
    <v-main class="flex-grow-1">
      <v-container>
        <div v-if="!isLoggedIn" class="text-center">
          <h2>Bienvenido a Business Tracker</h2>
        </div>
        <div v-if="isLoggedIn">
          <h2>Bienvenido a Business Tracker</h2>
        </div>

        <!-- Productos -->
        <product-search-grid :products="products" />
      </v-container>
    </v-main>

    <!-- FOOTER -->
    <v-footer app padless color="#9FC9FC" dark>
      <v-col class="text-center white--text">
        &copy; 2024 Business Tracker. Todos los derechos reservados.
      </v-col>
    </v-footer>
  </v-app>
</template>

<script>
import axios from 'axios';
import { API_URL, URL } from '@/main.js';

export default {
  data() {
    return {
      username: '',
      userRole: '',
      isLoggedIn: false,
      sidebarDrawer: false,
      products: [],
    };
  },
  mounted() {
    this.getRole();
    this.getProducts();
  },
  methods: {
    togglesidebarDrawer() {
      this.sidebarDrawer = !this.sidebarDrawer;
    },
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
      console.log('Productos con URLs actualizadas:', this.products);
    },
    handleLogin() {
      this.$router.push('/login');
    },
    handleLogout() {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      this.isLoggedIn = false;
      this.$router.push('/');
    },
    handleRegister() {
      this.$router.push('/register');
    },
    getRole() {
      const user = JSON.parse(localStorage.getItem('user')) || {};
      this.username = user.username || '';
      this.userRole = user.role || '';
      this.isLoggedIn = !!this.username;
    },
  },
};
</script>

<style scoped>
.v-application {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.v-footer {
  height: 50px;
  background-color: #9FC9FC;
}

.flex-grow-1 {
  flex-grow: 1;
}

.v-card {
  margin-bottom: 16px;
}
</style>
