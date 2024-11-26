<template>
  <v-main>
    <h1>Sumario de las empresas del sistema</h1>
    <div class="reports-container">
      <reports-table 
        v-if="enterprises.length" 
        :titles="tableTitles" 
        :reports="enterprises"
      />
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
        'Administrador',
        'Descripción',
        'Email',
        'Cantidad de empleados',
        'Número de teléfono',
        'Productos registrados',
      ],
    };
  },
  methods: {
    async getEnterprises() {
      try {
        const response = await axios.get(API_URL + '/Developer/getEnterprises');
        const responseData = response.data;
        this.enterprises = responseData.map(item => ({
          name: item.name,
          administrator: item.administrator,
          description: item.description,
          email: item.email,
          employeeQuantity: item.employeeQuantity,
          phoneNumber: item.phoneNumber,
          productQuantity: item.productQuantity,
        }));
      } catch (error) {
        console.error('Error fetching enterprises:', error);
      }
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
