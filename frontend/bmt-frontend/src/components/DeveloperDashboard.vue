<template>
  <v-main class="flex-grow-1">
    <div v-if="isLoading" class="d-flex justify-center align-center" style="height: 100vh">
      <v-progress-circular
        indeterminate
        color="primary"
        size="64"
      ></v-progress-circular>
    </div>
    <div v-else>
      <h1 class="text-h4 py-4 px-4 mb-4">Dashboard de Desarrollador</h1>
      <v-container>
        <v-row>
          <v-col cols="12" md="7">
              <StackedBarChart :datasets="barChartDatasets" :type="1"/>
          </v-col>
          <v-col cols="12" md="5">
            <PendingOrders />
        </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="7">
              <LineChart :dataset="LineDataset"/>
          </v-col>
        </v-row>
      </v-container>
    </div>
  </v-main>
</template>

<script>
import StackedBarChart from './StackedBarChart.vue';
import LineChart from './LineChart.vue';
import PendingOrders from './PendingOrders.vue';
import axios from 'axios';
import { API_URL } from '@/main.js';

export default {
  components: {
    StackedBarChart,
    LineChart,
    PendingOrders,
  },
  
  data() {
    return {
      isLoading: true,
      barChartDatasets: [],
      LineDataset: []
    };
  },

  async created() {
    try {
      const [datasetResponse1, datasetResponse2] = await Promise.all([
        axios.get(`${API_URL}/Developer/GetAllEnterprisesEarnings`),
        axios.get(`${API_URL}/Developer/GetSystemTotalDeliverysFee`)
      ]);

      const dataFromBackend = datasetResponse1.data.data;
      this.barChartDatasets = this.processDatasetResponse(dataFromBackend);

      const response = datasetResponse2.data.data;
      const array = [];
      for (const key in response) {
        array.push(response[key]);
      }
      this.LineDataset = array;
    } catch (error) {
      console.error('Error al cargar los datos:', error);
      if (error.response) {
        console.error('Detalles del error:', error.response.data);
      }
    } finally {
      this.isLoading = false;
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
