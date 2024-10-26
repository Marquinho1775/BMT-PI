<template>
<v-app>
	<v-app-bar :elevation="5" app color="#9FC9FC" scroll-behavior="hide" dark>
      <v-toolbar-title>Business Tracker</v-toolbar-title>
      <v-spacer></v-spacer>
	</v-app-bar>
  <v-container class="d-flex justify-center align-center" style="min-height: 100vh;">
		<v-form @submit.prevent="handleSubmit">
			<h1 v-if="title" class="text-center mb-5">{{ title }}</h1>
			<v-text-field
        v-model="selectedEnterprise"
        label="ID de la empresa"
        required
      ></v-text-field>
      <v-text-field
        v-model="collaboratorId"
        label="ID del usuario a invitar"
        required
      ></v-text-field>
      <v-btn type="submit" color="primary">Enviar Invitación</v-btn>
    </v-form>
    <v-alert v-if="successMessage" type="success">
      {{ successMessage }}
    </v-alert>
    <v-alert v-if="errorMessage" type="error">
      {{ errorMessage }}
    </v-alert>
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
	import { getToken } from '@/helpers/auth';

	export default {
		data() {
			return {
				title: 'Invitar colaborador',
				selectedEnterprise: '',
				collaboratorId: '',
				entrepreneur: {
					Id: '',
					Username: '',
					Identification: '',
				}
			};
		},
		mounted() {
			this.GetEnterprisesOfEntrepreneur();
		},
		methods: {
			async GetEnterprisesOfEntrepreneur() {
				try {
					const token = getToken();
					const user = JSON.parse(localStorage.getItem('user'));

					if (!user || !user.id || !user.name || !user.lastName || !user.username || !user.email || !user.password || user.isVerified === undefined) {
						console.error('Faltan datos del usuario');
						return;
					}
					const obtainEntrepreneurResponse = await axios.post(
						API_URL + '/Entrepreneur/ObtainEntrepreneurBasedOnUser',
						{
							Id: user.id,
							Name: user.name,
							LastName: user.lastName,
							Username: user.username,
							Email: user.email,
							Password: user.password,
							IsVerified: user.isVerified
						},
						{
							headers: {
								Authorization: `Bearer ${token}`
							}
						}
					);
					const entrepreneur = obtainEntrepreneurResponse.data;
					const enterprisesResponse = await axios.post(
						API_URL + '/Entrepreneur/my-registered-enterprises',
						entrepreneur,
						{
							headers: {
								Authorization: `Bearer ${token}`
							}
						}
					);
					this.enterprises = enterprisesResponse.data;
					console.log(this.enterprises);

				} catch (error) {
					console.error('Error al obtener las empresas:', error);
					if (error.response) {
						console.error('Datos de la respuesta del servidor:', error.response.data);
					}
				}
			},
			goBack() {
				window.location.href = "/";
			},
			async handleSubmit() {
				try {
					const userDetailsResponse = await axios.get(`${API_URL}/User/Unity/${this.collaboratorId}`); // Asegúrate de que este endpoint exista y devuelva los detalles del usuario
					const collabUser = userDetailsResponse.data;

					// Ahora, usamos el correo electrónico para enviar la invitación
					await axios.post(`${API_URL}/email/sendcollabmail`, {
						Email: collabUser.Email,
						Id: this.selectedEnterprise
					});

					this.successMessage = 'Correo de invitación enviado exitosamente';
					this.errorMessage = '';
				} catch (error) {
					console.error('Error al enviar la invitación:', error);
					this.errorMessage = 'No se pudo enviar la invitación';
					this.successMessage = '';
				}
			},
			getuserId() {
				const user = JSON.parse(localStorage.getItem('user')) || {};
				return user.Id;
			}
		}
	};
</script>

<style scoped>
	.v-container {
		max-width: 600px;
		margin: 0 auto;
	}
	.v-application {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
	}

	.v-footer {
		height: 50px;
		background-color: #9FC9FC;
	}

	.flex-grow-1 {
		flex-grow: 1;
	}

	.v-card {
		margin-bottom: 16px;
	}
</style>