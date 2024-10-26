<template>
  <v-app>
    <v-app-bar :elevation="5" app color="#9FC9FC" scroll-behavior="hide" dark>
      <v-toolbar-title>Business Tracker</v-toolbar-title>
      <v-spacer></v-spacer>
    </v-app-bar>

    <v-container class="d-flex justify-center align-center" style="min-height: 100vh;">
      <v-form @submit.prevent="handleSubmit" style="width: 100%; max-width: 500px;">
        <h1 v-if="title" class="text-center mb-5">{{ title }}</h1>
        <v-text-field
          v-model="verificationCode"
          label="Ingresa tu código"
          required
        ></v-text-field>
        <v-btn @click="goBack" color="secondary" class="mr-2">Volver</v-btn>
        <v-btn type="submit" color="primary">Verificar Código</v-btn>
        <v-alert v-if="successMessage" type="success" class="mt-4">
          {{ successMessage }}
        </v-alert>
        <v-alert v-if="errorMessage" type="error" class="mt-4">
          {{ errorMessage }}
        </v-alert>
      </v-form>
    </v-container>

    <v-footer app padless color="#9FC9FC" dark>
      <v-col class="text-center white--text">
        &copy; 2024 Business Tracker. Todos los derechos reservados.
      </v-col>
    </v-footer>
  </v-app>
</template>

<script>
import axios from 'axios';
import { API_URL } from '@/main.js';

export default {
  data() {
    return {
      title: 'Verificación de Código',
      verificationCode: '',
      successMessage: '',
      errorMessage: ''
    };
  },
  methods: {
    goBack() {
      window.location.href = "/";
    },
    async handleSubmit() {
      try {
        const response = await axios.post(`${API_URL}/email/verifycode`, {
          Code: this.verificationCode,
          Id: this.selectedEnterprise 
        });

        if (response.status === 200) {
          this.successMessage = 'Código verificado exitosamente';
          this.errorMessage = '';
        }
      } catch (error) {
        console.error('Error al verificar el código:', error);
        this.errorMessage = 'No se pudo verificar el código';
        this.successMessage = '';
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