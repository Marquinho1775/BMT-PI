<template>
  <v-navigation-drawer v-model="localSidebarDrawer" app color="#39517B">
    <v-list dense>
      <v-list-item-group v-if="userRole === 'cli'">
        <v-list-item @click="handleProfileInfo">
          <v-list-item-title>Mis Datos</v-list-item-title>
        </v-list-item>
        <v-list-item @click="handleRegisterEnterprise">
          <v-list-item-title>Registrar Emprendimiento</v-list-item-title>
        </v-list-item>
      </v-list-item-group>
      <v-list-item-group v-if="userRole === 'emp'">
        <v-list-item @click="handleProfileInfo">
          <v-list-item-title>Mi perfil</v-list-item-title>
        </v-list-item>
        <v-list-item @click="handleEntrepreneurEnterprises">
          <v-list-item-title>Mis emprendimientos</v-list-item-title>
        </v-list-item>
        <v-list-item @click="handleCollaboratorRegister">
          <v-list-item-title>Registrar Colaborador</v-list-item-title>
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
</template>

<script>
export default {
  name: 'AppSidebar',
  props: {
    userRole: {
      type: String,
      required: true,
    },
    sidebarDrawer: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      localSidebarDrawer: this.sidebarDrawer,
    };
  },
  watch: {
    sidebarDrawer(val) {
      this.localSidebarDrawer = val;
    },
    localSidebarDrawer(val) {
      this.$emit('update:sidebarDrawer', val);
    },
  },
  methods: {
    handleProfileInfo() {
      this.$router.push('/profile');
      this.closeDrawer();
    },
    handleRegisterEnterprise() {
      this.$router.push('/enterprise-register');
      this.closeDrawer();
    },
    handleEntrepreneurEnterprises() {
      this.$router.push('/entrepreneur-enterprises');
      this.closeDrawer();
    },
    handleCollaboratorRegister() {
      this.$router.push('/collaborator-register');
      this.closeDrawer();
    },
    handleProductRegister() {
      this.$router.push('/product');
      this.closeDrawer();
    },
    goToEnterprises() {
      this.$router.push('/developer-enterprises');
      this.closeDrawer();
    },
    goToProducts() {
      this.$router.push('/developer-products');
      this.closeDrawer();
    },
    goToUsers() {
      this.$router.push('/developer-users');
      this.closeDrawer();
    },
    closeDrawer() {
      this.localSidebarDrawer = false;
    },
  },
};
</script>

<style scoped>
</style>
