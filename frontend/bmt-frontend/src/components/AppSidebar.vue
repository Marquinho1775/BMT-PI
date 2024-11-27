<template>
  <v-navigation-drawer v-if="isLoggedIn" app color="#39517B" expand-on-hover rail width="320">
    <v-list>
      <v-list-item :prepend-avatar="this.userImage" :subtitle="this.email" :title="this.fullName"></v-list-item>
    </v-list>
    <v-divider></v-divider>
    <v-list density="compact" nav>
      <v-list-item @click="handleProfileInfo" prepend-icon="mdi-account" title="Mi Perfil"></v-list-item>
      <v-list-item-group v-if="userRole === 'cli' || userRole === 'emp'">
        <v-list-item @click="handleOrders" prepend-icon="mdi-newspaper" title="Mis pedidos"></v-list-item>
        <v-list-item @click="handleReports" prepend-icon="mdi-chart-bar" title="Mis reportes"></v-list-item>
        <v-list-item-group v-if="userRole === 'emp'">
          <v-list-item @click="handleEntrepreneurEnterprises" prepend-icon="mdi-domain"
            title="Mis Emprendimientos">
          </v-list-item>
          <v-list-item @click="GoToHomePage" prepend-icon="mdi-atlassian"
            title="Comprar">
          </v-list-item>
        </v-list-item-group>
        <v-list-item @click="handleEnterpriseRegister" prepend-icon="mdi-domain-plus"
          title="Registrar Emprendimiento"></v-list-item>
      </v-list-item-group>
      <v-list-item-group v-if="userRole === 'dev'">
        <v-list-item @click="goToEnterprises" prepend-icon="mdi-domain" title="Emprendimientos"></v-list-item>
        <v-list-item @click="goToProducts" prepend-icon="mdi-package-variant" title="Productos"></v-list-item>
        <v-list-item @click="goToUsers" prepend-icon="mdi-account-multiple" title="Usuarios"></v-list-item>
        <v-list-item @click="handleReports" prepend-icon="mdi-chart-bar" title="Mis reportes"></v-list-item>
        <v-list-item @click="goToProductConfirmation" prepend-icon="mdi-list-status"
          title="Solicitudes de Pedidos"></v-list-item>
      </v-list-item-group>
    </v-list>
  </v-navigation-drawer>
</template>

<script>
import { URL } from '@/main.js';
export default {
  name: 'AppSidebar',
  data() {
    return {
      userRole: '',
      userImage: '',
      fullName: '',
      email: '',
      isLoggedIn: false,
    };
  },
  mounted() {
    this.getUserInfo();
  },
  methods: {
    getUserInfo() {
      const user = JSON.parse(localStorage.getItem('user')) || {};
      this.isLoggedIn = !!user.username;
      this.fullName = user.name + ' ' + user.lastName || '';
      this.email = user.email || '';
      this.userRole = user.role || '';
      this.userImage = URL + user.profilePictureURL || '';
    },
    handleProfileInfo() {
      this.$router.push('/profile');
    },
    GoToHomePage() {
      this.$router.push('/home');
    },
    handleEntrepreneurEnterprises() {
      this.$router.push('/enterprises');
    },
    handleEnterpriseRegister() {
      this.$router.push('/enterprise-register');
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
    goToProductConfirmation() {
      this.$router.push('/products-confirmation');
    },
    handleOrders() {
      this.$router.push('/orders');
    },
    handleReports() {
      this.$router.push('/reports');
    },
    closeDrawer() {
      this.sidebarDrawer = false;
    },
    handleDeleteAccount() {
      this.$router.push('/userDeleteConfirmation'); // Ruta de la vista
    },
  },
};
</script>