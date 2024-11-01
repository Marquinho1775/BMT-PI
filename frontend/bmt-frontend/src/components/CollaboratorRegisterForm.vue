<template>
	<v-app class="d-flex flex-column">
		<AppHeader />
		<v-main class="flex-grow-1">
			<v-container class="d-flex justify-center align-center" style="min-height: 100vh;">
				<v-form @submit.prevent="handleSubmit" style="width: 100%; max-width: 500px;">
					<h1 v-if="title" class="text-center mb-5">{{ title }}</h1>
					
					<v-text-field
						v-model="collaboratorUsername"
						label="Username del usuario a invitar"
						required
					></v-text-field>
					<v-row class="mt-4">
						<v-col cols="6">
								<v-btn @click="goBack" color="secondary" class="w-100">Volver</v-btn>
						</v-col>
						<v-col cols="6">
								<v-btn type="submit" color="primary" class="w-100">Enviar Invitación</v-btn>
						</v-col>
					</v-row>
				</v-form>

				<v-alert v-if="successMessage" type="success" class="mt-4">
					{{ successMessage }}
				</v-alert>
				<v-alert v-if="errorMessage" type="error" class="mt-4">
					{{ errorMessage }}
				</v-alert>
			</v-container>
		</v-main>
		<AppFooter />
	</v-app>
</template>

<script>
	import axios from 'axios';
	import { API_URL } from '@/main.js';
	import { getToken } from '@/helpers/auth';

	export default {
		data() {
			return {
				title: 'Invitar colaborador',
				collaboratorUsername: '',
				successMessage: '',
				errorMessage: '',
				enterpriseId: this.$route.params.id,
			};
		},
		async created() {
			const token = getToken();
			
			try {
				const response = await axios.get(`${API_URL}/Enterprise/${this.enterpriseId}`, {
					headers: { Authorization: `Bearer ${token}` }
				});
				this.enterprise = response.data;
			} catch (error) {
				console.error('Error al cargar la empresa:', error);
				this.errorMessage = 'No se pudo obtener información de la empresa';
			}
		},
		methods: {
			async handleSubmit() {
				try {
					const userDetailsResponse = await axios.get(`${API_URL}/User/GetUserByUserName/`, {
						params: { username: this.collaboratorUsername }
					});
					const collabUser = userDetailsResponse.data;
					const collabMail = {
						Email: collabUser.email,
						Id: collabUser.id,
						EntCode: this.enterpriseId,
					};
					axios.post(API_URL + '/Email/sendcollabmail', collabMail)
					.then(() => {
						this.$swal.fire({
						title: 'Envío exitoso',
						text: '¡Haz invitado al colaborador correctamente!',
						icon: 'success',
						confirmButtonText: 'Ok'
						}).then(() => {
							this.goBack();
						});
					})
					.catch(error => {
						console.error(error);
						this.$swal.fire({
							title: 'Error',
							text: 'Hubo un error al invitar al colaborador. Inténtalo de nuevo.',
							icon: 'error',
							confirmButtonText: 'Ok'
						});
					});
				
				} catch (error) {
					console.error(error);
					this.$swal.fire({
						title: 'Error',
						text: 'Hubo un error al invitar al colaborador. Inténtalo de nuevo.',
						icon: 'error',
						confirmButtonText: 'Ok'
					});
				}
			},
			goBack() {
				this.$router.push("/enterprise/" + this.enterprise.id);
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