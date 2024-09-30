<template>
  <div class="enterprise-register-container d-flex justify-content-center align-items-center vh-100">
    <b-form @submit.prevent="registerEnterprise" @reset="onReset">
      <div id="form" class="card custom-card my-4">
        <h3 id="title" class="text-center card-header-custom">Datos del emprendedor</h3>
        <div class="card-body">
          <b-form-group id="group-identification-number-e" label="Número de identificación"
            label-for="identification-number-e">
            <b-form-input id="identification-number-e" class="form-input" v-model="entrepreneurData.identification"
              placeholder="Ingrese su número de cédula" required>
            </b-form-input>
          </b-form-group>
        </div>
      </div>

      <div id="form" class="card custom-card my-4">
        <h3 id="title" class="text-center card-header-custom">Datos de la empresa</h3>
        <div class="card-body">
          <b-form-group id="selection-group-id-type" label="Seleccione el tipo de identificación de su negocio"
            label-for="group-id-type">
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
    async registerEnterprise() {
      try {
        const entrepreneurResponse = await axios.post('https://localhost:7189/api/Entrepreneur', {
          id: '',
          username: JSON.parse(localStorage.getItem('user')).username,
          identification: this.entrepreneurData.identification.trim(),
        });
        const enterpriseResponse = await axios.post('https://localhost:7189/api/Enterprise', {
          id: '',
          identificationType: parseInt(this.enterpriseData.identificationType),
          identificationNumber: this.enterpriseData.identificationNumber.trim(),
          name: this.enterpriseData.name.trim(),
          description: this.enterpriseData.description.trim(),
        });
        const addToEnterpriseResponse = await axios.post('https://localhost:7189/api/Entrepreneur/add-to-enterprise', {
          entrepreneurIdentification: this.entrepreneurData.identification.trim(),
          enterpriseIdentification: this.enterpriseData.identificationNumber.trim(),
          isAdmin: true,
        });
        console.log(entrepreneurResponse, enterpriseResponse, addToEnterpriseResponse);
        await this.$swal.fire({
          title: 'Registro exitoso',
          text: '¡Su empresa ha sido registrada correctamente!',
          icon: 'success',
          confirmButtonText: 'Ok'
        });
        this.$router.push('/entrepeneurhome');
      } catch (error) {
        this.$swal.fire({
          title: 'Error',
          text: 'Hubo un error al registrar su empresa. Inténtalo de nuevo.',
          icon: 'error',
          confirmButtonText: 'Ok'
        });
        console.log(error);
      }
    },

    onReset(event) {
      event.preventDefault();
      this.entrepreneurData.identification = '';
      this.enterpriseData.identificationType = null;
      this.enterpriseData.identificationNumber = '';
      this.enterpriseData.name = '';
      this.enterpriseData.description = '';
    },

    goBack() {
      this.$router.push('/entrepeneurhome');
    }
  },
};
</script>

<style>
.enterprise-register-container {
  background-color: #D1E4FF;
}

div.custom-card {
  max-width: 600px;
  background-color: #9FC9FC;
  border-radius: 20px;
  margin: 0px;
}

.card-header-custom {
  background-color: #36618E;
  color: white;
  padding: 20px;
  border-radius: 20px 20px 0 0;
  width: 100%;
  height: 100%;
}

.button {
  background-color: #39517B;
}

.button:hover {
  background-color: #02174B;
}

#form {
  background-color: #9FC9FC;
}

#title {
  color: white;
  background-color: #39517B;
}

.form-input {
  background-color: #D0EDA0 !important;
  ;
}

#select-id-type {
  background-color: #D0EDA0;
}

#identification-number {
  background-color: #D0EDA0;
}

#name {
  background-color: #D0EDA0;
}

#description {
  background-color: #D0EDA0;
}
</style>