<template>
  <v-main class="flex-grow-1">
    <v-container>
      <v-card class="pa-6 mb-6 elevation-3">
        <v-row>
          <v-col>
            <v-card-title class="text-h5 text-center">{{ enterprise.name }}</v-card-title>
          </v-col>
        </v-row>
        <v-divider></v-divider>
  
        <v-card-text>
          <v-row>
            <!-- Sección izquierda: Información del emprendimiento -->
            <v-col cols="12" md="6">
              <v-list dense>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title class="font-weight-bold">
                      Cédula del emprendimiento:
                    </v-list-item-title>
                    <v-list-item-subtitle>
                      {{ enterprise.identificationNumber ? formatIdentification(enterprise.identificationNumber) : 'N/A' }}
                    </v-list-item-subtitle>
                  </v-list-item-content>
                </v-list-item>
  
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title class="font-weight-bold">Correo empresarial:</v-list-item-title>
                    <v-list-item-subtitle>{{ enterprise.email || 'N/A' }}</v-list-item-subtitle>
                  </v-list-item-content>
                </v-list-item>
  
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title class="font-weight-bold">Número de teléfono:</v-list-item-title>
                    <v-list-item-subtitle>{{ enterprise.phoneNumber || 'N/A' }}</v-list-item-subtitle>
                  </v-list-item-content>
                </v-list-item>
              </v-list>

              <v-btn color="primary" class="mt-4" elevation="1" outlined @click="handleEditEnterprise">
                Editar Emprendimiento
                <v-icon right>mdi-pencil</v-icon>
              </v-btn>
            </v-col>

            <!-- Sección derecha: Directorio de colaboradores -->
            <v-col cols="12" md="6">
              <v-row class="align-center justify-space-between mb-2">
                <v-card-subtitle class="text-h6 mb-0">Emprendedores asociados</v-card-subtitle>
                <v-btn color="success" elevation="1" icon @click="handleInviteEntrepreneur">
                  <v-icon>mdi-plus</v-icon>
                </v-btn>
              </v-row>

              <v-virtual-scroll :items="enterprise.staff" v-if="enterprise.staff && enterprise.staff.length">
                <template v-slot:default="{ item }">
                  <v-list-item :key="item.id">
                    <v-list-item-avatar>
                      <v-icon>mdi-account</v-icon>
                    </v-list-item-avatar>
                    <v-list-item-content>
                      <v-list-item-title>{{ item.name }} {{ item.lastName }}</v-list-item-title>
                      <v-list-item-subtitle>{{ item.email }}</v-list-item-subtitle>
                    </v-list-item-content>
                  </v-list-item>
  </template>
              </v-virtual-scroll>
              <v-alert type="info" v-else>
                No hay colaboradores asociados.
              </v-alert>
            </v-col>
          </v-row>
        </v-card-text>
  
        <v-divider class="my-4"></v-divider>

        <v-card-subtitle class="text-h6">Productos</v-card-subtitle>
        <v-card-text>
          <v-row>
            <v-col cols="12" md="6">
              <v-btn color="primary" elevation="1" outlined @click="handleRegisterProduct">
                Registrar nuevo producto
                <v-icon right>mdi-plus</v-icon>
              </v-btn>
            </v-col>
            <v-col cols="12" md="6" class="d-flex justify-end">
              <v-btn color="secondary" elevation="1" outlined @click="handleInventoryView">
                Ver Inventario
              </v-btn>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>

      <!-- Sección de Selección de Gráfico -->
      <v-row class="mb-6">
        <v-col cols="12" md="4">
          <v-select
          label="Selecciona el gráfico"
            :items="chartOptionsList"
            v-model="selectedChart"
            outlined
          ></v-select>
        </v-col>
      </v-row>

      <!-- Gráfico Seleccionado con Transición -->
      <transition name="fade" mode="out-in">
        <v-row v-if="selectedChart === 'Ganancias Mensuales'" key="Ganancias Mensuales">
          <v-col cols="12" md="7">
            <StackedBarChart
              :datasets="barChartDatasets"
              :type="0"
              :labels="[
                'Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun',
                'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'
              ]"
            />
          </v-col>
          <v-col cols="12" md="5">
            <PendingOrders />
          </v-col>
        </v-row>
        <v-row v-else-if="selectedChart === 'Ganancias Semanales'" key="Ganancias Semanales">
          <v-col cols="12" md="7">
            <StackedBarChart
              :datasets="weeklyBarChartDatasets"
              :type="1"
              :labels="['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']"
            />
          </v-col>
          <v-col cols="12" md="5">
            <PendingOrders />
          </v-col>
        </v-row>
      </transition>

    </v-container>
  </v-main>
</template>

  <script>
  import axios from 'axios';
import { API_URL } from '@/main.js';
  import { getToken } from '@/helpers/auth';
import StackedBarChart from './StackedBarChart.vue';
import PendingOrders from './PendingOrders.vue';
  
  export default {
  components: {
    StackedBarChart,
    PendingOrders,
  },
    data() {
      return {
      enterprise: {},
      barChartDatasets: [],
      weeklyBarChartDatasets: [],
      selectedChart: 'Ganancias Mensuales',
      chartOptionsList: [
        'Ganancias Mensuales',
        'Ganancias Semanales',
      ],
      };
    },
    async created() {
      const enterpriseId = this.$route.params.id;
      try {
        const token = getToken();
      const enterpriseResponse = await axios.get(
        `${API_URL}/Enterprise/GetEnterpriseById?enterpriseId=${enterpriseId}`,
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );
      this.enterprise = enterpriseResponse.data.data;
      } catch (error) {
        console.error('Error al cargar la empresa:', error);
        if (error.response) {
          console.error('Detalles del error:', error.response.data);
        }
      }

    try {
      const datasetResponse = await axios.get(
        `${API_URL}/Enterprise/GetEnterpriseEarnings?enterpriseId=${enterpriseId}`
      );
      const dataFromBackend = datasetResponse.data.data;
      this.barChartDatasets = this.processDatasetResponse(dataFromBackend);
    } catch (error) {
      console.error('Error al cargar los datos mensuales:', error);
      if (error.response) {
        console.error('Detalles del error:', error.response.data);
      }
    }

    try {
      const weeklyDatasetResponse = await axios.get(
        `${API_URL}/Enterprise/GetEnterpriseWeeklyEarnings?enterpriseId=${enterpriseId}`
      );
      const weeklyDataFromBackend = weeklyDatasetResponse.data.data;
      this.weeklyBarChartDatasets = this.processWeeklyDatasetResponse(weeklyDataFromBackend);
    } catch (error) {
      console.error('Error al cargar los datos semanales:', error);
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

      formatIdentification(identification) {
        if (identification && identification.length === 9) {
          return `${identification.slice(0, 1)}-${identification.slice(1, 5)}-${identification.slice(5)}`;
        }
        return identification || 'N/A';
      },
    handleInviteEntrepreneur() {
      this.$router.push(`/enterprise/${this.enterprise.id}/invite`);
    },
    handleRegisterProduct() {
      this.$router.push(`${this.enterprise.id}/new-product`);
    },
    handleInventoryView() {
      this.$router.push(`${this.enterprise.id}/inventory`);
    },
    handleEditEnterprise() {
      this.$router.push(`/enterprise/${this.enterprise.id}/edit`);
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
.fade-enter-active, .fade-leave-active {
  transition: opacity 0.5s;
  }
.fade-enter, .fade-leave-to /* .fade-leave-active en versiones anteriores de Vue */ {
  opacity: 0;
  }
  </style>
  