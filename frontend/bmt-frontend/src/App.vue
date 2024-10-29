<template>
  <v-app class="d-flex flex-column">
    <AppHeader />
    <router-view />
    <AppFooter />
    <AppSidebar />
  </v-app>
</template>

<script>
import AppHeader from './components/AppHeader.vue';
import AppFooter from './components/AppFooter.vue';
import AppSidebar from './components/AppSidebar.vue';

export default {
  components: {
    AppHeader,
    AppFooter,
    AppSidebar,
  },
};
</script>
