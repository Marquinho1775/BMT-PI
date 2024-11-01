<template>
  <v-main>

    <div class="d-flex justify-content-center align-items-center vh-100">
      <b-form @submit.prevent="submitForm" @reset="onReset">
        <div v-if="userRole === 'cli'" class="card custom-card my-4">
          <h3 class="text-center card-header-custom">Datos del emprendedor</h3>
          <div class="card-body">
            <b-form-group label="Número de identificación" label-for="identification-number-e">
              <b-form-input id="identification-number-e" class="form-input" v-model="entrepreneurData.identification"
                placeholder="Ingrese su número de cédula" required :state="identificationValid1"
                @input="validateIdentification1">
              </b-form-input>
              <b-form-invalid-feedback v-if="!identificationValid1">El número de cédula debe tener 9
                dígitos.</b-form-invalid-feedback>
            </b-form-group>
          </div>
        </div>

        <div id="form" class="card custom-card my-4">
          <h3 class="text-center card-header-custom">Datos de la empresa</h3>
          <div class="card-body">

            <b-form-group id="selection-group-id-type" label="Seleccione el tipo de identificación de su negocio"
              label-for="select-id-type">
              <b-form-select id="select-id-type" class="form-input" v-model="enterpriseData.identificationType" required
                :options="idTypeOptions" @change="validateIdentification2">
              </b-form-select>
            </b-form-group>

            <b-form-group id="group-identification-number-en" label="Número de identificación" k
              label-for="identification-number-en">
              <b-form-input id="identification-number-en" class="form-input"
                v-model="enterpriseData.identificationNumber" placeholder="Ingresar número de identificación" required
                :state="identificationValid2" @input="validateIdentification2">
              </b-form-input>
              <b-form-invalid-feedback v-if="identificationValid2 === false">
                El número de identificación debe tener {{ enterpriseData.identificationType === 1 ? '9' : '10' }}
                dígitos.
              </b-form-invalid-feedback>
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

            <b-form-group id="group-email" label="Correo electrónico:" label-for="email">
              <b-form-input id="email" class="form-input" v-model="enterpriseData.email"
                placeholder="Ingresar correo electrónico" type="email" required :state="emailValid"
                @input="validateEmail">
              </b-form-input>
              <b-form-invalid-feedback v-if="!emailValid">El correo debe tener el formato
                xxxx@xxx.xxx</b-form-invalid-feedback>
            </b-form-group>

            <b-form-group id="group-phone-number" label="Número de teléfono:" label-for="phone-number">
              <b-form-input id="phone-number" class="form-input" v-model="enterpriseData.phoneNumber"
                placeholder="Ingresar número de teléfono" required :state="phoneValid" @input="validatePhone">
              </b-form-input>
              <b-form-invalid-feedback v-if="!phoneValid">El número de teléfono debe tener 8
                dígitos.</b-form-invalid-feedback>
            </b-form-group>
          </div>
        </div>
        <div class="d-flex justify-content-between">
          <b-button variant="secondary" @click="goBack">Volver</b-button>
          <b-button type="submit" class="button">Registrar</b-button>
        </div>
      </b-form>
    </div>
  </v-main>
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
        email: '',
        phoneNumber: '',
      },
      identificationValid1: null,
      identificationValid2: null,
      emailValid: null,
      phoneValid: null,
      userRole: '',
    };
  },
  mounted() {
    this.getUserRole();
  },
  methods: {
    getUserRole() {
      const user = JSON.parse(localStorage.getItem('user')) || {};
      this.userRole = user.role || '';
    },
    generateSweetAlert(title, icon, text) {
      this.$swal.fire({ title, icon, confirmButtonText: 'Ok', text });
    },
    async submitForm() {
      try {
        let entrepreneurIdentification = this.entrepreneurData.identification.trim();
        if (entrepreneurIdentification !== '') {
          const entrepreneurExists = await this.checkExistingEntrepreneur();
          if (entrepreneurExists) {
            this.generateSweetAlert('Error', 'error', 'Ya existe un emprendedor registrado con este número de identificación.');
            return;
          }
          await this.registerEntrepreneur();
          await this.changeRole();
        } else {
          entrepreneurIdentification = await this.getExistingEntrepreneur();
        }
        const enterpriseChecks = [
          { value: this.enterpriseData.identificationNumber, message: 'Ya existe una empresa registrada con este número de identificación.', },
          { value: this.enterpriseData.name, message: 'Ya existe una empresa registrada con este nombre.', },
          { value: this.enterpriseData.email, message: 'Ya existe una empresa registrada con este correo electrónico.', },
          { value: this.enterpriseData.phoneNumber, message: 'Ya existe una empresa registrada con este número de teléfono.', },
        ];
        for (const check of enterpriseChecks) {
          const enterpriseExists = await this.checkExistingEnterprise(check.value);
          if (enterpriseExists) {
            this.generateSweetAlert('Error', 'error', check.message);
            return;
          }
        }
        await this.registerEnterprise();
        await this.addEntrepreneurToEnterprise(entrepreneurIdentification, this.enterpriseData.identificationNumber);
        this.generateSweetAlert('Registro exitoso', 'success', 'Se ha registrado exitosamente su empresa.');
        this.$router.push('/');
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
        const user = JSON.parse(localStorage.getItem('user'));
        const userId = user.id;
        const role = 'emp';
        const url = `${API_URL}/User/Role?id=${encodeURIComponent(userId)}&role=${encodeURIComponent(role)}`;
        await axios.post(url);
        user.role = role;
        localStorage.setItem('user', JSON.stringify(user));
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
          email: this.enterpriseData.email.trim(),
          phoneNumber: this.enterpriseData.phoneNumber.trim(),
        });
        return response.data;
      } catch (error) {
        this.generateSweetAlert('Error', 'error', 'Hubo un error al registrar su empresa. Inténtalo de nuevo.');
        console.error(error);
        throw error;
      }
    },

    async addEntrepreneurToEnterprise(entrepreneurId, enterpriseId) {
      try {
        await axios.post(`${API_URL}/Entrepreneur/add-to-enterprise`, {
          entrepreneurIdentification: entrepreneurId.trim(),
          enterpriseIdentification: enterpriseId.trim(),
          isAdmin: true,
        });
      } catch (error) {
        this.generateSweetAlert('Error', 'error', 'Hubo un error al agregar el emprendedor a la empresa.');
        console.error(error);
        throw error;
      }
    },

    async getExistingEntrepreneur() {
      const user = JSON.parse(localStorage.getItem('user'));
      let response = await axios.post(API_URL + '/Entrepreneur/GetEntrepreneurByUserId?id=' + user.id, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`,
        },
      });
      return response.data.identification;
    },

    validateIdentification1() {
      const regex = /^[0-9]{9}$/;
      this.identificationValid1 = regex.test(this.entrepreneurData.identification);
    },

    validateIdentification2() {
      const idNumber = this.enterpriseData.identificationNumber.trim();
      const idType = this.enterpriseData.identificationType;
      let isValid = false;
      if (idType === 1) {
        const regex = /^[0-9]{9}$/;
        isValid = regex.test(idNumber);
      } else if (idType === 2) {
        const regex = /^[0-9]{10}$/;
        isValid = regex.test(idNumber);
      } else {
        isValid = null;
      }
      this.identificationValid2 = isValid;
    },

    validateEmail() {
      const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      this.emailValid = regex.test(this.enterpriseData.email);
    },

    validatePhone() {
      const regex = /^[0-9]{8}$/;
      this.phoneValid = regex.test(this.enterpriseData.phoneNumber);
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
      this.$router.push('/');
    },
  },

};
</script>

<style></style>