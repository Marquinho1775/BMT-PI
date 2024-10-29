<template>
	<v-app class="d-flex flex-column">
		<AppHeader />
    <v-container v-if = "loggedIn" class="d-flex justify-center align-center" style="min-height: 100vh;">
      <v-form @submit.prevent="handleSubmit" style="width: 100%; max-width: 500px;">
        <h1 v-if="title" class="text-center mb-5">{{ title }}</h1>
        <v-text-field
          v-model="verificationCode"
          label="Ingresa tu código"
          required
        ></v-text-field>
        <v-btn @click="goBack" color="secondary" class="mr-2">Volver</v-btn>
        <v-btn @click="handleSubmit" type="submit" color="primary">Verificar Código</v-btn>
        <v-alert v-if="successMessage" type="success" class="mt-4">
          {{ successMessage }}
        </v-alert>
        <v-alert v-if="errorMessage" type="error" class="mt-4">
          {{ errorMessage }}
        </v-alert>
      </v-form>
    </v-container>
    <v-container v-if="enterpriseExist" class="d-flex justify-center align-center" style="min-height: 100vh;">
      <v-card class="pa-5">
        <v-card-title>
          <h2 class="text-center">{{ enterprise.enterpriseName }}</h2>
        </v-card-title>
        <v-card-text>
          <p><strong>Nombre de la Empresa:</strong> {{ enterprise.enterpriseName || 'No disponible' }}</p>
          <p><strong>Descripción:</strong> {{ enterprise.enterpriseDescription || 'No disponible' }}</p>
          <v-container>
          <v-form v-if = 'isEntrepeneur' ref="form" v-model="valid" lazy-validation>
            <v-text-field
              v-model="this.Identification"
              label="Cédula"
              maxlength="9"
              pattern="\d{9}"
              required
              :rules="[
                v => !!v || 'Requerido',
                v => (v.length === 9) || 'La cédula debe contener exactamente 9 dígitos',
                v => /^\d+$/.test(v) || 'Solo se permiten dígitos'
              ]"
              @keydown="handleInput"
            ></v-text-field>
          </v-form>
  </v-container>
        </v-card-text>
        <v-row class="mt-4" justify="space-between">
          <v-col cols="auto">
            <v-btn color="red" @click="declineInvitation">Declinar Invitación</v-btn>
          </v-col>
          <v-col cols="auto">
            <v-btn color="green" @click="acceptInvitation">Aceptar Invitación</v-btn>
          </v-col>
        </v-row>
      </v-card>
    </v-container>
    <AppFooter />
  </v-app>
</template>

<script>
  import axios from 'axios';
  import { API_URL } from '@/main.js';
  import { getToken } from '@/helpers/auth';

  export default {
    async created() {
      const token = getToken();
      if (!localStorage.getItem('token')) {
        window.location.href = "/login";
      } else {
        this.loggedIn = true;
        this.user = JSON.parse(localStorage.getItem('user')) || this.user;
      }
      if (this.user.role === 'emp') {
        this.isEntrepeneur = true;
        const obtainEntrepreneurResponse = await axios.post(API_URL + '/Entrepreneur/GetEntrepreneurByUserId?id=' + this.user.id,
          {
            headers: {
              Authorization: `Bearer ${token}`
            }
          }
        );
        this.entrepreneur = obtainEntrepreneurResponse.data;
      }
    },
    data() {
      return {
        isEntrepeneur: false,
        loggedIn: false,
        title: 'Verificación de Código',
        verificationCode: '',
        successMessage: '',
        errorMessage: '',
        enterpriseExist: false,
        user: {
          userId: '',
          Identification: '',
          username: '',
          email: '',
          role: '',
        },
        enterprise: {
          enterpriseId: '',
          enterpriseName: '',
          enterpriseDescription: '',
        },
        entrepreneur: {
          Id: '',
          Username: '',
          Identification: '',
        }
      };
    },
    methods: {
      goBack() {
        window.location.href = "/";
      },
      async declineInvitation(){
        this.goBack();
      },
      async acceptInvitation() {
        const token = getToken();
        try {
          const requestPayload = {
            EntrepreneurIdentification: this.Identification,
            EnterpriseIdentification: this.verificationCode,
            IsAdmin: false
          };

          const response = await axios.post(`${API_URL}/Entrepreneur/add-to-enterprise`, requestPayload, {
            headers: { Authorization: `Bearer ${token}` }
          });

          if (response.status === 200) {
            this.successMessage = 'Invitación aceptada exitosamente';
            this.errorMessage = '';
            this.goBack();
          }
        } catch (error) {
          console.error('Error al aceptar la invitación:', error);
          this.errorMessage = 'No se pudo aceptar la invitación';
          this.successMessage = '';
        }
      },
      async handleSubmit() {
        try {
          const token = getToken();
          const enterpriseResponse = await axios.get(API_URL + `/Enterprise/${this.verificationCode}`, {
            headers: { Authorization: `Bearer ${token}` }
          });
          this.enterprise = enterpriseResponse.data;
          this.enterpriseExist = true;
        } catch (error) {
          console.error('Error al cargar la empresa:', error);
          if (error.response) {
            console.error('Detalles del error:', error.response.data);
          }
        }
      }
    }
  };
</script>

<style scoped>
  .v-container {
    max-width: 600px;
    margin: 0 auto;
  }
</style>