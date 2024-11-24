<template>
  <v-card>
    <!-- Título -->
    <v-row justify="center" class="mb-6">
      <h2 class="report-title">Reporte de ganancias anuales</h2>
    </v-row>

    <v-row align="center" justify="space-between">
      <!-- Selección del año -->
      <v-col cols="12" sm="4">
        <v-text-field
          v-model="year"
          label="Año"
          type="number"
          min="1900"
          max="2100"
          placeholder="Ingrese el año"
          outlined
        ></v-text-field>
      </v-col>

      <!-- Botón para seleccionar emprendimientos -->
      <v-col cols="12" sm="4">
        <v-btn color="primary" @click="openDialog" outlined>
          Seleccionar emprendimientos
        </v-btn>
      </v-col>
    </v-row>

    <!-- Botón para generar reporte -->
    <v-row justify="center" class="mt-4 mb-6">
      <v-btn color="success" @click="generateReport" :disabled="!isValid">
        Generar Reporte
      </v-btn>
    </v-row>

    <!-- Mostrar mensaje de error o la tabla -->
    <v-row>
      <div v-if="reportError" class="error-message">
        No se pudo generar un reporte con estos datos.
      </div>
      <reports-table
        v-else-if="reportData.length"
        :titles="tableTitles"
        :reports="reportData"
        :header-style="headerStyle"
      ></reports-table>
    </v-row>

    <!-- Diálogo de selección de emprendimientos -->
    <v-dialog v-model="dialog" max-width="500">
      <v-card>
        <v-card-title>Seleccione los emprendimientos</v-card-title>
        <v-card-text>
          <p class="font-weight-medium">Por favor seleccione los emprendimientos:</p>
          <v-checkbox
            v-model="selectAll"
            label="Seleccionar todos"
            @change="toggleSelectAll"
          ></v-checkbox>
          <v-checkbox
            v-for="enterprise in enterpriseOptions"
            :key="enterprise.value"
            v-model="selectedEnterprises"
            :value="enterprise.value"
            :label="enterprise.text"
          ></v-checkbox>
        </v-card-text>
        <v-card-actions>
          <v-btn color="primary" @click="closeDialog">Aceptar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-card>
</template>

<script>
import axios from "axios";
import { API_URL } from "@/main.js";

export default {
  name: "YearlyEarnings",
  data() {
    return {
      year: new Date().getFullYear(), // Año actual
      enterprises: [], // Lista de emprendimientos
      enterpriseOptions: [], // Opciones de emprendimientos
      selectedEnterprises: [], // Emprendimientos seleccionados
      selectAll: false, // Estado del checkbox "Seleccionar todos"
      reportData: [], // Datos del reporte
      reportError: false, // Indica si hubo un error al generar el reporte
      dialog: false, // Control del diálogo
      headerStyle: {
        backgroundColor: "#A9C5FF", // Color del encabezado de la tabla
        color: "white",
      }, // Estilo del encabezado
      tableTitles: [
        "Emprendimiento",
        "Mes",
        "Total de compra",
        "Total de envío",
        "Costo total de compra",
      ], // Títulos de la tabla
    };
  },
  computed: {
    isValid() {
      return this.selectedEnterprises.length > 0 && this.year;
    },
  },
  methods: {
    async fetchEnterprises() {
      try {
        const token = localStorage.getItem("token");
        const user = JSON.parse(localStorage.getItem("user"));

        if (user.role === "dev") {
          // Si el usuario es "dev", obtener todos los emprendimientos
          const response = await axios.get(`${API_URL}/Enterprise`, {
            headers: { Authorization: `Bearer ${token}` },
          });

          this.enterprises = response.data.data; // Asumimos que los datos están en 'data'
        } else if (user.role === "emp") {
          // Si el usuario es "emp", obtener sus emprendimientos asociados
          const entrepreneurResponse = await axios.get(
            `${API_URL}/Entrepreneur/GetEntrepreneurByUserId?id=${user.id}`,
            {
              headers: { Authorization: `Bearer ${token}` },
            }
          );
          const entrepreneurId = entrepreneurResponse.data.identification;

          const response = await axios.get(
            `${API_URL}/Entrepreneur/my-registered-enterprises?Identification=${entrepreneurId}`,
            {
              headers: { Authorization: `Bearer ${token}` },
            }
          );

          this.enterprises = response.data.success;
        }

        // Construir opciones de la lista
        this.enterpriseOptions = this.enterprises.map((enterprise) => ({
          text: enterprise.name,
          value: enterprise.id,
        }));
      } catch (error) {
        console.error("Error al obtener los emprendimientos:", error);
      }
    },
    toggleSelectAll() {
      if (this.selectAll) {
        // Seleccionar todos
        this.selectedEnterprises = this.enterpriseOptions.map(
          (enterprise) => enterprise.value
        );
      } else {
        // Deseleccionar todos
        this.selectedEnterprises = [];
      }
    },
    openDialog() {
      this.dialog = true;
    },
    closeDialog() {
      this.dialog = false;
    },
    async generateReport() {
      const selectedEnterpriseIds = this.selectedEnterprises.join(",");
      const requestData = {
        enterpriseIds: selectedEnterpriseIds,
        year: this.year,
      };

      console.log("Datos a enviar:", requestData);

      try {
        const response = await axios.post(
          `${API_URL}/Enterprise/GetYearlyEarningsData`,
          requestData,
          {
            headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
          }
        );

        console.log("Respuesta del backend:", response.data);

        this.reportError = false; // No hay error
        this.reportData = response.data.data.map((item) => ({
          Emprendimiento: item.enterpriseName,
          Mes: item.monthName, // Ajusta según las propiedades de tu respuesta
          "Total de compra": item.totalPurchase,
          "Total de envío": item.totalDelivery,
          "Costo total de compra": item.totalCost,
        }));
      } catch (error) {
        console.error("Error al generar el reporte:", error);
        this.reportError = true; // Hubo un error
        this.reportData = []; // Limpiar datos del reporte
      }
    },
  },
  mounted() {
    this.fetchEnterprises();
  },
};
</script>


<style scoped>
.v-card {
  padding: 20px;
}
.report-title {
  font-size: 24px;
  font-weight: bold;
}
.error-message {
  color: red;
  font-size: 18px;
  font-weight: bold;
  text-align: center;
  margin-top: 20px;
}
</style>
