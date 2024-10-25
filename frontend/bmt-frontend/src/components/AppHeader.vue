<template>
  <v-app-bar :elevation="0" app color="#9FC9FC" scroll-behavior="hide" dark>
    <v-toolbar-title>Business Tracker</v-toolbar-title>
    <v-spacer></v-spacer>
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
      isLoggedIn: false,
    };
  },

  mounted() {
    this.getUserStatus();
  },

  methods: {
    getUserStatus() {
      const user = JSON.parse(localStorage.getItem('user')) || {};
      this.isLoggedIn = !!user.username;
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
  },
};
</script>

<style scoped>

</style>
