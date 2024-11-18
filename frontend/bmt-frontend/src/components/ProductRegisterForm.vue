<template>
  <v-main>
    <v-container
      class="d-flex justify-center align-center"
      style="min-height: 100vh;"
    >
      <v-card class="pa-4 elevation-2" max-width="600px" width="100%">
        <v-card-title class="text-h5 text-center">
          Agregar un producto
        </v-card-title>
        <v-divider></v-divider>
        <v-card-text>
          <v-form @submit.prevent="addNewProduct">
            <!-- Nombre del producto -->
            <v-text-field
              v-model="productData.name"
              label="Nombre del producto"
              required
              outlined
            ></v-text-field>

            <!-- Descripción -->
            <v-textarea
              v-model="productData.description"
              label="Descripción"
              required
              outlined
            ></v-textarea>

            <!-- Tags -->
            <v-combobox
              v-model="value"
              :items="availableOptions"
              label="Etiquetas de producto"
              multiple
              chips
              outlined
              clearable
              item-value="value"
              item-text="text"
            ></v-combobox>

            <!-- Peso -->
            <v-text-field
              v-model="productData.weight"
              label="Peso (kg)"
              type="number"
              required
              outlined
              :error-messages="weightValid === false ? ['Por favor, ingrese un peso válido.'] : []"
              @input="validateWeight"
            ></v-text-field>

            <!-- Precio -->
            <v-text-field
              v-model="productData.price"
              label="Precio"
              type="number"
              required
              outlined
              :error-messages="priceValid === false ? ['Por favor, ingrese un precio válido.'] : []"
              @input="validatePrice"
            ></v-text-field>

            <!-- Tipo de Producto -->
            <v-radio-group v-model="productData.type" label="Tipo de producto" row>
              <v-radio label="Perecedero" value="Perecedero"></v-radio>
              <v-radio label="No perecedero" value="No perecedero"></v-radio>
            </v-radio-group>

            <!-- Stock -->
            <v-text-field
              v-if="productData.type === 'No perecedero'"
              v-model.number="productData.stock"
              label="Cantidad de stock"
              type="number"
              min="0"
              step="1"
              required
              outlined
              :error-messages="stockValid === false ? ['Por favor, ingrese una cantidad de stock válida.'] : []"
              @input="validateStock"
            ></v-text-field>

            <!-- Días de la semana -->
            <v-checkbox-group
              v-if="productData.type === 'Perecedero'"
              v-model="productData.weekDaysAvailable"
              label="Días de disponibilidad"
              column
            >
              <v-checkbox v-for="day in weekdays" :key="day.value" :label="day.text" :value="day.value"></v-checkbox>
            </v-checkbox-group>

            <!-- Límite por día -->
            <v-text-field
              v-if="productData.type === 'Perecedero'"
              v-model.number="productData.limit"
              label="Límite por día"
              type="number"
              min="0"
              step="1"
              required
              outlined
              :error-messages="limitValid === false ? ['Por favor, ingrese un límite por día válido.'] : []"
              @input="validateLimit"
            ></v-text-field>

            <!-- Imagenes -->
            <v-file-input
              label="Añada imágenes"
              multiple
              outlined
              @change="handleFileChange"
            ></v-file-input>

            <!-- Botones de acción -->
            <v-btn
              color="secondary"
              class="mt-4"
              @click="goBack"
              outlined
              block
            >
              Volver
            </v-btn>
            <v-btn
              color="primary"
              class="mt-4"
              type="submit"
              block
            >
              Registrar
            </v-btn>
          </v-form>
        </v-card-text>
      </v-card>
    </v-container>
  </v-main>
</template>


<script>
import axios from 'axios'
import { API_URL } from '@/main.js';

export default {
  data() {
    return {
      enterpriseId: '',
      productData: {
        name: '',
        description: '',
        weight: '',
        price: '',
        type: '',
        tags: [],
        stock: null,
        limit: null,
        weekDaysAvailable: [],
        images: [],
      },
      weekdays: [
        { text: 'Lunes', value: '1' },
        { text: 'Martes', value: '2' },
        { text: 'Miércoles', value: '3' },
        { text: 'Jueves', value: '4' },
        { text: 'Viernes', value: '5' },
        { text: 'Sábado', value: '6' },
        { text: 'Domingo', value: '0' },
      ],
      options: [],
      value: [],
      weightValid: null,
      priceValid: null,
      stockValid: null,
      limitValid: null,
    }
  },
  methods: {
    async addNewProduct() {
      try {
        const formData = new FormData();
        formData.append('enterpriseId', this.enterpriseId);
        formData.append('name', this.productData.name);
        formData.append('description', this.productData.description);
        formData.append('weight', this.productData.weight);
        formData.append('price', this.productData.price);
        formData.append('type', this.productData.type === "Perecedero" ? "Perishable" : "NonPerishable");

        if (this.value && this.value.length > 0) {
          this.value.forEach(tag => {
            formData.append('Tags', tag);
          });
        }

        if (this.productData.type === "No perecedero") {
          formData.append('stock', this.productData.stock != null ? this.productData.stock : 0);
        } else if (this.productData.type === "Perecedero") {
          formData.append('limit', this.productData.limit != null ? this.productData.limit : 0);

          if (this.productData.weekDaysAvailable && this.productData.weekDaysAvailable.length > 0) {
            this.productData.weekDaysAvailable.forEach(day => {
              formData.append('WeekDaysAvailable', day);
            });
          }
        }

        if (this.productData.images && this.productData.images.length > 0) {
          for (const file of this.productData.images) {
            formData.append('ImagesFiles', file);
          }
        }

        const response = await axios.post(API_URL + '/Product', formData, {
          headers: {
            'Content-Type': 'multipart/form-data'
          }
        });

        if (response.data.success) {
          this.$swal.fire({
            title: 'Éxito',
            text: 'Producto agregado exitosamente.',
            icon: 'success',
            confirmButtonText: 'Ok'
          }).then(() => {
            this.$router.push('/');
          });
        } else {
          this.$swal.fire({
            title: 'Error',
            text: 'Hubo un error al agregar el producto.',
            icon: 'error',
            confirmButtonText: 'Ok'
          });
        }
      } catch (error) {
        this.$swal.fire({
          title: 'Error',
          text: 'Hubo un error al agregar el producto.',
          icon: 'error',
          confirmButtonText: 'Ok'
        });
        console.error('Error al agregar el producto:', error);
      }
    },

    validateWeight() {
      const str = this.productData.weight;
      this.weightValid = !isNaN(str) && !isNaN(parseFloat(str)) && str >= 0 && str !== null;
    },
    validatePrice() {
      const str = this.productData.price;
      this.priceValid = !isNaN(str) && !isNaN(parseFloat(str)) && str >= 0 && str !== null;
    },
    validateStock() {
      const stock = this.productData.stock;
      this.stockValid = stock !== null && Number.isInteger(stock) && stock >= 0;
    },
    validateLimit() {
      const limit = this.productData.limit;
      this.limitValid = limit !== null && Number.isInteger(limit) && limit >= 0;
    },
    goBack() {
      this.$router.push('/');
    },
    handleFileChange(event) {
      const files = event.target.files;
      this.productData.images = files;
    },
  },
  computed: {
    availableOptions() {
      return this.options.filter(opt => this.value.indexOf(opt) === -1)
    }
  },
  async created() {
    try {
      const response = await axios.get(API_URL + '/Tag');
      const array = response.data.data;
      for (let i = 0; i < array.length; i++) {
        this.options.push(array[i].name);
      }
    } catch (error) {
      console.error('Error al obtener los tags:', error);
    }
    this.enterpriseId = this.$route.params.id;
  },
}
</script>

<style>
#tags-component-select {
  background-color: #D0EDA0;
}

.custom-button {
  background-color: #36618E !important;
  border-color: #36618E !important;
  color: white !important;
}

#form .custom-tag {
  background-color: #36618E !important;
  border-color: #36618E !important;
  color: white !important;
}

#form .b-form-tag {
  background-color: #36618E !important;
  border-color: #36618E !important;
  color: white !important;
}

#form .b-form-tag .close {
  color: white !important;
}

form .select-component {
  background-color: #D0EDA0;
  border: 1px solid #36618E;
}
</style>
