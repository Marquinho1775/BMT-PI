<template>
  <v-card>
    <v-row justify="center" class="mb-6">
      <h2 class="report-title">Reporte de ganancias anuales</h2>
    </v-row>

    <v-row align="center" justify="flex-start">
      <v-col cols="12" sm="4">
        <v-btn color="success" @click="openDialog" outlined variant="tonal">
          Seleccionar emprendimientos
        </v-btn>
      </v-col>

      <v-col cols="12" sm="4">
        <v-text-field v-model="year" label="Año" type="number" min="1900" max="2100" placeholder="Ingrese el año"
          outlined></v-text-field>
      </v-col>
    </v-row>

    <v-row justify="flex-start" class="mt-4 mb-6">
      <v-col cols="auto">
        <v-btn color="success" @click="generateReport" :disabled="!isValid">
          Generar Reporte
        </v-btn>
      </v-col>
      <v-col cols="auto">
        <v-btn color="secondary" @click="exportToPDF" :disabled="!reportData.length">
          Exportar a PDF
        </v-btn>
      </v-col>
    </v-row>

    <v-row>
      <div v-if="reportError" class="error-message">
        No se pudo generar un reporte con estos datos.
      </div>
      <reports-table v-else-if="reportData.length" :titles="tableTitles" :keys="tableKeys" :reports="reportData"
        :header-style="headerStyle"></reports-table>
    </v-row>

    <v-dialog v-model="dialog" max-width="500">
      <v-card>
        <v-card-title>Seleccione los emprendimientos</v-card-title>
        <v-card-text>
          <p class="font-weight-medium">Por favor seleccione los emprendimientos:</p>
          <v-checkbox v-model="selectAll" label="Seleccionar todos" @change="toggleSelectAll"></v-checkbox>
          <v-checkbox v-for="enterprise in enterpriseOptions" :key="enterprise.value" v-model="selectedEnterprises"
            :value="enterprise.value" :label="enterprise.text"></v-checkbox>
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
import jsPDF from "jspdf";
import "jspdf-autotable";
import { API_URL } from "@/main.js";

export default {
  name: "YearlyEarnings",
  data() {
    return {
      year: new Date().getFullYear(),
      enterprises: [],
      enterpriseOptions: [],
      selectedEnterprises: [],
      selectAll: false,
      reportData: [],
      reportError: false,
      dialog: false,
      headerStyle: {
        backgroundColor: "#A9C5FF",
        color: "white",
      },
      tableTitles: [
        "Emprendimiento",
        "Mes",
        "Total de compra",
        "Total de envío",
        "Costo total de compra",
      ],
      tableKeys: [
        'Emprendimiento',
        'Mes',
        'Total de compra',
        'Total de envío',
        'Costo total de compra',
      ],
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
          const response = await axios.get(`${API_URL}/Enterprise`, {
            headers: { Authorization: `Bearer ${token}` },
          });

          this.enterprises = response.data.data;
        } else if (user.role === "emp") {
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
        this.selectedEnterprises = this.enterpriseOptions.map(
          (enterprise) => enterprise.value
        );
      } else {
        this.selectedEnterprises = [];
      }
    },
    openDialog() {
      this.dialog = true;
    },
    closeDialog() {
      this.dialog = false;
    },
    translateMonth(englishMonth) {
      const months = {
        January: "Enero",
        February: "Febrero",
        March: "Marzo",
        April: "Abril",
        May: "Mayo",
        June: "Junio",
        July: "Julio",
        August: "Agosto",
        September: "Septiembre",
        October: "Octubre",
        November: "Noviembre",
        December: "Diciembre",
      };
      return months[englishMonth] || englishMonth; // Devuelve el nombre traducido o el original si no coincide
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

        this.reportError = false;
        this.reportData = response.data.data.map((item) => ({
          Emprendimiento: item.enterpriseName,
          Mes: this.translateMonth(item.monthName),
          "Total de compra": item.totalPurchase,
          "Total de envío": item.totalDelivery,
          "Costo total de compra": item.totalCost,
        }));
      } catch (error) {
        console.error("Error al generar el reporte:", error);
        this.reportError = true;
        this.reportData = [];
      }
    },
    exportToPDF() {
      const doc = new jsPDF();
      doc.text("Reporte de Ganancias Anuales", 20, 20);

      const tableData = this.reportData.map((row) =>
        Object.values(row)
      );

      doc.autoTable({
        head: [this.tableTitles],
        body: tableData,
        startY: 30,
        theme: "grid",
        headStyles: { fillColor: [169, 197, 255] },
      });

      doc.save(`reporte_ganancias_${this.year}.pdf`);
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
