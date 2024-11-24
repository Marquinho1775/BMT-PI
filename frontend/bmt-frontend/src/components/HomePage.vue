<template>
  <v-main class="flex-grow-1"> 
    <UserDashboard v-if="isUserLoggedIn" />
    <productGrid :products="products" v-else/>
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
      uerRole: '',
      products: [],
    };
  },
  created() {
    this.isUserLoggedIn = getToken();
    if (!this.isUserLoggedIn) {
      this.getProducts();
    } else {
      const user = JSON.parse(localStorage.getItem('user'));
      this.userRole = user.role;
      if (this.userRole === 'emp') {
        this.goToEnterprises();
      } else if (this.userRole === 'dev') {
        this.gotToDeveloperDashboard();
      }
    }
  },
  methods: {
    goToEnterprises() {
      this.$router.push('/enterprises');
    },
    gotToDeveloperDashboard() {
      this.$router.push('/developer-dashboard');
    },
    async getProducts() {
      try {
        const response = await axios.get(`${API_URL}/Product/GetProductsDetails`);
        this.products = response.data.data;
      } catch (error) {
        console.error('Error fetching products:', error);
      }
    },
  },
};
</script>

<style scoped>
.flex-grow-1 {
  flex-grow: 1;
}
</style>
