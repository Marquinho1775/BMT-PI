<template>
  <div class="enterprise-register-container d-flex justify-content-center align-items-center vh-100">
    <div id="form" class="card custom-card my-4">
      <h3 id="title" class="text-center card-header-custom">Agregar un producto</h3>
      <div class="card-body">
        <b-form @submit.prevent="addNewProduct" @reset="onReset">

          <!-- Nombre del producto -->
          <b-form-group id="group-name" label="Nombre del producto:" label-for="name">
            <b-form-input id="name" class="form-input" v-model="productData.name"
              placeholder="Ingresar el nombre del producto" required>
            </b-form-input>
          </b-form-group>

          <!-- Descripción -->
          <b-form-group id="group-description" label="Descripción:" label-for="description">
            <b-form-input id="description" class="form-input" v-model="productData.description"
              placeholder="Ingresar la descripción del producto" required>
            </b-form-input>
          </b-form-group>

          <!-- Tags -->
          <b-form-group label="Etiquetas de producto:" label-for="tags-component-select">
            <b-form-tags id="tags-component-select" v-model="value" size="lg"  class="mb-2" add-on-change no-outer-focus>
              <template v-slot="{ tags, inputAttrs, inputHandlers, disabled, removeTag }">
                <b-form-select class = "select-component" v-bind="inputAttrs" v-on="inputHandlers" :disabled="disabled || availableOptions.length === 0" :options="availableOptions">
                  <template #first>
                    <option disabled value="" class="'select-component'">Escoge una o varias etiquetas...</option>
                  </template>
                </b-form-select>
                <ul v-if="tags.length > 0" class="list-inline d-inline-block mt-2">
                  <li v-for="tag in tags" :key="tag" class="list-inline-item">
                    <b-form-tag id="tag-component" class="custom-tag" @remove="removeTag(tag)" :title="tag" :disabled="disabled">{{ tag }}</b-form-tag>
                  </li>
                </ul>
              </template>
            </b-form-tags>
          </b-form-group>

          <!-- Peso -->
          <b-form-group id="group-weight" label="Peso (kg):" label-for="weight">
            <b-form-input id="weight" class="form-input" v-model="productData.weight" type="number"
              placeholder="Ingrese el peso del producto" required>
            </b-form-input>
          </b-form-group>

          <!-- Precio -->
          <b-form-group id="group-price" label="Precio:" label-for="price">
            <b-form-input id="price" class="form-input price-input-bt" v-model="productData.price" type="number"
              placeholder="Ingrese el precio del producto" required>
            </b-form-input>
          </b-form-group>

          <!-- Tipo de Producto -->
          <b-form-group id="group-type" label="Tipo de producto:" label-for="type">
            <b-form-radio-group id="type" v-model="productData.type" :options="['Perecedero', 'No perecedero']"
              name="type">
            </b-form-radio-group>
          </b-form-group>

          <!-- Campos adicionales según el tipo de producto -->
          <div v-if="productData.type === 'No perecedero'">
            <b-form-group id="group-stock" label="Cantidad de stock:" label-for="stock">
              <b-form-input id="stock" class="form-input" v-model="productData.stock" type="number"
                placeholder="Ingrese la cantidad de stock" required>
              </b-form-input>
            </b-form-group>
          </div>

          <div v-if="productData.type === 'Perecedero'">
            <!-- Días de la semana -->
            <b-form-group id="group-weekdays" label="Días de disponibilidad:" label-for="weekdays">
              <b-form-checkbox-group id="weekdays" v-model="productData.weekDaysAvailable" :options="weekdays"
                name="weekdays">
              </b-form-checkbox-group>
            </b-form-group>

            <!-- Límite por día -->
            <b-form-group id="group-limit" label="Límite por día:" label-for="limit">
              <b-form-input id="limit" class="form-input" v-model="productData.limit" type="number"
                placeholder="Ingrese el límite por día" required>
              </b-form-input>
            </b-form-group>
          </div>

          <!-- Imagenes -->
          <b-form-group label="Añada imagenes:" label-for="input-images">
            <input
              id="input-images"
              type="file"
              multiple
              @change="handleFileChange"/>
          </b-form-group>

          <div class="d-flex justify-content-between mt-4">
            <b-button variant="secondary" class="custom-button" @click="goBack">Volver</b-button>
            <b-button type="submit" class="custom-button" variant="primary">Registrar</b-button>
          </div>
        </b-form>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios'

export default {
  data() {
    return {
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
      value: []
    }
  },
  methods: {
    async addNewProduct() {
      try {
        const response = await axios.post('https://localhost:7189/api/Product', {
          id: '',
          username: JSON.parse(localStorage.getItem('user')).username,
          name: this.productData.name,
          description: this.productData.description,
          weight: this.productData.weight,
          price: this.productData.price,
          type: this.productData.type == "Perecedero" ? "Perishable" : "NonPerishable",
          tags: this.value,
          stock: this.productData.stock != null ? this.productData.stock : 0,
          limit: this.productData.limit != null ? this.productData.limit : 0,
          weekDaysAvailable: this.productData.weekDaysAvailable.sort().join(''),
        });
        const productId = response.data;
        const formData = new FormData();
        formData.append("ownerId", productId);
        formData.append("ownerType", "Product");
        for (const file of this.productData.images) {
          formData.append("images", file);
        }
        await axios.post("https://localhost:7189/api/ImageFile/upload",
          formData,
          {
            headers: {
              "Content-Type": "multipart/form-data",
            },
          }
        );
        this.$swal.fire({
          title: 'Éxito',
          text: 'Producto agregado exitosamente.',
          icon: 'success',
          confirmButtonText: 'Ok'
        }).then(() => {
          this.$router.push('/entrepeneur-home');
        });
      } catch (error) {
        this.$swal.fire({
          title: 'Error',
          text: 'Hubo un error al agregar el producto.',
          icon: 'error',
          confirmButtonText: 'Ok'
        });
        console.log(error);
      }
    },
    goBack() {
      this.$router.push('/entrepeneur-home');
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
      const response = await axios.get('https://localhost:7189/api/Product/get-tags');
      this.options = response.data;
    } catch (error) {
      console.error('Error al obtener los tags:', error);
    }
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
  border : 1px solid #36618E;
}

</style>
