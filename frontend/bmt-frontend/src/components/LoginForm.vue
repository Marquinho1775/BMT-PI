<template>
  <div class="d-flex justify-content-center align-items-center vh-100">
    <div class="card p-4 shadow" style="max-width: 400px; width: 100%">
      <h3 class="text-center">
        Iniciar sesión
      </h3>
      <b-form @submit.prevent="loginUser">
        <!-- Email -->
        <b-form-group id="input-group-email" label="Correo electrónico:">
          <b-form-input id="email" v-model="loginForm.Email" type="email" placeholder="Ingresar correo electrónico"
            required></b-form-input>
        </b-form-group>

        <!-- Password -->
        <b-form-group id="input-group-password" label="Contraseña:">
          <b-form-input id="password" v-model="loginForm.Password" type="password" placeholder="Ingresar contraseña"
            required></b-form-input>
        </b-form-group>

        <!-- Submit Button -->
        <div class="d-flex justify-content-between">
          <b-button type="submit" variant="primary">Iniciar sesión</b-button>
        </div>
      </b-form>
    </div>
  </div>
</template>

<script>
import axios from 'axios';
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
    async loginUser(e) {
      e.preventDefault();
      axios.post('https://localhost:7189/api/User', {
        Email: this.loginForm.Email,
        Password: this.loginForm.Password
      })
        .then(response => {
          console.log(response);
        })
        .catch(error => {
          console.log(error);
        });
    },
  },
};
</script>

<style></style>