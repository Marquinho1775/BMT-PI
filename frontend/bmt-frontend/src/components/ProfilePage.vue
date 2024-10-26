<template>
  <v-container class="fill-height d-flex justify-center align-center">
    <v-card max-width="900" width="100%" class="profile-card pa-6">
      <!-- Header with Profile Picture, Name, and Username -->
      <v-row class="align-center">
        <v-avatar :size="112" class="mr-4">
          <v-img :src="imageURL"></v-img>
        </v-avatar>

        <v-col class="d-flex flex-column">
          <h2 class="mb-1">{{ user.name }} {{ user.lastName }}</h2>
          <span class="text-body-2 grey--text">@{{ user.username }}</span>
        </v-col>
      </v-row>

      <!-- Profile Information Section -->
      <v-row class="mt-6">
        <!-- Email and Password Section -->
        <v-col cols="12" md="6">
          <v-row>
            <v-col cols="12" class="mb-4">
              <v-card-title>
                <v-icon class="mr-2">mdi-email</v-icon>
                Correo Electrónico
              </v-card-title>
              <v-card max-width="350">
                {{ user.email }}
              </v-card>
            </v-col>

            <v-col cols="12" class="mb-4">
              <v-card-title>
                <v-icon class="mr-2">mdi-lock</v-icon>
                Contraseña
              </v-card-title>
              <v-card max-width="350">
                <span v-if="showPassword">{{ user.password }}</span>
                <span v-else>••••••••</span>
                <v-btn @click="togglePasswordVisibility" icon size="x-small">
                  <v-icon>{{ showPassword ? 'mdi-eye-off' : 'mdi-eye' }}</v-icon>
                </v-btn>
              </v-card>
            </v-col>
          </v-row>
        </v-col>

        <!-- Address Section with Toggle Button -->
        <v-col cols="12" md="6">
          <v-btn @click="toggleAddressSection" color="primary">
            <v-icon class="mr-2">mdi-map-marker</v-icon>
            Mi libreta de direcciones
            <v-icon>{{ isAddressOpen ? 'mdi-chevron-up' : 'mdi-chevron-down' }}</v-icon>
          </v-btn>

          <v-expand-transition>
            <div v-show="isAddressOpen">
              <v-row>
                <v-col v-for="(direction, index) in directions" :key="index" cols="12" class="mb-3">
                  <v-card class="mx-auto custom-card" max-width="400" elevation="5" hover>
                    <v-card-item>
                      <v-card-title>{{ direction.numDirection }}</v-card-title>
                      <v-card-subtitle>
                        {{ direction.province }}, {{ direction.canton }}, {{ direction.district }}
                      </v-card-subtitle>
                    </v-card-item>
                    <v-divider></v-divider>
                    <v-card-text>{{ direction.otherSigns }}</v-card-text>
                    <v-card-actions>
                      <v-btn prepend-icon="mdi-pencil" size="x-small" color="primary">Editar</v-btn>
                      <v-btn prepend-icon="mdi-delete" size="x-small" color="error">Borrar</v-btn>
                    </v-card-actions>
                  </v-card>
                </v-col>
              </v-row>
            </div>
          </v-expand-transition>
        </v-col>
      </v-row>

      <!-- Action Buttons -->
      <v-row class="d-flex justify-space-between mt-4">
        <v-btn color="secondary" @click="goBack">Regresar</v-btn>
        <v-btn color="primary" @click="redirectToAddDirection">Agregar Dirección</v-btn>
      </v-row>
    </v-card>
  </v-container>
</template>

<script>
import axios from 'axios';
import { API_URL, URL } from '@/main.js';
import { getToken } from '@/helpers/auth';

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
      isAddressOpen: false, // Controla si se muestra la sección de direcciones
    };
  },
  created() {
    if (!localStorage.getItem('token')) {
      window.location.href = "/login";
    } else {
      this.user = JSON.parse(localStorage.getItem('user')) || this.user;
      this.GetDirectionsOfUser();
      this.imageURL = URL + this.user.profilePictureURL;
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
    async GetDirectionsOfUser() {
      const token = getToken();
      const user = JSON.parse(localStorage.getItem('user'));
      try {
        if (!user || !user.id) {
          throw new Error('El usuario no tiene todos los campos requeridos');
        }
        const response = await axios.post(
          API_URL + '/Direction/ObtainDirectionsFromUser',
          user,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        this.directions = response.data;
      } catch (error) {
        console.error('Error al obtener las direcciones del usuario:', error);
      }
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
</style>
