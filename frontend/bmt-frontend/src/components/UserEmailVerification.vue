<template>
  <div class="d-flex justify-center align-center full-height background-main">
    <v-card class="pa-0 elevation-4" elevation="12" max-width="450" width="100%">
      <v-card-title class="text-h6 mb-4 card-header-custom">Verifica tu correo</v-card-title>

      <v-card-text class="text-center justify-center align-center">
        Por favor introduce el código de verificación que se envió al correo electrónico que proporcionaste
        <br /> ({{ userEmail }})
      </v-card-text>

      <!-- Sección del OTP Input de Vuetify -->
      <v-sheet color="transparent" class="mt-4">
        <v-otp-input v-model="verificationCode" variant="solo"></v-otp-input>
      </v-sheet>

      <v-card-actions>
        <v-btn class="my-4 btn-custom" :loading="isLoading" :disabled="isLoading" block @click="verifyCode">
          {{ isLoading ? 'Verificando...' : 'Verificar Correo' }}
        </v-btn>
      </v-card-actions>

      <v-card-text class="mt-2 text-center justify-center align-center">
        ¿No recibiste el código?
        <a href="#" @click.prevent="resendCode" class="resend-link">Haz click para reenviar</a>
      </v-card-text>

      <v-alert v-if="message" type="error" class="mt-3" dense>
        {{ message }}
      </v-alert>
    </v-card>
  </div>
</template>


<script>
import { getUser } from '@/helpers/auth';
import axios from 'axios';
import { API_URL } from '@/main.js';

export default {
  data() {
    return {
      verificationCode: '',
      message: '',
      isLoading: false,
      userEmail: getUser().email,
    };
  },
  mounted() {
    this.resendCode();
  },
  methods: {
    async verifyCode() {
      this.isLoading = true;
      this.message = '';
      try {
        const codeTaken = {
          Code: this.verificationCode,
          Id: getUser().id,
        };
        await axios
          .post(`${API_URL}/User/VerifyCode`, codeTaken)
          .then(() => {
            this.$swal
              .fire({
                title: 'Verificación exitosa',
                text: '¡La cuenta ha sido verificada correctamente!',
                icon: 'success',
                confirmButtonText: 'Ok',
              })
              .then(() => {
                return axios.post(`${API_URL}/User/VerifyAccount`, codeTaken);
              })
              .then(() => {
                this.$router.push('/');
              });
          })
          .catch((error) => {
            console.error('Error en la verificación:', error);
            this.message = 'Error en la verificación. Inténtalo de nuevo más tarde.';
          });
      } catch (error) {
        this.message = 'Error en la verificación. Inténtalo de nuevo más tarde.';
      } finally {
        this.isLoading = false;
      }
    },
    async resendCode() {
      try {
        const correo = {
          Email: getUser().email,
          Id: getUser().id,
        };
        axios
          .post(`${API_URL}/Email/sendemail`, correo)
          .then((response) => {
            console.log('Reenvío con éxito:', response);
            alert('Se ha enviado un correo con el código de verificación.');
          })
          .catch((error) => {
            console.error('Error en el reenvío:', error);
          });
      } catch (error) {
        this.message = 'Error en el envío del código.';
      } finally {
        this.isLoading = false;
      }
    },
  },
};
</script>

<style scoped>
.full-height {
  height: 100vh;
}

.custom-card {
  max-width: 450px;
  border-radius: 16px;
}

.card-header-custom {
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

.text-custom {
  font-weight: 550;
  color: #49454F;
}

.btn-custom {
  background-color: #36618E;
  color: white;
  border-radius: 50px;
  font-weight: 600;
  font-size: medium;
}

.btn-custom:hover {
  background-color: #447cb9;
  color: white;
}

.resend-link {
  color: #36618E;
  text-decoration: none;
  font-weight: 550;
}

.resend-link:hover {
  text-decoration: underline;
}

.otp-input {
  display: flex;
  justify-content: center;
}
</style>
