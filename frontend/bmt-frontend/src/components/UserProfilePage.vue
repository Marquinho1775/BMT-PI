<template>
  <v-main class="flex-grow-1">
    <v-container class="d-flex justify-center align-center">
      <v-card max-width="900" width="100%" class="profile-card pa-6">

        <!-- Header with Profile Picture, Name, and Username -->
        <v-row class="align-center custom-header">
          <div class="avatar-container" @click="triggerFileInput">
            <v-avatar :size="112" class="mr-4">
              <v-img :src="imageURL" class="profile-image"></v-img>
              <div class="overlay">
                <v-icon class="edit-icon">mdi-pencil</v-icon>
              </div>
            </v-avatar>
          </div>
          <input type="file" ref="fileInput" accept="image/*" @change="onFileChange" class="d-none" />
          <v-col class="d-flex flex-column">
            <h2 class="mb-1">{{ user.name }} {{ user.lastName }}</h2>
            <span class="text-body-2 grey--text">@{{ user.username }}</span>
          </v-col>
        </v-row>

        <!-- Email and Password Section -->
        <v-row class="mt-6">
          <v-col cols="12" md="6">
            <v-col cols="12" class="mb-4">
              <v-card-title>
                <v-icon class="mr-2">mdi-email</v-icon>
                Correo Electrónico
              </v-card-title>
              <v-card max-width="350">
                {{ user.email }}
              </v-card>
            </v-col>
          </v-col>

          <v-col cols="12" md="6">
            <v-card-title>
              <v-icon class="mr-2">mdi-lock</v-icon>
              Contraseña
            </v-card-title>
            <v-card max-width="350">
              <v-btn @click="togglePasswordVisibility" variant="plain" icon size="x-small">
                <v-icon>{{ showPassword ? 'mdi-eye-off' : 'mdi-eye' }}</v-icon>
              </v-btn>
              <span v-if="showPassword">{{ user.password }}</span>
              <span v-else>•••••••••••••</span>
            </v-card>
          </v-col>
        </v-row>

        <!-- Address Section with Toggle Button -->
        <v-card-actions>

          <v-btn @click="toggleAddressSection" color="primary">
            <v-icon class="mr-2">mdi-map-marker</v-icon>
            Mi libreta de direcciones
            <v-icon>{{ isAddressOpen ? 'mdi-chevron-up' : 'mdi-chevron-down' }}</v-icon>
          </v-btn>
          <v-btn color="primary" @click="redirectToAddDirection">Agregar Dirección</v-btn>
        </v-card-actions>


          <v-expand-transition>
            <div v-show="isAddressOpen">
              <v-col v-for="(direction, index) in directions" :key="index" cols="12" class="mb-3">
                <v-card class="mx-auto custom-card" width="100%" max-width="700" elevation="5" hover>
                  <v-card-item>
                    <v-card-title>{{ direction.numDirection }}</v-card-title>
                    <v-card-subtitle>
                      {{ direction.coordinates }}
                    </v-card-subtitle>
                  </v-card-item>
                  <v-divider></v-divider>
                  <v-card-text>{{ direction.otherSigns }}</v-card-text>
                  <v-card-actions>
                    <v-btn prepend-icon="mdi-pencil" size="x-small" color="primary" @click="openEditDialog(direction)">Editar</v-btn>
                    <v-btn prepend-icon="mdi-delete" size="x-small" color="error">Borrar</v-btn>
                  </v-card-actions>
                </v-card>
              </v-col>
            </div>
          </v-expand-transition>

          <!-- Edit Address Dialog -->
          <v-dialog v-model="isEditDialogOpen" max-width="600">
            <v-card>
              <v-card-title>Editar Dirección</v-card-title>
              <v-card-text>
                <v-form @submit.prevent="saveEditedDirection">
                  <v-text-field
                    label="Nombre de la dirección"
                    v-model="currentDirection.numDirection"
                    required
                  ></v-text-field>

                  <v-row class="my-4">
                    <v-col>
                      <GMapMap
                        :center="editMapCenter"
                        :zoom="12"
                        style="width: 100%; height: 400px"
                        @click="updateMarker"
                      >
                        <Marker v-if="editMarker" :position="editMarker.position" />
                      </GMapMap>
                    </v-col>
                  </v-row>
                  
                  <v-text-field
                    label="Coordenadas"
                    v-model="currentDirection.coordinates"
                    readonly
                  ></v-text-field>
                  
                  <v-text-field
                    label="Otras señas"
                    v-model="currentDirection.otherSigns"
                  ></v-text-field>
                </v-form>
              </v-card-text>
              <v-card-actions>
                <v-btn color="secondary" @click="closeEditDialog">Cancelar</v-btn>
                <v-btn color="primary" @click="saveEditedDirection">Guardar</v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>

          <!-- Action Buttons -->
          <v-row class="d-flex justify-space-between mt-4">
            <v-btn color="secondary" @click="goBack">Volver</v-btn>
            <v-btn color="primary" @click="handleEditInfo">Editar Perfil</v-btn>
          </v-row>
        </v-card>
      </v-container>
    </v-main>
</template>

<script>
import axios from 'axios';
import { API_URL, URL } from '@/main.js';
import { getToken } from '@/helpers/auth';
import { GMapMap, Marker } from '@fawmi/vue-google-maps'; // eslint-disable-line no-unused-vars

export default {
  data() {
    return {
      imageURL: '',
      user: {
        id: '',
        name: '',
        lastName: '',
        username: '',
        email: '',
        isVerified: false,
        password: '',
        role: '',
        profilePictureURL: '',
      },
      directions: [],
      showPassword: false,
      isAddressOpen: false,
      isEditDialogOpen: false,
      editAddressData: { numDirection: '', coordinates: '', otherSigns: '' },
      currentDirection: { numDirection: '', coordinates: '', otherSigns: '' },
      editMapCenter: { lat: 9.9359015, lng: -84.0506458 },
      editMarker: null,
    

    };
  },
  created() {
    if (!localStorage.getItem('token')) {
      window.location.href = "/login";
    } else {
      this.user = JSON.parse(localStorage.getItem('user')) || this.user;
      this.GetDirectionsOfUser();
      this.imageURL = this.user.profilePictureURL ? URL + this.user.profilePictureURL : '';
    }
  },
  methods: {
    goBack() {
      this.$router.push('/');
    },
    togglePasswordVisibility() {
      this.showPassword = !this.showPassword;
    },
    toggleAddressSection() {
      this.isAddressOpen = !this.isAddressOpen;
    },
    redirectToAddDirection() {
      this.$router.push('/register-address');
    },
    handleEditInfo() {
      this.$router.push(`profile/edit`);
    },
    async GetDirectionsOfUser() {
      const token = getToken();
      const user = JSON.parse(localStorage.getItem('user'));
      try {
        if (!user || !user.id) {
          throw new Error('El usuario no tiene todos los campos requeridos');
        }
        const response = await axios.post(
          `${API_URL}/Direction/ObtainDirectionsFromUser`,
          user,
          { headers: { Authorization: `Bearer ${token}` } }
        );
        this.directions = response.data;
      } catch (error) {
        console.error('Error al obtener las direcciones del usuario:', error);
      }
    },
    triggerFileInput() {
      this.$refs.fileInput.click();
    },
    onFileChange(event) {
      const file = event.target.files[0];
      if (file) {
        const reader = new FileReader();
        reader.onload = async (e) => {
          this.imageURL = e.target.result;
          
          const formData = new FormData();
          formData.append("ownerId", this.user.id);
          formData.append("ownerType", "User");
          formData.append("images", file);

          try {
            const response = await axios.post(API_URL + "/ImageFile/upload", formData, {
              headers: { "Content-Type": "multipart/form-data" },
            });
            const uploadedImageURL = response.data.imageURL;
            this.imageURL = URL + uploadedImageURL;

            let user = JSON.parse(localStorage.getItem('user')) || {};
            user.profilePictureURL = uploadedImageURL;
            localStorage.setItem('user', JSON.stringify(user));

          } catch (error) {
            console.error("Error al subir la imagen:", error);
          }
        };
        reader.readAsDataURL(file);
      }
    },
    openEditDialog(direction) {
      this.isEditDialogOpen = true;
      this.currentDirection = { ...direction };
      
      // Parse coordinates for map center
      const [lat, lng] = direction.coordinates.split(', ');
      this.editMapCenter = { lat: parseFloat(lat), lng: parseFloat(lng) };
      this.editMarker = { position: this.editMapCenter };
    },
    updateMarker(event) {
      const position = { lat: event.latLng.lat(), lng: event.latLng.lng() };
      this.editMarker = { position };
      this.currentDirection.coordinates = `${position.lat}, ${position.lng}`;
    },
    async saveEditedDirection() {
      const token = getToken();
      try {
        const response = await axios.put(
          `${API_URL}/Direction/UpdateDirection/${this.currentDirection.id}`,  // Cambiar numDirection por Id
          this.currentDirection,
          { headers: { Authorization: `Bearer ${token}` } }
        );

        if (response.data) {
          this.GetDirectionsOfUser();
          this.isEditDialogOpen = false;

          await this.$swal.fire({
            title: 'Dirección actualizada',
            text: '¡Su dirección ha sido actualizada correctamente!',
            icon: 'success',
            confirmButtonText: 'Ok',
          });
        } else {
          console.error('Error: No se pudo actualizar la dirección.');
        }
      } catch (error) {
        console.error('Error al guardar la dirección:', error);

        this.$swal.fire({
          title: 'Error',
          text: 'Hubo un error al actualizar su dirección. Inténtelo de nuevo.',
          icon: 'error',
          confirmButtonText: 'Ok',
        });
      }
    },
    closeEditDialog() {
      this.isEditDialogOpen = false;
    },
  }
};
</script>

<style scoped>
.profile-card {
  background-color: #9FC9FC;
  border-radius: 20px;
}

.custom-card {
  border-left: 5px solid #4e88e6;
  background-color: #FFFFFF;
}

.text-body-2 {
  font-size: 14px;
  color: #6c757d;
}

.flex-grow-1 {
  flex-grow: 1;
}

.avatar-container {
  position: relative;
  cursor: pointer;
}

.profile-image {
  transition: 0.3s ease;
}

.overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: 0.3s ease;
  border-radius: 50%;
}

.avatar-container:hover .profile-image {
  filter: brightness(0.5);
}

.avatar-container:hover .overlay {
  opacity: 1;
}

.edit-icon {
  color: white;
  font-size: 32px;
}
</style>