<template>
  <v-app class="d-flex flex-column">
    <AppHeader />
    <v-main>
      <ChatGPT />
      <router-view />
    </v-main>
    <AppFooter />
    <AppSidebar />
  </v-app>
</template>

<script>
import AppHeader from './components/AppHeader.vue';
import AppFooter from './components/AppFooter.vue';
import AppSidebar from './components/AppSidebar.vue';
import ChatGPT from './components/ChatGPT.vue'; // Importar el componente ChatGPT

export default {
  components: {
    AppHeader,
    AppFooter,
    AppSidebar,
    ChatGPT, // Registrar el componente
  },
};
</script>

<style>
/* Puedes añadir estilos globales si es necesario */
</style>
