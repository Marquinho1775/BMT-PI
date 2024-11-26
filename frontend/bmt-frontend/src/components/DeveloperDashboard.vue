<template>
  <v-main class="flex-grow-1">
    <!-- Pantalla de Carga -->
    <div v-if="isLoading" class="d-flex justify-center align-center" style="height: 100vh">
      <v-progress-circular
        indeterminate
        color="primary"
        size="64"
      ></v-progress-circular>
    </div>
    
    <!-- Dashboard Principal -->
    <div v-else>
      <h1 class="text-h4 py-4 px-4 mb-4">Dashboard de Desarrollador</h1>

      <!-- Contenedor de los Gráficos -->
      <v-container>
        <!-- Sección de Selección de Gráfico -->
        <v-row class="mb-6">
          <v-col cols="12" md="4">
            <v-select
              :items="chartOptionsList"
              item-text="text"
              item-value="value"
              label="Selecciona el gráfico"
              v-model="selectedChart"
              outlined
            ></v-select>
          </v-col>
        </v-row>

        <!-- Gráfico Seleccionado con Transición -->
        <v-row>
          <!-- Columna para el Gráfico -->
          <v-col cols="12" md="7">
            <transition name="fade" mode="out-in">
              <StackedBarChart
                v-if="selectedChart === 'Ganancias Mensuales'"
                key="Ganancias Mensuales"
                :datasets="barChartDatasets"
                :type="0"
                :labels="[
                  'Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun',
                  'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'
                ]"
              />
              <StackedBarChart
                v-else-if="selectedChart === 'Ganancias Semanales'"
                key="Ganancias Semanales"
                :datasets="weeklyBarChartDatasets"
                :type="1"
                :labels="['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']"
              />
            </transition>
          </v-col>

          <!-- Columna para PendingOrders (Fija, Fuera de la Transición) -->
          <v-col cols="12" md="5">
            <PendingOrders />
          </v-col>
        </v-row>

        <!-- Gráfico LineChart  -->
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
import axios from 'axios';
import { API_URL } from '@/main.js';
import StackedBarChart from './StackedBarChart.vue';
import LineChart from './LineChart.vue';
import PendingOrders from './PendingOrders.vue';

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
      weeklyBarChartDatasets: [],
      LineDataset: [],
      selectedChart: 'Ganancias Mensuales', // Valor por defecto
      chartOptionsList: [
        'Ganancias Mensuales',
        'Ganancias Semanales',
      ],
    };
  },
  async created() {
    try {
      // Realizar todas las solicitudes de datos en paralelo
      const [datasetResponse1, datasetResponse2, weeklyDatasetResponse] = await Promise.all([
        axios.get(`${API_URL}/Developer/GetAllEnterprisesEarnings`),
        axios.get(`${API_URL}/Developer/GetSystemTotalDeliverysFee`),
        axios.get(`${API_URL}/Developer/GetSystemWeeklyEarnings`), // Nueva llamada a la API para ganancias semanales
      ]);

      // Procesar ganancias mensuales
      const dataFromBackend = datasetResponse1.data.data;
      this.barChartDatasets = this.processDatasetResponse(dataFromBackend);

      // Procesar ganancias semanales
      const weeklyDataFromBackend = weeklyDatasetResponse.data.data;
      this.weeklyBarChartDatasets = this.processWeeklyDatasetResponse(weeklyDataFromBackend);

      // Procesar LineDataset
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
    processWeeklyDatasetResponse(data) {
      const predefinedColors = [
        'rgba(75, 192, 192, 0.6)',
        'rgba(153, 102, 255, 0.6)',
        'rgba(255, 159, 64, 0.6)',
        'rgba(54, 162, 235, 0.6)',
        'rgba(255, 206, 86, 0.6)',
        'rgba(255, 99, 132, 0.6)',
        'rgba(201, 203, 207, 0.6)',
      ];
      const datasets = data.map((item, index) => {
        const color = predefinedColors[index % predefinedColors.length];
        return {
          label: item.label,
          data: item.earningsPerMonth, // Tratar como earningsPerDay
          backgroundColor: color,
        };
      });
      return datasets;
    },
  },
  computed: {
    hasDataForSelectedChart() {
      if (this.selectedChart === 'Ganancias Mensuales') {
        return this.barChartDatasets.length > 0;
      } else if (this.selectedChart === 'Ganancias Semanales') {
        return this.weeklyBarChartDatasets.length > 0;
      }
      return false;
    },
  },
};
</script>

<style scoped>
.v-card {
  border-radius: 12px;
}
.v-btn {
  text-transform: none;
}
.v-list-item-title {
  font-weight: 500;
}
.v-divider {
  margin: 16px 0;
}
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.5s;
}
.fade-enter,
.fade-leave-to /* .fade-leave-active en versiones anteriores de Vue */ {
  opacity: 0;
}
</style>
