<template>
  <v-main>
    <h1>Sumario de los productos registrados en el sistema</h1>
    <div class="reports-container">
      <table class="custom-table">
        <thead>
          <tr>
            <th>Nombre</th>
            <th>Empresa</th>
            <th>Descripción</th>
            <th>Tipo</th>
            <th>Peso</th>
            <th>Precio</th>
            <th>Cantidad en Inventario</th>
            <th>Límite</th>
            <th>Días Disponibles</th>
            <th>Tags</th>
            <th>Imágenes</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(product, index) in filteredProducts" :key="index">
            <td>{{ product.name }}</td>
            <td>{{ product.enterpriseName }}</td>
            <td>{{ product.description }}</td>
            <td>{{ formatProductType(product.type) }}</td>
            <td>{{ product.weight }} kg</td>
            <td>₡{{ product.price }}</td>
            <td>{{ product.stock }}</td>
            <td>{{ product.limit }}</td>
            <td>{{ formatWeekDays(product.weekDaysAvailable) }}</td>
            <td>
              <v-chip-group>
                <v-chip
                  v-for="(tag, i) in product.tags"
                  :key="i"
                  class="ma-1"
                  color="primary"
                  outlined
                >
                  {{ tag }}
                </v-chip>
              </v-chip-group>
            </td>
            <td>
              <v-carousel show-arrows="hover" height="100" width="200" hide-delimiters>
                <v-carousel-item v-for="(image, i) in product.imagesURLs" :key="i">
                  <v-img :src="image" height="200" width="100"></v-img>
                </v-carousel-item>
              </v-carousel>
            </td>
          </tr>
        </tbody>
      </table>
        </div>
  </v-main>
</template>

<script>
import axios from "axios";
import { API_URL, URL } from "@/main.js";

export default {
  data() {
    return {
      search: "",
      products: [],
      isEditDialogOpen: false,
      weekdays: [
        { text: "Lunes", value: "1" },
        { text: "Martes", value: "2" },
        { text: "Miércoles", value: "3" },
        { text: "Jueves", value: "4" },
        { text: "Viernes", value: "5" },
        { text: "Sábado", value: "6" },
        { text: "Domingo", value: "0" },
      ],
    };
  },
  computed: {
    filteredProducts() {
      if (!this.products || !Array.isArray(this.products)) {
        return [];
      }
      return this.products.filter((product) =>
        product.name.toLowerCase().includes(this.search.toLowerCase())
      );
    },
  },
  created() {
    this.loadProducts();
  },
  methods: {
    async loadProducts() {
      try {
        const response = await axios.get(`${API_URL}/Developer/getProducts`);
        console.log("Respuesta del API:", response.data);
        if (Array.isArray(response.data)) {
        this.products = response.data;
            this.formatProductImages();
        } else {
            console.warn("La respuesta no es un arreglo.");
            this.products = [];
        }
    } catch (error) {
        console.error("Error al obtener productos:", error);
        this.products = [];
    }
    },

    formatProductImages() {
      this.products.forEach(product => {
        if (Array.isArray(product.imagesURLs)) {
          product.imagesURLs = product.imagesURLs.map(image => 
            image.startsWith("http") ? image : `${URL}${image}`
          );
        }
            });
    },

    formatWeekDays(weekDaysString) {
      if (!weekDaysString) {
        return "Disponible todos los días";
      }
      const daysMap = [
        "Domingo",
        "Lunes",
        "Martes",
        "Miércoles",
        "Jueves",
        "Viernes",
        "Sábado",
      ];
      return weekDaysString
        .split("")
        .map((day) => daysMap[Number(day.trim())])
        .filter(Boolean)
        .join(", ");
    },
    formatProductType(type) {
      return type === "NonPerishable"
        ? "No perecedero"
        : type === "Perishable"
        ? "Perecedero"
        : "Desconocido";
    },
  },
};
</script>

<style scoped>
.custom-table {
  width: 100%;
  border-collapse: collapse;
}

.custom-table th,
.custom-table td {
  padding: 16px;
  border-bottom: 1px solid #e0e0e0;
  text-align: left;
}

.swal-overlay {
  z-index: 2050 !important;
}

h1 {
  margin: 30px;
  padding: 30px;
}

.reports-container {
  margin: 30px;
  padding: 30px;
  border-radius: 8px;
}
</style>
