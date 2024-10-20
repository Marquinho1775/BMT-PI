<template>

  <div class="d-flex justify-content-center align-items-center vh-100">
    <b-form @submit.prevent="submitForm" @reset="onReset">

      <div class="card custom-card my-4">
        <h3 class="text-center card-header-custom">Datos del emprendedor</h3>
        <div class="card-body">

          <b-form-group label="Número de identificación"
            label-for="identification-number-e">
            <b-form-input id="identification-number-e" class="form-input" v-model="entrepreneurData.identification"
              placeholder="Ingrese su número de cédula" required>
            </b-form-input>
          </b-form-group>

        </div>
      </div>

      <div id="form" class="card custom-card my-4">
        <h3 class="text-center card-header-custom">Datos de la empresa</h3>
        <div class="card-body">

          <b-form-group id="selection-group-id-type" label="Seleccione el tipo de identificación de su negocio" label-for="select-id-type">
            <b-form-select id="select-id-type" class="form-input" v-model="enterpriseData.identificationType" required
              :options="idTypeOptions">
            </b-form-select>
          </b-form-group>

          <b-form-group id="group-identification-number-en" label="Número de identificación"
            label-for="identification-number-en">
            <b-form-input id="identification-number-en" class="form-input" v-model="enterpriseData.identificationNumber"
              placeholder="Ingresar número de identificación" required>
            </b-form-input>
          </b-form-group>

          <b-form-group id="group-name" label="Nombre:" label-for="name">
            <b-form-input id="name" class="form-input" v-model="enterpriseData.name"
              placeholder="Ingresar el nombre de su emprendimiento" required>
            </b-form-input>
          </b-form-group>

          <b-form-group id="group-description" label="Descripción:" label-for="description">
            <b-form-textarea id="description" class="form-input" v-model="enterpriseData.description"
              placeholder="Ingresar una descripción de su emprendimiento" rows="4" max-rows="6" no-resize>
            </b-form-textarea>
          </b-form-group>

        </div>
      </div>

      <div class="d-flex justify-content-between">
        <b-button variant="secondary" @click="goBack">Volver</b-button>
        <b-button type="submit" class="button">Registrar</b-button>
      </div>

    </b-form>
  </div>

</template>

<script>
import axios from 'axios';
import { API_URL } from '@/main.js';

export default {
  data() {
    return {
      idTypeOptions: [
        { value: 1, text: 'Persona física' },
        { value: 2, text: 'Persona jurídica' },
        { value: null, text: 'Seleccione una de las anteriores', disabled: true }
      ],
      entrepreneurData: {
        identification: '',
        username: '',
      },
      enterpriseData: {
        identificationType: null,
        identificationNumber: '',
        name: '',
        description: '',
      }
    };
  },
  methods: {
    generateSweetAlert(title, icon, text) {
      this.$swal.fire({ title, icon, confirmButtonText: 'Ok', text });
    },
    async submitForm() {
      try {
        const entrepreneurResponse = await this.checkExistingEntrepreneur();
        if (entrepreneurResponse) {
          this.generateSweetAlert('Error', 'error', 'Ya existe un emprendedor registrado con este número de identificación.');
          this.restoreForm();
          return;
        }
        let enterpriseResponse = await this.checkExistingEnterprise(this.enterpriseData.identificationNumber);
        if (enterpriseResponse) {
          this.generateSweetAlert('Error', 'error', 'Ya existe una empresa registrada con este número de identificación.');
          this.restoreForm();
          return;
        }
        enterpriseResponse = await this.checkExistingEnterprise(this.enterpriseData.name);
        if (enterpriseResponse) {
          this.generateSweetAlert('Error', 'error', 'Ya existe una empresa registrada con este nombre.');
          this.restoreForm();
          return;
        }
        await this.registerEntrepreneur();
        await this.changeRole();
        await this.registerEnterprise();
        await this.addEntrepreneurToEnterprise();
        this.generateSweetAlert('Registro exitoso', 'success', 'Se ha registrado exitosamente su empresa.');
        this.$router.push('/entrepreneur-home');
      } catch (error) {
        this.generateSweetAlert('Error', 'error', 'Ocurrió un error durante el registro. Por favor, inténtalo de nuevo.');
        console.error(error);
      }
    },

    async checkExistingEntrepreneur() {
      try {
        const response = await axios.get(`${API_URL}/Entrepreneur/CheckExistingEntrepreneur?identification=${this.entrepreneurData.identification.trim()}`);
        return response.data;
      } catch (error) {
        this.generateSweetAlert('Error', 'error', 'Hubo un error al verificar si el emprendedor ya existe.');
        console.error(error);
        throw error;
      }
    },

    async registerEntrepreneur() {
      try {
        const response = await axios.post(`${API_URL}/Entrepreneur`, {
          id: '',
          username: JSON.parse(localStorage.getItem('user')).username,
          identification: this.entrepreneurData.identification.trim(),
        });
        return response.data;
      } catch (error) {
        this.generateSweetAlert('Error', 'error', 'Hubo un error al registrarse como emprendedor.');
        console.error(error);
        throw error;
      }
    },

    async changeRole() {
      try {
        const userId = JSON.parse(localStorage.getItem('user')).id;
        const role = 'emp';
        const url = `${API_URL}/User/Role?id=${encodeURIComponent(userId)}&role=${encodeURIComponent(role)}`;
        await axios.post(url);
      } catch (error) {
        this.generateSweetAlert('Error', 'error', 'Hubo un error al cambiar el rol del usuario.');
        console.error(error);
        throw error;
      }
    },

    async checkExistingEnterprise(identification) {
      try {
        const response = await axios.get(`${API_URL}/Enterprise/CheckExistingEnterprise?identification=${identification.trim()}`);
        return response.data;
      } catch (error) {
        this.generateSweetAlert('Error', 'error', 'Hubo un error al verificar si la empresa ya existe.');
        console.error(error);
        throw error;
      }
    },

    async registerEnterprise() {
      try {
        const response = await axios.post(`${API_URL}/Enterprise`, {
          id: '',
          identificationType: parseInt(this.enterpriseData.identificationType),
          identificationNumber: this.enterpriseData.identificationNumber.trim(),
          name: this.enterpriseData.name.trim(),
          description: this.enterpriseData.description.trim(),
        }); 
        return response.data;
      } catch (error) {
        this.generateSweetAlert('Error', 'error', 'Hubo un error al registrar su empresa. Inténtalo de nuevo.');
        console.error(error);
        throw error;
      }
    },

    async addEntrepreneurToEnterprise() {
      try {
        await axios.post(`${API_URL}/Entrepreneur/add-to-enterprise`, {
          entrepreneurIdentification: this.entrepreneurData.identification.trim(),
          enterpriseIdentification: this.enterpriseData.identificationNumber.trim(),
          isAdmin: true,
        });
      } catch (error) {
        this.generateSweetAlert('Error', 'error', 'Hubo un error al agregar el emprendedor a la empresa.');
        console.error(error);
        throw error;
      }
    },

    onReset(event) {
      event.preventDefault();
      this.restoreForm();
    },
    restoreForm() {
      this.entrepreneurData.identification = '';
      this.enterpriseData.identificationType = null;
      this.enterpriseData.identificationNumber = '';
      this.enterpriseData.name = '';
      this.enterpriseData.description = '';
    },
    goBack() {
      this.$router.push('/client-home');
    },
  },

};
</script>

<style>

</style>