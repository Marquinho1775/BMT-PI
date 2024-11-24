<template>
  <v-main class="flex-grow-1">
    <h1 class="text-h4 py-4 px-4 mb-4">Dashboard de Desarrollador</h1>
  <v-container>
    <v-row>
      <v-col cols="12" md="7">
        <v-card class="pa-4 elevation-3">
          <StackedBarChart :datasets="barChartDatasets" :type="1"/>
        </v-card>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12" md="7">
        <v-card class="pa-4 elevation-3">
          <LineChart :dataset="LineDataset"/>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
  </v-main>
</template>

<script>
import StackedBarChart from './StackedBarChart.vue';
import LineChart from './LineChart.vue';
import axios from 'axios';
import { API_URL } from '@/main.js';

export default {
  components: {
    StackedBarChart,
    LineChart,
  },
  data() {
    return {
      barChartDatasets: [],
      LineDataset: []
    };
  },

  async created() {
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

    try {
      const datasetResponse = await axios.get(
        `${API_URL}/Developer/GetSystemTotalDeliverysFee`);
        this.barChartDatasets = datasetResponse.data.data;
        console.log(this.barChartDatasets);
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
          label: item.label,
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