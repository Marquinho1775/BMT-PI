<template>
  <div class="enterprise-register-container d-flex justify-content-center align-items-center vh-100">
    <b-form @submit.prevent="registerCollab" @reset="onReset">
      <div id="form" class="card custom-card my-4">
        <h3 id="title" class="text-center card-header-custom">Datos del emprendedor</h3>
        <div class="card-body"> 
          <b-form-group
            id="group-identification-number-e"
            label="Número de identificación" 
            label-for="identification-number-e">
            <b-form-input 
              id="identification-number-e" 
              class="form-input"
              v-model="collabData.identification" 
              placeholder="Ingrese su número de cédula" required>
            </b-form-input> 
          </b-form-group>
					<b-form-group
            id="group-identification-number-e"
            label="Código de la empresa" 
            label-for="identification-number-e">
            <b-form-input 
              id="identification-number-e" 
              class="form-input"
              v-model="collabData.collabEnterprise" 
              placeholder="Ingrese el código que le enviamos" required>
            </b-form-input> 
          </b-form-group>
					<div class="d-flex justify-content-between">
						<b-button type="submit" class="button">Registrar</b-button>
					</div>
        </div>
      </div>
    </b-form>
  </div>
</template>
	
<script>
import Bootstrap from 'bootstrap/dist/js/bootstrap.bundle.min.js';
import axios from 'axios';
export default {
	data() {
		return {
			collabData: {
				identification: '',
				collabEnterprise: '',
			}
		};
	},
	methods: {
		mounted() {
			Bootstrap();
		},
		async registerCollab() {
			try {
				const addToEnterpriseResponse = await axios.post('https://localhost:7189/api/Entrepreneur/add-to-enterprise', {
					entrepreneurIdentification: this.collabData.identification.trim(),
					enterpriseIdentification: this.collabData.collabEnterprise.trim(),
					isAdmin: false,
				});
				console.log(addToEnterpriseResponse);
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
	}
}
</script>
	
<style >
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
    background-color: #D0EDA0;
  }
</style>