<template>
  <v-app-bar app color="#9FC9FC" scroll-behavior="hide" elevation="0">

    <v-app-bar-title>
      <v-btn variant="text" @click="handleHome">Business Tracker</v-btn>
    </v-app-bar-title>

    <v-text-field v-model="searchText" bg-color="white" color="black" :loading="loading"
      placeholder="Buscar productos o empresas" hide-details density="compact" variant="outlined"
      style="max-width: 300px;" append-inner-icon="mdi-magnify" @click:append-inner="search" @keyup.enter="search">
    </v-text-field>

    <v-btn v-if="role !== dev" icon color="primary" @click="goToCart">
      <v-icon>mdi-cart</v-icon>
    </v-btn>

    <v-btn v-if="!isLoggedIn" color="primary" @click="handleLogin">Iniciar Sesión</v-btn>
    <v-btn v-if="!isLoggedIn" color="secondary" @click="handleRegister">Registrarse</v-btn>
    <v-btn v-if="isLoggedIn" color="primary" @click="handleLogout">Cerrar Sesión</v-btn>

  </v-app-bar>
</template>

<script>

export default {
  name: 'AppHeader',
  data() {
    return {
      searchText: '',
      loaded: false,
      loading: false,
      role: '',
      isLoggedIn: false,
    };
  },

  mounted() {
    this.getUserStatus();
  },

  methods: {
    async search() {
      this.loading = true;
      try {
        const searchText = this.searchText.replace(/\s/g, '-');
        await this.$router.push(`/search/${searchText}`);
      } finally {
        this.loading = false;
      }
    },

    getUserStatus() {
      const user = JSON.parse(localStorage.getItem('user')) || {};
      this.role = user.role;
      this.isLoggedIn = !!user.username;
    },
    goToCart() {
      this.$router.push('/shopping-cart');
    },
    handleLogin() {
      this.$router.push('/login');
    },
    handleLogout() {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      this.isLoggedIn = false;
      location.reload();
      this.$router.push('/');
    },
    handleRegister() {
      this.$router.push('/register');
    },
    handleHome() {
      this.$router.push('/');
      location.reload();
    },
  },
};
</script>

<style scoped></style>
