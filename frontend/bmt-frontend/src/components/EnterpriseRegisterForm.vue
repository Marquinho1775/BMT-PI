<template>
  <v-main class="d-flex justify-center align-center" style="min-height: 100vh;">
    <v-container max-width="600px">
      <!-- Sección para Datos del Emprendedor (solo si userRole === 'cli') -->
      <v-card v-if="userRole === 'cli'" class="pa-0 mb-4 elevation-2">
        <v-card-title class="title-background text-h5">
          Datos del emprendedor
        </v-card-title>
        <v-divider></v-divider>
        <v-card-text>
          <v-form @submit.prevent="submitForm">
            <v-text-field label="Número de identificación" v-model="entrepreneurData.identification"
              placeholder="Ingrese su número de cédula" outlined
              :error-messages="touchedFields.identification && !identificationValid1 ? ['El número de cédula debe tener 9 dígitos.'] : []"
              @input="validateIdentification1" @blur="touchedFields.identification = true" required></v-text-field>
          </v-form>
        </v-card-text>
      </v-card>

      <!-- Sección para Datos de la Empresa -->
      <v-card class="pa-0 elevation-2">
        <v-card-title class="title-background text-h5">
          Datos de la empresa
        </v-card-title>
        <v-divider></v-divider>
        <v-card-text>
          <v-form @submit.prevent="submitForm" @reset="onReset">
            <!-- Tipo de identificación -->
            <v-select label="Seleccione el tipo de identificación de su negocio"
              v-model="enterpriseData.identificationType" :items="idTypeOptions" outlined
              @change="validateIdentification2" @blur="touchedFields.identificationType = true" required></v-select>

            <!-- Número de identificación -->
            <v-text-field label="Número de identificación" v-model="enterpriseData.identificationNumber"
              placeholder="Ingrese el número de identificación" outlined :error-messages="touchedFields.identificationNumber &&
                enterpriseData.identificationType !== null &&
                identificationValid2 === false
                ? [`El número de identificación debe tener ${enterpriseData.identificationType === 1 ? '9' : '10'} dígitos.`]
                : []
                " @input="validateIdentification2" @blur="touchedFields.identificationNumber = true"
              required></v-text-field>

            <!-- Nombre -->
            <v-text-field label="Nombre del emprendimiento" v-model="enterpriseData.name"
              placeholder="Ingrese el nombre de su emprendimiento" outlined
              :error-messages="touchedFields.name && !enterpriseData.name ? ['El nombre es obligatorio.'] : []"
              @blur="touchedFields.name = true" required></v-text-field>

            <!-- Descripción -->
            <v-textarea label="Descripción" v-model="enterpriseData.description"
              placeholder="Ingrese una descripción de su emprendimiento" outlined rows="4" max-rows="6"
              :error-messages="touchedFields.description && !enterpriseData.description ? ['La descripción es obligatoria.'] : []"
              @blur="touchedFields.description = true" required></v-textarea>

            <!-- Correo electrónico -->
            <v-text-field label="Correo electrónico" v-model="enterpriseData.email"
              placeholder="Ingrese el correo electrónico" outlined
              :error-messages="touchedFields.email && !emailValid ? ['El correo debe tener el formato xxxx@xxx.xxx'] : []"
              @input="validateEmail" @blur="touchedFields.email = true" required></v-text-field>

            <!-- Número de teléfono -->
            <v-text-field label="Número de teléfono" v-model="enterpriseData.phoneNumber"
              placeholder="Ingrese el número de teléfono" outlined
              :error-messages="touchedFields.phoneNumber && !phoneValid ? ['El número de teléfono debe tener 8 dígitos.'] : []"
              @input="validatePhone" @blur="touchedFields.phoneNumber = true" required></v-text-field>

            <!-- Botones -->
            <div class="d-flex justify-end mt-4">
              <v-btn color="secondary" class="mr-2" outlined @click="goBack">Volver</v-btn>
              <v-btn type="submit" color="primary">Registrar</v-btn>
            </div>
          </v-form>
        </v-card-text>
      </v-card>
    </v-container>
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
      ],
      entrepreneurData: {
        identification: '',
      },
      enterpriseData: {
        identificationType: null,
        identificationNumber: '',
        name: '',
        description: '',
        email: '',
        phoneNumber: '',
      },
      identificationValid1: true,
      identificationValid2: true,
      emailValid: true,
      phoneValid: true,
      userRole: '',
      touchedFields: {
        identification: false,
        identificationType: false,
        identificationNumber: false,
        name: false,
        description: false,
        email: false,
        phoneNumber: false,
      },
    };
  },
  mounted() {
    const user = JSON.parse(localStorage.getItem('user')) || {};
    this.userRole = user.role || '';
  },
  methods: {
    async submitForm() {
      try {
        let entrepreneurIdentification = this.entrepreneurData.identification.trim();
        if (entrepreneurIdentification !== '') {
          await this.registerEntrepreneur();
        } else {
          entrepreneurIdentification = await this.getExistingEntrepreneur();
        }
        console.log('Entrepreneur identification:', entrepreneurIdentification);
        await this.registerEnterprise();
        await this.addEntrepreneurToEnterprise(entrepreneurIdentification, this.enterpriseData.identificationNumber);
        this.generateSweetAlert('Registro exitoso', 'success', 'Se ha registrado exitosamente su empresa.');
        this.$router.push('/');
      } catch (error) {
        console.error(error);
      }
    },

    async registerEntrepreneur() {
      try {
        const response = await axios.post(`${API_URL}/Entrepreneur`, {
          userId: JSON.parse(localStorage.getItem('user')).id,
          identification: this.entrepreneurData.identification.trim(),
        });
        const user = JSON.parse(localStorage.getItem('user'));
        user.role = 'emp';
        localStorage.setItem('user', JSON.stringify(user));
        console.log('User after role change:', user);
        return response.data;
      } catch (error) {
        if (error.response.status === 409) {
          this.generateSweetAlert('Error', 'error', 'Ya existe un emprendedor con este número de identificación.');
          throw error;
        } else {
          this.generateSweetAlert('Error', 'error', 'Hubo un error al registrar el emprendedor.');
          console.error(error);
          throw error;
        }
      }
    },

    async registerEnterprise() {
      try {
        console.log('Enterprise data:', this.enterpriseData);
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
        if (error.response.status === 409) {
          const errorMessage = error.response.data.message;
          this.generateSweetAlert('Error', 'error', errorMessage);
          throw error;
        } else {
          this.generateSweetAlert('Error', 'error', 'Hubo un error al registrar la empresa.');
          console.error(error);
          throw error;
        }
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
      let response = await axios.get(API_URL + '/Entrepreneur/GetEntrepreneurByUserId?id=' + user.id, {
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
      this.identificationValid2 =
        idType === 1 ? /^[0-9]{9}$/.test(idNumber) : /^[0-9]{10}$/.test(idNumber);
    },
    validateEmail() {
      const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      this.emailValid = regex.test(this.enterpriseData.email);
    },
    validatePhone() {
      const regex = /^[0-9]{8}$/;
      this.phoneValid = regex.test(this.enterpriseData.phoneNumber);
    },
    goBack() {
      this.$router.push('/');
    },
    onReset(event) {
      event.preventDefault();
      Object.keys(this.touchedFields).forEach((key) => (this.touchedFields[key] = false));
      this.entrepreneurData.identification = '';
      this.enterpriseData = {
        identificationType: null,
        identificationNumber: '',
        name: '',
        description: '',
        email: '',
        phoneNumber: '',
      };
    },
  },
};
</script>

<style scoped>
.title-background {
  background-color: #39517B;
  color: white;
  padding: 16px;
  margin: 0;
  border-radius: 4px 4px 0 0;
  width: 100%;
  display: block;
  text-align: center;
  box-sizing: border-box;
}

body {
  background-color: #D1E4FF;
}
</style>
