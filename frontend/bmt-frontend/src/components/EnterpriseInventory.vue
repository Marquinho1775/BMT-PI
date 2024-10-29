<template>
  <v-app class="d-flex flex-column">
    <AppHeader />
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
                </td>
              </tr>
            </tbody>
          </table>
        </v-card>

        <!-- Dialog for Editing Product -->
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
    <AppFooter />
    <AppSidebar />
  </v-app>
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
          WeekDaysAvailable: '',
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
      this.enterpriseId = enterpriseId; // Almacena enterpriseId
      try {
        const token = getToken();
        const enterpriseResponse = await axios.get(API_URL + `/Enterprise/${enterpriseId}`, {
          headers: { Authorization: `Bearer ${token}` }
        });
        this.enterprise = enterpriseResponse.data;
        await this.getProducts();
      } catch (error) {
        console.error('Error al cargar la empresa:', error);
        if (error.response) {
          console.error('Detalles del error:', error.response.data);
        }
      }
    },
    methods: {
      async getProducts() {
        try {
          const response = await axios.get(`${API_URL}/Product/${this.enterprise.name}`);
          this.products = response.data;
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

        const daysMap = ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"];

        return weekDaysString
            .split("")
            .map(day => daysMap[Number(day.trim())])
            .filter(Boolean)
            .join(", ");
      },
      formatProductType(type) {
        return type === "NonPerishable" ? "No perecedero" : type === "Perishable" ? "Perecedero" : "Desconocido";
      },
      openEditDialog(product) {
        this.editProductData = { ...product }; // Clona el producto seleccionado
        this.isEditDialogOpen = true;
      },
      closeEditDialog() {
        this.isEditDialogOpen = false;
        this.editProductData = {}; // Limpia los datos de edición
      },
      async getTagIdsByNames(tags) {
        const token = getToken();
        const tagIds = [];
        try {
          for (const tag of tags) {
            const response = await axios.get(`${API_URL}/Product/get-tags-by-name/${tag}`, {
              headers: { Authorization: `Bearer ${token}` }
            });
            tagIds.push(...response.data);
          }
        } catch (error) {
          console.error("Error al obtener IDs de los tags:", error);
        }
        return tagIds;
      },
      async uploadImages() {
        try {
          const formData = new FormData();
          formData.append("ownerId", this.editProductData.id); // Asigna el ProductId del producto en edición
          formData.append("ownerType", "Product");


          console.log("Imágenes seleccionadas:", this.editProductData.newImages);
          this.editProductData.newImages.forEach((file) => {
            formData.append("images", file); // Agrega cada imagen al formData
          });

          await axios.post(
            `${API_URL}/ImageFile/upload`,
            formData,
            {
              headers: {
                "Content-Type": "multipart/form-data",
                Authorization: `Bearer ${getToken()}`,
              },
            }
          );
          console.log("Imágenes subidas correctamente");
        } catch (error) {
          console.error("Error al subir imágenes:", error);
        }
      },
      onImageChange() {
        console.log("Imágenes seleccionadas:", this.editProductData.newImages);
      },
      async updateProduct() {
        try {
            console.log(this.editProductData); // Verificar datos

            // Subir imágenes solo si hay nuevas imágenes seleccionadas
            if (this.editProductData.newImages && this.editProductData.newImages.length > 0) {
              // Limpia la URL actual antes de subir la nueva imagen
              this.editProductData.imagesURLs = [];
              console.log("Subiendo nuevas imágenes...");
              await this.uploadImages();
            }

            // Obtener IDs de los tags antes de enviar los datos
            this.editProductData.tags = await this.getTagIdsByNames(this.editProductData.tags);

            // Crear un objeto limpio de los datos a actualizar
            const updatedProductData = {
                ...this.editProductData,
                enterpriseId: this.enterpriseId,
                price: Number(this.editProductData.price),
                stock: Number(this.editProductData.stock),
                limit: this.editProductData.limit ? Number(this.editProductData.limit) : null
            };

            // Si no se seleccionaron nuevas imágenes, eliminamos `imagesURLs` y `newImages` del payload
            if (!this.editProductData.newImages || this.editProductData.newImages.length === 0) {
                delete updatedProductData.imagesURLs;
                delete updatedProductData.newImages;
            }

            const token = getToken();

            await axios.put(
                `${API_URL}/Product`,
                updatedProductData,
                { headers: { Authorization: `Bearer ${token}` } }
            );

            this.isEditDialogOpen = false;

            await this.$swal.fire({
                title: 'Producto actualizado',
                text: '¡Su producto ha sido actualizado correctamente!',
                icon: 'success',
                confirmButtonText: 'Ok',
                backdrop: true,
                customClass: {
                    popup: 'swal-overlay',
                }
            });
            
            this.getProducts();
            this.closeEditDialog();
        } catch (error) {
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
  