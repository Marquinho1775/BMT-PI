<template>
	<div class="enterprise-register-container">
		<div class="d-flex justify-content-center align-items-center vh-100">
			<div id="form" class="card custom-card" style="max-width: 400px; width: 100%">
				<h3 id="title" class="text-center card-header-custom">Registro de Colaboradores</h3>
				<div class="card-body">
					<b-form @submit.prevent="registerEnterpriseCollab" @reset="onReset">
						<!-- Enterprise Identification number -->
						<b-form-group 
							id="input-group-input-box" 
							label="Número de identificación de la empresa" 
							label-for="input-box">
							<b-form-input 
								id="input-box" 
								v-model="collaboratorData.enterpriseCollab" 
								placeholder="Ingresar número de identificación de la empresa" required>
							</b-form-input> 
						</b-form-group>
						<!-- username -->
						<b-form-group 
							id="input-group-input-box" 
							label="Nombre de Usuario del colaborador" 
							label-for="input-box">
							<b-form-input 
								id="input-box" 
								v-model="collaboratorData.username" 
								placeholder="Ingresar nombre de usuario del colaborador" required>
							</b-form-input>
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
				collaboratorData: {
					username: '',
					enterpriseCollab: '',
				}
			};
		},
		methods: {
			async registerEnterpriseCollab() {
        try {
					const user = JSON.parse(localStorage.getItem(this.collaboratorData.username));
					const correo = user.email;
					axios.post('https:/localhost:7189/api/Email/sendcollabmail', correo);
          await this.$swal.fire({
            title: 'Registro exitoso',
            text: '¡Su empresa ha sido registrada correctamente!',
            icon: 'success',
            confirmButtonText: 'Ok'
          });
          this.$router.push('/entrepeneur-home');
        } catch (error) {
          this.$swal.fire({
            title: 'Error',
            text: 'Hubo un error al registrar a su colaborador. Inténtalo de nuevo.',
            icon: 'error',
            confirmButtonText: 'Ok'
          });
          console.log(error);
        }
      }, 
			onReset(event) {
				event.preventDefault();
				this.collaboratorData.username = null;
				this.collaboratorData.enterpriseCollab = '';
			},
			goBack() {
				window.location.href = "/entrepeneur-home";
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

	#input-box {
		background-color: #D0EDA0;
	}

</style>