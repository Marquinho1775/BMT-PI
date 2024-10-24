<template>
  <v-app class="d-flex flex-column">
    <!-- HEADER: App Bar -->
    <v-app-bar :elevation="10" app color="#9FC9FC" scroll-behavior="hide" dark>
      <v-btn icon @click="menuDrawer = !menuDrawer">
        <v-icon>mdi-menu</v-icon>
      </v-btn>
      <v-toolbar-title>Business Tracker</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn v-if="!isLoggedIn" color="primary" @click="handleLogin">Iniciar Sesión</v-btn>
      <v-btn v-if="!isLoggedIn" color="secondary" @click="handleRegister">Registrarse</v-btn>
      <v-btn v-if="isLoggedIn" color="primary" @click="handleLogout">Cerrar Sesión</v-btn>
    </v-app-bar>

    <!-- SIDEBAR: Navigation Drawer -->
    <v-navigation-drawer v-model="menuDrawer" app color="#39517B">
      <v-list dense>
        <v-list-item-group v-if="userRole === 'cli'">
          <v-list-item @click="handleProfileInfo">
            <v-list-item-title>Mis Datos</v-list-item-title>
          </v-list-item>
          <!-- <v-list-item @click="handleRegisterEntrepreneur">
            <v-list-item-title>Registrarme como emprendedor</v-list-item-title>
          </v-list-item> -->
          <v-list-item @click="handleRegisterEnterprise">
            <v-list-item-title>Registrarme Emprendimiento</v-list-item-title>
          </v-list-item>
        </v-list-item-group>
        <v-list-item-group v-if="userRole === 'emp'">
          <v-list-item @click="handleEntrepreneurInvitation">
            <v-list-item-title>Invitar Colaborador</v-list-item-title>
          </v-list-item>
          <v-list-item @click="handleRegisterEnterprise">
            <v-list-item-title>Registrar Emprendimiento</v-list-item-title>
          </v-list-item>
          <v-list-item @click="handleProductRegister">
            <v-list-item-title>Registrar Producto</v-list-item-title>
          </v-list-item>
        </v-list-item-group>
        <v-list-item-group v-if="userRole === 'dev'">
          <v-list-item @click="goToEnterprises">
            <v-list-item-title>Emprendimientos</v-list-item-title>
          </v-list-item>
          <v-list-item @click="goToProducts">
            <v-list-item-title>Productos</v-list-item-title>
          </v-list-item>
          <v-list-item @click="goToUsers">
            <v-list-item-title>Usuarios</v-list-item-title>
          </v-list-item>
        </v-list-item-group>
      </v-list>
    </v-navigation-drawer>

    <!-- MAIN CONTENT: Mostrar productos en cards -->
    <v-main class="flex-grow-1">
      <v-container>
        <div>
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
      menuDrawer: false,
      products: [],
    };
  },
  mounted() {
    this.getRole();
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
    handleProfileInfo() {
      this.$router.push('/profile');
    },
    handleEntrepreneurInvitation() {
      this.$router.push('/entrepreneur-invitation');
    },
    handleRegisterEnterprise() {
      this.$router.push('/enterprise-register');
    },
    handleProductRegister() {
      this.$router.push('/product');
    },
    goToEnterprises() {
      this.$router.push('/developer-enterprises');
    },
    goToProducts() {
      this.$router.push('/developer-products');
    },
    goToUsers() {
      this.$router.push('/developer-users');
    },
    getRole() {
      const user = JSON.parse(localStorage.getItem('user')) || {};
      this.username = user.username || '';
      this.userRole = user.role || '';
      this.isLoggedIn = !!this.username;
    }
  }
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