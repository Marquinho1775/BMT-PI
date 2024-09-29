<template>
  <div class="enterprise-register-container">
    <div class="d-flex justify-content-center align-items-center vh-100">
      <div id="form" class="card custom-card" style="max-width: 400px; width: 100%">
        <h3 id="title" class="text-center card-header-custom">Registro de empresa</h3>
        <div class="card-body">
          <b-form @submit.prevent="registerEnterprise" @reset="onReset">
            <!--Identificaton type-->
            <b-form-group 
              id="selection-group-id-type" 
              label="Seleccione el tipo de identificación de su negocio"
              label-for="select-id-type">
              <b-form-select 
                id="select-id-type"
                v-model="enterpriseData.identificationType"
                required
                :options="idTypeOptions">
              </b-form-select>
            </b-form-group>
            <!-- Identification number -->
            <b-form-group 
              id="input-group-identification-number" 
              label="Número de identificación" 
              label-for="identification-number">
              <b-form-input 
                id="identification-number" 
                v-model="enterpriseData.identificationNumber" 
                placeholder="Ingresar número de identificación" required>
              </b-form-input> 
            </b-form-group>
            <!-- Name -->
            <b-form-group 
              id="input-group-name"
              label="Nombre:" 
              label-for="name">
              <b-form-input 
                id="name" 
                v-model="enterpriseData.name" 
                placeholder="Ingresar el nombre de su emprendimiento" 
                required>
              </b-form-input>
            </b-form-group>
          <!-- Description in textarea-->
            <b-form-group 
              id="input-group-description" 
              label="Descripción:" 
              label-for="description">
              <b-form-textarea 
                id="description" 
                v-model="enterpriseData.description" 
                placeholder="Ingresar una descripción de su emprendimiento" 
                rows="3">
              </b-form-textarea>
            </b-form-group>
          <!-- Submit and Reset Buttons -->
            <div class="d-flex justify-content-between">
              <b-button variant="secondary" @click="goBack">Volver</b-button>
              <b-button variant="secondary" @click="onReset">Limpiar</b-button>
              <b-button type="submit" class="button">Registrar</b-button>
            </div>
          </b-form>
        </div>
      </div>
    </div>
  </div>
  
</template>
  
<script>
  import axios from 'axios';
  
  export default {
    data() {
      return {
        idTypeOptions: [
          {value: 1, text: 'Persona física'},
          {value: 2, text: 'Persona jurídica'},
          {value: null, text: 'Seleccione una de las anteriores', disabled: true}
        ],
        enterpriseData: {
          identificationType: null,
          identificationNumber: '',
          name: '',
          description: '',
        }
      };
    },
    methods: {
      registerEnterprise() {
        console.log(this.enterpriseData.identificationType);
        console.log(this.enterpriseData.identificationNumber);
        console.log(this.enterpriseData.name);
        console.log(this.enterpriseData.description);

        // {
        //   "id": "",
        //   "identificationType": 1,
        //   "identificationNumber": "201110111",
        //   "name": "SwaggerTest",
        //   "description": "SwaggerTest"
        // }

        axios.post('https://localhost:7189/api/Enterprise', {
          id: '',
          identificationType: parseInt(this.enterpriseData.identificationType),
          identificationNumber: this.enterpriseData.identificationNumber,
          name: this.enterpriseData.name,
          description: this.enterpriseData.description,
        })
        .then((response) => {
          this.$swal.fire({
            title: 'Registro exitoso',
            text: '¡Su empresa ha sido registrada correctamente!',
            icon: 'success',
            confirmButtonText: 'Ok'
          })
          .then(() => {
            console.log(response);
            window.location.href = "/entrepeneurhome";
          });
        })
        .catch((error) => {
          this.$swal.fire({
            title: 'Error',
            text: 'Hubo un error al registrar su empresa. Inténtalo de nuevo.',
            icon: 'error',
            confirmButtonText: 'Ok'
          });
          console.log(error);
        });
      },
      onReset(event) {
        event.preventDefault();
        this.enterpriseData.identificationType = null;
        this.enterpriseData.identificationNumber = '';
        this.enterpriseData.name = '';
        this.enterpriseData.description = '';
      },
      goBack() {
        window.location.href = "/entrepeneurhome";
      }
    }
  };
</script>
  
<style >
  .enterprise-register-container {
    background-color: #D1E4FF;
  }

  div.custom-card {
  width: 650px;
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
    background-color: #D0EDA0;
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