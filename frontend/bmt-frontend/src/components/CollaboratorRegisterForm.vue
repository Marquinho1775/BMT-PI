<template>
	<v-app class="d-flex flex-column">
		<AppHeader />
		<v-main class="flex-grow-1">
			<v-container class="d-flex justify-center align-center" style="min-height: 100vh;">
				<v-form @submit.prevent="handleSubmit" style="width: 100%; max-width: 500px;">
					<h1 v-if="title" class="text-center mb-5">{{ title }}</h1>
					
					<v-text-field
						v-model="collaboratorId"
						label="ID del usuario a invitar"
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
				collaboratorId: '',
				successMessage: '',
				errorMessage: '',
				enterprise: {}
			};
		},
		async created() {
			const enterpriseId = this.$route.params.id;
			const token = getToken();
			
			try {
				const response = await axios.get(`${API_URL}/Enterprise/${enterpriseId}`, {
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
					const userDetailsResponse = await axios.get(`${API_URL}/User/Unity/${this.collaboratorId}`);
					const collabUser = userDetailsResponse.data;
					await axios.post(`${API_URL}/email/sendcollabmail`, {
						Email: collabUser.Email,
						Id: this.enterprise.id
					});

					this.successMessage = 'Correo de invitación enviado exitosamente';
					this.errorMessage = '';
				} catch (error) {
					console.error('Error al enviar la invitación:', error);
					this.errorMessage = 'No se pudo enviar la invitación';
					this.successMessage = '';
				}
				this.goBack();
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