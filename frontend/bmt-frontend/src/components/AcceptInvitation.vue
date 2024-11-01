<template>
	<v-app class="d-flex flex-column">
		<AppHeader />
    <v-container v-if = "loggedIn" class="d-flex justify-center align-center" style="min-height: 100vh;">
      <v-form @submit.prevent="handleSubmit" style="width: 100%; max-width: 500px;">
        <h1 v-if="title" class="text-center mb-5">{{ title }}</h1>
        <v-text-field
          v-model="enterpriseCode"
          label="Ingresa el código de la empresa"
          required
        ></v-text-field>
        <v-text-field
          v-model="verificationCode"
          label="Ingresa tu código de seguridad"
          required
        ></v-text-field>
        <v-btn @click="goBack" color="secondary" class="mr-2">Volver</v-btn>
        <v-btn @click="handleSubmit" type="submit" color="primary">Verificar Códigos</v-btn>
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
          <h2 class="text-center">{{ enterprise.name }}</h2>
        </v-card-title>
        <v-card-text>
          <p><strong>Nombre de la Empresa:</strong> {{ enterprise.name || 'No disponible' }}</p>
          <p><strong>Descripción:</strong> {{ enterprise.description || 'No disponible' }}</p>
          <v-container>
          <v-form v-if = "!isEntrepeneur" ref="form" v-model="valid" lazy-validation>
            <v-text-field
              v-model="user.Identification"
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
import {
  API_URL
} from '@/main.js';
import {
  getToken
} from '@/helpers/auth';

export default {
  async created() {
    const token = getToken();
    if (!localStorage.getItem('token')) {
      window.location.href = "/login";
    } else {
      this.loggedIn = true;
      this.user = JSON.parse(localStorage.getItem('user')) || this.user;
      this.user.userId = this.user.id;
    }
    if (this.user.role === 'emp') {
      this.isEntrepeneur = true;
      const obtainEntrepreneurResponse = await axios.post(API_URL + '/Entrepreneur/GetEntrepreneurByUserId?id=' + this.user.id, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      this.entrepreneur = obtainEntrepreneurResponse.data;
    }
  },
  data() {
    return {
      isEntrepeneur: false,
      loggedIn: false,
      title: 'Verificación de Código',
      verificationCode: '',
      enterpriseCode: '',
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
      enterprise: {},
      entrepreneur: {
        Id: '',
        Username: '',
        Identification: '',
      }
    };
  },
  methods: {

    async handleSubmit() {
      try {
        const token = getToken();
        const enterpriseResponse = await axios.get(API_URL + `/Enterprise/${this.enterpriseCode}`, {
          headers: {
            Authorization: `Bearer ${token}`
          }
        });

        const codeTaken = {
          Code: this.verificationCode,
          Id: this.user.userId
        };
        await axios.post(API_URL + '/Email/verifycode', codeTaken);

        this.enterprise = enterpriseResponse.data;
        if (this.enterprise) {
          this.enterpriseExist = true;
          this.$swal.fire({
            title: 'Código Verificado correctamente!',
            text: '¡Verificación exitosa!',
            icon: 'success',
            confirmButtonText: 'Ok'
          });
        }
        this.enterpriseExist = true;
        this.loggedIn = false;
      } catch (error) {
        this.$swal.fire({
          title: 'Error',
          text: 'Hubo un error al verificar el código. Inténtalo de nuevo.',
          icon: 'error',
          confirmButtonText: 'Ok'
        });
      }
    },

    async acceptInvitation() {
      try {
        if (this.isEntrepeneur === false) {
          await this.registerEntrepreneur();
        }

        const obtainEntrepreneurResponse = await axios.post(API_URL + '/Entrepreneur/GetEntrepreneurByUserId?id=' + this.user.userId);
        this.entrepreneur = obtainEntrepreneurResponse.data;

        await this.addEntrepreneurToEnterprise(this.entrepreneur.identification, this.enterprise.identificationNumber);
        console.log('Entrepreneur added to enterprise');

        this.$swal.fire({
          title: 'Ahora formas parte de una empresa!',
          text: '¡Bienvenido!',
          icon: 'success',
          confirmButtonText: 'Ok'
        });
      } catch (error) {
        console.error('Error al anexarte a la empresa:', error);
        this.$swal.fire({
          title: 'Error',
          text: 'Hubo un error al anexarte a la empresa. Inténtalo de nuevo.',
          icon: 'error',
          confirmButtonText: 'Ok'
        });
      }
      this.sendConfirmationMail();
      this.goBack();
    },

    async sendConfirmationMail(){
      await axios.post(API_URL + '/Email/sendconfirmationmail', {
        Email: this.enterprise.email,
        Id: this.user.username
      });
    },

    async registerEntrepreneur() {
      try {
        const response = await axios.post(`${API_URL}/Entrepreneur`, {
          id: '',
          username: JSON.parse(localStorage.getItem('user')).username,
          identification: this.identification,
        });
        const userId = JSON.parse(localStorage.getItem('user')).id;
        await axios.post(`${API_URL}/User/Role?id=${encodeURIComponent(userId)}&role=${encodeURIComponent('emp')}`);
        return response.data;
      } catch (error) {
        this.$swal.fire({
          title: 'Error',
          text: 'Hubo un error al anexarte a la empresa. Inténtalo de nuevo.',
          icon: 'error',
          confirmButtonText: 'Ok'
        });
      }
    },

    async addEntrepreneurToEnterprise(entrepreneurIdentification, enterpriseIdentification) {
      try {
        await axios.post(`${API_URL}/Entrepreneur/add-to-enterprise`, {
          entrepreneurIdentification: entrepreneurIdentification,
          enterpriseIdentification: enterpriseIdentification,
          isAdmin: false,
        });
        this.$swal.fire({
          title: 'Ahora formas parte de una empresa!',
          text: '¡Bienvenido!',
          icon: 'success',
          confirmButtonText: 'Ok'
        });
      } catch (error) {
        this.$swal.fire({
          title: 'Error',
          text: 'Hubo un error al anexarte a la empresa. Inténtalo de nuevo.',
          icon: 'error',
          confirmButtonText: 'Ok'
        });
      }
    },

    goBack() {
      window.location.href = "/";
    },
  }
};
</script>

<style scoped>
.v-container {
  max-width: 600px;
  margin: 0 auto;
}
</style>
