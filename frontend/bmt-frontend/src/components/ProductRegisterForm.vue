<template>
  <v-main>
    <v-container class="fill-height">
      <v-row align="center" justify="center">
        <v-col cols="12" sm="8" md="6">
          <v-card id="form" class = "my-4">
            <v-card-title id="title" class="text-center">
              Agregar un producto
            </v-card-title>
            <v-card-text>
              <v-form ref="form" @submit.prevent="addNewProduct">
                <!-- Nombre del producto -->
                <v-text-field
                  id="name"
                  v-model="productData.name"
                  label="Nombre del producto:"
                  placeholder="Ingresar el nombre del producto"
                  required
                ></v-text-field>

                <!-- Descripción -->
                <v-text-field
                  id="description"
                  v-model="productData.description"
                  label="Descripción:"
                  placeholder="Ingresar la descripción del producto"
                  required
                ></v-text-field>

                <!-- Tags -->
                <v-autocomplete
                  v-model="value"
                  :items="availableOptions"
                  label="Etiquetas de producto:"
                  placeholder="Escoge una o varias etiquetas..."
                  multiple
                  chips
                  deletable-chips
                ></v-autocomplete>

                <!-- Peso -->
                <v-text-field
                  id="weight"
                  v-model="productData.weight"
                  label="Peso (kg):"
                  placeholder="Ingrese el peso del producto"
                  required
                  :error="weightValid === false"
                  :error-messages="weightValid === false ? ['Por favor, ingrese un peso válido.'] : []"
                  @input="validateWeight"
                ></v-text-field>

                <!-- Precio -->
                <v-text-field
                  id="price"
                  v-model="productData.price"
                  label="Precio:"
                  placeholder="Ingrese el precio del producto"
                  required
                  :error="priceValid === false"
                  :error-messages="priceValid === false ? ['Por favor, ingrese un precio válido.'] : []"
                  @input="validatePrice"
                ></v-text-field>

                <!-- Tipo de Producto -->
                <v-radio-group
                  v-model="productData.type"
                  label="Tipo de producto:"
                  required
                >
                  <v-radio label="Perecedero" value="Perecedero"></v-radio>
                  <v-radio label="No perecedero" value="No perecedero"></v-radio>
                </v-radio-group>

                <div v-if="productData.type === 'No perecedero'">
                  <!-- Stock -->
                  <v-text-field
                    id="stock"
                    v-model.number="productData.stock"
                    label="Cantidad de stock:"
                    placeholder="Ingrese la cantidad de stock"
                    type="number"
                    min="0"
                    step="1"
                    required
                    :error="stockValid === false"
                    :error-messages="stockValid === false ? ['Por favor, ingrese una cantidad de stock válida.'] : []"
                    @input="validateStock"
                  ></v-text-field>
                </div>

                <div v-if="productData.type === 'Perecedero'">
                  <!-- Días de la semana -->
                  <div>
                  <span>Días de disponibilidad:</span>
                  <v-row>
                    <v-col
                    v-for="weekday in weekdays"
                    :key="weekday.value"
                    cols="auto"
                    >
                    <v-checkbox
                      v-model="productData.weekDaysAvailable"
                      :label="weekday.text"
                      :value="weekday.value"
                      dense
                    ></v-checkbox>
                    </v-col>
                  </v-row>
                  </div>

                  <!-- Límite por día -->
                  <v-text-field
                    id="limit"
                    v-model.number="productData.limit"
                    label="Límite por día:"
                    placeholder="Ingrese el límite por día"
                    type="number"
                    min="0"
                    step="1"
                    required
                    :error="limitValid === false"
                    :error-messages="limitValid === false ? ['Por favor, ingrese un límite por día válido.'] : []"
                    @input="validateLimit"
                  ></v-text-field>
                </div>
                
                <!-- Imágenes -->
                <v-file-input
                  v-model="productData.images"
                  label="Añada imágenes:"
                  multiple
                ></v-file-input>

                <!-- Botones -->
                <v-row class="mt-4" justify="space-between">
                  <v-btn color="secondary" @click="goBack">Volver</v-btn>
                  <v-btn type="submit" color="primary">Registrar</v-btn>
                </v-row>
              </v-form>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
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
            let days = '';
            this.productData.weekDaysAvailable.forEach(day => {
              days += day;
            });
            formData.append('WeekDaysAvailable', days);
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

<style scoped>
#form {
  padding: 20px;
  background-color: #f5f5f5;
}
</style>
