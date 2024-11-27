<template>
  <v-main class="flex-grow-1">
    <productGrid :products="products"/>
  </v-main>
</template>

<script>
import axios from 'axios';
import { getToken } from '@/helpers/auth';
import { API_URL} from '@/main.js';

export default {
  data() {
    return {
      isUserLoggedIn: false,
      userRole: '',
      products: [],
    }
  },
  created() {
    this.isUserLoggedIn = getToken();
    if (this.isUserLoggedIn) {
      const user = JSON.parse(localStorage.getItem('user'));
      this.userRole = user.role;
      if (this.userRole === 'emp') {
        this.goToEnterprises();
      } else if (this.userRole === 'dev') {
        this.gotToDeveloperDashboard();
      } else if (this.userRole === 'cli') {
        console.log('Cliente');
        this.goToHomePage();
      }
    } else {
      this.getProducts();
    }
  },
  methods: {
    goToEnterprises() {
      this.$router.push('/enterprises');
    },
    gotToDeveloperDashboard() {
      this.$router.push('/developer-dashboard');
    },
    goToHomePage() {
      this.$router.push('/home');
    },
    async getProducts() {
      try {
        const response = await axios.get(`${API_URL}/Product/GetProductsDetails`);
        this.products = response.data.data;
      } catch (error) {
        console.error('Error fetching products:', error);
      }
    },
  }
}
</script>

<style lang="scss" scoped>

</style>