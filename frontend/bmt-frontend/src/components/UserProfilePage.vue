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
            <v-row cols="12" class="mb-4">
              <v-col cols="12">
                <v-card-title>
                  <v-icon class="mr-2">mdi-email</v-icon>
                  Correo Electrónico
                </v-card-title>
                <v-card class="pa-3" outlined>
                  {{ user.email }}
                </v-card>
              </v-col>
            </v-row>
          </v-col>

          <v-col cols="12" md="6">
            <v-row cols="12" class="mb-4">
              <v-col cols="12">
                <v-card-title>
                  <v-icon class="mr-2">mdi-lock</v-icon>
                  Contraseña
                </v-card-title>
                <v-card class="pa-3" outlined>
                  <v-btn @click="togglePasswordVisibility" variant="text" icon size="x-small">
                    <v-icon>{{ showPassword ? 'mdi-eye-off' : 'mdi-eye' }}</v-icon>
                  </v-btn>
                  <span v-if="showPassword">{{ user.password }}</span>
                  <span v-else>•••••••••••••</span>
                </v-card>
              </v-col>
            </v-row>
          </v-col>
        </v-row>

        <!-- Sección de libreta de direcciones con botones -->
        <v-card-actions>
          <v-btn @click="toggleAddressSection" color="primary">
            <v-icon class="mr-2">mdi-map-marker</v-icon>
            Mi libreta de direcciones
            <v-icon>{{ isAddressOpen ? 'mdi-chevron-up' : 'mdi-chevron-down' }}</v-icon>
          </v-btn>
          <v-btn color="primary" @click="redirectToAddDirection">Agregar Dirección</v-btn>
        </v-card-actions>

        <!-- Lista de direcciones -->
        <v-slide-y-transition>
          <div v-if="isAddressOpen">
            <v-row>
              <v-col v-for="(direction, index) in directions" :key="index" cols="12" class="mb-3">
                <v-card class="mx-auto custom-card" width="100%" max-width="700" elevation="5" hover>
                  <v-card-title>{{ direction.numDirection }}</v-card-title>
                  <v-card-subtitle>{{ direction.coordinates }}</v-card-subtitle>
                  <v-divider></v-divider>
                  <v-card-text>{{ direction.otherSigns }}</v-card-text>
                  <v-card-actions>
                    <v-btn icon small color="primary" @click="openEditDialog(direction)">
                      <v-icon>mdi-pencil</v-icon>
                    </v-btn>
                    <v-btn icon small color="error" @click="deleteDirection(direction.id)">
                      <v-icon>mdi-delete</v-icon>
                    </v-btn>
                  </v-card-actions>
                </v-card>
              </v-col>
            </v-row>
          </div>
        </v-slide-y-transition>

        <!-- Diálogo para editar dirección -->
        <v-dialog v-model="isEditDialogOpen" max-width="600">
          <v-card>
            <v-card-title>Editar Dirección</v-card-title>
            <v-card-text>
              <v-form @submit.prevent="saveEditedDirection">
                <v-text-field label="Nombre de la dirección" v-model="currentDirection.numDirection"
                  required></v-text-field>

                <v-row class="my-4">
                  <v-col>
                    <GMapMap :center="editMapCenter" :zoom="12" style="width: 100%; height: 400px"
                      @click="updateMarker">
                      <Marker v-if="editMarker" :position="editMarker.position" />
                    </GMapMap>
                  </v-col>
                </v-row>

                <v-text-field label="Coordenadas" v-model="currentDirection.coordinates" readonly></v-text-field>

                <v-text-field label="Otras señas" v-model="currentDirection.otherSigns"></v-text-field>
              </v-form>
            </v-card-text>
            <v-card-actions>
              <v-btn color="secondary" @click="closeEditDialog">Cancelar</v-btn>
              <v-btn color="primary" @click="saveEditedDirection">Guardar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>

        <!-- Botones de tarjeta de crédito -->
        <v-card-actions>
          <v-btn @click="toggleCreditCardSection" color="primary">
            <v-icon class="mr-2">mdi-credit-card</v-icon>
            Mis Tarjetas
            <v-icon>{{ isCreditCardOpen ? 'mdi-chevron-up' : 'mdi-chevron-down' }}</v-icon>
          </v-btn>
          <v-btn color="primary" @click="redirectToAddCreditCard">Agregar Tarjeta</v-btn>
        </v-card-actions>

        <!-- Lista de tarjetas de crédito -->
        <v-slide-y-transition>
          <div v-if="isCreditCardOpen">
            <v-row>
              <v-col v-for="(card, index) in creditCards" :key="index" cols="12" class="mb-3">
                <v-card class="mx-auto custom-card" width="100%" max-width="700" elevation="5" hover>
                  <v-card-title>{{ card.name }}</v-card-title>
                  <v-card-subtitle>{{ card.number }}</v-card-subtitle>
                  <v-divider></v-divider>
                  <v-card-text>Fecha de Vencimiento: {{ card.dateVenc }}</v-card-text>
                  <v-card-actions>
                    <v-btn icon small color="error" @click="deleteCreditCard(card.id)">
                      <v-icon>mdi-delete</v-icon>
                    </v-btn>
                  </v-card-actions>
                </v-card>
              </v-col>
            </v-row>
          </div>
        </v-slide-y-transition>

        <!-- Botones de navegación al final -->
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
  components: {
    GMapMap,
    Marker,
  },
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
      creditCards: [],
      showPassword: false,
      isAddressOpen: false,
      isCreditCardOpen: false, // Añadido
      isEditDialogOpen: false,
      currentDirection: { id: '', numDirection: '', coordinates: '', otherSigns: '' },
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
      this.GetCreditCardsOfUser();
      this.imageURL = this.user.profilePictureURL ? `${URL}${this.user.profilePictureURL}` : '';
    }
  },

  methods: {
    // Métodos de dirección y edición
    goBack() {
      this.$router.push('/');
    },
    togglePasswordVisibility() {
      this.showPassword = !this.showPassword;
    },
    toggleAddressSection() {
      this.isAddressOpen = !this.isAddressOpen;
    },
    toggleCreditCardSection() {
      this.isCreditCardOpen = !this.isCreditCardOpen;
    },
    redirectToAddDirection() {
      this.$router.push('/register-address');
    },
    redirectToAddCreditCard() {
      this.$router.push('/card-form');
    },
    handleEditInfo() {
      this.$router.push('/profile/edit'); // Ruta corregida
    },
    async GetDirectionsOfUser() {
      const token = getToken();
      const user = JSON.parse(localStorage.getItem('user'));
      try {
        const response = await axios.post(
          `${API_URL}/Direction/ObtainDirectionsFromUser`,
          user,
          { headers: { Authorization: `Bearer ${token}` } }
        );
        this.directions = response.data;
      } catch (error) {
        console.error('Error al obtener las direcciones del usuario:', error);
        this.$swal.fire({
          title: 'Error',
          text: 'No se pudieron obtener las direcciones.',
          icon: 'error',
          confirmButtonText: 'Ok',
        });
      }
    },
    async GetCreditCardsOfUser() {
      const userId = JSON.parse(localStorage.getItem('user')).id;
      try {
        const result = await axios.get(
          `${API_URL}/CreditCard/User?userId=${encodeURIComponent(userId)}`
        );
        this.creditCards = result.data;
      } catch (error) {
        console.error('Error al obtener las tarjetas del usuario:', error);
        this.$swal.fire({
          title: 'Error',
          text: 'No se pudieron obtener las tarjetas de crédito.',
          icon: 'error',
          confirmButtonText: 'Ok',
        });
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
            const response = await axios.post(`${API_URL}/ImageFile/upload`, formData, {
              headers: { "Content-Type": "multipart/form-data" },
            });
            const uploadedImageURL = response.data.imageURL;
            this.imageURL = `${URL}${uploadedImageURL}`;

            let user = JSON.parse(localStorage.getItem('user')) || {};
            user.profilePictureURL = uploadedImageURL;
            localStorage.setItem('user', JSON.stringify(user));
          } catch (error) {
            console.error("Error al subir la imagen:", error);
            this.$swal.fire({
              title: 'Error',
              text: 'No se pudo subir la imagen.',
              icon: 'error',
              confirmButtonText: 'Ok',
            });
          }
        };
        reader.readAsDataURL(file);
      }
    },
    openEditDialog(direction) {
      this.isEditDialogOpen = true;
      this.currentDirection = { ...direction };

      // Parse coordinates for map center
      const [lat, lng] = direction.coordinates.split(', ').map(Number);
      this.editMapCenter = { lat, lng };
      this.editMarker = { position: this.editMapCenter };
    },
    updateMarker(event) {
      const position = {
        lat: event.latLng.lat(),
        lng: event.latLng.lng(),
      };
      this.editMarker = { position };
      this.currentDirection.coordinates = `${position.lat}, ${position.lng}`;
    },
    async saveEditedDirection() {
      const token = getToken();
      try {
        const response = await axios.put(
          `${API_URL}/Direction/UpdateDirection/${this.currentDirection.id}`,
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
          this.$swal.fire({
            title: 'Error',
            text: 'No se pudo actualizar la dirección.',
            icon: 'error',
            confirmButtonText: 'Ok',
          });
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
    async deleteDirection(directionId) {
      // Implementa la lógica para eliminar una dirección
      const token = getToken();
      try {
        await axios.delete(`${API_URL}/Direction/DeleteDirection/${directionId}`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        this.GetDirectionsOfUser();
        this.$swal.fire({
          title: 'Éxito',
          text: 'Dirección eliminada correctamente.',
          icon: 'success',
          confirmButtonText: 'Ok',
        });
      } catch (error) {
        console.error('Error al eliminar la dirección:', error);
        this.$swal.fire({
          title: 'Error',
          text: 'No se pudo eliminar la dirección.',
          icon: 'error',
          confirmButtonText: 'Ok',
        });
      }
    },
    async deleteCreditCard(cardId) {
      // Implementa la lógica para eliminar una tarjeta de crédito
      const token = getToken();
      try {
        await axios.delete(`${API_URL}/CreditCard/DeleteCard/${cardId}`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        this.GetCreditCardsOfUser();
        this.$swal.fire({
          title: 'Éxito',
          text: 'Tarjeta de crédito eliminada correctamente.',
          icon: 'success',
          confirmButtonText: 'Ok',
        });
      } catch (error) {
        console.error('Error al eliminar la tarjeta de crédito:', error);
        this.$swal.fire({
          title: 'Error',
          text: 'No se pudo eliminar la tarjeta de crédito.',
          icon: 'error',
          confirmButtonText: 'Ok',
        });
      }
    },
  },
};
</script>



<style scoped>
.profile-card {
  background-color: #9fc9fc;
  border-radius: 20px;
}

.custom-card {
  border-left: 5px solid #4e88e6;
  background-color: #ffffff;
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

.v-card-title {
  display: flex;
  align-items: center;
}

.v-btn {
  text-transform: none;
}

.v-snackbar {
  max-width: 400px;
}
</style>
