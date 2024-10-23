<template>

  <body>
    <div class="login-container">
      <div class="d-flex justify-content-center align-items-center vh-100">
        <div id="formLogin" class="card custom-card" style="max-width: 400px; width: 100%">
          <h3 id="tituloLogin" class="text-center card-header-custom">Iniciar sesión</h3>
          <div class="card-body">
            <b-form @submit.prevent="loginUser">

              <b-form-group id="input-group-email" label="Correo electrónico:">
                <b-form-input id="email" v-model="loginForm.Email" type="email"
                  placeholder="Ingresar correo electrónico" required></b-form-input>
              </b-form-group>

              <b-form-group id="input-group-password" label="Contraseña:">
                <b-form-input id="password" v-model="loginForm.Password" type="password"
                  placeholder="Ingresar contraseña" required></b-form-input>
              </b-form-group>

              <div class="d-flex justify-content-between">
                <b-button variant="secondary" @click="Volver">Volver</b-button>
                <b-button class="button" type="submit">Iniciar sesión</b-button>
              </div>
            </b-form>
          </div>
        </div>
      </div>
    </div>
  </body>
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
        const response = await axios.post(API_URL + '/User/login',
          {
            Email: this.loginForm.Email.trim(),
            Password: this.loginForm.Password.trim()
          },
          {
            headers: {
              'Content-Type': 'application/json'
            }
          }
        );

        if (response && response.data) {
          const { token, user } = response.data;
          if (token && user) {
            localStorage.setItem('token', token);
            localStorage.setItem('user', JSON.stringify(user));

            this.$swal.fire({
              title: 'Inicio de sesión exitoso',
              text: '¡Haz iniciado sesión correctamente!',
              icon: 'success',
              confirmButtonText: 'Ok'
            }).then(() => {
              if (user.isVerified) {
                window.location.href = "/";
              } else {
                window.location.href = "/email-verification";
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
          confirmButtonText: 'Ok'
        });
      }
    },
    Volver() {
      window.location.href = "/";
    }
  },
  created() {
  }
};
</script>

<style>
.login-container {
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

#titulo {
  color: white;
  background-color: #39517B;
}

#email {
  background-color: #D0EDA0;
}

#password {
  background-color: #D0EDA0;
}
</style>