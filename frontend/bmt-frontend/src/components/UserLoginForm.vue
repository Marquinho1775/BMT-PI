<template>
  <v-main class="d-flex justify-center align-center" style="min-height: 100vh;">
    <v-container max-width="400px">
      <v-card class="pa-0 elevation-4">
        <!-- Encabezado del título -->
        <v-card-title class="title-background text-h5">
          Iniciar sesión
        </v-card-title>

        <v-divider></v-divider>

        <v-card-text>
          <v-form @submit.prevent="loginUser">
            <!-- Campo de correo -->
            <a href="/inputCorreo">
              <v-text-field label="Correo electrónico" v-model="loginForm.Email" type="email"
                placeholder="Ingresar correo electrónico" outlined required></v-text-field>
            </a>
            <!-- Campo de contraseña -->
            <a href="/inputContrasena">
              <v-text-field label="Contraseña" v-model="loginForm.Password" type="password"
                placeholder="Ingresar contraseña" outlined required></v-text-field>
            </a>
            <!-- Botones de acción -->
            <v-card-actions class="d-flex justify-end mt-4">
              <v-btn color="secondary" class="mr-2" outlined @click="Volver">Volver</v-btn>
              <v-spacer />
              <v-btn type="submit" color="primary">Iniciar sesión</v-btn>
            </v-card-actions>
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
      loginForm: {
        Email: '',
        Password: ''
      }
    };
  },
  methods: {
    async loginUser() {
      try {
        const response = await axios.post(
          API_URL + '/User/login',
          {
            Email: this.loginForm.Email.trim(),
            Password: this.loginForm.Password.trim(),
          },
          {
            headers: {
              'Content-Type': 'application/json',
            }
          }
        );

        if (response && response.data) {
          const { token, user } = response.data;
          if (token && user) {
            const result = await axios.get(API_URL + '/ShoppingCart/GetCartId?userId=' + user.id, {
              headers: {
                Authorization: `Bearer ${token}`,
              },
            });

            localStorage.setItem('shoppingCartId', result.data);
            localStorage.setItem('token', token);
            localStorage.setItem('user', JSON.stringify(user));

            this.$swal.fire({
              title: 'Inicio de sesión exitoso',
              text: '¡Haz iniciado sesión correctamente!',
              icon: 'success',
              confirmButtonText: 'Ok',
            }).then(() => {
              if (user.isVerified) {
                window.location.href = '/';
              } else {
                window.location.href = '/email-verification';
              }
            });
          } else {
            throw new Error('Token o User no están presentes en la respuesta.');
          }
        } else {
          throw new Error('Respuesta no válida del servidor.');
        }
      } catch (error) {
        console.error(error);
        this.$swal.fire({
          title: 'Error',
          text: 'Hubo un error al iniciar sesión. Inténtalo de nuevo.',
          icon: 'error',
          confirmButtonText: 'Ok',
        });
      }
    },
    Volver() {
      window.location.href = '/';
    }
  }
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
</style>
