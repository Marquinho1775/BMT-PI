<template>
    <v-main class="flex-grow-1">
      <v-container>
        <v-card flat>
          <v-card-title class="d-flex align-center pe-2">
            <v-icon icon="mdi-warehouse"></v-icon> &nbsp;
            Inventario de Productos
            <v-spacer></v-spacer>
            <v-text-field
              v-model="search"
              density="compact"
              label="Buscar"
              prepend-inner-icon="mdi-magnify"
              variant="solo-filled"
              flat
              hide-details
              single-line
            ></v-text-field>
          </v-card-title>
          <v-divider></v-divider>

          <table class="custom-table">
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Tipo</th>
                <th>Peso</th>
                <th>Precio</th>
                <th>Cantidad en Inventario</th>
                <th>Límite</th>
                <th>Días Disponibles</th>
                <th>Tags</th>
                <th>Imágenes</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(product, index) in filteredProducts" :key="index">
                <td>{{ product.name }}</td>
                <td>{{ formatProductType(product.type) }}</td>
                <td>{{ product.weight }} kg</td>
                <td>₡{{ product.price }}</td>
                <td>{{ product.stock }}</td>
                <td>{{ product.limit }}</td>
                <td>{{ formatWeekDays(product.weekDaysAvailable) }}</td>
                <td>
                  <v-chip-group>
                    <v-chip v-for="(tag, i) in product.tags" :key="i" class="ma-1" color="primary" outlined>
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
                <td>
                  <v-btn icon @click="openEditDialog(product)">
                    <v-icon>mdi-pencil</v-icon>
                  </v-btn>
                  <v-btn icon color="error" @click="confirmDelete(product)">
                    <v-icon >mdi-delete</v-icon>
                  </v-btn>
                </td>
              </tr>
            </tbody>
          </table>
        </v-card>

        <!-- Diálogo para edición de producto -->
        <v-dialog v-model="isEditDialogOpen" max-width="600px">
          <v-card>
            <v-card-title>
              <span class="text-h5">Editar Producto</span>
            </v-card-title>
              <v-card-text>
                <!-- Formulario de edición de producto -->
                <v-form ref="editForm" @submit.prevent="updateProduct">
                  <v-text-field
                    label="Nombre del producto"
                    v-model="editProductData.name"
                  ></v-text-field>
                  
                  <v-textarea
                    label="Descripción"
                    v-model="editProductData.description"
                  ></v-textarea>
                  
                  <v-text-field
                    label="Tipo de producto"
                    v-model="editProductData.type"
                    readonly
                  ></v-text-field>
                  
                  <v-text-field
                    label="Precio"
                    v-model="editProductData.price"
                    type="number"
                    prefix="₡"
                  ></v-text-field>
                  
                  <!-- Condicional para Cantidad en Inventario -->
                  <v-text-field
                    label="Cantidad en Inventario"
                    v-model="editProductData.stock"
                    type="number"
                    :disabled="editProductData.type === 'Perishable'"
                  ></v-text-field>
                  
                  <!-- Condicional para Límite de Inventario -->
                  <v-text-field
                    label="Límite de inventario"
                    v-model="editProductData.limit"
                    type="number"
                    :disabled="editProductData.type === 'NonPerishable'"
                  ></v-text-field>

                  <v-combobox
                    label="Tags"
                    v-model="editProductData.tags"
                    multiple
                    chips
                    outlined
                  ></v-combobox>
                  
                  <!-- Campo de URLs de Imágenes -->
                  <v-file-input
                    label="Actualizar Imágenes"
                    v-model="editProductData.newImages"
                    prepend-icon="mdi-image"
                    multiple
                    accept="image/*"
                    hint="Seleccione una o más imágenes para cargar"
                    @change="onImageChange"
                  ></v-file-input>

                </v-form>
              </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="primary" @click="updateProduct">Guardar</v-btn>
              <v-btn text @click="closeEditDialog">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
      </v-container>
    </v-main>
</template>
  
  
  <script>
  import axios from 'axios';
  import { API_URL, URL } from '@/main.js';
  import { getToken } from '@/helpers/auth';
  
  export default {
    data() {
      return {
        search: '',
        products: [],
        isEditDialogOpen: false,

        weekdays: [
          { text: 'Lunes', value: '1' },
          { text: 'Martes', value: '2' },
          { text: 'Miércoles', value: '3' },
          { text: 'Jueves', value: '4' },
          { text: 'Viernes', value: '5' },
          { text: 'Sábado', value: '6' },
          { text: 'Domingo', value: '0' },
        ],

        editProductData: {
          name: '',
          description: '',
          type: '',
          price: 0,
          stock: 0,
          limit: null,
          tags: [],
          imagesURLs: [],
          newImages: [],
          deliveryDays: [],
          weekDaysAvailable: [],
        },
        enterpriseId: null,
      };
    },
    computed: {
      filteredProducts() {
        return this.products.filter((product) => {
          return product.name.toLowerCase().includes(this.search.toLowerCase());
        });
      },
    },
    async created() {
      const enterpriseId = this.$route.params.id;
      this.enterpriseId = enterpriseId;
      await this.getProducts();
    },
    methods: {
      async getProducts() {
        try {
          const response = await axios.get(`${API_URL}/Enterprise/GetEnterpriseProducts?enterpriseId=${this.enterpriseId}`);
          this.products = response.data.data;
          this.URLImage();
        } catch (error) {
          console.error('Error al obtener productos:', error);
        }
      },
      
      URLImage() {
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
      const daysMap = ["Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo"];
      return weekDaysString
      .toString()
      .split("")
      .map(day => daysMap[Number(day.trim())])
      .filter(Boolean)
      .join(", ");
    },
    
    formatProductType(type) {
      return type === "NonPerishable" ? "No perecedero" : type === "Perishable" ? "Perecedero" : "Desconocido";
    },
    
    openEditDialog(product) {
      this.editProductData = { ...product };
      this.isEditDialogOpen = true;
    },
    
    closeEditDialog() {
      this.isEditDialogOpen = false;
    },
    onImageChange() {
    },
    async updateProduct() {
      try {
        const formData = new FormData();
        formData.append('id', this.editProductData.id);
        formData.append('enterpriseId', this.enterpriseId);
        formData.append('name', this.editProductData.name);
        formData.append('description', this.editProductData.description);
        formData.append('weight', this.editProductData.weight);
        formData.append('price', this.editProductData.price);
        formData.append('type', this.editProductData.type);
        
        if (this.editProductData.tags && this.editProductData.tags.length > 0) {
          this.editProductData.tags.forEach(tag => {
            formData.append('Tags', tag);
          });
        }
        
        if (this.editProductData.type === "NonPerishable") {
          formData.append('stock', this.editProductData.stock != null ? this.editProductData.stock : 0);
        } 
        
        if (this.editProductData.type === "Perishable") {
          formData.append('limit', this.editProductData.limit != null ? this.editProductData.limit : 0);
        }
        
        if (this.editProductData.newImages && this.editProductData.newImages.length > 0) {
          for (const file of this.editProductData.newImages) {
            formData.append('ImagesFiles', file);
          }
        }
        
        const token = getToken();
        const response = await axios.put(
          `${API_URL}/Product`,
          formData,
          {
            headers: {
              'Content-Type': 'multipart/form-data',
              Authorization: `Bearer ${token}`
            }
          }
        );        
        
        console.log('Response:', response.data);
        if (response.data.success) {
          this.$swal.fire({
            title: 'Producto actualizado',
            text: '¡Su producto ha sido actualizado correctamente!',
            icon: 'success',
            confirmButtonText: 'Ok',
            backdrop: true,
            customClass: {
              popup: 'swal-overlay',
            }
          });
        } else {
            this.$swal.fire({
              title: 'Error',
              text: 'Hubo un error al actualizar el producto.',
              icon: 'error',
              confirmButtonText: 'Ok'
            });
          }
        } catch (error) {
          console.error('Error al actualizar producto:', error);
          this.$swal.fire({
            title: 'Error',
            text: 'Hubo un error al actualizar su producto. Inténtelo de nuevo.',
            icon: 'error',
            confirmButtonText: 'Ok',
            backdrop: true,
            customClass: {
              popup: 'swal-overlay',
            }
          });
        }
        await this.getProducts();
        this.closeEditDialog();
      }
            console.error('Error al actualizar producto:', error);
            this.isEditDialogOpen = false;

            this.$swal.fire({
                title: 'Error',
                text: 'Hubo un error al actualizar su producto. Inténtelo de nuevo.',
                icon: 'error',
                confirmButtonText: 'Ok',
                backdrop: true,
                customClass: {
                    popup: 'swal-overlay',
                }
            });
        } finally {
            this.closeEditDialog();
        }
      },
      async confirmDelete(product) {
        const result = await this.$swal.fire({
          title: "¿Estás seguro?",
          text: `Vas a borrar el producto: ${product.name}`,
          icon: "warning",
          showCancelButton: true,
          confirmButtonColor: "#3085d6",
          cancelButtonColor: "#d33",
          confirmButtonText: "Sí, borrar",
          cancelButtonText: "Cancelar",
        });

        if (result.isConfirmed) {
          this.deleteProduct(product);
        }
      },
      async deleteProduct(product) {
        try {
          // Llamada al backend
          const response = await axios.delete(`${API_URL}/Product/Delete/${product.id}`);
          
          // Si el borrado fue exitoso
          if (response.status === 200) {
            // Elimina el producto del array local
            this.products = this.products.filter((p) => p.id !== product.id);

            // Muestra una alerta de éxito
            this.$swal.fire({
              title: "¡Borrado!",
              text: `El producto ${product.name} ha sido eliminado.`,
              icon: "success",
            });
          } else {
            throw new Error("Error inesperado al borrar el producto.");
          }
        } catch (error) {
          console.error(error);
          this.$swal.fire({
            title: "Error",
            text: "No se pudo borrar el producto. Inténtalo nuevamente.",
            icon: "error",
          });
        }
      },
    }
  };
  </script>
  
  <style scoped>
  .custom-table {
    width: 100%;
    border-collapse: collapse;
  }
  .custom-table th, .custom-table td {
    padding: 16px;
    border-bottom: 1px solid #e0e0e0;
    text-align: left;
  }

  .swal-overlay {
  z-index: 2050 !important;
}

  </style>
  