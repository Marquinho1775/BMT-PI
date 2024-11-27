<template>
  <v-main>
    <h1>Sumario de los usuarios del sistema</h1>
    <div class="reports-container">
      <reports-table v-if="enterprises.length" :titles="tableTitles" :keys="tableKeys" :reports="enterprises" />
        </div>
  </v-main>
</template>

<script>
import axios from 'axios';
import { API_URL } from '@/main.js';

export default {
  data() {
    return {
      enterprises: [],
      tableTitles: [
        'Nombre',
        'Nombre de usuario',
        'Email',
        'Rol',
        'Empresas asociadas',
      ],
      tableKeys: [
        'name',
        'username',
        'email',
        'role',
        'associatedCompanies',
      ],
    };
  },
  methods: {
    async getEnterprises() {
      try {
        const response = await axios.get(API_URL + '/Developer/getUsers');
        const responseData = response.data;
        this.enterprises = responseData.map(item => ({
          name: item.name,
          username: item.username,
          email: item.email,
          role: item.role === 'cli' ? 'Cliente' : item.role === 'emp' ? 'Emprendedor' : 'Desarrollador',
          associatedCompanies: item.associatedCompanies.join(', ') || 'Sin empresas asociadas',
        }));
      } catch (error) {
        console.error('Error fetching enterprises:', error);
      }
    },
    goToMainPage() {
      this.$router.push('/developer-home');
    },
  },
  mounted() {
    this.getEnterprises();
  }
};
</script>

<style scoped>
h1 {
  margin: 20px;
  padding: 20px;
}

.reports-container {
  margin: 20px;
  padding: 20px;
  border-radius: 8px;
}
</style>
