<template>
  <v-navigation-drawer v-if="isLoggedIn" app color="#39517B" expand-on-hover rail>
    <v-list>
      <v-list-item
        :prepend-avatar="this.userImage"
        :subtitle="this.email"
        :title="this.fullName"
      ></v-list-item>
    </v-list>
    <v-divider></v-divider>
    <v-list density="compact" nav>
      <v-list-item-group v-if="userRole === 'cli' || userRole==='emp'">
        <v-list-item @click="handleProfileInfo" prepend-icon="mdi-account" title="Mis Perfil"></v-list-item>
        <v-list-item-group v-if="userRole === 'emp'">
          <v-list-item @click="handleEntrepreneurEnterprises" prepend-icon="mdi-domain" title="Mis Emprendimientos"></v-list-item>
        </v-list-item-group>
        <v-list-item @click="handleEnterpriseRegister" prepend-icon="mdi-domain-plus" title="Registrar Emprendimiento"></v-list-item>
      </v-list-item-group>
      <v-list-item-group v-if="userRole === 'dev'">
        <v-list-item @click="goToEnterprises" prepend-icon="mdi-domain" title="Emprendimientos"></v-list-item>
        <v-list-item @click="goToProducts" prepend-icon="mdi-package-variant" title="Productos"></v-list-item>ø
        <v-list-item @click="goToUsers" prepend-icon="mdi-account-multiple" title="Usuarios"
        ></v-list-item>
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
    closeDrawer() {
      this.sidebarDrawer = false;
    },
  },
};
</script>