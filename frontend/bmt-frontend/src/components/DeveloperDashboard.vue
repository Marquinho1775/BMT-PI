<template>
  <v-container v-if="role === 'dev'">
    <v-row>
      <v-col cols="12" md="7">
        <v-card class="pa-4 elevation-3">
          <StackedBarChart :datasets="barChartDatasets" :type="1"/>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import StackedBarChart from './StackedBarChart.vue';
import axios from 'axios';
import { API_URL } from '@/main.js';

export default {
  components: {
    StackedBarChart,
  },
  data() {
    return {
      barChartDatasets: [],
      role:''
    };
  },

  async created() {
    const user = JSON.parse(localStorage.getItem('user'));
    this.role = user.role;
    try {
      const datasetResponse = await axios.get(
        `${API_URL}/Developer/GetAllEnterprisesEarnings`);
        const dataFromBackend = datasetResponse.data.data;
        this.barChartDatasets = this.processDatasetResponse(dataFromBackend);
    }
    catch (error) {
      console.error('Error al cargar los datos:', error);
      if (error.response) {
        console.error('Detalles del error:', error.response.data);
      }
    }
  },

  methods: {
    processDatasetResponse(data) {
      const predefinedColors = [
        'rgba(255, 99, 132, 0.6)',
        'rgba(54, 162, 235, 0.6)',
        'rgba(75, 192, 192, 0.6)',
        'rgba(153, 102, 255, 0.6)',
        'rgba(255, 206, 86, 0.6)',
        'rgba(255, 159, 64, 0.6)',
      ];
      const datasets = data.map((item, index) => {
        const color = predefinedColors[index % predefinedColors.length];
        return {
          label: item.productLabel,
          data: item.earningsPerMonth,
          backgroundColor: color,
        };
      });
      return datasets;
    },
  },
}
</script>

<style lang="scss" scoped>

</style>