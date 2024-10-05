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
          <b-form-group id="group-tags" label="Tags:" label-for="tags">
            <b-form-tags
              v-model="productData.tags"
              :input-id="'tags'"
              :options="tags"
              placeholder="Seleccione o agregue tags"
              add-button-text="Añadir"
              tag-variant="primary"
              tag-remove-label="Eliminar este tag"
            >
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

          <!-- Subir Imágenes -->
          <!-- <b-form-group id="group-images" label="Subir imágenes:" label-for="formFileMultiple">
            <label for="formFileMultiple" class="form-label">Seleccionar múltiples archivos</label>
            <input class="form-control" type="file" id="formFileMultiple" multiple @change="handleFileChange" />
          </b-form-group> -->

          <div class="d-flex justify-content-between mt-4">
            <b-button variant="secondary" @click="goBack">Volver</b-button>
            <b-button type="submit" variant="primary">Registrar</b-button>
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
      },
      tags: [],
      weekdays: [
        { text: 'Lunes', value: '1' },
        { text: 'Martes', value: '2' },
        { text: 'Miércoles', value: '3' },
        { text: 'Jueves', value: '4' },
        { text: 'Viernes', value: '5' },
        { text: 'Sábado', value: '6' },
        { text: 'Domingo', value: '0' },
      ],
      productImages: [],
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
          tags: this.productData.tags,
          stock: this.productData.stock != null ? this.productData.stock : 0,
          limit: this.productData.limit != null ? this.productData.limit : 0,
          weekDaysAvailable: this.productData.weekDaysAvailable.sort().join(''),
        });
      
        const productId = response.data;
        console.log(productId);

        // for (let image of this.productImages) {
        //   const formData = new FormData();
        //   formData.append('image', image);  // Archivo de imagen
        //   formData.append('ownerId', productId);  // ID del propietario

        //   await axios.post('https://localhost:7189/api/ImageUpload/upload', formData, {
        //     headers: {
        //       'Content-Type': 'multipart/form-data',
        //     },
        //   });
        // }

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
    // handleFileChange(event) {
    //   const files = event.target.files;
    //   this.productImages = Array.from(files); // Convertir el FileList a un array
    //   console.log(this.productImages); // Asegurarte de que los archivos se están cargando
    // },
    goBack() {
      this.$router.push('/entrepeneur-home');
    },
    removeTag(tag) {
      // Elimina la etiqueta seleccionada
      const index = this.productData.tags.indexOf(tag);
      if (index > -1) {
        this.productData.tags.splice(index, 1);
      }
    },
  },
  async created() {
    try {
      const response = await axios.get('https://localhost:7189/api/Product/get-tags');
      this.tags = response.data;
    } catch (error) {
      console.error('Error al obtener los tags:', error);
    }
  },
}
</script>

<style>

.formFileMultiple {
  background-color: #D0EDA0
}

.form-control {
  background-color: #D0EDA0;
}

.price-input-bt {
  background-color: #D0EDA0
}
</style>
